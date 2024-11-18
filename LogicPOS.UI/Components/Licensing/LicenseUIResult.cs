using Gtk;

namespace LogicPOS.UI.Components.Licensing
{
    public struct LicenseUIResult
    {
        public ResponseType Response { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string FiscalNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
