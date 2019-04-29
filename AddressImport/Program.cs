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
        static readonly int maxThreads = 12;
        static readonly string fileStorage = @"D:\Users\Kevin\Downloads\openaddr\us";
        static int amountOfLines = 0;

        static void Main(string[] args)
        {
            Stopwatch t = new Stopwatch();
            t.Start();
            List<Batch> batches = new List<Batch>();

            // Get all files from directory
            FileReader reader = new FileReader(fileStorage);
            FileInfo[] files = reader.Read("*.csv", recursive: true);

            // Loop through all files
            List<FileInfo> batchList = new List<FileInfo>();
            foreach(FileInfo fi in files)
            {
                batchList.Add(fi);

                // Create batch if list got the right amount
                if(batchList.Count == (Math.Round((decimal)files.Length / maxThreads)))
                {
                    Batch batch = new Batch();
                    batch.Files = batchList;
                    // Add callback to oncomplete of file read
                    batch.OnComplete += new Batch.CompleteHandler(WriteStatus);
                    batch.OnComplete += new Batch.CompleteHandler(IncreaseLines);

                    batches.Add(batch);
                    // Empty list for next batch
                    batchList = new List<FileInfo>();
                }

                // Make sure we get the last couple of files
                if(files.Last() == fi)
                {
                    Batch batch = new Batch
                    {
                        Files = batchList
                    };

                    batches.Add(batch);
                }
            }

            // Let all batches run
            int i = 1;
            foreach(Batch batch in batches)
            {
                batch.Name = "Batch#" + i.ToString();
                Task.Run(async () => await batch.Run());
                i++;
            }
            Console.ReadKey();
            Console.WriteLine("Amount of lines: {0}", amountOfLines); // 14.4 mil expected
            Console.WriteLine(t.ElapsedMilliseconds / 1000);
            Console.ReadKey();
        }

        public static void IncreaseLines(object sender, FileReadEventArgs e)
        {
            amountOfLines += e.Lines;
        }

        public static void WriteStatus(object sender, FileReadEventArgs e)
        {
            Batch batch = (Batch)sender;
            Console.WriteLine("{0} : {1}/{2}", batch.Name, batch.Completed, batch.Amount);
        }
    }
}
