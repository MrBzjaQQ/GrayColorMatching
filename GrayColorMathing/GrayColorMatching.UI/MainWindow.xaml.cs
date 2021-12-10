using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using GrayColorMatching.UI.Events;
using GrayColorMatching.UI.Models;

namespace GrayColorMatching.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void OnHighlightChanged(object sender, HighlightChangedEventArgs e)
        {
            List<HighlightedEntry> groupedEntries = new List<HighlightedEntry>(e.HighlightedEntries);
            groupedEntries.Sort((a, b) => a.Index - b.Index);
            HighlightEntries(groupedEntries);
        }

        private void HighlightEntries(List<HighlightedEntry> entries)
        {
            var textRange = new TextRange(ResultBox.Document.ContentStart, ResultBox.Document.ContentEnd);
            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);

            foreach (var highlightedEntry in entries)
            {
                TextPointer text = ResultBox.Document.ContentStart;
                int offset = 0;
                while (offset <= highlightedEntry.Index)
                {
                    var newText = text.GetPositionAtOffset(1);
                    if (newText == null)
                        break;
                    text = newText;
                    var context = text.GetPointerContext(LogicalDirection.Forward);
                    if (context is not TextPointerContext.Text)
                        continue;
                    offset++;
                }

                int offsetEnd = 0;
                TextPointer entryEnd = text;
                while (offsetEnd < highlightedEntry.Length)
                {
                    var newEnd = entryEnd.GetPositionAtOffset(1);
                    if (newEnd == null)
                        break;
                    entryEnd = newEnd;
                    var context = entryEnd.GetPointerContext(LogicalDirection.Forward);
                    if (context is not TextPointerContext.Text)
                        continue;
                    offsetEnd++;
                }

                var range = new TextRange(text, entryEnd);
                range.ApplyPropertyValue(TextElement.BackgroundProperty, new BrushConverter().ConvertFromString(highlightedEntry.ColorName)!);
            }

        }
    }
}
