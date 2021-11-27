using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GrayColorMatching.BL.Models
{
    public class RegexMatchCollection
    {
        public IEnumerable<Match> MatchCollection { get; set; }

        public ColorType ColorType { get; set; }
    }
}
