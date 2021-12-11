using System;
using System.Collections.Generic;
using System.Linq;
using GrayColorMatching.BL.Models;
using GrayColorMatching.BL.Services;
using Moq;
using NUnit.Framework;
using Match = System.Text.RegularExpressions.Match;

namespace GrayColorMatching.BL.Tests
{
    public class Tests
    {
        private Mock<IAppSettingsService> _appSettingsService;
        private ColorMatchService _service;

        [SetUp]
        public void Setup()
        {
            _appSettingsService = new Mock<IAppSettingsService>();
            _service = new ColorMatchService(_appSettingsService.Object);
        }

        [TestCaseSource(nameof(ShortHexPositiveTests))]
        [TestCaseSource(nameof(HexPositiveTests))]
        [TestCaseSource(nameof(RgbPositiveTests))]
        [TestCaseSource(nameof(CombinedPositiveTests))]
        public void PositiveTests(
            string text,
            ColorType colorType,
            string[] matches,
            List<int> matchIndexes,
            AppSettings settings)
        {
            // Arrange
            List<Match> colorMatches = new List<Match>();
            _appSettingsService.SetupGet(x => x.Settings)
                .Returns(settings);

            // Act
            var matchCollections = _service.Match(text, colorType);

            // Assert
            foreach (var collection in matchCollections)
            {
                Assert.True(colorType.HasFlag(collection.ColorType));
                colorMatches = colorMatches.Concat(collection.MatchCollection).ToList();
            }

            Assert.AreEqual(matches.Length, colorMatches.Count);

            for (int i = 0; i < matches.Length; i++)
            {
                var colorMatch = colorMatches[i];
                var expectedMatch = matches[i];
                var expectedIndex = matchIndexes[i];

                Assert.AreEqual(expectedMatch, colorMatch.Value);
                Assert.AreEqual(expectedIndex, colorMatch.Index);
            }
        }

        [TestCaseSource(nameof(ShortHexNegativeTests))]
        [TestCaseSource(nameof(HexNegativeTests))]
        [TestCaseSource(nameof(RgbNegativeTests))]
        [TestCaseSource(nameof(CombinedNegativeTests))]
        public void NegativeTests(
            string text,
            ColorType colorType,
            AppSettings settings)
        {
            // Arrange
            List<Match> colorMatches = new List<Match>();
            _appSettingsService.SetupGet(x => x.Settings)
                .Returns(settings);

            // Act
            var matchCollections = _service.Match(text, colorType);

            foreach (var collection in matchCollections)
            {
                Assert.True(colorType.HasFlag(collection.ColorType));
                colorMatches = colorMatches.Concat(collection.MatchCollection).ToList();
            }

            // Assert
            Assert.AreEqual(0, colorMatches.Count);
        }

        public static IEnumerable<TestCaseData> ShortHexPositiveTests()
        {
            yield return new TestCaseData(
                "#EEE",
                ColorType.ShortHex,
                new[] { "#EEE" },
                new List<int> { 0 },
                new AppSettings());

            yield return new TestCaseData(
                "#111",
                ColorType.ShortHex,
                new[] { "#111" },
                new List<int> { 0 },
                new AppSettings());

            yield return new TestCaseData(
                "hsdlifuh #DDD",
                ColorType.ShortHex,
                new[] { "#DDD" },
                new List<int> { 9 },
                new AppSettings());

            yield return new TestCaseData(
                "#DDD #111",
                ColorType.ShortHex,
                new[] { "#DDD", "#111" },
                new List<int> { 0, 5 },
                new AppSettings());

            yield return new TestCaseData(
                "#DDD #DDDDDD #DDD",
                ColorType.ShortHex,
                new[] { "#DDD", "#DDD" },
                new List<int> { 0, 13 },
                new AppSettings());

            yield return new TestCaseData(
                "<span style=\"background: #EEE;\"></span>",
                ColorType.ShortHex,
                new[] { "#EEE" },
                new List<int> { 25 },
                new AppSettings());

            yield return new TestCaseData(
                "jhg#ddd",
                ColorType.ShortHex,
                new[] { "#ddd" },
                new List<int> { 3 },
                new AppSettings());

            yield return new TestCaseData(
                "#dddjhg",
                ColorType.ShortHex,
                new[] { "#ddd" },
                new List<int> { 0 },
                new AppSettings());

            yield return new TestCaseData(
                "#11111",
                ColorType.ShortHex,
                new[] { "#111" },
                new List<int> { 0 },
                new AppSettings());

            yield return new TestCaseData(
                "#DdD #DDDDDD #DdD",
                ColorType.ShortHex,
                new[] { "#DdD", "#DdD" },
                new List<int> { 0, 13 },
                new AppSettings());

            yield return new TestCaseData(
                "#ddd#ddd",
                ColorType.ShortHex,
                new[] { "#ddd", "#ddd" },
                new List<int> { 0, 4 },
                new AppSettings());
        }

