using Badger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddressImport
{
    public class FileBatch : Batch
    {
        public override async Task<object> BatchSingle(object obj)
        {
            FileInfo item = (FileInfo)obj;

            // Initial
            int amountOfLines = 0;

            // Check if file exists
            if (!item.Exists)
                return await Task.FromResult(0);

            // Open file
            using (FileStream fs = item.OpenRead())
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);

                // Read file
                while (fs.Read(b, 0, b.Length) > 0)
                {
                    amountOfLines++;
                }
            }

            // End task
            return await Task.FromResult(amountOfLines);
        }
    }
}
