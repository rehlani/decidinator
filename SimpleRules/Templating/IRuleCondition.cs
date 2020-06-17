using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleRules.Templating
{
    /// <summary>
    /// Provides and example ruleCondition
    /// </summary>
    /// <typeparam name="TFact">Fact to assert the condition against</typeparam>
    public interface IRuleCondition<TFact>
    {
        /// <summary>
        /// Provides the lambda expression to a condition
        /// </summary>
        /// <returns>Lambda expression to a condition</returns>
        Func<TFact, bool> GetCondition();
    }
}
