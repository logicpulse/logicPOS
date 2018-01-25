using CryptographyUtils;
using logicpos.shared.App;
using System;

namespace logicpos.financial.library.Classes.Utils
{
    public class ProtectedFile
    {
        private string _md5 = String.Empty;
        public string Md5
        {
            get { return _md5; }
            set { _md5 = value; }
        }

        private string _md5Salted = String.Empty;
        public string Md5Encrypted
        {
            get { return _md5Salted; }
            set { _md5Salted = value; }
        }

        private bool _valid = false;
        public bool Valid
        {
            get { return _valid; }
            set { _valid = value; }
        }

        //Create ProtectedFile Properties from FilePath, used by Developer, when recreate FileCSV
        public ProtectedFile(string pFilePath)
        {
            _md5 = FrameworkUtils.MD5HashFile(pFilePath);
            _md5Salted = SaltedString.GenerateSaltedString(_md5);
            _valid = true;
        }

        //Used by Application, to check if files are valid, create from Dictionary(File)
        public ProtectedFile(string pFilePath, string pMd5Salted)
        {
            _md5 = FrameworkUtils.MD5HashFile(pFilePath);
            _md5Salted = pMd5Salted;
            _valid = (SaltedString.ValidateSaltedString(pMd5Salted, _md5));
        }
    }
}
