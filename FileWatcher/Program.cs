using System;
using System.IO;
using System.Threading;

namespace FileWatcher
{
    class Program
    {
        static readonly string _filePath = @"test.txt";
        static readonly AutoResetEvent FileChangeTo1ResetEvent = new AutoResetEvent(false);
        static readonly AutoResetEvent FileChangeTo0ResetEvent = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            FileWatcher fw = new FileWatcher(_filePath);
            fw.FileChanged += FwOnChanged;
            fw.Start();

            File.WriteAllText(_filePath, "1");

            Thread t = new Thread(DoWork);
            t.Start();

            string enteredLine;
            do
            {
                FileChangeTo0ResetEvent.WaitOne();
                Console.WriteLine("Do you want to change file content to '1'? Press 'Y' to change or 'Q' to exit.");
                enteredLine = Console.ReadLine();
                if (enteredLine == "Y" || enteredLine == "y")
                {
                    while (true)
                    {
                        try
                        {
                            File.WriteAllText(_filePath, "1");
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        break;
                    }
                }

                if (enteredLine == "Q" || enteredLine == "q")
                {
                    t.Abort();
                    fw.Stop();
                    break;
                }

            } while (true);
        }

        private static void DoWork()
        {
            while (true)
            {
                if (!File.Exists(_filePath))
                {
                    continue;
                }

                FileChangeTo1ResetEvent.WaitOne();

                try
                {
                    string content = File.ReadAllText(_filePath);
                    if (content == "1")
                    {
                        File.WriteAllText(_filePath, "0");
                        Thread.Sleep(10000);
                        Console.WriteLine("File content changed to 0 ten seconds ago");
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        private static void FwOnChanged(object sender, FileWatcherEventHandlerArgs args)
        {
            Console.WriteLine($"File content: {args.Content}");
            Console.WriteLine($"File time of change: {args.ChangeTime}");
            if (args.Content == "1")
            {
                FileChangeTo1ResetEvent.Set();
            }
            else if(args.Content == "0")
            {
                FileChangeTo0ResetEvent.Set();
            }
        }
    }
}
