using CommonLibs.GeneralLibrary.Exceptions;
using CommonLibs.MathLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLibs.MathLibrary.Probability
{
    public class ProbabilityTreeNode
    {
        public string Name { get; protected set; }

        public int NumberOfItemsLeft { get; protected set; }

        public int TotalItems { get; protected set; }

        public float ProbabilityFraction { get; protected set; }

        public float ProbabilityPercent
        {
            get { return ProbabilityFraction.FractionToPercent(); }
        }


        public ProbabilityTreeNode Parent { get; }

        public List<ProbabilityTreeNode> ChildNodes { get; } = new List<ProbabilityTreeNode>();

        protected ProbabilityTreeNode() { }



        public ProbabilityTreeNode(string pName, int pNumberOfItemsLeft, int pTotalItems, ProbabilityTreeNode pParent)
        {
            if (pNumberOfItemsLeft <= 0)
                throw new InvalidArgumentException("Number of items left must be greater zero.");

            Name = pName;
            NumberOfItemsLeft = pNumberOfItemsLeft;
            TotalItems = pTotalItems;
            Parent = pParent;

            ProbabilityFraction = CalculateProbability();
        }

        public void BuildNextNodesRecursively(List<ProbabilityTreeNode> pNodes, int pTotalItems, int pCurrentDepth, int pTargetDepth)
        {
            if (pCurrentDepth == pTargetDepth)
                return;

            foreach (var node in pNodes)
            {
                int numberOfItemsLeft = node.NumberOfItemsLeft;

                if (node.Name.Equals(this.Name))
                    numberOfItemsLeft--;

                if (numberOfItemsLeft > 0)
                {
                    var childNode = new ProbabilityTreeNode(node.Name, numberOfItemsLeft, pTotalItems, this);
                    ChildNodes.Add(childNode);
                }
            }


            pCurrentDepth++;
            foreach (var childNode in ChildNodes)
                childNode.BuildNextNodesRecursively(ChildNodes, pTotalItems - 1, pCurrentDepth, pTargetDepth);
        }

        public virtual ProbabilityTreePath VisitNode(ProbabilityTreePath pCurrentPath, int pTotalItemsLeft)
        {
            var path = new ProbabilityTreePath(pCurrentPath);
            path.VisitedSymbolsOrdered.Add(Name);
            path.Probability *= ((float)NumberOfItemsLeft / (float)pTotalItemsLeft);
            return path;
        }


        public List<ProbabilityTreePath> TakeAllPathsRecursively(ProbabilityTreePath pCurrentPath, int pTotalItemsLeft)
        {
            if (ChildNodes.Count == 0)
                return null;

            var result = new List<ProbabilityTreePath>();

            foreach (var node in ChildNodes)
            {
                var path = node.VisitNode(pCurrentPath, pTotalItemsLeft);
                var paths = node.TakeAllPathsRecursively(path, pTotalItemsLeft - 1);

                if (paths == null)
                    result.Add(path);
                else
                    result.AddRange(paths);
            }

            return result;
        }

        private float CalculateProbability()
        {
            var probability = (float)NumberOfItemsLeft / (float)TotalItems;
            var current = Parent;
            while (current != null)
            {
                if (current.Parent == null)
                    break;

                probability *= (float)current.NumberOfItemsLeft / (float)current.TotalItems;
                current = current.Parent;
            }

            return probability;
        }

        private int CalculateTotalPossibilities()
        {
            int totalPossibilities = TotalItems;
            var current = Parent;
            while (current != null)
            {
                if (current.Parent == null)
                    break;

                totalPossibilities *= current.TotalItems;
                current = current.Parent;
            }

            return totalPossibilities;
        }
    }
}
