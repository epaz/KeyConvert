using System;

namespace KeyConvert.Utils
{
    public class ConsoleProgressBar
    {
        private readonly int _progressBarLength;

        public ConsoleProgressBar(int progressBarLength)
        {
            _progressBarLength = progressBarLength;
        }

        /// <summary>
        /// Updates progress bar based on the percentage done.
        /// </summary>
        /// <param name="percentProgress">The percentage, from 0 to 100, of the background operation that is complete.</param>
        public void UpdateProgress(int percentProgress)
        {
            // update progress bar
            var progressBarToFill = Convert.ToInt32(((decimal)percentProgress/100) * _progressBarLength);

            Console.Write("\r|");
            for (var i = 0; i < _progressBarLength; i++)
            {
                Console.Write(i < progressBarToFill ? "-" : " ");
            }
            Console.Write("| ({0}%)", percentProgress);
        }

        /// <summary>
        /// Initialize progress bar
        /// </summary>
        public void InitConsoleProgress()
        {
            Console.CursorVisible = false;
            Console.Write("|");
            
            for (var i = 0; i < _progressBarLength; i++)
            {
                Console.Write(" ");
            }
            Console.Write("| 0%");
        }
    }
}
