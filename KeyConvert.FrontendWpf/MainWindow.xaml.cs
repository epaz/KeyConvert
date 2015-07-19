using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using KeyConvert.Convert;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FrontendWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Title = string.Format("Key Convert v{0}", Assembly.GetExecutingAssembly().GetName().Version);
            _log = new WpfLogger(ref LogTextBlock);
        }

        private readonly WpfLogger _log;

        private void SelectFolderButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog
            {
                Title = "Select Music Folder",
                IsFolderPicker = true,
                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                DefaultDirectory = Directory.GetCurrentDirectory()
            };

            if (dlg.ShowDialog() != CommonFileDialogResult.Ok) return;
            
            MusicDirectoryTextBox.Text = dlg.FileName;
            ConversionResultTextBox.Inlines.Clear();
        }

        private void SaveLogFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                FileName = "keyConvertLog.txt",
                InitialDirectory = Directory.GetCurrentDirectory(),
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    _log.SaveToFile(saveDialog.FileName);
                    var successMsg = new Run(string.Format("Log saved to {0}", saveDialog.FileName))
                    {
                        Foreground = Brushes.Green
                    };

                    SaveLogResultTextBox.Inlines.Clear();
                    SaveLogResultTextBox.Inlines.Add(successMsg);
                }
                catch (Exception)
                {
                    var errorMsg = new Run("Error saving log file.") {Foreground = Brushes.Red};

                    SaveLogResultTextBox.Inlines.Clear();
                    SaveLogResultTextBox.Inlines.Add(errorMsg);
                    throw;
                }
            }
        }

        private void ConvertKeysButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Run resultMsg;

                // first check if directory exists
                if (!Directory.Exists(MusicDirectoryTextBox.Text))
                {
                    resultMsg = new Run("Directory does not exist.") { Foreground = Brushes.DarkOrange };
                }
                else if (!Directory.EnumerateFileSystemEntries(MusicDirectoryTextBox.Text).Any(file => file.EndsWith(".mp3")))
                {
                    resultMsg = new Run("No mp3's found in directory.") { Foreground = Brushes.DarkOrange };
                }
                else
                {
                    var converter = new Id3SharpKeyConverter();
                    var converterResult = converter.ConvertFiles(MusicDirectoryTextBox.Text, false, KeyDictionaries.CamelotDictionary, _log);

                    if (converterResult.Success)
                    {
                        resultMsg = new Run(string.Format("Done! {0} of {1} files successfully converted. See log for more details.",
                                        converterResult.ConvertedFilesCount, converterResult.TotalFilesCount))
                        {
                            Foreground = Brushes.Green
                        };
                    }
                    else
                    {
                        var errorMsg = new StringBuilder("Conversion failed.");
                        foreach (string error in converterResult.Errors)
                        {
                            errorMsg.Append(" " + error);
                        }
                        resultMsg = new Run(errorMsg.ToString()) { Foreground = Brushes.Red };
                    }
                }

                ConversionResultTextBox.Inlines.Clear();
                ConversionResultTextBox.Inlines.Add(resultMsg);
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Exception caught while attempting to convert files: {0}. Stacktrace: {1}", ex.Message, ex.StackTrace));
                ConversionResultTextBox.Inlines.Clear();
                ConversionResultTextBox.Inlines.Add(new Run("Error encountered. See log for more details.") { Foreground = Brushes.Red });
            }
        }

        private static void AnimateWindowHeight(Window window)
        {
            window.BeginInit();
            //setting SizeToContent of window to Height get you the exact value of window height required to display completely
            window.SizeToContent = SizeToContent.Height;
            double height = window.ActualHeight;
            window.SizeToContent = SizeToContent.Manual;
            //run the animation code at backgroud for smoothness
            window.Dispatcher.BeginInvoke(new Action(() =>
            {
                var heightAnimation = new DoubleAnimation
                {
                    Duration = new Duration(TimeSpan.FromSeconds(0.2)),
                    From = window.ActualHeight,
                    To = height,
                    FillBehavior = FillBehavior.HoldEnd
                };
                window.BeginAnimation(HeightProperty, heightAnimation);
            }), null);
            window.EndInit();
        }

        private void ShowLogButton_OnClick(object sender, RoutedEventArgs e)
        {
            LogPanel.Visibility = Visibility.Visible;
            ShowLogButton.Visibility = Visibility.Collapsed;
        }

        private void HideLogButton_OnClick(object sender, RoutedEventArgs e)
        {
            LogPanel.Visibility = Visibility.Collapsed;
            ShowLogButton.Visibility = Visibility.Visible;
        }
    }
}
