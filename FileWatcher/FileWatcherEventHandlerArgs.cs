using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher
{
    public class FileWatcherEventHandlerArgs
    {
        public string Content { get; private set; }

        public DateTime ChangeTime { get; private set; }

        public FileWatcherEventHandlerArgs(string content, DateTime changeTime)
        {
            Content = content;
            ChangeTime = changeTime;
        }
    }
}
