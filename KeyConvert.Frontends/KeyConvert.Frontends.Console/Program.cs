using System;
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
            {
                _musicFilesDirectoryPath = string.IsNullOrWhiteSpace(args[0]) ? Environment.CurrentDirectory : args[0];
            }
            else
            {
                _musicFilesDirectoryPath = Environment.CurrentDirectory;
            }

            // log beginning on Console & log
            var initialMsg = string.Format("Beginning conversion on all files in directory: {0}", _musicFilesDirectoryPath);
            System.Console.WriteLine(initialMsg);
            _log.Info(initialMsg);

            // run converter
            try
            {
                var converter = new Id3SharpKeyConverter();
                
                // first check if directory exists
                if (!Directory.Exists(_musicFilesDirectoryPath))
                {
                    _log.Error("Music directory given does not exist.");
                    System.Console.WriteLine("Music directory given does not exist.");
                }
                else
                {
                    converter.ConvertFiles(_musicFilesDirectoryPath, true, KeyDictionaries.CamelotDictionary, _log);
                }

                System.Console.Write("\n");
            }
            catch (Exception e)
            {
                // log error
                _log.Error(string.Format("Exception caught running conversion. Message: {0}. Stacktrace: {1}", e.Message, e.StackTrace));

                // display error on console
                System.Console.WriteLine("Error running conversion. Exception message: {0}, Stacktrace: {1}", e.Message, e.StackTrace);
            }

            // write last msg to console & log
            System.Console.WriteLine("Done converting files. Press any key to exit.");
            _log.Error("Done converting files. Exiting...");

            // save log to file
            try
            {
                var logFilename = string.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, "keyConverterLog.txt");
                _log.SaveToFile(logFilename);
                System.Console.WriteLine("Log written to {0}", logFilename);
            }
            catch (UnauthorizedAccessException unauthException)
            {
                string response;
                do
                {
                    System.Console.WriteLine("Error writing log file. Would you like to have log dumped to console? (Y/N)");
                    response = System.Console.ReadKey().Key.ToString();
                } while (response.ToUpper() != "Y" && response.ToUpper() != "N");

                if (response.ToUpper() == "Y")
                    System.Console.WriteLine(_log.GetFullLogText());
            }

            // exit on keystroke
            System.Console.ReadKey();
        }
    }
}
