using CryptographyUtils;
using logicpos.shared.App;
using System;
using System.Collections.Generic;
using System.IO;

namespace logicpos.financial.library.Classes.Utils
{
    public class ProtectedFiles : Dictionary<string, ProtectedFile>
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        //Private Members
        private bool _debug = false;
        private char _splitter = ',';

        //Constructor from List of Files (Developer Config), Used to Recreate the CSVFileName
        public ProtectedFiles(List<string> pFileList, string pFilePath)
        {
            string fileName = String.Empty;
            List<string> fileList = pFileList;

            if (fileList.Count > 0)
            {
                //Convert filename to Os filePaths
                for (int i = 0; i < fileList.Count; i++)
                {
                    fileName = FrameworkUtils.OSSlash(fileList[i]);
                    fileList[i] = fileName;
                    if (File.Exists(fileName))
                    {
                        ProtectedFile protectedFile = new ProtectedFile(fileName);
                        this.Add(fileName, protectedFile);
                        if (_debug) _log.Debug(string.Format("fileName: [{0}], Md5: [{1}], Md5Encrypted: [{2}], Valid: [{3}]", fileName, protectedFile.Md5, protectedFile.Md5Encrypted, protectedFile.Valid));
                    }
                }
            }
            Save(pFilePath);
        }

        //Constructor from with List of Files (Developer Config), Recreate the CSVFileName
        public ProtectedFiles(string pFilePath)
        {
            if (File.Exists(pFilePath))
            {
                Load(pFilePath);
            }
        }

        //Save Generated File from List of Files (Developer Config)
        public bool Save(string pFilePath)
        {
            bool result = false;

            try
            {
                using (StreamWriter streamWriter = new StreamWriter(pFilePath, false))
                {
                    foreach (var item in this)
                    {
                        if (_debug) _log.Debug(string.Format("ProtectedFile: [{0}], [{1}]", item.Key, item.Value.Md5Encrypted));
                        streamWriter.WriteLine(string.Format("{1}{0}{2}", _splitter, item.Key, item.Value.Md5Encrypted));
                    }
                }

                //Force copy from bin to source path, else on next run, file is blank or missing, or un-updated
                string targetFile = Environment.CurrentDirectory.Replace(@"bin\Debug", pFilePath);
                File.Copy(pFilePath, targetFile, true);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //Load CSV File
        public bool Load(string pFilePath)
        {
            bool result = false;
            string fileName = String.Empty;

            try
            {
                Dictionary<string, string> filesDictionary = FrameworkUtils.CSVFileToDictionary(pFilePath);
                //Clear before Load 
                if (this.Count > 0) this.Clear();

                foreach (var item in filesDictionary)
                {
                    fileName = item.Key;
                    ProtectedFile protectedFile = new ProtectedFile(fileName, item.Value);
                    this.Add(fileName, protectedFile);
                    if (_debug) _log.Debug(string.Format("fileName: [{0}], Md5: [{1}], Md5Encrypted: [{2}], Valid: [{3}]", fileName, protectedFile.Md5, protectedFile.Md5Encrypted, protectedFile.Valid));
                }
                
                result = true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        public bool IsValidFile(string pKey)
        {
            bool result = false;

            //if (SettingsApp.DeveloperMode) return true;

            if (File.Exists(pKey)) {
                //Get In Memory values
                string md5Encryptd = this[pKey].Md5Encrypted;
                string md5FromMem = this[pKey].Md5;
                //get Fresh Hash
                string md5FromFile = FrameworkUtils.MD5HashFile(pKey);
                //Check if created are equal to encrypted in memory, if false user change file after BootStrap
                bool valid = SaltedString.ValidateSaltedString(md5Encryptd, md5FromFile);
                //Update Protected File
                this[pKey].Md5 = md5FromFile;
                this[pKey].Valid = valid;
                //debug
                if (_debug) _log.Debug(string.Format("IsValidFile md5FromMem: [{0}], md5FromFile: [{1}], valid: [{2}]", md5FromMem, md5FromFile, valid));
                //Assign to final Result
                result = valid;
            }
            
            return result;
        }

        public List<string> GetInvalidFiles()
        {
            List<string> result = new List<string>();

            foreach (var item in this)
            {
                if (!item.Value.Valid)
                {
                    result.Add(item.Key);
                }
            }

            return result;
        }

        //Get Missing Files from SettingsApp and Compare with CSV, if Miss any file Return it
        public List<string> GetMissingFiles(List<string> pFileList)
        {
            List<string> result = new List<string>();
            string fileName = String.Empty;

            foreach (var item in pFileList)
            {
                fileName = FrameworkUtils.OSSlash(item);

                if (! this.ContainsKey(fileName))
                {
                    if (_debug) _log.Debug(string.Format("Miss filename : [{0}]", fileName));
                    result.Add(fileName);
                }
            }

            return result;
        }

        //Get Combo of Invalid and Missing Files
        public List<string> GetInvalidAndMissingFiles(List<string> pFileList)
        {
            //Get Both Lists
            List<string> getInvalidFiles = GetInvalidFiles();
            List<string> getMissingFiles = GetMissingFiles(pFileList);
            List<List<string>> listList = new List<List<string>>();
            listList.Add(getInvalidFiles); 
            listList.Add(getMissingFiles);
            //Get Distinct Filenames
            List<string> result = FrameworkUtils.MergeGenericLists(listList);

            return result;
        }
    }
}
