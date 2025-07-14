using System;
using System.Collections;
using System.Data;

namespace LogicPOS.Plugin.Licensing
{
    public class LicenseData
    {
        public bool Registered { get; set; } = false;
        public string Version { get; set; }
        public string Date { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string FiscalNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string HardwareId { get; set; }
        public string Reseller { get; set; }
        public bool ModuleStocks { get; set; }
        public DateTime UpdateDate { get; set; }
        public DataTable Keys { get; set; }
        public SortedList Informations { get; set; }

        public LicenseData GetDemoData()
        {
            var data = new LicenseData
            {
                Date = DateTime.Now.ToString("dd/MM/yyyy"),
                Version = "POS_CORPORATE",
                Name = "DEBUG",
                HardwareId = "####-####-####-####-####-####",
                Company = "Empresa Demonstração",
                FiscalNumber = "NIF Demonstração",
                Address = "Morada Demonstração",
                Email = "mail@demonstracao.tld",
                Telephone = "DEBUG",
                ModuleStocks = true,
                Keys = new DataTable("keysLicence")
            };

            data.Keys.Columns.Add("name", typeof(string));
            data.Keys.Columns.Add("value", typeof(string));
            data.Keys.Rows.Clear();
            data.Reseller = "LogicPulse";
            data.UpdateDate = DateTime.Now.AddDays(-1);

            return data;

        }
    }
}
