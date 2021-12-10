using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GrayColorMatching.BL.Specifications
{
    public class ColorFilteringRgbSpecification : ColorFilteringSpecification
    {
        public ColorFilteringRgbSpecification(int maxBlackComponent, int minWhiteComponent, int delta)
            : base(maxBlackComponent, minWhiteComponent, delta)
        {
        }

        public override IEnumerable<Match> ApplyFilter(IEnumerable<Match> matches)
        {
            return matches.Where(IsSatisfyingFilteringCondition);
        }

        private bool IsSatisfyingFilteringCondition(Match match)
        {
            string entry = match.Value;
            int indexOfOpenBracket = entry.IndexOf("(", StringComparison.InvariantCulture);
            int indexOfCloseBracket = entry.IndexOf(")", StringComparison.InvariantCulture);

            string[] components = entry.Substring(indexOfOpenBracket + 1, indexOfCloseBracket - indexOfOpenBracket - 1)
                .Split(',')
                .Select(x => x.Trim())
                .ToArray();

            int red = int.Parse(components[0]);
            int green = int.Parse(components[1]);
            int blue = int.Parse(components[2]);

            return IsBlackLessOrEqual(red, green, blue)
                   && IsWhiteMoreOrEqual(red, green, blue)
                   && IsDeltaLessOrEqual(red, green, blue);
        }
    }
}
