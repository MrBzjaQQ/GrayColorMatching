using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using GrayColorMatching.BL;
using GrayColorMatching.BL.Models;
using GrayColorMatching.BL.Services;
using GrayColorMatching.UI.Annotations;
using GrayColorMatching.UI.Events;
using GrayColorMatching.UI.Infrastructure;
using GrayColorMatching.UI.Models;
using Microsoft.Win32;

namespace GrayColorMatching.UI.ViewModels
{
    public class ColorMatchingViewModel : INotifyPropertyChanged
    {
        private readonly IColorMatchService _matchService;
        private readonly IAppSettingsService _appSettingsService;
        private string _sourceText;
        private string _resultText;
        private bool _isShortHexChecked;
        private bool _isHexChecked;
        private bool _isRgbChecked;
        private ICommand _findColorCommand;
        private ICommand _openFileCommand;
        private Dictionary<ColorType, string> _highlightTable;

        public ColorMatchingViewModel(IColorMatchService matchService, IAppSettingsService appSettingsService)
        {
            _matchService = matchService;
            _appSettingsService = appSettingsService;
            SelectDefaultColorPatternIfNecessary();

            _highlightTable = new Dictionary<ColorType, string>()
            {
                {ColorType.ShortHex, "Pink"},
                {ColorType.Hex, "BlanchedAlmond"},
                {ColorType.Rgb, "Gainsboro"}
            };
        }

        public string SourceText
        {
            get => _sourceText;

            set
            {
                _sourceText = value;
                OnPropertyChanged(nameof(SourceText));
                OnPropertyChanged(nameof(IsFindColorEnabled));
            }
        }

        public string ResultText
        {
            get => _resultText;

            set
            {
                _resultText = value;
                OnPropertyChanged(nameof(ResultText));
            }
        }

        public bool IsShortHexChecked
        {
            get => _isShortHexChecked;

            set
            {
                _isShortHexChecked = value;
                ColorType |= ColorType.ShortHex;
                SelectDefaultColorPatternIfNecessary();
                OnPropertyChanged(nameof(IsShortHexChecked));
            }
        }

        public string ShortHexColor => _highlightTable[ColorType.ShortHex];

        public bool IsHexChecked
        {
            get => _isHexChecked;

            set
            {
                _isHexChecked = value;
                ColorType |= ColorType.Hex;
                SelectDefaultColorPatternIfNecessary();
                OnPropertyChanged(nameof(IsHexChecked));
            }
        }

        public string HexColor => _highlightTable[ColorType.Hex];

        public bool IsRgbChecked
        {
            get => _isRgbChecked;

            set
            {
                _isRgbChecked = value;
                ColorType |= ColorType.Rgb;
                SelectDefaultColorPatternIfNecessary();
                OnPropertyChanged(nameof(IsRgbChecked));
            }
        }

        public string RgbColor => _highlightTable[ColorType.Rgb];

        public int MinWhiteComponent
        {
            get => _appSettingsService.Settings.MinWhiteComponent;

            set
            {
                _appSettingsService.Settings.MinWhiteComponent = value; 
                OnPropertyChanged(nameof(MinWhiteComponent));
            }
        }

        public int MaxBlackComponent
        {
            get => _appSettingsService.Settings.MaxBlackComponent;

            set
            {
                _appSettingsService.Settings.MaxBlackComponent = value;
                OnPropertyChanged(nameof(MaxBlackComponent));
            } 
        }

        public int Delta
        {
            get => _appSettingsService.Settings.Delta;

            set
            {
                _appSettingsService.Settings.Delta = value;
                OnPropertyChanged(nameof(Delta));
            }
        }

        public bool IsFindColorEnabled => !string.IsNullOrWhiteSpace(SourceText);

        public ColorType ColorType
        {
            get => (ColorType)_appSettingsService.Settings.ColorType;

            set => _appSettingsService.Settings.ColorType = (short)value;
        }

        public ICommand FindColorCommand => _findColorCommand ??= new RelayCommand(_ =>
        {
            OnFindColorClick();
        });

        public ICommand OpenFileCommand => _openFileCommand ??= new RelayCommand(_ =>
        {
            OnOpenFileClick();
        });

        public event PropertyChangedEventHandler PropertyChanged;
        public event HighlightChangedEventHandler HighlightChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnFindColorClick()
        {
            if (string.IsNullOrWhiteSpace(SourceText))
                return;

            ResultText = SourceText;
            var matches = _matchService.Match(ResultText, ColorType);
            HighlightEntries(matches);
        }

        private void HighlightEntries(IEnumerable<RegexMatchCollection> collections)
        {
            List<HighlightedEntry> entries = new List<HighlightedEntry>();

            foreach (var collection in collections)
            {
                var color = _highlightTable[collection.ColorType];
                foreach(var match in collection.MatchCollection)
                    entries.Add(new HighlightedEntry
                    {
                        ColorName = color,
                        Index = match.Index,
                        Text = match.Value,
                        Length = match.Length
                    });
            }

            HighlightChanged?.Invoke(this, new HighlightChangedEventArgs
            {
                HighlightedEntries = entries
            });
        }

        private void OnOpenFileClick()
        {
            OpenFileDialog openFileDialog = new()
            {
                Multiselect = false
            };
            bool? fileSelected = openFileDialog.ShowDialog();
            if (fileSelected.HasValue && fileSelected.Value)
            {
                using FileStream stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                using StreamReader reader = new StreamReader(stream);
                SourceText = reader.ReadToEnd();
                reader.Close();
                stream.Close();
            }
        }

        private void SelectDefaultColorPatternIfNecessary()
        {
            if (!_isHexChecked && !_isRgbChecked && !_isShortHexChecked)
            {
                _isShortHexChecked = true;
                ColorType = ColorType.ShortHex;
                OnPropertyChanged(nameof(IsShortHexChecked));
            }
        }
    }
}
