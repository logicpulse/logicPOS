using Ionic.Zip;
using System.IO;

namespace LogicPOS.Utility
{
    public static class CompressionUtils
    {
        public static bool ZipPack(
            string[] pFiles,
            string pFileDestination,
            string pPassword)
        {
            return ZipPack(
                pFiles,
                pFileDestination,
                pPassword,
                EncryptionAlgorithm.WinZipAes256,
                CompressionMethod.BZip2,
                Ionic.Zlib.CompressionLevel.BestCompression);
        }

        public static bool ZipPack(
            string[] pFiles,
            string pDestinationFileName,
            string pPassword,
            EncryptionAlgorithm pEncryptionAlgorithm,
            CompressionMethod pCompressionMethod,
            Ionic.Zlib.CompressionLevel pCompressionLevel)
        {
            bool result = false;

            using (ZipFile zip = new ZipFile())
            {
                //Set zip properties except compression ones: Must be here
                if (pPassword != string.Empty)
                {
                    zip.Password = pPassword;
                    zip.Encryption = pEncryptionAlgorithm;
                }

                //Add Files
                for (int i = 0; i < pFiles.Length; i++)
                {
                    if (File.Exists(pFiles[i]))
                    {
                        zip.AddFile(pFiles[i], Path.GetDirectoryName(pFiles[i]));
                    }
                }

                //Set compression just before Save()
                zip.CompressionMethod = pCompressionMethod;
                zip.CompressionLevel = pCompressionLevel;
                zip.Save(pDestinationFileName);

                result = true;
            }

            return result;
        }

        public static bool ZipUnPack(
            string pFileName,
            string pDestinationPath,
            string pPassword,
            bool pFlattenFoldersOnExtract = false)
        {
            return ZipUnPack(pFileName, pDestinationPath, pPassword, ExtractExistingFileAction.OverwriteSilently, pFlattenFoldersOnExtract);
        }

        public static bool ZipUnPack(
            string pFileName,
            string pDestinationPath,
            string pPassword,
            ExtractExistingFileAction pExtractExistingFileAction,
            bool pFlattenFoldersOnExtract = false)
        {
            bool result = false;

            using (ZipFile zip = ZipFile.Read(pFileName))
            {
                zip.FlattenFoldersOnExtract = pFlattenFoldersOnExtract;

                foreach (ZipEntry e in zip)
                {
                    e.ExtractWithPassword(pDestinationPath, pExtractExistingFileAction, pPassword);
                }
                result = true;
            }

            return result;
        }
    }
}