        public static IEnumerable<TestCaseData> HexPositiveTests()
        {
            yield return new TestCaseData(
                "#FEFEFE",
                ColorType.Hex,
                new[] { "#FEFEFE" },
                new List<int> { 0 },
                new AppSettings());

            yield return new TestCaseData(
                "#010101",
                ColorType.Hex,
                new[] { "#010101" },
                new List<int> { 0 },
                new AppSettings());

            yield return new TestCaseData(
                "hsdlifuh #FEFEFE",
                ColorType.Hex,
                new[] { "#FEFEFE" },
                new List<int> { 9 },
                new AppSettings());

            yield return new TestCaseData(
                "#FEFEFE #010101",
                ColorType.Hex,
                new[] { "#FEFEFE", "#010101" },
                new List<int> { 0, 8 },
                new AppSettings());

            yield return new TestCaseData(
                "<span style=\"background: #EEEEEE;\"></span>",
                ColorType.Hex,
                new[] { "#EEEEEE" },
                new List<int> { 25 },
                new AppSettings());

            yield return new TestCaseData(
                "jhg#dddddd",
                ColorType.Hex,
                new[] { "#dddddd" },
                new List<int> { 3 },
                new AppSettings());

            yield return new TestCaseData(
                "#ddddddjhg",
                ColorType.Hex,
                new[] { "#dddddd" },
                new List<int> { 0 },
                new AppSettings());

            yield return new TestCaseData(
                "#FEFDFE",
                ColorType.Hex,
                new[] { "#FEFDFE" },
                new List<int> { 0 },
                new AppSettings
                {
                    Delta = 1
                });

            yield return new TestCaseData(
                "#010301",
                ColorType.Hex,
                new[] { "#010301" },
                new List<int> { 0 },
                new AppSettings
                {
                    Delta = 3
                });


            yield return new TestCaseData(
                "#01040100",
                ColorType.Hex,
                new[] { "#010401" },
                new List<int> { 0 },
                new AppSettings
                {
                    Delta = 3
                });

            yield return new TestCaseData(
                "#F0f0F0",
                ColorType.Hex,
                new[] { "#F0f0F0" },
                new List<int> { 0 },
                new AppSettings());

            yield return new TestCaseData(
                "#F0f0F0#F0f0F0",
                ColorType.Hex,
                new[] { "#F0f0F0", "#F0f0F0" },
                new List<int> { 0, 7 },
                new AppSettings());
        }

        public static IEnumerable<TestCaseData> RgbPositiveTests()
        {
            yield return new TestCaseData(
                "rgb(254, 254, 254)",
                ColorType.Rgb,
                new[] { "rgb(254, 254, 254)" },
                new List<int> { 0 },
                new AppSettings());

            yield return new TestCaseData(
                "RGB(254, 254, 254)",
                ColorType.Rgb,
                new[] { "RGB(254, 254, 254)" },
                new List<int> { 0 },
                new AppSettings());

            yield return new TestCaseData(
                "RgB(254, 254, 254)",
                ColorType.Rgb,
                new[] { "RgB(254, 254, 254)" },
                new List<int> { 0 },
                new AppSettings());

            yield return new TestCaseData(
                "rgb(25, 25, 25)",
                ColorType.Rgb,
                new[] { "rgb(25, 25, 25)" },
                new List<int> { 0 },
                new AppSettings());

            yield return new TestCaseData(
                "rgb(1, 1, 1)",
                ColorType.Rgb,
                new[] { "rgb(1, 1, 1)" },
                new List<int> { 0 },
                new AppSettings());

            yield return new TestCaseData(
                "srgb(1, 1, 1)",
                ColorType.Rgb,
                new[] { "rgb(1, 1, 1)" },
                new List<int> { 1 },
                new AppSettings());

            yield return new TestCaseData(
                "rgb(1, 1, 1)s",
                ColorType.Rgb,
                new[] { "rgb(1, 1, 1)" },
                new List<int> { 0 },
                new AppSettings());

            yield return new TestCaseData(
                "rgb ( 128 , 128 , 128 )",
                ColorType.Rgb,
                new[] { "rgb ( 128 , 128 , 128 )" },
                new List<int> { 0 },
                new AppSettings());

            yield return new TestCaseData(
                "rgb(128,125,128)",
                ColorType.Rgb,
                new[] { "rgb(128,125,128)" },
                new List<int> { 0 },
                new AppSettings
                {
                    Delta = 3
                });

            yield return new TestCaseData(
                "<span style=\"background: rgb(128,125,128);\"></span>",
                ColorType.Rgb,
                new[] { "rgb(128,125,128)" },
                new List<int> { 25 },
                new AppSettings()
                {
                    Delta = 3
                });

            yield return new TestCaseData(
                "<span style=\"background: rgb(128,125,128); color: rgb(3, 4, 5);\"></span>",
                ColorType.Rgb,
                new[]
                {
                    "rgb(128,125,128)",
                    "rgb(3, 4, 5)"
                },
                new List<int> { 25, 50 },
                new AppSettings
                {
                    Delta = 3
                });

            yield return new TestCaseData(
                "rgb(128,125,128)rgb(3, 4, 5)",
                ColorType.Rgb,
                new[]
                {
                    "rgb(128,125,128)",
                    "rgb(3, 4, 5)"
                },
                new List<int> { 0, 16 },
                new AppSettings
                {
                    Delta = 3
                });
        }

