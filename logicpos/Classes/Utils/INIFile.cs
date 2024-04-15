using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace logicpos
{
    internal class INIFile
    {
        private readonly object _lock = new object();
        internal string FileName { get; private set; }
        private bool _lazyLoading = false;

        private readonly Dictionary<string, Dictionary<string, string>> _sections = new Dictionary<string, Dictionary<string, string>>();
        private readonly Dictionary<string, Dictionary<string, string>> _modifiedValues = new Dictionary<string, Dictionary<string, string>>();

        private bool _cacheModified = false;

        public INIFile(string FileName)
        {
            Initialize(FileName, false);
        }

        private void Initialize(
            string fileName, 
            bool lazyLoading)
        {
             FileName = fileName;
            _lazyLoading = lazyLoading;
            if (!_lazyLoading) ReadFileContentsIntoLocalCache();
        }

        private string ParseSectionName(string line)
        {
            if (line.StartsWith("[") == false) return null;
            if (line.EndsWith("]") == false) return null;
            if (line.Length < 3) return null;

            return line.Substring(1, line.Length - 2);
        }

        private bool ParseKeyValuePair(
            string line, 
            ref string key, 
            ref string value)
        {
            // *** Check for key+value pair ***
            int i;
            if ((i = line.IndexOf('=')) <= 0) return false;

            int j = line.Length - i - 1;
            key = line.Substring(0, i).Trim();
            if (key.Length <= 0) return false;

            value = (j > 0) ? (line.Substring(i + 1, j).Trim()) : string.Empty;
            return true;
        }

        internal void ReadFileContentsIntoLocalCache()
        {
            lock (_lock)
            {
                StreamReader streamReader = null;
                try
                {
                    _sections.Clear();
                    _modifiedValues.Clear();

                    try
                    {
                        streamReader = new StreamReader(FileName);
                    }
                    catch (FileNotFoundException)
                    {
                        return;
                    }

                    // *** Read up the file content ***
                    Dictionary<string, string> currentSection = null;
                   
                    string sectionName;
                    string key = null;
                    string value = null;

                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        line = line.Trim();

                        sectionName = ParseSectionName(line);

                        if (sectionName != null)
                        {
                            if (_sections.ContainsKey(sectionName) == false)
                            {
                                currentSection = new Dictionary<string, string>();
                                _sections.Add(sectionName, currentSection);
                            }
                        }
                        else if (currentSection != null)
                        {
                            if (ParseKeyValuePair(line, ref key, ref value))
                            {
                                if (currentSection.ContainsKey(key) == false)
                                {
                                    currentSection.Add(key, value);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    streamReader?.Close();
                    streamReader = null;
                }
            }
        }

        internal void Flush()
        {
            lock (_lock)
            {
                PerformFlush();
            }
        }

        private void PerformFlush()
        {

            if (_cacheModified == false) return;

            _cacheModified = false;

            bool originalFileExists = File.Exists(FileName);

            string tempFileName = Path.ChangeExtension(FileName, "$n$");

            // *** Copy content of original file to temporary file, replace modified values ***
            // *** Create the temporary file ***
            StreamWriter tempFileStreamWriter = new StreamWriter(tempFileName);

            try
            {
                Dictionary<string, string> CurrentSection = null;

                if (originalFileExists)
                {
                    StreamReader sr = null;
                    try
                    {
                        // *** Open the original file ***
                        sr = new StreamReader(FileName);

                        // *** Read the file original content, replace changes with local cache values ***
                        string s;
                        string SectionName;
                        string Key = null;
                        string Value = null;
                        bool Unmodified;
                        bool Reading = true;
                        while (Reading)
                        {
                            s = sr.ReadLine();
                            Reading = (s != null);

                            // *** Check for end of file ***
                            if (Reading)
                            {
                                Unmodified = true;
                                s = s.Trim();
                                SectionName = ParseSectionName(s);
                            }
                            else
                            {
                                Unmodified = false;
                                SectionName = null;
                            }

                            // *** Check for section names ***
                            if ((SectionName != null) || (!Reading))
                            {
                                if (CurrentSection != null)
                                {
                                    // *** Write all remaining modified values before leaving a section ****
                                    if (CurrentSection.Count > 0)
                                    {
                                        foreach (string fkey in CurrentSection.Keys)
                                        {
                                            if (CurrentSection.TryGetValue(fkey, out Value))
                                            {
                                                tempFileStreamWriter.Write(fkey);
                                                tempFileStreamWriter.Write('=');
                                                tempFileStreamWriter.WriteLine(Value);
                                            }
                                        }
                                        tempFileStreamWriter.WriteLine();
                                        CurrentSection.Clear();
                                    }
                                }

                                if (Reading)
                                {
                                    // *** Check if current section is in local modified cache ***
                                    if (!_modifiedValues.TryGetValue(SectionName, out CurrentSection))
                                    {
                                        CurrentSection = null;
                                    }
                                }
                            }
                            else if (CurrentSection != null)
                            {
                                // *** Check for key+value pair ***
                                if (ParseKeyValuePair(s, ref Key, ref Value))
                                {
                                    if (CurrentSection.TryGetValue(Key, out Value))
                                    {
                                        // *** Write modified value to temporary file ***
                                        Unmodified = false;
                                        CurrentSection.Remove(Key);

                                        tempFileStreamWriter.Write(Key);
                                        tempFileStreamWriter.Write('=');
                                        tempFileStreamWriter.WriteLine(Value);
                                    }
                                }
                            }

                            // *** Write unmodified lines from the original file ***
                            if (Unmodified)
                            {
                                tempFileStreamWriter.WriteLine(s);
                            }
                        }

                        // *** Close the original file ***
                        sr.Close();
                        sr = null;
                    }
                    finally
                    {
                        // *** Cleanup: close files ***                  
                        if (sr != null) sr.Close();
                        sr = null;
                    }
                }

                // *** Cycle on all remaining modified values ***
                foreach (KeyValuePair<string, Dictionary<string, string>> sectionPair in _modifiedValues)
                {
                    CurrentSection = sectionPair.Value;
                    if (CurrentSection.Count > 0)
                    {
                        tempFileStreamWriter.WriteLine();

                        // *** Write the section name ***
                        tempFileStreamWriter.Write('[');
                        tempFileStreamWriter.Write(sectionPair.Key);
                        tempFileStreamWriter.WriteLine(']');

                        // *** Cycle on all key+value pairs in the section ***
                        foreach (KeyValuePair<string, string> ValuePair in CurrentSection)
                        {
                            // *** Write the key+value pair ***
                            tempFileStreamWriter.Write(ValuePair.Key);
                            tempFileStreamWriter.Write('=');
                            tempFileStreamWriter.WriteLine(ValuePair.Value);
                        }
                        CurrentSection.Clear();
                    }
                }
                _modifiedValues.Clear();

                // *** Close the temporary file ***
                tempFileStreamWriter.Close();
                tempFileStreamWriter = null;

                // *** Rename the temporary file ***
                File.Copy(tempFileName, FileName, true);

                // *** Delete the temporary file ***
                File.Delete(tempFileName);
            }
            finally
            {
                // *** Cleanup: close files ***                  
                if (tempFileStreamWriter != null) tempFileStreamWriter.Close();
            }
        }

        // *** Read a value from local cache ***
        internal string GetValue(string SectionName, string Key, string DefaultValue)
        {
            // *** Lazy loading ***
            if (_lazyLoading)
            {
                _lazyLoading = false;
                ReadFileContentsIntoLocalCache();
            }

            lock (_lock)
            {
                // *** Check if the section exists ***
                Dictionary<string, string> Section;
                if (!_sections.TryGetValue(SectionName, out Section)) return DefaultValue;

                // *** Check if the key exists ***
                string Value;
                if (!Section.TryGetValue(Key, out Value)) return DefaultValue;

                // *** Return the found value ***
                return Value;
            }
        }

    }
}