using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.MathLibrary.Probability.TreeSearch
{
    public class ProbabilityTreeSearchTerm
    {
        public List<string> SymbolSet { get; } = new List<string>();

        public ProbabilityTreeSearchTerm(params string[] pSymbolSet)
        {
            SymbolSet.AddRange(pSymbolSet);
        }
    }
}
