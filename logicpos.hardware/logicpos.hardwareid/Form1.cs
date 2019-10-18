using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace logicpos.hardwareid
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            label1.Text = checkLicenceFile();
        }


        public string GenValueID()
        {
            string fingerPrint = string.Empty;

            Random rnd = new Random();

            fingerPrint = GetHash("CPU >> " + rnd.Next(1, 999) + "\nBIOS >> " + rnd.Next(1, 999) + "\nBASE >> " + rnd.Next(1, 999) + "\nMAC >> " + rnd.Next(1, 999));

            return fingerPrint;
        }
        private string GetHash(string s)
        {
            MD5 sec = new MD5CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            byte[] bt = enc.GetBytes(s);
            return GetHexString(sec.ComputeHash(bt));
        }
        private string GetHexString(byte[] bt)
        {
            string s = string.Empty;
            for (int i = 0; i < bt.Length; i++)
            {
                byte b = bt[i];
                int n, n1, n2;
                n = (int)b;
                n1 = n & 15;
                n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char)(n2 - 10 + (int)'A')).ToString();
                else
                    s += n2.ToString();
                if (n1 > 9)
                    s += ((char)(n1 - 10 + (int)'A')).ToString();
                else
                    s += n1.ToString();
                if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "-";
            }
            return s;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = checkLicenceFile();
        }

        private string checkLicenceFile()
        {
            string fileName = @"logicpos.val";
            FileInfo fi = new FileInfo(fileName);
            string machineid = string.Empty;

            try
            {
                // Check if file already exists. If yes, read it.     
                if (fi.Exists)
                {
                    // read file contents.     
                    using (StreamReader sr = File.OpenText(fileName))
                    {
                        string s = "";
                        while ((s = sr.ReadLine()) != null)
                        {
                            machineid = s;
                        }
                    }

                    machineid = EncryptString.Decrypt(machineid, "lpt#123$#!");
                }
                else
                {
                    // Create a new file     
                    using (StreamWriter sw = fi.CreateText())
                    {
                        machineid = GenValueID();
                        sw.WriteLine(EncryptString.Encrypt(machineid, "lpt#123$#!"));
                    }
                    
                }

                return machineid;
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
                return "";
            }
        }



    }
}
