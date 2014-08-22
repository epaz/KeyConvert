using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3Sharp
{
    public class KeyConverterResult
    {
        public KeyConverterResult(bool success, int totalFilesCount, int convertedFilesCount, string error = null)
        {
            TotalFilesCount = totalFilesCount;
            ConvertedFilesCount = convertedFilesCount;
            Success = success;
            Errors = string.IsNullOrWhiteSpace(error) ? new List<string>() : new List<string> { error };
        }

        public KeyConverterResult(bool success, int totalFilesCount, int convertedFilesCount, List<string> errors)
        {
            TotalFilesCount = totalFilesCount;
            ConvertedFilesCount = convertedFilesCount;
            Success = success;
            Errors = errors;
        }

        public int TotalFilesCount { get; set; }
        public int ConvertedFilesCount { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; private set; }
    }
}
