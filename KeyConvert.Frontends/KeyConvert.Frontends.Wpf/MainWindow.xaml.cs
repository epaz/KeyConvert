using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using KeyConvert.Convert;
using KeyConvert.Utils;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace KeyConvert.FrontendWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly BackgroundWorker _converterBackgroundWorker = new BackgroundWorker { WorkerReportsProgress = true };
        private readonly FileLogger _log;
        private string _filesToConvertDirectory;

        public MainWindow()
        {
            InitializeComponent();

            Title = string.Format("Key Convert v{0}", Assembly.GetExecutingAssembly().GetName().Version);
            _log = new FileLogger();
        }

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

            if (saveDialog.ShowDialog() != true) return;
            
            try
            {
                _log.SaveToFile(saveDialog.FileName);

                MessageBox.Show("Log saved to file.");
            }
            catch (Exception)
            {
                MessageBox.Show("Error saving log to file.");
            }
        }

        void converterBackgroundWorker_DoWork(object doWorkSender, DoWorkEventArgs doWorkEventArgs)
        {
            try
            {
                var dirInfo = new DirectoryInfo(_filesToConvertDirectory);
                var files = dirInfo.EnumerateFiles().Where(f => TagLibSharpConverter.SupportedExtensions.Contains(f.Extension)).ToList();

                // check if any files were found
                if (files.Count == 0)
                {
                    var noFilesWarning = string.Format("No supported files found in directory {0}", _filesToConvertDirectory);
                    _log.Warn(noFilesWarning);
                    doWorkEventArgs.Result = new ConverterResult(false, 0, 0, noFilesWarning);
                }

                _log.Info(string.Format("Found {0} files to convert.", files.Count));


                // iterate through all files found
                var converter = new Id3SharpKeyConverter(_log);
                var conversionResult = new ConverterResult(true, files.Count, 0);
                for (var i = 0; i < files.Count; i++)
                {
                    _log.Info(string.Format("Converting key on {0}", files[i].Name));
                    var result = converter.ConvertFile(files[i], KeyDictionaries.CamelotDictionary);
                    if (result.Success)
                    {
                        conversionResult.ConvertedFilesCount++;
                    }
                    else if(result.Errors.Any())
                    {
                        conversionResult.Errors.Add(result.Errors.First());
                    }

                    // report progress
                    var percentProgress = (int)(((i + 1) / (double)files.Count) * 100);
                    _converterBackgroundWorker.ReportProgress(percentProgress);
                }
                doWorkEventArgs.Result = conversionResult;
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Exception caught while converting files: {0}. Stacktrace: {1}", ex.Message, ex.StackTrace));
                
                doWorkEventArgs.Result = new ConverterResult(false, -1, -1,
                    string.Format("Exception caught while converting files: {0}. Stacktrace: {1}", ex.Message,
                        ex.StackTrace));
            }

        }

        void converterBackgroundWorker_Completed(object completedSender, RunWorkerCompletedEventArgs completedEventArgs)
        {
            var converterResult = (ConverterResult) completedEventArgs.Result;
            Run resultMsg = null;

            if (converterResult.Success)
            {
                
                resultMsg =
                    new Run(
                        string.Format(
                            "Done! {0} of {1} files successfully converted. See log for more details.",
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
                resultMsg = new Run(errorMsg.ToString()) {Foreground = Brushes.Red};
            }

            ConversionResultTextBox.Inlines.Clear();
            ConversionResultTextBox.Inlines.Add(resultMsg);
        }

        void converterBackgroundWorker_ProgressChanged(object progressSender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            ConversionProgressBar.Value = progressChangedEventArgs.ProgressPercentage;
        }

        private void ConvertKeysButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Run resultMsg = null;

                // first check if directory exists
                if (!Directory.Exists(MusicDirectoryTextBox.Text))
                {
                    resultMsg = new Run("Directory does not exist.") { Foreground = Brushes.DarkOrange };
                }
                else if (!Directory.EnumerateFileSystemEntries(MusicDirectoryTextBox.Text).Any(file => file.EndsWith(".mp3") || file.EndsWith(".aiff")))
                {
                    resultMsg = new Run("No music files found in directory.") { Foreground = Brushes.DarkOrange };
                }

                if (resultMsg != null)
                {
                    ConversionResultTextBox.Inlines.Clear();
                    ConversionResultTextBox.Inlines.Add(resultMsg);
                    return;
                }
                    
                // show progress bar
                ConversionProgressBar.Visibility = Visibility.Visible;

                // setup background worker


                _converterBackgroundWorker.DoWork += converterBackgroundWorker_DoWork;
                _converterBackgroundWorker.RunWorkerCompleted += converterBackgroundWorker_Completed;
                _converterBackgroundWorker.ProgressChanged += converterBackgroundWorker_ProgressChanged;

                // get music directory so background worker doesnt have to read UI
                _filesToConvertDirectory = MusicDirectoryTextBox.Text;
                _converterBackgroundWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Application error. Message: {0}, Stacktrace: {1}", ex.Message, ex.StackTrace));
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
    }
}
