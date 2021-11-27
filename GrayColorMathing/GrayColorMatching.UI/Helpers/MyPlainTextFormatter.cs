using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using Xceed.Wpf.Toolkit;

namespace GrayColorMatching.UI.Helpers
{
    public class MyPlainTextFormatter : ITextFormatter
    {
        public string GetText(FlowDocument document)
        {
            var range = new TextRange(document.ContentStart, document.ContentEnd);

            using (var stream = new MemoryStream())
            {
                range.Save(stream, DataFormats.Text);

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public void SetText(FlowDocument document, string text)
        {
            new TextRange(document.ContentStart, document.ContentEnd).Text = text;

            if (string.IsNullOrEmpty(text))
            {
                document.Blocks.Clear();
            }
            else
            {
                TextRange tr = new TextRange(document.ContentStart, document.ContentEnd);
                var unicode = Encoding.Unicode.GetBytes(text);
                var utf8 = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, unicode);
                using (MemoryStream ms = new MemoryStream(utf8))
                {
                    tr.Load(ms, DataFormats.Text);
                }
            }
        }
    }
}
