using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleRules.Tests
{
    public class SampleOutput : IEquatable<SampleOutput>
    {
        public SampleOutput()
        {
        }

        public SampleOutput(int output)
        {
            OutputInteger = output;
        }

        public int OutputInteger { get; set; }

        public bool Equals(SampleOutput other)
        {
            return other != null && other.OutputInteger == OutputInteger;
        }
    }
}
