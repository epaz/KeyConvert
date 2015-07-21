using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using KeyConvert.Utils;

namespace KeyConvert.Convert
{
    public class TagLibSharpConverter : IConverter
    {
        private readonly string[] _supportedExtensions = {"mp3", "aiff"};
        private ILogger _log;
        
        public TagLibSharpConverter(ILogger log)
        {
            _log = log;
        }

        public ConverterResult ConvertFiles(string directoryPath, IDictionary<string, string> keyDictionary, BackgroundWorker backgroundWorker)
        {
            var dirInfo = new DirectoryInfo(directoryPath);
            var files = dirInfo.EnumerateFiles().Where(f => _supportedExtensions.Contains(f.Extension)).ToList();

            // check if any files were found
            if (files.Count == 0)
            {
                var noFilesWarning = string.Format("No supported files found in directory {0}", directoryPath);
                _log.Warn(noFilesWarning);
                return new ConverterResult(false, 0, 0, noFilesWarning);
            }

            _log.Info(string.Format("Found {0} files to convert.", files.Count));


            // iterate through all files found
            for (var i = 0; i < files.Count; i++)
            {
                var tagFile = TagLib.File.Create(files[i].FullName);

                tagFile.Tag.
            }


            return null; //todo
        }
    }
}
