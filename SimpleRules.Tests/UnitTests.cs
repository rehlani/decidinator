using Moq;
using SimpleRules.Context;
using SimpleRules.Templating;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SimpleRules.Tests
{
    public class UnitTests
    {
        private Mock<IRuleRepository> _moqRuleRepository = new Mock<IRuleRepository>();
        private Mock<IConsequencePrioritizer<SampleFact, SampleOutput>> _moqConsequencePriorizter =
            new Mock<IConsequencePrioritizer<SampleFact, SampleOutput>>();

        private Mock<IRuleCondition<SampleFact>> _moqCondition = new Mock<IRuleCondition<SampleFact>>();
        private Mock<IRuleConsequence<SampleFact, SampleOutput>> _moqConsequence =
            new Mock<IRuleConsequence<SampleFact, SampleOutput>>();

        private Mock<IRuleTemplateFactory<SampleFact, SampleOutput>> _moqTemplateFactory =
            new Mock<IRuleTemplateFactory<SampleFact, SampleOutput>>();

        private Mock<IRuleExecuter<SampleFact, SampleOutput>> _moqRuleExecuter =
            new Mock<IRuleExecuter<SampleFact, SampleOutput>>();

        [Fact]
        public void Rule_ConstructorShouldDefineConditionConsequenceAndPriority()
        {
            //Set up
            Func<SampleFact, bool> cond = x => x.SampleInt == 0;
            Func<SampleFact, SampleOutput> cons = y => new SampleOutput(3);

            //Execution
            var rule = new Rule<SampleFact, SampleOutput>(cond, cons);

            //Assertion
            Assert.Equal(cond, rule.Condition);
            Assert.Equal(cons, rule.Consequence);
        }

        [Fact]
        public void RuleExecuter_MustHaveARuleCountOfZero_OnCreation()
        {
            //Execution
            var ruleContext = new RuleExecuter<SampleFact, SampleOutput>(_moqConsequencePriorizter.Object);

            //Assert
            Assert.Equal(0, ruleContext.RuleCount);
        }

        [Fact]
        public void RuleExecuter_IncreaseRuleCountByOne_WhenAddingARule()
        {
            //Setup
            var ruleContext = new RuleExecuter<SampleFact, SampleOutput>(_moqConsequencePriorizter.Object);
            int previousCount = ruleContext.RuleCount;
            var rule = GetSampleRule();

            //Exectuion
            ruleContext.AddRule(rule);

            //Assertion
            Assert.Equal(previousCount + 1, ruleContext.RuleCount);
        }

        [Fact]
        public void RuleContext_ShouldHaveTheSameRuleConditionAndConsequence_WhenGettingARule()
        {
            //Setup
            var ruleContext = GetSampleExecuter();
            Func<SampleFact, bool> cond = x => x.SampleInt == 0;
            Func<SampleFact, SampleOutput> cons = y => new SampleOutput(3);
            var sampleRule = GetSampleRule(cond, cons);
            var identifier = ruleContext.AddRule(sampleRule);

            //Execution
            var rule = ruleContext.GetRule(identifier);

            //Assertion
            Assert.Equal(cond, rule.Condition);
            Assert.Equal(cons, rule.Consequence);
        }

        [Fact]
        public void RuleContext_ShouldGetTheSameGuidAsPassedIn()
        {
            //Setup
            var ruleContext = GetSampleExecuter();
            Func<SampleFact, bool> cond = x => x.SampleInt == 0;
            Func<SampleFact, SampleOutput> cons = y => new SampleOutput(3);
            var sampleRule = GetSampleRule(cond, cons);
            var identifier = Guid.NewGuid();

            //Execution
            var identifierOutput = ruleContext.AddRule(identifier, sampleRule);

            //Assertion
            Assert.Equal(identifier, identifierOutput);
        }

        [Fact]
        public void RuleContext_ShouldHaveTheSameRuleConditionAndConsequence_WhenGettingARuleWithExternalGuid()
        {
            //Setup
            var ruleContext = GetSampleExecuter();
            Func<SampleFact, bool> cond = x => x.SampleInt == 0;
            Func<SampleFact, SampleOutput> cons = y => new SampleOutput(3);
            var sampleRule = GetSampleRule(cond, cons);
            var identifier = Guid.NewGuid();

            //Execution
            var identifierOutput = ruleContext.AddRule(identifier, sampleRule);
            var rule = ruleContext.GetRule(identifier);

            //Assertion
            Assert.Equal(cond, rule.Condition);
            Assert.Equal(cons, rule.Consequence);
        }

        [Fact]
        public void RuleContext_ShouldReduceCountByOne_WhenDeletingRule()
        {
            //Setup
            var ruleContext = GetSampleExecuter();
            var sampleRule = GetSampleRule(x => x.SampleInt == 0, y => new SampleOutput(3));
            var identifier = ruleContext.AddRule(sampleRule);
            int previousCount = ruleContext.RuleCount;

            //Execution
            ruleContext.DeleteRule(identifier);

            //Assertion
            Assert.Equal(previousCount - 1, ruleContext.RuleCount);
        }

        [Fact]
        public void RuleContext_AddedRuleShouldNotExist_AfterDeletedRule()
        {
            //Setup
            var ruleContext = GetSampleExecuter();
            var sampleRule = GetSampleRule(x => x.SampleInt == 0, y => new SampleOutput(3));
            var identifier = ruleContext.AddRule(sampleRule);

            //Execution
            ruleContext.DeleteRule(identifier);

            //Assert
            Assert.Throws(typeof(KeyNotFoundException), () => ruleContext.GetRule(identifier));
        }

        [Fact]
        public void RuleContext_ShouldReturnExecutedRuleOutput_WhenSuccessfulRuleIsTriggered()
        {
            //Setup
            var sampleFact = new SampleFact()
            {
                SampleInt = 0 //Making sure that the ruleCondition is matched
            };
            SetUpDefaultMoqConsequence(sampleFact);

            var ruleContext = GetSampleExecuter();
            Func<SampleFact, bool> cond = x => x.SampleInt == 0;
            Func<SampleFact, SampleOutput> cons = y => new SampleOutput(3);
            var sampleRule = GetSampleRule(cond, cons);
            ruleContext.AddRule(sampleRule);

            //Execution
            var output = ruleContext.ExecuteFact(sampleFact);

            //Assert
            Assert.Equal(cons.Invoke(sampleFact), output);
        }

        [Fact]
        public void RuleContext_ShouldReturnNullOrDefault_WhenNoRuleIsTriggered()
        {
            //Setup
            var ruleContext = GetSampleExecuter();
            Func<SampleFact, bool> cond = x => x.SampleInt == 0;
            Func<SampleFact, SampleOutput> cons = y => new SampleOutput(3);
            var sampleRule = GetSampleRule(cond, cons);
            var sampleFact = new SampleFact()
            {
                SampleInt = 3 //Making sure that the ruleCondition is not matched
            };
            ruleContext.AddRule(sampleRule);

            //Execution
            var output = ruleContext.ExecuteFact(sampleFact);

            //Assert
            Assert.NotEqual(cons.Invoke(sampleFact), output);
        }

        [Fact]
        public void RuleContext_ShouldReturnTheHighestPriorityBasedOnIConsequencePrioritizer_WhenMultipleRulesAreTriggered()
        {
            //Setup
            Func<SampleFact, bool> cond = x => x.SampleInt == 0;
            Func<SampleFact, SampleOutput> cons = y => new SampleOutput(3);
            var sampleRule1 = GetSampleRule(cond, cons);
            var sampleRule2 = GetSampleRule(cond, y => new SampleOutput(4));
            var sampleFact = new SampleFact()
            {
                SampleInt = 0 //Making sure that the ruleCondition is matched
            };

            _moqConsequencePriorizter
                .Setup(x => x.GetHighestPriorityOutput(
                It.IsAny<IEnumerable<Rule<SampleFact, SampleOutput>>>(), It.IsAny<SampleFact>())).Returns(cons.Invoke(sampleFact));

            var ruleContext = GetSampleExecuter();
            ruleContext.AddRule(sampleRule2);
            ruleContext.AddRule(sampleRule1);

            //Execution
            var result = ruleContext.ExecuteFact(sampleFact);

            //Assertion
            Assert.Equal(cons(sampleFact), result);
        }

        [Fact]
        public void RuleTemplate_ConstructorProvidesInstanceOfConditionAndConsequence()
        {
            //Setup
            Func<SampleFact, bool> cond = x => x.SampleInt == 0;
            Func<SampleFact, SampleOutput> cons = y => new SampleOutput(3);

            //Execution
            var ruleTemplate = new RuleTemplate<SampleFact, SampleOutput>(
                _moqCondition.Object, _moqConsequence.Object);

            //Setup
            Assert.Equal(_moqCondition.Object, ruleTemplate.RuleCondition);
            Assert.Equal(_moqConsequence.Object, ruleTemplate.RuleConsequence);
        }

        [Fact]
        public void RuleTemplate_TemplateShouldHaveCompiledRule()
        {
            //Setup
            Func<SampleFact, bool> cond = x => x.SampleInt == 0;
            Func<SampleFact, SampleOutput> cons = y => new SampleOutput(3);
            _moqCondition.Setup(x => x.GetCondition()).Returns(cond);
            _moqConsequence.Setup(x => x.GetConsequence()).Returns(cons);
            var rule = GetSampleRule(cond, cons);

            //Exection
            var ruleTemplate = new RuleTemplate<SampleFact, SampleOutput>(
                _moqCondition.Object, _moqConsequence.Object);

            //Assert
            Assert.Equal(rule.Condition, ruleTemplate.Rule.Condition);
            Assert.Equal(rule.Consequence, ruleTemplate.Rule.Consequence);
        }

        [Fact]
        public void RuleContext_PopulatesRuleExecuter_OnConstruction()
        {
            //Setup
            var moqRuleExecuter = GetSampleExecuter();

            //Exection
            var ruleContext = RuleContext<SampleFact, SampleOutput>.CreateContext(
                moqRuleExecuter,
                _moqRuleRepository.Object,
                _moqTemplateFactory.Object).Result;

            //Assertion
            Assert.Equal(moqRuleExecuter, ruleContext.RuleExecuter);
            Assert.NotNull(ruleContext.ContextId);
        }

        [Fact]
        public void RuleContext_UniqueContextIds_OnCreation()
        {
            //Setup
            var moqRuleExecuter = GetSampleExecuter();

            //Exection
            var ruleContext1 = RuleContext<SampleFact, SampleOutput>.CreateContext(
                moqRuleExecuter,
                _moqRuleRepository.Object,
                _moqTemplateFactory.Object).Result;

            var ruleContext2 = RuleContext<SampleFact, SampleOutput>.CreateContext(
                moqRuleExecuter,
                _moqRuleRepository.Object,
                _moqTemplateFactory.Object).Result;

            //Assert
            Assert.NotEqual(ruleContext1.ContextId, ruleContext2.ContextId);
        }

        [Fact]
        public void RuleContext_LoadContextExecuterHasRulesFromRepository()
        {
            //Setup
            Guid contextId = Guid.NewGuid();
            Guid ruleId1 = Guid.NewGuid();
            Guid ruleId2 = Guid.NewGuid();
            _moqRuleRepository
                .Setup(x => x.GetRulesByContext(It.IsAny<Guid>()))
                .Returns(Task.FromResult(GetSampleRuleData(contextId, ruleId1, ruleId2)));
            var moqRuleExecuter = GetSampleExecuter();
            _moqTemplateFactory
                .Setup(x => x.CreateCondition(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(_moqCondition.Object);
            _moqTemplateFactory
                .Setup(x => x.CreateConsequence(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(_moqConsequence.Object);

            //Execution
            var context = RuleContext<SampleFact, SampleOutput>.LoadContext(
                contextId,
                moqRuleExecuter,
                _moqRuleRepository.Object,
                _moqTemplateFactory.Object).Result;

            //Assertion
            Assert.NotNull(context.RuleExecuter.GetRule(ruleId1));
            Assert.NotNull(context.RuleExecuter.GetRule(ruleId2));
        }

        [Fact]
        public async Task RuleContext_ShouldAddARuleToRuleExecuter_WhenAddingRule()
        {
            //Setup
            Func<SampleFact, bool> cond = x => x.SampleInt == 0;
            Func<SampleFact, SampleOutput> cons = y => new SampleOutput(3);
            Guid contextId = Guid.NewGuid();
            Guid ruleId = Guid.NewGuid();
            _moqCondition.Setup(x => x.GetCondition()).Returns(cond);
            _moqConsequence.Setup(x => x.GetConsequence()).Returns(cons);
            _moqTemplateFactory.Setup(x => x.CreateCondition(It.IsAny<int>(), It.IsAny<string>())).Returns(
                _moqCondition.Object);
            _moqTemplateFactory.Setup(x => x.CreateConsequence(It.IsAny<int>(), It.IsAny<string>())).Returns(
                _moqConsequence.Object);
            Rule<SampleFact, SampleOutput> rulePassedIntoRuleExecuter = null;
            _moqRuleExecuter.Setup(x => x.AddRule(It.IsAny<Rule<SampleFact, SampleOutput>>()))
                .Returns(ruleId)
                .Callback<Rule<SampleFact, SampleOutput>>(x => rulePassedIntoRuleExecuter = x);
            var context = RuleContext<SampleFact, SampleOutput>.CreateContext(
                _moqRuleExecuter.Object,
                _moqRuleRepository.Object,
                _moqTemplateFactory.Object);

            //Execution
            await context.Result.AddRule(0, string.Empty, 0, string.Empty);

            //Assertion
            Assert.Equal(cond, rulePassedIntoRuleExecuter.Condition);
            Assert.Equal(cons, rulePassedIntoRuleExecuter.Consequence);
        }

        [Fact]
        public void RuleContext_ShouldAddRuleToRepository_WhenAddingRule()
        {
            //Setup
            Func<SampleFact, bool> cond = x => x.SampleInt == 0;
            Func<SampleFact, SampleOutput> cons = y => new SampleOutput(3);
            Guid ruleId = Guid.NewGuid();
            _moqCondition.Setup(x => x.GetCondition()).Returns(cond);
            _moqConsequence.Setup(x => x.GetConsequence()).Returns(cons);
            _moqTemplateFactory.Setup(x => x.CreateCondition(It.IsAny<int>(), It.IsAny<string>())).Returns(
                _moqCondition.Object);
            _moqTemplateFactory.Setup(x => x.CreateConsequence(It.IsAny<int>(), It.IsAny<string>())).Returns(
                _moqConsequence.Object);
            _moqRuleExecuter.Setup(x => x.AddRule(It.IsAny<Rule<SampleFact, SampleOutput>>()))
                .Returns(ruleId);
            RuleData ruleData = null;
            _moqRuleRepository.Setup(x => x.AddRule(It.IsAny<RuleData>())).Callback<RuleData>(x => ruleData = x);

            var context = RuleContext<SampleFact, SampleOutput>.CreateContext(
                _moqRuleExecuter.Object,
                _moqRuleRepository.Object,
                _moqTemplateFactory.Object);

            //Execution
#pragma warning disable 4014
            context.Result.AddRule(0, string.Empty, 0, string.Empty);
#pragma warning restore 4014

            //Assertion
            Assert.NotNull(ruleData);
        }

        [Fact]
        public void RuleContext_ShouldCallDeleteInRuleExecuter_WhenDeletingRule()
        {
            //Setup
            var ruleId = Guid.NewGuid();

            SetupMockDependenciesForContext();
            Guid guidToDelete = Guid.Empty;
            _moqRuleExecuter
                .Setup(x => x.DeleteRule(It.IsAny<Guid>()))
                .Callback<Guid>(x => guidToDelete = x);
            var context = RuleContext<SampleFact, SampleOutput>.CreateContext(
                _moqRuleExecuter.Object,
                _moqRuleRepository.Object,
                _moqTemplateFactory.Object);

            //Execution
            context.Result.DeleteRule(ruleId);

            //Assert
            Assert.Equal(ruleId, guidToDelete);
        }

        [Fact]
        public void RuleContext_ShouldCallDeleteInRuleExecuterRepository_WhenDeletingRule()
        {
            //Setup
            var ruleId = Guid.NewGuid();

            SetupMockDependenciesForContext();
            Guid guidToDelete = Guid.Empty;
            _moqRuleRepository.Setup(x => x.DeleteRule(It.IsAny<Guid>()))
                .Callback<Guid>(x => guidToDelete = x);
            var context = RuleContext<SampleFact, SampleOutput>.CreateContext(
                _moqRuleExecuter.Object,
                _moqRuleRepository.Object,
                _moqTemplateFactory.Object).Result;

            //Execution
            context.DeleteRule(ruleId);

            //Assert
            Assert.Equal(ruleId, guidToDelete);
        }

        private void SetupMockDependenciesForContext()
        {
            Func<SampleFact, bool> cond = x => x.SampleInt == 0;
            Func<SampleFact, SampleOutput> cons = y => new SampleOutput(3);
            Guid contextId = Guid.NewGuid();
            Guid ruleId = Guid.NewGuid();
            _moqCondition.Setup(x => x.GetCondition()).Returns(cond);
            _moqConsequence.Setup(x => x.GetConsequence()).Returns(cons);
            _moqTemplateFactory.Setup(x => x.CreateCondition(It.IsAny<int>(), It.IsAny<string>())).Returns(
                _moqCondition.Object);
            _moqTemplateFactory.Setup(x => x.CreateConsequence(It.IsAny<int>(), It.IsAny<string>())).Returns(
                _moqConsequence.Object);
        }

        private Rule<SampleFact, SampleOutput> GetSampleRule()
        {
            return GetSampleRule(x => x.SampleInt == 0, y => new SampleOutput(3));
        }

        private Rule<SampleFact, SampleOutput> GetSampleRule(Func<SampleFact, bool> cond, Func<SampleFact, SampleOutput> cons)
        {
            return new Rule<SampleFact, SampleOutput>(cond, cons);
        }

        private RuleExecuter<SampleFact, SampleOutput> GetSampleExecuter()
        {
            return new RuleExecuter<SampleFact, SampleOutput>(
                _moqConsequencePriorizter.Object);
        }

        private void SetUpDefaultMoqConsequence(SampleFact sampleFact)
        {
            _moqConsequencePriorizter.Setup(x => x.GetHighestPriorityOutput(It.IsAny<IEnumerable<Rule<SampleFact, SampleOutput>>>(), It.IsAny<SampleFact>()))
                .Returns(new SampleOutput(3));
        }

        private IEnumerable<RuleData> GetSampleRuleData(Guid contextId, Guid ruleId1, Guid ruleId2)
        {
            return new List<RuleData>()
            {
                GetSampleRuleData(ruleId1, contextId),
                GetSampleRuleData(ruleId2, contextId)
            };
        }

        private RuleData GetSampleRuleData(Guid ruleId, Guid contextId)
        {
            return new RuleData()
            {
                Id = ruleId,
                ContextId = contextId,
                ConditionType = 1,
                ConditionData = "stuff",
                ConsequenceType = 1,
                ConsequenceData = "ruleConsequence"
            };
        }

    }
}
