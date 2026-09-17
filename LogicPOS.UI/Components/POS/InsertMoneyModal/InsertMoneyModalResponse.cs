using Gtk;

namespace LogicPOS.UI.Components.Modals
{
    public struct InsertMoneyModalResponse
    {
        public ResponseType Response { get; set; }

        public decimal Value { get; set; }

        public InsertMoneyModalResponse(ResponseType response, decimal value)
        {
            Response = response;
            Value = value;
        }
    }
}
