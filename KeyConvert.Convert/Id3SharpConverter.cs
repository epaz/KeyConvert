using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using ID3Sharp.Models;
using KeyConvert.Utils;

namespace KeyConvert.Convert
{
    public class Id3SharpKeyConverter : IConverter
    {
        //private readonly IDictionary<string, string> _keyDictionary;
        private readonly ILogger _log;

        /// <summary>
        /// Constructs a Converter to convert mp3 files' keys using the ID3Sharp Library.
        /// </summary>
        /// <param name="log">Logger to use to log messages.</param>
        public Id3SharpKeyConverter(ILogger log)
        {
            _log = log;
        }

        /// <summary>
        /// Converts key on files in _musicFilesDirectoryPath to Camelot notation
        /// </summary>
        /// <param name="directoryPath">Path of folder containing files whose keys will be converted</param>
        /// <param name="backgroundWorker">The BackgroundWorker used to track progress</param>
        /// <returns>KeyConverterResult which contains the number of files successfully converted and the total number of files processed.</returns>
        public ConverterResult ConvertFiles(string directoryPath, IDictionary<String, String> keyDictionary, BackgroundWorker backgroundWorker)
        {
            try
            {
                // Get all mp3 files
                var files = new List<string>(Directory.GetFiles(directoryPath, "*.mp3",
                        SearchOption.TopDirectoryOnly));

                // TODO fix ID3 tags for AIFF files
                //files.AddRange(Directory.GetFiles(musicFilesDirectoryPath, "*.aiff",
                //    SearchOption.TopDirectoryOnly));

                // check if any files were found
                if (files.Count == 0)
                {
                    var noFilesWarning = string.Format("No files with .mp3 or .aiff extensions found in directory {0}", directoryPath);
                    _log.Warn(noFilesWarning);
                    return new ConverterResult(false, 0, 0, noFilesWarning);
                }

                _log.Info(string.Format("Found {0} files to convert.", files.Count));

                // iterate through all files found
                var successfullFilesCount = 0;
                for (var index = 0; index < files.Count; index++)
                {
                    var filePath = files[index];

                    // try to do conversion of key
                    try
                    {
                        var success = ConvertKeyOnFile(filePath, keyDictionary);
                        if (success) successfullFilesCount++;
                    }
                    catch (Exception ex)
                    {
                        var conversionError =
                            string.Format("Exception thrown converting key on {0}. Exception message: {1}. Stacktrace: {2}", filePath, ex.Message, ex.StackTrace);
                        _log.Error(conversionError);
                        return new ConverterResult(false, files.Count, successfullFilesCount, conversionError);
                    }

                    // report progress
                    var percentProgress = (int)(((index + 1) / (double)files.Count) * 100);
                    backgroundWorker.ReportProgress(percentProgress);
                }

                _log.Info(string.Format("Done! Successfully converted {0} of {1} files.", files.Count, successfullFilesCount));

                return new ConverterResult(true, files.Count, successfullFilesCount);
            }
            catch (Exception ex)
            {
                throw;
            }

            
        }

        private bool ConvertKeyOnFile(string filePath, IDictionary<String, String> keyDictionary)
        {
            var filename = Path.GetFileName(filePath);
            _log.Info(string.Format("Reading file {0}", filename));

            // check if file has ID3 tag
            if (!ID3v2Tag.HasTag(filePath))
            {
                _log.Warn(string.Format("No ID3 tag found. Skipping file {0}", filename));
                return false;
            }

            var tag = ID3v2Tag.ReadTag(filePath);

            // check if file has a Key
            if (!tag.HasKey || string.IsNullOrEmpty(tag.Key) ||
                tag.Key.Trim() == string.Empty)
            {
                _log.Warn(string.Format("No key found. Skipping {0}", filePath));
                tag.WriteTag(filePath, ID3Versions.V2_3);
                return false;
            }

            // check if we have a new key value for the file's original key 
            if (!keyDictionary.ContainsKey(tag.Key))
            {
                _log.Warn(string.Format("No replacement key found for orginal key \"{0}\" for file {1}. Skipping file.",
                    tag.Key, filename));
                tag.WriteTag(filePath, ID3Versions.V2_3);
                return false;
            }

            // get original key, new camelot key and comment so we can save original key
            var originalKey = tag.Key;
            var camelotKey = keyDictionary[originalKey];
            var orginalComment = tag.Comments;

            // store original key in comments
            tag.Comments = string.Format("{0} ({1})", orginalComment, originalKey);

            // save new camelot key value
            tag.Key = camelotKey;

            // save tag
            _log.Info(string.Format("Writing tag to {0}", filename));
            tag.WriteTag(filePath, ID3Versions.V2_3);

            return true;
        }
    }
}
