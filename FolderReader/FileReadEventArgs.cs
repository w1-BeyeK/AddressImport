using System;
using System.Collections.Generic;
using System.Text;

namespace FolderReader
{
    public class FileReadEventArgs : EventArgs
    {
        public int Lines { get; set; }
    }
}
