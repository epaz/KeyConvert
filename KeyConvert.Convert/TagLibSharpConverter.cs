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
        public static readonly string[] SupportedExtensions = {".mp3", ".aiff"};
        private readonly ILogger _log;
        
        public TagLibSharpConverter(ILogger log)
        {
            _log = log;
        }

        public ConverterResult ConvertFile(FileInfo fileInfo, IDictionary<string, string> keyDictionary)
        {
            try
            {
                var file = TagLib.File.Create(fileInfo.FullName);

                if (string.IsNullOrWhiteSpace(file.Tag.Key) || !keyDictionary.ContainsKey(file.Tag.Key))
                {
                    _log.Info(string.Format("No supported key found on {0}. Skipping file.", fileInfo.Name));
                    return new ConverterResult(false);
                }

                file.Tag.Key = keyDictionary[file.Tag.Key];
                file.Save();

                return new ConverterResult(true);
            }
            catch (Exception e)
            {
                var error = string.Format("Error converting key on {0}. Exception message: {1}", fileInfo.Name, e.Message);
                _log.Error(error);
                return new ConverterResult(false, 0, 0, error);
            }
        }
    }
}
