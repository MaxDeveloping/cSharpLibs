using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.MathLibrary.Probability
{
    public enum SelectorType
    {
        And, Or, ExclusiveOr
    }

    public class PathSelector
    {
        public List<string> DesiredSymbols { get; }

        public SelectorType Type { get; }

        public bool OnlyExactMatches { get; }


        public PathSelector(List<string> pAllowedSymbols, SelectorType pType, bool pOnlyExactMatches)
        {
            DesiredSymbols = pAllowedSymbols;
            Type = pType;
            OnlyExactMatches = pOnlyExactMatches;
        }
    }
}
