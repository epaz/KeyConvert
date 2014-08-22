using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using KeyConvert.Utils;

namespace FrontendWpf
{
    public class WpfLogger : Logger
    {
        private readonly TextBlock _logTextBlock;

        public WpfLogger(ref TextBlock logTextBlock)
        {
            if(logTextBlock == null)
                throw new ArgumentNullException("logTextBlock");
         
            _logTextBlock = logTextBlock;
        }

        public override void Log(Level level, string message)
        {
            switch (level)
            {
                case Level.Error:
                    _logTextBlock.Inlines.Add(new Run(message) {Foreground = Brushes.Red});
                    break;
                case Level.Warn:
                    _logTextBlock.Inlines.Add(new Run(message) {Foreground = Brushes.DarkOrange});
                    break;
                case Level.Info:
                    _logTextBlock.Inlines.Add(message);
                    break;
            }
            _logTextBlock.Inlines.Add(new LineBreak());
            
            base.Log(level, message);
        }
    }
}
