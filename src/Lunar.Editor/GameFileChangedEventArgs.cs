using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.Editor
{
    public class GameFileChangedEventArgs : EventArgs
    {
        public FileInfo OldFile { get; }
        public FileInfo NewFile { get; }

        public GameFileChangedEventArgs(FileInfo oldFile, FileInfo newFile)
        {
            this.OldFile = oldFile;
            this.NewFile = newFile;
        }
    }
}
