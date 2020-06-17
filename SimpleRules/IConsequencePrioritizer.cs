using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleRules
{
    /// <summary>
    /// Organizes the priorities
    /// </summary>
    ///
    /// <typeparam name="TOutput">Represents the object for the rule to return</typeparam>
    /// <typeparam name="TFact">The Object type for the Fact we are asserting the rule against</typeparam>
    public interface IConsequencePrioritizer<TFact, TOutput>
    {
        /// <summary>
        /// The gets the highest priority ruleConsequence
        /// </summary>
        /// <param name="triggeredConsequences">The list of consequences of the triggered rules</param>
        /// <param name="fact">Fact we are asserting the rules against</param>
        /// <returns>Selected output</returns>
        TOutput GetHighestPriorityOutput(IEnumerable<Rule<TFact,TOutput>> triggeredRules, TFact fact);
    }
}
