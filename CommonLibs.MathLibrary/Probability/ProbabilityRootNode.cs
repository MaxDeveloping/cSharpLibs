using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.MathLibrary.Probability
{
    public class ProbabilityRootNode : ProbabilityTreeNode
    {
        public ProbabilityRootNode()
        {
            Name = "root";
            NumberOfItemsLeft = 1;
            TotalItems = 1;
            ProbabilityFraction = 1f;
        }

        public override ProbabilityTreePath VisitNode(ProbabilityTreePath pCurrentPath, int pTotalItemsLeft)
        {
            throw new InvalidOperationException("You can't visit the root node. It always is the start already.");
        }
    }
}
