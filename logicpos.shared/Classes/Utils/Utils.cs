using Ionic.Zip;
using System;
using System.IO;
using System.IO.Compression;

namespace logicpos.shared.Classes.Utils
{
    public class Utils
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //DotNetZipLib Helper Classes

        public static bool ZipPack(string[] pFiles, string pFileDestination)
        {
            string password = null;
            return ZipPack(pFiles, pFileDestination, password, EncryptionAlgorithm.WinZipAes256, CompressionMethod.BZip2, CompressionLevel.Optimal);
        }

        public static bool ZipPack(string[] pFiles, string pFileDestination, string pPassword)
        {
            return ZipPack(pFiles, pFileDestination, pPassword, EncryptionAlgorithm.WinZipAes256, CompressionMethod.BZip2, CompressionLevel.Optimal);
        }

        public static bool ZipPack(string[] pFiles, string pDestinationFileName, string pPassword, EncryptionAlgorithm pEncryptionAlgorithm, CompressionMethod pCompressionMethod, CompressionLevel pCompressionLevel)
        {
            bool debug = false;
            bool result = false;

            try
            {
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
                            if (debug) _log.Debug(string.Format("Add File:[{0}] to Zip:[{1}]", pFiles[i], pDestinationFileName));
                            zip.AddFile(pFiles[i], System.IO.Path.GetDirectoryName(pFiles[i]));
                        }
                        else
                        {
                            if (debug) _log.Debug(string.Format("Error Adding File:[{0}] to Zip:[{1}]. Cant Find the File", pFiles[i], pDestinationFileName));
                        }
                    }

                    //Set compression just before Save()
                    zip.CompressionMethod = CompressionMethod.BZip2;
                    zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                    zip.Save(pDestinationFileName);

                    result = true;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                result = false;
            }
            return result;
        }

        public static bool ZipUnPack(string pFileName, string pDestinationPath, bool pFlattenFoldersOnExtract = false)
        {
            string password = null;
            return ZipUnPack(pFileName, pDestinationPath, password, pFlattenFoldersOnExtract);
        }

        public static bool ZipUnPack(string pFileName, string pDestinationPath, string pPassword, bool pFlattenFoldersOnExtract = false)
        {
            return ZipUnPack(pFileName, pDestinationPath, pPassword, ExtractExistingFileAction.OverwriteSilently, pFlattenFoldersOnExtract);
        }

        public static bool ZipUnPack(string pFileName, string pDestinationPath, string pPassword, ExtractExistingFileAction pExtractExistingFileAction, bool pFlattenFoldersOnExtract = false)
        {
            bool debug = false;
            bool result = false;

            try
            {
                using (ZipFile zip = ZipFile.Read(pFileName))
                {
                    zip.FlattenFoldersOnExtract = pFlattenFoldersOnExtract;

                    foreach (ZipEntry e in zip)
                    {
                        if (debug) _log.Debug(string.Format("Extract file: [{0}] from [{1}] to destinationPath: [{2}]", e.FileName, pFileName, pDestinationPath));
                        e.ExtractWithPassword(pDestinationPath, pExtractExistingFileAction, pPassword);
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                result = false;
            }
            return result;
        }

        //public static string GenerateRandomString(int size)
        //{
        //    var b = new byte[size];
        //    new RNGCryptoServiceProvider().GetBytes(b);
        //    return Encoding.ASCII.GetString(b);
        //}

        public static string GenerateRandomString(int size)
        {
            Random rand = new Random();
            string stringchars = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = stringchars[rand.Next(stringchars.Length)];
            }
            return new string(chars);
        }
    }
}
