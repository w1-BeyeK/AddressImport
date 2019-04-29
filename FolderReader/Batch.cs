using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FolderReader
{
    /// <summary>
    /// Batch class
    /// </summary>
    public class Batch
    {
        public string Name { get; set; }
        public int Amount
        {
            get
            {
                return Files.Count;
            }
        }
        public List<FileInfo> Files { get; set; }
        public int Completed { get; protected set; }

        /// <summary>
        /// Main method - run the batch
        /// </summary>
        /// <returns></returns>
        public async Task Run()
        {
            foreach (FileInfo fi in Files)
            {
                // Await all file reads
                await ReadSingle(fi);
            }
        }

        /// <summary>
        /// On complete of file read
        /// </summary>
        public event CompleteHandler OnComplete;
        /// <summary>
        /// On complete of file read handler
        /// </summary>
        /// <param name="sender">Sender of event</param>
        /// <param name="e">Event arguments</param>
        public delegate void CompleteHandler(object sender, FileReadEventArgs frea = null);

        /// <summary>
        /// Read single file of batch
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public async Task ReadSingle(FileInfo fileInfo)
        {
            // Initial
            int amountOfLines = 0;

            // Check if file exists
            if (!fileInfo.Exists)
                return;

            // Open file
            using (FileStream fs = fileInfo.OpenRead())
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);

                // Read file
                while (fs.Read(b, 0, b.Length) > 0)
                {
                    amountOfLines++;
                }
            }
            // Check completion
            Completed++;
            FileReadEventArgs frea = new FileReadEventArgs()
            {
                Lines = amountOfLines
            };
            OnComplete?.Invoke(this, frea);

            // End task
            await Task.FromResult<object>(null);
        }
    }
}
