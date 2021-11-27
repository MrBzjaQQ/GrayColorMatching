using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GrayColorMatching.BL.Specifications
{
    public abstract class ColorFilteringSpecification
    {
        protected int MaxBlackComponent { get; init; }

        protected int MinWhiteComponent { get; init; }

        protected int Delta { get; init; }

        protected ColorFilteringSpecification(int maxBlackComponent, int minWhiteComponent, int delta)
        {
            MaxBlackComponent = maxBlackComponent;
            MinWhiteComponent = minWhiteComponent;
            Delta = delta;
        }

        public abstract IEnumerable<Match> ApplyFilter(IEnumerable<Match> matches);

        protected bool IsDeltaLessOrEqual(int red, int green, int blue)
        {
            return Math.Abs(red - green) <= Delta
                   && Math.Abs(green - blue) <= Delta
                   && Math.Abs(red - blue) <= Delta;
        }

        protected bool IsBlackLessOrEqual(int red, int green, int blue)
        {
            return Math.Abs(red - green) <= MaxBlackComponent
                   && Math.Abs(green - blue) <= MaxBlackComponent
                   && Math.Abs(red - blue) <= MaxBlackComponent;
        }

        protected bool IsWhiteMoreOrEqual(int red, int green, int blue)
        {
            return Math.Abs(red - green) >= MaxBlackComponent
                   && Math.Abs(green - blue) >= MaxBlackComponent
                   && Math.Abs(red - blue) >= MaxBlackComponent;
        }
    }
}
