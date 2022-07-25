using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.MathLibrary.Probability.TreeSearch
{
    public enum ProbabilityTreeSearchConnector
    {
        And, Or
    }

    public class ProbabilityTreeSearchExpression
    {
        private List<ProbabilityTreeSearchTerm> m_Terms = new List<ProbabilityTreeSearchTerm>();

        private List<ProbabilityTreeSearchConnector> m_Connectors = new List<ProbabilityTreeSearchConnector>();

        public ProbabilityTreeSearchExpression(ProbabilityTreeSearchTerm pFirstTerm)
        {
            m_Terms.Add(pFirstTerm);
        }

        public void Connect(ProbabilityTreeSearchConnector pConnector, ProbabilityTreeSearchTerm pNextTerm)
        {
            m_Connectors.Add(pConnector);
            m_Terms.Add(pNextTerm);
        }
    }
}
