using System;
using System.Threading;
using System.IO;

namespace FileWatcher
{
    public class FileWatcher
    {
        public delegate void FileWatcherEventHandler(object sender, FileWatcherEventHandlerArgs args);

        public event FileWatcherEventHandler FileChanged;

        private readonly Thread _thread;

        private bool _isStop;

        public string FilePath { get; }        

        public FileWatcher(string filePath)
        {
            FilePath = filePath;
            _thread = new Thread(Watch);
            _isStop = false;
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Stop()
        {
            _isStop = true;
        }


        private void Watch()
        {
            DateTime lastWriteTime = DateTime.MinValue;
            do
            {
                if (!File.Exists(FilePath))
                {
                    continue;
                }

                DateTime lastWriteTimeToCheck = File.GetLastWriteTime(FilePath);

                if (lastWriteTime != lastWriteTimeToCheck)
                {
                    string content;
                    try
                    {
                        content = File.ReadAllText(FilePath);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    FileChanged?.Invoke(null, new FileWatcherEventHandlerArgs(content, lastWriteTimeToCheck));
                    lastWriteTime = lastWriteTimeToCheck;
                }
            } while (!_isStop);
        }
    }
}