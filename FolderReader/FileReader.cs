using System;
using System.IO;

namespace FolderReader
{
    public class FileReader
    {
        protected string initialDirectory;

        public FileReader(string initial)
        {
            initialDirectory = initial;
        }

        public FileInfo[] Read(string searchPattern, string subDirectory = null, bool recursive = false)
        {
            DirectoryInfo di = new DirectoryInfo(initialDirectory);

            FileInfo[] files;
            if (!recursive)
                files = di.GetFiles(searchPattern: searchPattern);
            else
                files = di.GetFiles(searchPattern: searchPattern, searchOption: SearchOption.AllDirectories);

            return files;
        }
    }
}
