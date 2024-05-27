namespace LogicPOS.Utility
{
    public class ProtectedFile
    {
        public string Md5 { get; set; } = string.Empty;

        public string Md5Encrypted { get; set; } = string.Empty;

        public bool Valid { get; set; } = false;

        //Create ProtectedFile Properties from FilePath, used by Developer, when recreate FileCSV
        public ProtectedFile(string pFilePath)
        {
            Md5 = CryptographyUtils.MD5HashFile(pFilePath);
            Md5Encrypted = CryptographyUtils.GenerateSaltedString(Md5);
            Valid = true;
        }

        //Used by Application, to check if files are valid, create from Dictionary(File)
        public ProtectedFile(string pFilePath, string pMd5Salted)
        {
            Md5 = CryptographyUtils.MD5HashFile(pFilePath);
            Md5Encrypted = pMd5Salted;
            Valid = (CryptographyUtils.ValidateSaltedString(pMd5Salted, Md5));
        }
    }
}
