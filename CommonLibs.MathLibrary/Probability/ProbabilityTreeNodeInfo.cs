using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.MathLibrary.Probability
{
    public class ProbabilityTreeNodeInfo
    {
        public string Name { get; }

        public int NumberOfItemsLeft { get; }

        public ProbabilityTreeNodeInfo(string pName, int pNumberOfItemsLeft)
        {
            Name = pName;
            NumberOfItemsLeft = pNumberOfItemsLeft;
        }
    }
}
