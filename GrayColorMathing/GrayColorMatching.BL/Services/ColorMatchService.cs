using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GrayColorMatching.BL.Models;
using GrayColorMatching.BL.Specifications;

namespace GrayColorMatching.BL.Services
{
    public class ColorMatchService : IColorMatchService
    {
        private readonly Dictionary<ColorType, string> _colorPatterns;
        private readonly IAppSettingsService _appSettingsService;

        public ColorMatchService(IAppSettingsService appSettingsService)
        {
            _colorPatterns = new()
            {
                { ColorType.ShortHex, "(?!#[0-9A-Fa-f]{6})#[0-9A-Fa-f]{3}" },
                { ColorType.Hex, "#[0-9A-Fa-f]{6}" },
                { ColorType.Rgb, @"([rR][gG][bB])\s*\(\s*(2([0-4][0-9]|[5][0-5])|1[0-9]{2}|\d{1,2})\s*,\s*(2([0-4][0-9]|[5][0-5])|1[0-9]{2}|\d{1,2})\s*,\s*(2([0-4][0-9]|[5][0-5])|1[0-9]{2}|\d{1,2})\s*\)" }
            };
            _appSettingsService = appSettingsService;
        }

        public IEnumerable<RegexMatchCollection> Match(string text, ColorType colorType)
        {
            var patterns = GetColorPatterns(colorType);

            foreach (var pattern in patterns)
            {
                Regex reg = new Regex(pattern.Pattern);
                yield return new RegexMatchCollection
                {
                    MatchCollection = FilterCollection(
                        reg.Matches(text),
                        pattern.ColorType,
                        _appSettingsService.Settings.MaxBlackComponent,
                        _appSettingsService.Settings.MinWhiteComponent,
                        _appSettingsService.Settings.Delta),

                    ColorType = pattern.ColorType
                };
            }
        }

        private IEnumerable<RegexPattern> GetColorPatterns(ColorType type)
        {
            foreach (var colorType in Enum.GetValues<ColorType>())
            {
                if (type.HasFlag(colorType))
                    yield return new RegexPattern
                    {
                        ColorType = colorType,
                        Pattern = _colorPatterns[colorType]
                    };
            }
        }

        private IEnumerable<Match> FilterCollection(
            IEnumerable<Match> matchCollection,
            ColorType colorType,
            int maxBlackComponent,
            int minWhiteComponent,
            int delta)
        {
            ColorFilteringSpecification specification;
            switch (colorType)
            {
                case ColorType.ShortHex:
                    specification = new ColorFilteringShortHexSpecification(
                        maxBlackComponent,
                        minWhiteComponent,
                        delta);
                    break;

                case ColorType.Hex:
                    specification = new ColorFilteringHexSpecification(
                        maxBlackComponent,
                        minWhiteComponent,
                        delta);
                    break;

                case ColorType.Rgb:
                    specification = new ColorFilteringRgbSpecification(
                        maxBlackComponent,
                        minWhiteComponent,
                        delta);
                    break;

                default:
                    throw new ArgumentException(nameof(colorType));
            }

            return specification.ApplyFilter(matchCollection);
        }
    }
}
