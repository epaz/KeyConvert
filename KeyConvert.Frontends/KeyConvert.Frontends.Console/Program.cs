using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using KeyConvert.Convert;
using KeyConvert.Utils;

namespace KeyConvert.Frontends.Console
{
    class Program
    {
        private static string _musicFilesDirectoryPath;
        private static readonly FileLogger _log = new FileLogger();

        static void Main(string[] args)
        {
            // get directory from cmd line arg
            if (args.Any())
                _musicFilesDirectoryPath = string.IsNullOrWhiteSpace(args[0]) ? Environment.CurrentDirectory : args[0];
            else
                _musicFilesDirectoryPath = Environment.CurrentDirectory;

            // log beginning on Console & log
            var initialMsg = string.Format("Beginning conversion on all files in directory: {0}", _musicFilesDirectoryPath);
            System.Console.WriteLine(initialMsg);
            _log.Info(initialMsg);

            // check if directory exists
            if (!Directory.Exists(_musicFilesDirectoryPath))
            {
                _log.Error("Music directory given does not exist.");
                SaveLogToFile();

                System.Console.WriteLine("Music directory given does not exist. Press any key to exit.\n");
                System.Console.ReadKey();
                return;
            }
            
            // setup background worker and DoWork
            var converterBackgroundWorker = new BackgroundWorker{WorkerReportsProgress = true};
            converterBackgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs e)
            {
                try
                {
                    var converter = new Id3SharpKeyConverter(_log);
                    var converterResult = converter.ConvertFiles(_musicFilesDirectoryPath, KeyDictionaries.CamelotDictionary, converterBackgroundWorker);
                    System.Console.WriteLine("\n\nSuccessfully converted {0} of {1} files. See log for more details.",
                        converterResult.ConvertedFilesCount,
                        converterResult.TotalFilesCount);

                    e.Result = converterResult;
                }
                catch (Exception ex)
                {
                    // log error
                    _log.Error(string.Format("Exception caught running conversion. Message: {0}. Stacktrace: {1}", ex.Message, ex.StackTrace));

                    // display error on console
                    System.Console.WriteLine("Error running conversion. Exception message: {0}, Stacktrace: {1}", ex.Message, ex.StackTrace);
                }
            };
            converterBackgroundWorker.RunWorkerCompleted += delegate
            {
                // write last msg to console & log
                System.Console.WriteLine("Done converting files. Press any key to exit.");
                _log.Error("Done converting files. Press any key to exit...\n");

                // save log to file
                SaveLogToFile();

                // exit on keystroke
                System.Console.ReadKey();
            };

            // setup progress bar to use background worker
            var progressBar = new ConsoleProgressBar(70);
            converterBackgroundWorker.ProgressChanged += delegate(object sender, ProgressChangedEventArgs e)
            {
                progressBar.UpdateProgress(e.ProgressPercentage);
            };

            // run worker
            progressBar.InitConsoleProgress();

            try
            {
                converterBackgroundWorker.RunWorkerAsync(new object[] { _musicFilesDirectoryPath });
            }
            catch (InvalidOperationException ex)
            {
                // log error
                _log.Error(string.Format("Application error. Exception Message: {0}. Stacktrace: {1}", ex.Message, ex.StackTrace));

                // display error on console
                System.Console.WriteLine("Application error. Exception message: {0}, Stacktrace: {1}", ex.Message, ex.StackTrace);
            }
        }

        private static void SaveLogToFile()
        {
            try
            {
                var logFilename = string.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, "keyConverterLog.txt");
                _log.SaveToFile(logFilename);
                System.Console.WriteLine("Log written to {0}", logFilename);
            }
            catch (UnauthorizedAccessException)
            {
                string response;
                do
                {
                    System.Console.WriteLine(
                        "Error writing log file. Would you like to have log dumped to console? (Y/N)");
                    response = System.Console.ReadKey().Key.ToString();
                } while (response.ToUpper() != "Y" && response.ToUpper() != "N");

                if (response.ToUpper() == "Y")
                    System.Console.WriteLine(_log.GetFullLogText());
            }
        }   

    }
}
