using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyConvert.Utils;

namespace KeyConvert.Convert
{
    public interface IConverter
    {
        /// <summary>
        /// Converts key on all files in files directory
        /// </summary>
        /// <param name="directoryPath">Path of folder containing files whose keys will be converted</param>
        /// <param name="keyDictionary">Dictionary containing mapping of old to new key values</param>
        /// <param name="backgroundWorker"></param>
        /// <returns>KeyConverterResult which contains the number of files successfully converted and the total number of files processed.</returns>
        ConverterResult ConvertFiles(string directoryPath,  IDictionary<String, String> keyDictionary, BackgroundWorker backgroundWorker);
    }
}
