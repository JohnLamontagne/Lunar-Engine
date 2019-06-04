using System;
using System.IO;

namespace Lunar.Editor
{
    public class FileEventArgs : EventArgs
    {
        private readonly FileInfo _file;

        public FileInfo File => _file;

        public FileEventArgs(FileInfo file)
        {
            _file = file;
        }
    }

}