
using LogicPOS.UI.Components.InputFields;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UpdateUniqueArticleModal
    {
        private TextBox TxtSerialNumber { get; set; } = TextBox.Simple("global_serial_number", true);

        protected override void AddValidatableFields()
        {
            throw new System.NotImplementedException();
        }

    }
}
