using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRules.Context
{
    /// <summary>
    /// Repository for persisting rule data
    /// </summary>
    public interface IRuleRepository
    {
        /// <summary>
        /// Gets all the rules for a given context
        /// </summary>
        /// <param name="contextId"></param>
        /// <returns></returns>
        Task<IEnumerable<RuleData>> GetRulesByContext(Guid contextId);

        /// <summary>
        /// Adds a new context to the repository
        /// </summary>
        /// <param name="contextId">identifier for the context Id</param>
        Task AddContext(Guid contextId);

        /// <summary>
        /// Adds a rule by an existing context to the repository
        /// </summary>
        /// <param name="ruleData">Data object for the rule</param>
        Task AddRule(RuleData ruleData);

        /// <summary>
        /// Removes a rule from the repository
        /// </summary>
        /// <param name="ruleId">identifier for the rule</param>
        Task DeleteRule(Guid ruleId);
    }
}
