using System.Globalization;

namespace GrayColorMatching.BL.Specifications
{
    public abstract class ColorFilteringHexSpecificationBase : ColorFilteringSpecification
    {
        protected ColorFilteringHexSpecificationBase(int maxBlackComponent, int minWhiteComponent, int delta)
            : base(maxBlackComponent, minWhiteComponent, delta)
        {
        }

        protected bool IsSatisfyingFilteringCondition(string redStr, string greenStr, string blueStr)
        {
            int red = int.Parse(redStr, NumberStyles.HexNumber);
            int green = int.Parse(greenStr, NumberStyles.HexNumber);
            int blue = int.Parse(blueStr, NumberStyles.HexNumber);

            return IsBlackLessOrEqual(red, green, blue) &&
                   IsWhiteMoreOrEqual(red, green, blue) &&
                   IsDeltaLessOrEqual(red, green, blue);
        }
    }
}
