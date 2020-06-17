using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleRules
{
    /// <summary>
    /// Holds and executes a configured rule context
    /// </summary>
    /// <typeparam name="TFact">The Fact object type</typeparam>
    /// <typeparam name="TOutput">The returned results of executing a fact</typeparam>
    public interface IRuleExecuter<TFact, TOutput>
    {
        /// <summary>
        /// Count of rules in the currenct executer
        /// </summary>
        int RuleCount { get; }

        /// <summary>
        /// Adds a new rule to the executer
        /// </summary>
        /// <param name="newRule">Compiled rule</param>
        /// <returns>New unique identifier for the rule</returns>
        Guid AddRule(Rule<TFact, TOutput> newRule);

        /// <summary>
        /// Adds a rule to the executer with an already known identifier
        /// </summary>
        /// <param name="identifier">Known unique identifier</param>
        /// <param name="newRule">Rule to add</param>
        /// <returns>Known unique identifier</returns>
        Guid AddRule(Guid identifier, Rule<TFact, TOutput> newRule);

        /// <summary>
        /// Adds Gets a known rule object by identifier
        /// </summary>
        /// <param name="guid">Known unique identifier</param>
        /// <returns></returns>
        Rule<TFact, TOutput> GetRule(Guid guid);

        /// <summary>
        /// Removes a rule from the executer
        /// </summary>
        /// <param name="ruleId"></param>
        void DeleteRule(Guid ruleId);

        /// <summary>
        /// Applies the conditions to a fact to obtain the an executed consequence
        /// </summary>
        /// <param name="fact">The fact to apply the rules to</param>
        /// <returns>Executed chosen consequence</returns>
        TOutput ExecuteFact(TFact fact);
    }
}
