using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleRules.Templating
{
    /// <summary>
    /// Abstract factory for creating condition and consequence delagates
    /// </summary>
    /// <typeparam name="TFact">Fact to assert the rule against</typeparam>
    /// <typeparam name="TOutput">Object type for the rule executer to return</typeparam>
    public interface IRuleTemplateFactory<TFact, TOutput>
    {
        /// <summary>
        /// Creates the condition from the template logic
        /// </summary>
        /// <param name="templateType">The type of condition to use. It is recommended to use an enum when implementing</param>
        /// <param name="templateData">The serialized data that is needed to create the condition</param>
        /// <returns>An instance of the specific condition</returns>
        IRuleCondition<TFact> CreateCondition(int templateType, string templateData);

        /// <summary>
        /// Creates the consequence from the template logic
        /// </summary>
        /// <param name="templateType">The type of consequence to use. It is recommended to use an enum when implementing</param>
        /// <param name="templateData">The serialized data that is needed to create the consequence</param>
        /// <returns>An instance of the specific consequence</returns>
        IRuleConsequence<TFact, TOutput> CreateConsequence(int templateType, string templateData);
    }
}
