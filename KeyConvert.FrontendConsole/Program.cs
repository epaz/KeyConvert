using System;
using System.Linq;
using KeyConvert.Utils;
using KeyConvert;

namespace KeyConvert.FrontendConsole
{
    class Program
    {
        private static string _musicFilesDirectoryPath;
        private static readonly Logger _log = new Logger();

        static void Main(string[] args)
        {
            // get directory from cmd line arg
            if (args.Any())
            {
                _musicFilesDirectoryPath = string.IsNullOrWhiteSpace(args[0]) ? @"C:\KeyConvert" : args[0];
            }

            // log beginning on Console & log
            var initialMsg = string.Format("Beginning conversion on all files in directory: {0}", _musicFilesDirectoryPath);
            Console.WriteLine(initialMsg);
            _log.Info(initialMsg);

            // run converter
            try
            {
                ID3KeyConverter.ConvertFiles(_musicFilesDirectoryPath, true, _log);
                Console.Write("\n");
            }
            catch (Exception e)
            {
                // log error
                _log.Error(string.Format("Exception caught running conversion. Message: {0}. Stacktrace: {1}", e.Message, e.StackTrace));

                // display error on console
                Console.WriteLine("Error running conversion. Exception message: {0}, Stacktrace: {1}", e.Message, e.StackTrace);
            }

            // write last msg to console & log
            Console.WriteLine("Done converting files. Press any key to exit.");
            _log.Error("Done converting files. Exiting...");

            // save log to file
            try
            {
                var logFilename = string.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, "keyConverterLog.txt");
                _log.SaveToFile(logFilename);
                Console.WriteLine("Log written to {0}", logFilename);
            }
            catch (UnauthorizedAccessException unauthException)
            {
                string response;
                do
                {
                    Console.WriteLine("Error writing log file. Would you like to have log dumped to console? (Y/N)");
                    response = Console.ReadKey().Key.ToString();
                } while (response.ToUpper() != "Y" && response.ToUpper() != "N");

                if (response.ToUpper() == "Y")
                    Console.WriteLine(_log.GetFullLogText());
            }

            // exit on keystroke
            Console.ReadKey();
        }
    }
}