        public static IEnumerable<TestCaseData> CombinedPositiveTests()
        {
            yield return new TestCaseData
            (
                "<span style=\"color: #EEE; background: #F0F0F1; border: 1px solid rgb(122, 123, 124);\"></span>",
                ColorType.ShortHex | ColorType.Hex | ColorType.Rgb,
                new[]
                { "#EEE", "#F0F0F1", "rgb(122, 123, 124)" },
                new List<int> { 20, 38, 65 },
                new AppSettings
                {
                    Delta = 3
                }
            );

            yield return new TestCaseData
            (
                "<span style=\"color: #EEE; background: #F0F0F1; border: 1px solid rgb(122, 123, 124);\"></span>",
                ColorType.Hex | ColorType.Rgb,
                new[]
                    { "#F0F0F1", "rgb(122, 123, 124)" },
                new List<int> { 38, 65 },
                new AppSettings
                {
                    Delta = 3
                }
            );

            yield return new TestCaseData
            (
                "<span style=\"color: #EEE; background: #F0F0F1; border: 1px solid rgb(122, 123, 124);\"></span>",
                ColorType.ShortHex | ColorType.Rgb,
                new[]
                    { "#EEE", "rgb(122, 123, 124)" },
                new List<int> { 20, 65 },
                new AppSettings
                {
                    Delta = 3
                }
            );

            yield return new TestCaseData
            (
                "<span style=\"color: #EEE; background: #F0F0F1; border: 1px solid rgb(122, 123, 124);\"></span>",
                ColorType.ShortHex | ColorType.Hex,
                new[]
                    { "#EEE", "#F0F0F1" },
                new List<int> { 20, 38 },
                new AppSettings
                {
                    Delta = 3
                }
            );

            yield return new TestCaseData
            (
                "<span style=\"color: #F0F0F1; background: rgb(122, 123, 124); border: 1px solid #EEE;\"></span>",
                ColorType.ShortHex | ColorType.Hex | ColorType.Rgb,
                new[]
                    { "#EEE", "#F0F0F1", "rgb(122, 123, 124)" },
                new List<int> { 79, 20, 41 },
                new AppSettings
                {
                    Delta = 3
                }
            );

            yield return new TestCaseData
            (
                "#eeeeee",
                ColorType.ShortHex | ColorType.Hex,
                new[] { "#eeeeee" },
                new List<int> { 0 },
                new AppSettings()
            );
        }

        public static IEnumerable<TestCaseData> ShortHexNegativeTests()
        {
            yield return new TestCaseData(
                "#zzz",
                ColorType.ShortHex,
                new AppSettings());

            yield return new TestCaseData(
                "#000",
                ColorType.ShortHex,
                new AppSettings());

            yield return new TestCaseData(
                "#fff",
                ColorType.ShortHex,
                new AppSettings());

            yield return new TestCaseData(
                "#f0f",
                ColorType.ShortHex,
                new AppSettings());

            yield return new TestCaseData(
                "#fEf",
                ColorType.ShortHex,
                new AppSettings
                {
                    Delta = 5
                });

            yield return new TestCaseData(
                "#eeeeee",
                ColorType.ShortHex,
                new AppSettings());

            yield return new TestCaseData(
                "####",
                ColorType.ShortHex,
                new AppSettings());

            yield return new TestCaseData(
                "#E#E",
                ColorType.ShortHex,
                new AppSettings());

            yield return new TestCaseData(
                "<span style=\"background: #EFE;\"></span>",
                ColorType.ShortHex,
                new AppSettings());

            yield return new TestCaseData(
                "<span style=\"background: #EEEEEE;\"></span>",
                ColorType.ShortHex,
                new AppSettings());
        }

