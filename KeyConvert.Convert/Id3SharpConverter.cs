using System;
using System.Collections.Generic;
using System.IO;
using ID3Sharp.Models;
using KeyConvert.Utils;

namespace KeyConvert.Convert
{
    public class Id3SharpKeyConverter : IConverter
    {
        const int _progressBarLength = 70; // default console window width is 80 characters
        private IDictionary<string, string> _keyDictionary;

        /// <summary>
        /// Converts key on files in _musicFilesDirectoryPath to Camelot notation
        /// </summary>
        /// <param name="directoryPath">Path of folder containing files whose keys will be converted</param>
        /// <param name="outputToConsole">True if status should be output to console</param>
        /// <param name="keyDictionary">Dictionary containing mapping of old to new key values</param>
        /// <param name="log">Logger to use to log messages.</param>
        /// <returns>KeyConverterResult which contains the number of files successfully converted and the total number of files processed.</returns>
        public ConverterResult ConvertFiles(string directoryPath, bool outputToConsole, IDictionary<String, String> keyDictionary, Logger log)
        {
            _keyDictionary = keyDictionary;

            // first check if directory exists
            if (!Directory.Exists(directoryPath))
            {
                return new ConverterResult(false, 0, 0, "Music directory given does not exist.");
            }

            // Get all mp3 files
            var files = new List<string>(Directory.GetFiles(directoryPath, "*.mp3",
                    SearchOption.TopDirectoryOnly));

            // TODO fix ID3 tags for AIFF files
//            files.AddRange(Directory.GetFiles(musicFilesDirectoryPath, "*.aiff",
//                SearchOption.TopDirectoryOnly));

            // check if any files were found
            if (files.Count == 0)
            {
                var noFilesWarning = string.Format("No files with .mp3 or .aiff extensions found in directory {0}", directoryPath);
                log.Warn(noFilesWarning);
                return new ConverterResult(false, 0, 0, noFilesWarning);
            }

            log.Info(string.Format("Found {0} files to convert.", files.Count));

            // initialize console progress bar
            if (outputToConsole) InitConsoleProgress();

            // iterate through all files found
            int successfullFilesCount = 0;
            for (int index = 0; index < files.Count; index++)
            {
                string filePath = files[index];

                // update progress bar
                if(outputToConsole) UpdateProgress(index + 1, files.Count);
               
                // try to do conversion of key
                try
                {
                    var success = ConvertKeyOnFile(filePath, log);
                    
                    if (success) successfullFilesCount++;
                }
                catch (Exception e)
                {
                    var conversionError = 
                        string.Format("Exception thrown converting key on {0}. Exception message: {1}. Stacktrace: {2}", filePath, e.Message, e.StackTrace);
                    log.Error(conversionError);
                    return new ConverterResult(false, files.Count, successfullFilesCount, conversionError);
                }
            }

            log.Info(string.Format("Done! Successfully converted {0} of {1} files.", files.Count, successfullFilesCount));

            if (outputToConsole)
                Console.WriteLine(
                    "\n\nSuccessfully converted {0} of {1} files. See log for more details.",
                    successfullFilesCount, files.Count);

            return new ConverterResult(true, files.Count, successfullFilesCount);

        }

        private static void UpdateProgress(int filesProcessed, int totalFiles)
        {
            // update progress bar
            double progressPercentage = filesProcessed / (double)totalFiles;
            var progressBarToFill = (int)(progressPercentage * _progressBarLength);

            Console.Write("\r|");
            for (int i = 0; i < _progressBarLength; i++)
            {
                Console.Write(i < progressBarToFill ? "-" : " ");
            }
            Console.Write("| ({0:0%})", progressPercentage);

        }

        private static void InitConsoleProgress()
        {
            // initialize progress bar
            Console.CursorVisible = false;
            Console.Write("|");

            
            for (int i = 0; i < _progressBarLength; i++)
            {
                Console.Write(" ");
            }
            Console.Write("| 0%");
        }

        private bool ConvertKeyOnFile(string filePath, Logger log)
        {
            string filename = Path.GetFileName(filePath);
            log.Info(string.Format("Reading file {0}", filename));
            

            // check if file has ID3 tag
            if (!ID3v2Tag.HasTag(filePath))
            {
                log.Warn(string.Format("No ID3 tag found. Skipping file {0}", filename));
                return false;
            }

            ID3v2Tag tag = ID3v2Tag.ReadTag(filePath);

            // check if file has a Key
            if (!tag.HasKey || string.IsNullOrEmpty(tag.Key) ||
                tag.Key.Trim() == string.Empty)
            {
                log.Warn(string.Format("No key found. Skipping {0}", filePath));
                tag.WriteTag(filePath, ID3Versions.V2_3);
                return false;
            }

            // check if we have a new key value for the file's original key 
            if (!_keyDictionary.ContainsKey(tag.Key))
            {
                log.Warn(string.Format("No replacement key found for orginal key \"{0}\" for file {1}. Skipping file.",
                    tag.Key, filename));
                tag.WriteTag(filePath, ID3Versions.V2_3);
                return false;
            }

            // get original key, new camelot key and comment so we can save original key
            var originalKey = tag.Key;
            var camelotKey = _keyDictionary[originalKey];
            var orginalComment = tag.Comments;

            // store original key in comments
            tag.Comments = string.Format("{0} ({1})", orginalComment, originalKey);

            // save new camelot key value
            tag.Key = camelotKey;

            // save tag
            log.Info(string.Format("Writing tag to {0}", filename));
            tag.WriteTag(filePath, ID3Versions.V2_3);

            return true;
        }
    }

    
}
