using System.Collections.Generic;
using System.IO;

namespace KeyConvert.Convert
{
    public interface IConverter
    {
        /// <summary>
        /// Converts key on one file using the passed key dictionary
        /// </summary>
        /// <param name="fileInfo">The FileInfo for the file whose key will be converted</param>
        /// <param name="keyDictionary">Dictionary containing mapping of old to new key values</param>
        /// <returns>KeyConverterResult which contains the number of files successfully converted and the total number of files processed.</returns>
        ConverterResult ConvertFile(FileInfo fileInfo, IDictionary<string, string> keyDictionary);
    }
}
