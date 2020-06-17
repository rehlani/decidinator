using SimpleRules.Templating;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleRules.Context
{
    /// <summary>
    /// Holds an isolated context with a rule executer and the ability to persist
    /// </summary>
    /// <typeparam name="TFact"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public class RuleContext<TFact, TOutput>
    {
        private readonly IRuleRepository _ruleRepository;
        private readonly IRuleTemplateFactory<TFact, TOutput> _ruleTemplateFactory;

        private RuleContext(
            IRuleExecuter<TFact, TOutput> ruleExecuter,
            IRuleRepository ruleRepository,
            Guid contextId,
            IRuleTemplateFactory<TFact, TOutput> ruleTemplateFactory)
        {
            RuleExecuter = ruleExecuter;
            _ruleRepository = ruleRepository;
            ContextId = contextId;
            _ruleTemplateFactory = ruleTemplateFactory;
        }

        /// <summary>
        /// A unique identifier for the context
        /// </summary>
        public Guid ContextId { get; private set; }

        /// <summary>
        /// Instance of the rule exectuter. Retrieve to exectute a fact
        /// </summary>
        public IRuleExecuter<TFact, TOutput> RuleExecuter { get; private set; }

        /// <summary>
        /// Creates a new empty Context
        /// </summary>
        /// <param name="ruleExecuter"></param>
        /// <param name="ruleRepository"></param>
        /// <param name="ruleTemplateFactory">The injected abstract factory to create the rules</param>
        /// <returns></returns>
        public static async Task<RuleContext<TFact, TOutput>> CreateContext(
            IRuleExecuter<TFact, TOutput> ruleExecuter,
            IRuleRepository ruleRepository,
            IRuleTemplateFactory<TFact, TOutput> ruleTemplateFactory)
        {
            var newGuid = Guid.NewGuid();

            return
                await
                    CreateContext(newGuid, ruleExecuter, ruleRepository, ruleTemplateFactory);
        }

        /// <summary>
        /// Creates a new context from a passed in Context Id
        /// </summary>
        /// <param name="contextId"></param>
        /// <param name="ruleExecuter"></param>
        /// <param name="ruleRepository"></param>
        /// <param name="ruleTemplateFactory"></param>
        /// <returns></returns>
        public static async Task<RuleContext<TFact, TOutput>> CreateContext(
            Guid contextId,
            IRuleExecuter<TFact, TOutput> ruleExecuter,
            IRuleRepository ruleRepository,
            IRuleTemplateFactory<TFact, TOutput> ruleTemplateFactory)
        {
            await ruleRepository.AddContext(contextId);

            return new RuleContext<TFact, TOutput>(ruleExecuter, ruleRepository, contextId, ruleTemplateFactory);
        }

        /// <summary>
        /// Loads a context by a given identifier based on a passed in repository
        /// </summary>
        /// <param name="identifier">Identifier representing a context</param>
        /// <param name="ruleExecuter">Passed in empty rule executer</param>
        /// <param name="ruleRepository">Repository for saved rules</param>
        /// <param name="ruleTemplateFactory">Factory for service specific templates</param>
        /// <returns></returns>
        public static async Task<RuleContext<TFact, TOutput>> LoadContext(
            Guid identifier,
            IRuleExecuter<TFact, TOutput> ruleExecuter,
            IRuleRepository ruleRepository,
            IRuleTemplateFactory<TFact, TOutput> ruleTemplateFactory)
        {
            var rules = await ruleRepository.GetRulesByContext(identifier);

            var context = new RuleContext<TFact, TOutput>(ruleExecuter, ruleRepository, identifier, ruleTemplateFactory);

            foreach (var ruleData in rules)
            {
                var condition = ruleTemplateFactory
                    .CreateCondition(ruleData.ConditionType, ruleData.ConditionData);
                var consequence = ruleTemplateFactory
                    .CreateConsequence(ruleData.ConsequenceType, ruleData.ConsequenceData);

                var template = new RuleTemplate<TFact, TOutput>(condition, consequence);

                ruleExecuter.AddRule(ruleData.Id, template.Rule);
            }

            return context;
        }

        /// <summary>
        /// Adds a rule to the current context
        /// </summary>
        /// <param name="conditionTemplateType"></param>
        /// <param name="conditionData"></param>
        /// <param name="consequenceTemplateType"></param>
        /// <param name="consequenceData"></param>
        /// <returns></returns>
        public async Task<Guid> AddRule(
            int conditionTemplateType,
            string conditionData,
            int consequenceTemplateType,
            string consequenceData)
        {
            var condition = _ruleTemplateFactory
                    .CreateCondition(conditionTemplateType, conditionData);
            var consequence = _ruleTemplateFactory
                .CreateConsequence(consequenceTemplateType, consequenceData);

            var template = new RuleTemplate<TFact, TOutput>(condition, consequence);

            var ruleId = RuleExecuter.AddRule(template.Rule);

            var ruleData = new RuleData()
            {
                Id = ruleId,
                ContextId = ContextId,
                ConditionType = conditionTemplateType,
                ConditionData = conditionData,
                ConsequenceType = consequenceTemplateType,
                ConsequenceData = consequenceData
            };

            await _ruleRepository.AddRule(ruleData);

            return ruleId;
        }

        /// <summary>
        /// Removes a rule from the context and database
        /// </summary>
        /// <param name="ruleId"></param>
        public Task DeleteRule(Guid ruleId)
        {
            RuleExecuter.DeleteRule(ruleId);

            return _ruleRepository.DeleteRule(ruleId);
        }
    }
}
