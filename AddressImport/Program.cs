using Badger;
using FolderReader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AddressImport
{
    class Program
    {
        private static readonly int maxThreads = 12;
        static readonly string fileStorage = @"D:\Users\Kevin\Downloads\openaddr\us";
        static int amountOfLines = 0;

        private static Stopwatch stopwatch;

        static void Main(string[] args)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
            List<FileBatch> batches = new List<FileBatch>();

            // Get all files from directory
            FileReader reader = new FileReader(fileStorage);
            FileInfo[] files = reader.Read("*.csv", recursive: true);

            BatchFactory<FileBatch> factory = new BatchFactory<FileBatch>(maxThreads, new List<object>(files));
            factory.SubscribeCompleted(WriteStatus);
            factory.SubscribeCompleted(IncreaseLines);

            factory.OnDone += new BatchFactory<FileBatch>.OnDoneHandler(OnBatchDone);

            factory.Start();
            Console.ReadKey();
        }

        public static void OnBatchDone(object sender, EventArgs e)
        {
            stopwatch.Stop();
            Console.WriteLine("Amount of lines: {0}", amountOfLines); // 14.5 mil expected
            Console.WriteLine(stopwatch.ElapsedMilliseconds / 1000); // Good as long as its under 3 min
        }

        public static void IncreaseLines(object sender, BatchCompletedEventArgs e)
        {
            amountOfLines += (int)e.Result;
        }

        public static void WriteStatus(object sender, BatchCompletedEventArgs e)
        {
            FileBatch batch = (FileBatch)sender;
            Console.WriteLine("{0} : {1}/{2}", batch.Name, batch.Completed, batch.Amount);
        }
    }
}
