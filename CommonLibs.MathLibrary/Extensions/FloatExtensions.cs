using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibs.MathLibrary.Extensions
{
    public static class FloatExtensions
    {
        public static float FractionToPercent(this float pFraction)
        {
            return pFraction * 100;
        }

        public static float PercentToFraction(this float pPercent)
        {
            return pPercent / 100;
        }

        public static float FractionToOdds(this float pFraction)
        {
            return (1f / pFraction) - 1;
        }

        public static float PercentToOdds(this float pPercent)
        {
            return FractionToOdds(PercentToFraction(pPercent));
        }
    }
}
