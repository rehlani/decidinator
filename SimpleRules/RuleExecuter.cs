using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleRules
{
    /// <summary>
    /// Holds and executes a configured rule context
    /// </summary>
    /// <typeparam name="TFact">The Fact object type</typeparam>
    /// <typeparam name="TOutput">The returned results of executing a fact</typeparam>
    public class RuleExecuter<TFact, TOutput> : IRuleExecuter<TFact, TOutput>
    {
        /// <summary>
        /// Creates an instance of the configured rule context
        /// </summary>
        public RuleExecuter()
            : this(new PriorityConsequencePrioritizer<TFact, TOutput>())
        {

        }

        /// <summary>
        /// Creates an instance of the configured rule context
        /// </summary>
        /// <param name="prioritizer">Prioritizer to decide how to prioritize the triggered rules when there are multiple</param>
        public RuleExecuter(IConsequencePrioritizer<TFact, TOutput> prioritizer)
        {
            RuleList = new Dictionary<Guid, Rule<TFact, TOutput>>();
            Prioritizer = prioritizer;
        }

        /// <summary>
        /// Count of rules in the currenct executer
        /// </summary>
        public int RuleCount => RuleList.Count;

        protected Dictionary<Guid, Rule<TFact, TOutput>> RuleList { get; }

        protected IConsequencePrioritizer<TFact, TOutput> Prioritizer { get; }

        /// <summary>
        /// Adds a new rule to the executer
        /// </summary>
        /// <param name="newRule">Compiled rule</param>
        /// <returns>New unique identifier for the rule</returns>
        public Guid AddRule(Rule<TFact, TOutput> newRule)
        {
            return AddRule(Guid.NewGuid(), newRule);
        }

        /// <summary>
        /// Adds a rule to the executer with an already known identifier
        /// </summary>
        /// <param name="identifier">Known unique identifier</param>
        /// <param name="newRule">Rule to add</param>
        /// <returns>Known unique identifier</returns>
        public Guid AddRule(Guid identifier, Rule<TFact, TOutput> newRule)
        {
            RuleList.Add(identifier, newRule);

            return identifier;
        }

        /// <summary>
        /// Adds Gets a known rule object by identifier
        /// </summary>
        /// <param name="guid">Known unique identifier</param>
        /// <returns>The fully populated rule with the correct cooresponding identifier</returns>
        /// <exception cref="KeyNotFoundException">Throws key not found exception if the rule does not exist</exception>
        public Rule<TFact, TOutput> GetRule(Guid guid)
        {
            return RuleList[guid];
        }

        /// <summary>
        /// Removes a rule from the executer
        /// </summary>
        /// <param name="guid">Identifier of the rule to delete</param>
        public void DeleteRule(Guid guid)
        {
            RuleList.Remove(guid);
        }

        /// <summary>
        /// Applies the conditions to a fact to obtain the an executed consequence
        /// </summary>
        /// <param name="fact">The fact to apply the rules to</param>
        /// <returns>Executed chosen consequence</returns>
        public virtual TOutput ExecuteFact(TFact fact)
        {
            var validRules = RuleList.Select(rule => rule.Value)
                .Where(rule => rule.Condition(fact));

            if (validRules.Any())
            {
                return Prioritizer.GetHighestPriorityOutput(validRules, fact);
            }
            else
            {
                return default(TOutput);
            }
        }
    }
}