        public static IEnumerable<TestCaseData> HexNegativeTests()
        {
            yield return new TestCaseData(
                "#ZZZZZZ",
                ColorType.Hex,
                new AppSettings());

            yield return new TestCaseData(
                "#000000",
                ColorType.Hex,
                new AppSettings());

            yield return new TestCaseData(
                "#FFFFFF",
                ColorType.Hex,
                new AppSettings());

            yield return new TestCaseData(
                "#FEFDFD",
                ColorType.Hex,
                new AppSettings());

            yield return new TestCaseData(
                "#FEF",
                ColorType.Hex,
                new AppSettings());

            yield return new TestCaseData(
                "#######",
                ColorType.Hex,
                new AppSettings());

            yield return new TestCaseData(
                "EEEEEE",
                ColorType.Hex,
                new AppSettings());

            yield return new TestCaseData(
                "#010101",
                ColorType.Hex,
                new AppSettings
                {
                    MaxBlackComponent = 5
                });

            yield return new TestCaseData(
                "#fefefe",
                ColorType.Hex,
                new AppSettings
                {
                    MinWhiteComponent = 250
                });

            yield return new TestCaseData(
                "#fefafe",
                ColorType.Hex,
                new AppSettings
                {
                    Delta = 3
                });

            yield return new TestCaseData(
                "<span style=\"background: #EEE;\"></span>",
                ColorType.Hex,
                new AppSettings());

            yield return new TestCaseData(
                "<span style=\"background: #EDECEF;\"></span>",
                ColorType.ShortHex,
                new AppSettings()
                {
                    Delta = 5
                });
        }

        public static IEnumerable<TestCaseData> RgbNegativeTests()
        {
            yield return new TestCaseData(
                "rgb(256, 256, 256)",
                ColorType.Rgb,
                new AppSettings());

            yield return new TestCaseData(
                "rgb(0, 0, 0)",
                ColorType.Rgb,
                new AppSettings());

            yield return new TestCaseData(
                "rgb(-1, -1, -1)",
                ColorType.Rgb,
                new AppSettings());

            yield return new TestCaseData(
                "rgb(1, 1, 1)",
                ColorType.Rgb,
                new AppSettings
                {
                    MaxBlackComponent = 2
                });

            yield return new TestCaseData(
                "rgb(128, 125, 128)",
                ColorType.Rgb,
                new AppSettings());

            yield return new TestCaseData(
                "rgb(, 125, 128)",
                ColorType.Rgb,
                new AppSettings());

            yield return new TestCaseData(
                "(128, 125, 128)",
                ColorType.Rgb,
                new AppSettings
                {
                    Delta = 3
                });

            yield return new TestCaseData(
                "rgb(253, 253, 253)",
                ColorType.Rgb,
                new AppSettings
                {
                    MinWhiteComponent = 252
                });

            yield return new TestCaseData(
                "rgb(128)",
                ColorType.Rgb,
                new AppSettings
                {
                    Delta = 3
                });
        }

        public static IEnumerable<TestCaseData> CombinedNegativeTests()
        {
            yield return new TestCaseData
            (
                "<span style=\"color: #F0F0F1; background: rgb(122, 123, 124); border: 1px solid #CED;\"></span>",
                ColorType.ShortHex | ColorType.Hex | ColorType.Rgb,
                new AppSettings()
            );

            yield return new TestCaseData
            (
                "<span style=\"color: #F0F0F1; background: rgb(122, 123, 124); border: 1px solid #FFF;\"></span>",
                ColorType.ShortHex,
                new AppSettings()
            );

            yield return new TestCaseData
            (
                "<span style=\"color: #FFFFFF; background: rgb(122, 123, 124); border: 1px solid #EEE;\"></span>",
                ColorType.Hex,
                new AppSettings()
            );

            yield return new TestCaseData
            (
                "<span style=\"color: #F0F0F1; background: rgb(0, 0, 0); border: 1px solid #EEE;\"></span>",
                ColorType.Rgb,
                new AppSettings()
            );
        }
    }
}