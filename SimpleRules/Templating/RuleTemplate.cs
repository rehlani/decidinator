using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleRules.Templating
{
    /// <summary>
    /// Generates rules that so that the ruleCondition and ruleConsequence functions can be created
    /// </summary>
    /// <typeparam name="TFact">Type of fact to assert the rule against</typeparam>
    /// <typeparam name="TOutput">The type of output for the engine to return</typeparam>
    public class RuleTemplate<TFact, TOutput>
    {
        /// <summary>
        /// Creates an instance of the Rule Template Class
        /// </summary>
        /// <param name="ruleCondition">Rule Condition</param>
        /// <param name="ruleConsequence">Rule Consequence</param>
        public RuleTemplate(IRuleCondition<TFact> ruleCondition, IRuleConsequence<TFact, TOutput> ruleConsequence)
        {
            RuleCondition = ruleCondition;
            RuleConsequence = ruleConsequence;
        }

        /// <summary>
        /// Implementation of the condition logic
        /// </summary>
        public IRuleCondition<TFact> RuleCondition { get; set; }

        /// <summary>
        /// Implemention of the condition logic
        /// </summary>
        public IRuleConsequence<TFact, TOutput> RuleConsequence { get; set; }

        /// <summary>
        /// Provides the rule that the class generated
        /// </summary>
        public Rule<TFact, TOutput> Rule =>
            new Rule<TFact, TOutput>(
                RuleCondition.GetCondition(),
                RuleConsequence.GetConsequence());
    }
}
