using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleRules.Context
{
    /// <summary>
    /// The data object that the repository uses for persistance
    /// </summary>
    public class RuleData
    {
        /// <summary>
        /// Unique rule identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Serialized data for the condition
        /// </summary>
        public string ConditionData { get; set; }

        /// <summary>
        /// The type of condition logic
        /// </summary>
        public int ConditionType { get; set; }

        /// <summary>
        /// Serialized data for the consequence
        /// </summary>
        public string ConsequenceData { get; set; }

        /// <summary>
        /// The Type of consequence logic
        /// </summary>
        public int ConsequenceType { get; set; }

        /// <summary>
        /// The context that the rule belongs to
        /// </summary>
        public Guid ContextId { get; set; }
    }
}
