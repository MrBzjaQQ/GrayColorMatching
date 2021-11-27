using System.Collections.Generic;
using GrayColorMatching.BL.Models;

namespace GrayColorMatching.BL.Services
{
    public interface IColorMatchService
    {
        IEnumerable<RegexMatchCollection> Match(string text, ColorType colorType);
    }
}
