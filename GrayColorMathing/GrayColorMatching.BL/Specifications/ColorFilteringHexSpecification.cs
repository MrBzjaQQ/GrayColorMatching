using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GrayColorMatching.BL.Specifications
{
    public class ColorFilteringHexSpecification : ColorFilteringHexSpecificationBase
    {
        public ColorFilteringHexSpecification(int maxBlackComponent, int minWhiteComponent, int delta)
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

            return base.IsSatisfyingFilteringCondition(
                $"{entry[1]}{entry[2]}",
                $"{entry[3]}{entry[4]}",
                $"{entry[5]}{entry[6]}");
        }
    }
}
