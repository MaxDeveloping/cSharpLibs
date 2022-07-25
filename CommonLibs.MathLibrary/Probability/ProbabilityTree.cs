using CommonLibs.GeneralLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CommonLibs.MathLibrary.Probability
{
    public class ProbabilityTree
    {
        private List<ProbabilityTreeNode> m_PossibleNodes;

        private List<ProbabilityTreePath> m_AllPaths;

        public ProbabilityRootNode RootNode { get; private set; }

        public int TotalItemsCount { get; }



        public ProbabilityTree(List<ProbabilityTreeNodeInfo> pNodeInfos)
        {
            if (pNodeInfos.GroupBy(node => node.Name).Count() != pNodeInfos.Count)
                throw new InvalidArgumentException("All nodes must be distinct from one another.");

            m_PossibleNodes = new List<ProbabilityTreeNode>();

            TotalItemsCount = pNodeInfos.Sum(info => info.NumberOfItemsLeft);
            
            foreach (var info in pNodeInfos)
            {
                var node = new ProbabilityTreeNode(info.Name, info.NumberOfItemsLeft, TotalItemsCount, RootNode);
                m_PossibleNodes.Add(node);
            }
        }

        public float CalculateProbabilityOrdered(int pSearchDepth, params string[] pDesiredResults)
        {
            var resultsCount = pDesiredResults.Length;
            if (resultsCount > pSearchDepth)
                throw new InvalidArgumentException("The results count can't exceed the tree depth.");

            BuildTree(pSearchDepth);

            var validPaths = m_AllPaths.Where(path => path.ContainsSymbolsOrdered(pDesiredResults));
            return validPaths.Sum(path => path.Probability);
        }

        public float CalculateProbabilityUnordered(int pSearchDepth, PathSelector pSelector)
        {
            var resultsCount = pSelector.DesiredSymbols.Count;
            if (resultsCount > pSearchDepth)
                throw new InvalidArgumentException("The results count can't exceed the tree depth.");

            BuildTree(pSearchDepth);

            var validPaths = m_AllPaths.Where(path => path.IsValidPath(pSelector));
            return validPaths.Sum(path => path.Probability);
        }


        private void BuildTree(int pDepth)
        {
            if (pDepth > TotalItemsCount)
                throw new InvalidArgumentException("Depth can't exceed total count of items.");

            RootNode = new ProbabilityRootNode();
            RootNode.BuildNextNodesRecursively(m_PossibleNodes, TotalItemsCount, 0, pDepth);

            CalculatePaths();
        }

        private void CalculatePaths()
        {
            var paths = RootNode.TakeAllPathsRecursively(new ProbabilityTreePath(), TotalItemsCount);
            
            if (paths == null)
                m_AllPaths = new List<ProbabilityTreePath>();
            else
                m_AllPaths = paths;
        }
    }
}
