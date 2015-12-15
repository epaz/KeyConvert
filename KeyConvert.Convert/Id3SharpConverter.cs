using System;
using System.Collections.Generic;
using System.IO;
using ID3Sharp.Models;
using KeyConvert.Utils;

namespace KeyConvert.Convert
{
    public class Id3SharpKeyConverter : IConverter
    {
        private readonly ILogger _log;

        /// <summary>
        /// Constructs a Converter to convert mp3 files' keys using the ID3Sharp Library.
        /// </summary>
        /// <param name="log">Logger to use to log messages.</param>
        public Id3SharpKeyConverter(ILogger log)
        {
            _log = log;
        }

        public ConverterResult ConvertFile(FileInfo fileInfo, IDictionary<string, string> keyDictionary)
        {
            // check if file has ID3 tag
            if (!ID3v2Tag.HasTag(fileInfo.FullName))
            {
                _log.Warn(string.Format("No ID3 tag found. Skipping file {0}", fileInfo.Name));
                return new ConverterResult(false);
            }

            var tag = ID3v2Tag.ReadTag(fileInfo.FullName); // exceptions thrown here should bubble up

            try
            {
                // check if file has a Key
                if (!tag.HasKey || string.IsNullOrEmpty(tag.Key) ||
                    tag.Key.Trim() == string.Empty)
                {
                    _log.Warn(string.Format("No key found. Skipping {0}", fileInfo.Name));
                    tag.WriteTag(fileInfo.FullName, ID3Versions.V2_3); // done so that at least existing attributes are written using v2.3
                    return new ConverterResult(false);
                }

                // check if we have a new key value for the file's original key 
                if (!keyDictionary.ContainsKey(tag.Key))
                {
                    _log.Warn(string.Format("No replacement key found for orginal key \"{0}\" for file {1}. Skipping file.",
                        tag.Key, fileInfo.Name));

                    tag.WriteTag(fileInfo.FullName, ID3Versions.V2_3); // done so that at least existing attributes are written using v2.3
                    return new ConverterResult(false);
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
                _log.Info(string.Format("Writing tag to {0}", fileInfo.Name));
                tag.WriteTag(fileInfo.FullName, ID3Versions.V2_3);

                return new ConverterResult(true);
            }
            catch (Exception ex)
            {
                return new ConverterResult(false, 
                    string.Format("Exception thrown while converting file on {0}. Exception Message: {1}, Stacktrace: {2}", 
                        fileInfo.Name, ex.Message, ex.StackTrace));
            }
        }
    }
}
