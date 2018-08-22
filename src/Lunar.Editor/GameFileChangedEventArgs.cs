using System;
using System.IO;

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
