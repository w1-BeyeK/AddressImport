using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Badger
{
    /// <summary>
    /// Batch class
    /// </summary>
    public abstract class Batch
    {
        public string Name { get; set; }
        public int Amount
        {
            get
            {
                return Items.Count;
            }
        }

        public List<object> Items { get; set; }
        
        public int Completed { get; protected set; }

        /// <summary>
        /// Main method - run the batch
        /// </summary>
        /// <returns></returns>
        public async Task Run()
        {
            foreach (object item in Items)
            {
                // Await all file reads
                object result = await BatchSingle(item);

                // Check completion
                Completed++;
                
                BatchCompletedEventArgs frea = new BatchCompletedEventArgs()
                {
                    Result = result
                };
                OnProcessComplete?.Invoke(this, frea);

                if (Completed == Amount)
                {
                    frea = new BatchCompletedEventArgs();
                    OnBatchComplete?.Invoke(this, frea);
                }

            }
        }

        public abstract Task<object> BatchSingle(object item);

        /// <summary>
        /// On complete of file read
        /// </summary>
        public event ProcessCompleteHandler OnProcessComplete;
        /// <summary>
        /// On complete of file read handler
        /// </summary>
        /// <param name="sender">Sender of event</param>
        /// <param name="e">Event arguments</param>
        public delegate void ProcessCompleteHandler(object sender, BatchCompletedEventArgs frea);

        public event BatchCompleteHandler OnBatchComplete;
        public delegate void BatchCompleteHandler(object sender, BatchCompletedEventArgs frea);
    }
}
