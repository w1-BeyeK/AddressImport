using System;
using System.Collections.Generic;
using System.Text;

namespace Badger
{
    public class BatchCompletedEventArgs : EventArgs
    {
        public object Result { get; set; }
    }
}
