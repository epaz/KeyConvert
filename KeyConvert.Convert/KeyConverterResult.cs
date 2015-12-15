using System.Collections.Generic;

namespace KeyConvert.Convert
{
    public class ConverterResult
    {
        public ConverterResult(bool success)
        {
            Success = success;
            TotalFilesCount = 0;
            ConvertedFilesCount = 0;
            Errors = new List<string>();
            Infos = new List<string>();
        }
        
        public ConverterResult(bool success, string error = null, string info = null)
        {
            Success = success;
            TotalFilesCount = 0;
            ConvertedFilesCount = 0;
            Errors = string.IsNullOrWhiteSpace(error) ? new List<string>() : new List<string> { error };
            Infos = string.IsNullOrWhiteSpace(info) ? new List<string>() : new List<string> { info };
        }

        public ConverterResult(bool success, int totalFilesCount, int convertedFilesCount, string error = null, string info = null)
        {
            TotalFilesCount = totalFilesCount;
            ConvertedFilesCount = convertedFilesCount;
            Success = success;
            Errors = string.IsNullOrWhiteSpace(error) ? new List<string>() : new List<string> { error };
            Infos = string.IsNullOrWhiteSpace(info) ? new List<string>() : new List<string> { info };
        }

        public ConverterResult(bool success, int totalFilesCount, int convertedFilesCount, List<string> errors, List<string> infos )
        {
            TotalFilesCount = totalFilesCount;
            ConvertedFilesCount = convertedFilesCount;
            Success = success;
            Errors = errors;
            Infos = infos;
        }

        public int TotalFilesCount { get; set; }
        public int ConvertedFilesCount { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; private set; }
        public List<string> Infos { get; private set; } 
    }
}
