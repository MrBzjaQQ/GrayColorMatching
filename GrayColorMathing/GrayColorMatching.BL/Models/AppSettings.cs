using System;
using System.Text.Json.Serialization;

namespace GrayColorMatching.BL.Models
{
    [Serializable]
    public record AppSettings
    {
        [JsonPropertyName("colorType")]
        public short ColorType { get; set; }

        [JsonPropertyName("delta")]
        public int Delta { get; set; }

        [JsonPropertyName("maxBlackComponent")]
        public int MaxBlackComponent { get; set; } = 1;

        [JsonPropertyName("minWhiteComponent")]
        public int MinWhiteComponent { get; set; } = 254;
    }
}
