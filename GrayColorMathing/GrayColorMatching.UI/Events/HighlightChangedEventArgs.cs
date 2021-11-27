using System.Collections.Generic;
using GrayColorMatching.UI.Models;

namespace GrayColorMatching.UI.Events
{
    public class HighlightChangedEventArgs : System.EventArgs
    {
        public IEnumerable<HighlightedEntry> HighlightedEntries { get; init; }
    }
}
