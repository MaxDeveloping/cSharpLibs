using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CommonLibs.GeneralLibrary.Exceptions;

namespace CommonLibs.MathLibrary.Probability
{
    public class ProbabilityTreePath
    {
        public List<string> VisitedSymbolsOrdered { get; } = new List<string>();

        public float Probability { get; set; } = 1f;

        public ProbabilityTreePath()
        {
        }

        public ProbabilityTreePath(ProbabilityTreePath pOther)
        {
            VisitedSymbolsOrdered.AddRange(pOther.VisitedSymbolsOrdered);
            Probability = pOther.Probability;
        }

        public bool IsValidPath(PathSelector pSelector)
        {
            if (pSelector.DesiredSymbols.Count == 0)
                return true;

            switch (pSelector.Type)
            {
                case SelectorType.And:
                    return ContainsAllSymbols(pSelector);
                case SelectorType.Or:
                    return ContainsAnySymbol(pSelector);
                case SelectorType.ExclusiveOr:
                    return ContainsAnySymbolExclusive(pSelector);
                default:
                    throw new InvalidArgumentException("Unknown selector type.");
            }
        }
        
        //public bool ContainsSymbolsUnordered(params string[] pDesiredSymbols)
        //{
        //    if (pDesiredSymbols.Length == 0)
        //        return true;

        //    var groups = pDesiredSymbols.GroupBy(symbol => symbol);
        //    foreach (var group in groups)
        //    {
        //        var desiredSymbol = group.Key;
        //        var minimumCount = group.Count();

        //        if (VisitedSymbolsOrdered.Count(symbol => desiredSymbol.Equals(symbol)) < minimumCount)
        //            return false;
        //    }

        //    return true;
        //}

        public bool ContainsSymbolsOrdered(params string[] pDesiredSymbols)
        {
            if (pDesiredSymbols.Length == 0)
                return true;

            var desiredSymbolsCount = pDesiredSymbols.Length;
            var nextDesiredSymbol = pDesiredSymbols.First();
            var nextDesiredSymbolIndex = 0;


            for (var i = 0; i < VisitedSymbolsOrdered.Count; i++)
            {
                var currentSymbol = VisitedSymbolsOrdered[i];

                if (currentSymbol.Equals(nextDesiredSymbol))
                {
                    nextDesiredSymbolIndex++;
                    if (nextDesiredSymbolIndex >= desiredSymbolsCount)
                        return true; // we have found the exact order of the symbols on this path

                    nextDesiredSymbol = pDesiredSymbols[nextDesiredSymbolIndex];
                }
                else // reset the symbol as the order has been broken by an undesired symbol
                {
                    nextDesiredSymbol = pDesiredSymbols.First();
                    nextDesiredSymbolIndex = 0;

                    var countOfSymbolsLeftInPath = VisitedSymbolsOrdered.Count - i - 1;
                    if (countOfSymbolsLeftInPath < desiredSymbolsCount)
                        return false; // makes no sense to continue the search, as not enough symbols are left in the sequence
                }
            }

            return false;
        }

        private bool ContainsAllSymbols(PathSelector pSelector)
        {
            var groups = pSelector.DesiredSymbols.GroupBy(symbol => symbol);
            foreach (var group in groups)
            {
                var desiredSymbol = group.Key;
                var symbolCount = group.Count();

                if (pSelector.OnlyExactMatches)
                {
                    if (VisitedSymbolsOrdered.Count(symbol => desiredSymbol.Equals(symbol)) != symbolCount)
                        return false;
                }
                else
                {
                    if (VisitedSymbolsOrdered.Count(symbol => desiredSymbol.Equals(symbol)) < symbolCount)
                        return false;
                }
            }

            return true;
        }

        private bool ContainsAnySymbol(PathSelector pSelector)
        {
            var groups = pSelector.DesiredSymbols.GroupBy(symbol => symbol);
            foreach (var group in groups)
            {
                var desiredSymbol = group.Key;
                var symbolCount = group.Count();

                if (pSelector.OnlyExactMatches)
                {
                    if (VisitedSymbolsOrdered.Count(symbol => desiredSymbol.Equals(symbol)) == symbolCount)
                        return true;
                }
                else
                {
                    if (VisitedSymbolsOrdered.Count(symbol => desiredSymbol.Equals(symbol)) >= symbolCount)
                        return true;
                }
            }

            return false;
        }

        private bool ContainsAnySymbolExclusive(PathSelector pSelector)
        {
            var groups = pSelector.DesiredSymbols.GroupBy(symbol => symbol);
            var oneSymbolFound = false;

            foreach (var group in groups)
            {
                var desiredSymbol = group.Key;
                var symbolCount = group.Count();

                if (pSelector.OnlyExactMatches)
                {
                    if (VisitedSymbolsOrdered.Count(symbol => desiredSymbol.Equals(symbol)) == symbolCount)
                    {
                        if (oneSymbolFound)
                            return false; // one symbol was already found, now the second, so the exclusive condition is not met
                        else
                            oneSymbolFound = true;
                    }
                }
                else
                {
                    if (VisitedSymbolsOrdered.Count(symbol => desiredSymbol.Equals(symbol)) >= symbolCount)
                    {
                        if (oneSymbolFound)
                            return false; // one symbol was already found, now the second, so the exclusive condition is not met
                        else
                            oneSymbolFound = true;
                    }
                }
            }

            return oneSymbolFound;
        }
    }
}
