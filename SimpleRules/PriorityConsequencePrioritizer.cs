using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleRules
{
    public class PriorityConsequencePrioritizer<TFact, TOutput> : IConsequencePrioritizer<TFact, TOutput>
    {
        /// <summary>
        /// Gets the rule output with the highest priority
        /// </summary>
        /// <param name="triggeredRules"></param>
        /// <param name="fact"></param>
        /// <returns></returns>
        public TOutput GetHighestPriorityOutput(IEnumerable<Rule<TFact, TOutput>> triggeredRules, TFact fact)
        {
            return triggeredRules.OrderByDescending(r => r.Priority).First().Consequence.Invoke(fact);
        }
    }
}
