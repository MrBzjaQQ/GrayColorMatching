using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using GrayColorMatching.UI.Events;
using GrayColorMatching.UI.Models;
using GrayColorMatching.UI.ViewModels;

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
            HighlightEntries(e.HighlightedEntries);
        }

        private void HighlightEntries(IEnumerable<HighlightedEntry> entries)
        {
            var textRange = new TextRange(ResultBox.Document.ContentStart, ResultBox.Document.ContentEnd);
            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);

            TextPointer text = ResultBox.Document.ContentStart;

            foreach (var highlightedEntry in entries)
            {
                while (text.GetPointerContext(LogicalDirection.Forward) != TextPointerContext.Text)
                {
                    text = text.GetNextContextPosition(LogicalDirection.Forward);
                }

                int entryIndex = text.GetTextInRun(LogicalDirection.Forward).IndexOf(highlightedEntry.Text, StringComparison.InvariantCulture);

                text = text.GetPositionAtOffset(entryIndex);

                var entryEnd = text.GetPositionAtOffset(highlightedEntry.Length, LogicalDirection.Forward);
                var range = new TextRange(text, entryEnd);
                range.ApplyPropertyValue(TextElement.BackgroundProperty, new BrushConverter().ConvertFromString(highlightedEntry.ColorName));

                text = entryEnd;
            }
        }
    }
}
