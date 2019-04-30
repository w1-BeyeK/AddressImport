using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Badger
{
    public class BatchFactory<T> where T : Batch, new()
    {
        protected int _maxBatches;
        protected int _completed = 0;

        public List<T> Batches { get; set; }

        public void SubscribeCompleted(Action<object, BatchCompletedEventArgs> func)
        {
            for(int i = 0; i < Batches.Count; i++)
            {
                int index = i;
                Batches[index].OnProcessComplete += new Batch.ProcessCompleteHandler(func);
            }
        }

        public void Start()
        {
            for(int i = 0; i < Batches.Count; i++)
            {
                int index = i;
                Task.Run(() => Batches[index].Run());
                System.Threading.Thread.Sleep(100);
            }
        }

        public BatchFactory(int maxBatches, List<object> items)
        {
            Batches = new List<T>();
            _maxBatches = maxBatches;

            List<object> batchItems = new List<object>();
            for(int i = 0; i < items.Count; i++)
            {
                batchItems.Add(items[i]);

                // Create batch if list got the right amount
                if (batchItems.Count == (Math.Round((decimal)items.Count / _maxBatches)))
                {
                    T batch = new T
                    {
                        Items = batchItems
                    };
                    batch.Name = $"Batch#{Batches.Count + 1}";
                    batch.OnBatchComplete += CheckBatchProgress;


                    Batches.Add(batch);
                    // Empty list for next batch
                    batchItems = new List<object>();
                }

                // Make sure we get the last couple of files
                if (items.Last().Equals(items[i]))
                {
                    T batch = new T
                    {
                        Items = batchItems
                    };
                    batch.Name = $"Batch#{Batches.Count + 1}";
                    batch.OnBatchComplete += CheckBatchProgress;
                    Batches.Add(batch);
                }
            }
        }

        public void CheckBatchProgress(object sender, BatchCompletedEventArgs frea)
        {
            _completed++;

            if (_completed == Batches.Count)
                OnDone(this, new EventArgs());
        }

        public delegate void OnDoneHandler(object sender, EventArgs e);
        public event OnDoneHandler OnDone;

    }
}
