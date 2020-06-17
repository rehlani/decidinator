using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleRules.Templating
{
    /// <summary>
    /// Provides a reusable consequence logic
    /// </summary>
    /// <typeparam name="TFact">Fact to assert the rules against</typeparam>
    /// <typeparam name="TOutput">Output for the rule executer to return</typeparam>
    public interface IRuleConsequence<TFact, TOutput>
    {
        /// <summary>
        /// Gets the Lamdba expression for the rule consequence
        /// </summary>
        /// <returns>The Lamdba expression for the rule consequence</returns>
        Func<TFact, TOutput> GetConsequence();
    }
}
