using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleRules
{
    /// <summary>
    /// Defines a programmable rule
    /// </summary>
    /// <typeparam name="TFact">The object to assert the rule against</typeparam>
    /// <typeparam name="TOutput">The object to return when a rule is triggered</typeparam>
    public class Rule<TFact, TOutput>
    {
        /// <summary>
        /// Creates a new rule object
        /// </summary>
        /// <param name="condition">Trigger logic</param>
        /// <param name="consequence">Action to perform when rule is triggered</param>
        public Rule(Func<TFact, bool> condition, Func<TFact, TOutput> consequence)
            : this (condition, consequence, 0)
        {

        }

        /// <summary>
        /// Creates a new rule object
        /// </summary>
        /// <param name="condition">Trigger logic</param>
        /// <param name="consequence">Action to perform when rule is triggered</param>
        /// <param name="priority">Priority when the same rule is triggered multiple times</param>
        Rule(Func<TFact, bool> condition, Func<TFact, TOutput> consequence, int priority)
        {
            Condition = condition;
            Consequence = consequence;
            Priority = priority;
        }

        /// <summary>
        /// Function for triggering the rule
        /// </summary>
        public Func<TFact, bool> Condition { get; private set; }

        /// <summary>
        /// A function of the fact that will return the desired object when a rule is triggered
        /// </summary>
        public Func<TFact, TOutput> Consequence { get; private set; }

        /// <summary>
        /// Used by the default prioritizer when multiple rules are triggered
        /// </summary>
        public int Priority { get; set; }
    }
}
