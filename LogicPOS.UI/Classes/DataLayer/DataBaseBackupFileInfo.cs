using Gtk;

namespace logicpos.Classes.DataLayer
{
    internal class DataBaseBackupFileInfo
    {
        public string FileName { get; set; } = string.Empty;

        public string FileNamePacked { get; set; } = string.Empty;
        private string _fileHashDB = string.Empty;
        public string FileHashDB
        {
            get { return _fileHashDB; }
            set { _fileHashDB = value; FileHashValid = (_fileHashDB == _fileHashFile); }
        }
        private string _fileHashFile = string.Empty;
        public string FileHashFile
        {
            get { return _fileHashFile; }
            set { _fileHashFile = value; FileHashValid = (_fileHashDB == _fileHashFile); }
        }

        public bool FileHashValid { get; set; } = false;

        public ResponseType Response { get; set; } = ResponseType.Cancel;

        public DataBaseBackupFileInfo() { }
        public DataBaseBackupFileInfo(string pFileName, string pFileNamePacked, string pFileHashDB, string pFileHashFile, bool pFileHashValid)
        {
            FileName = pFileName;
            FileNamePacked = pFileNamePacked;
            _fileHashDB = pFileHashDB;
            _fileHashFile = pFileHashFile;
            FileHashValid = pFileHashValid;
        }
    }
}
