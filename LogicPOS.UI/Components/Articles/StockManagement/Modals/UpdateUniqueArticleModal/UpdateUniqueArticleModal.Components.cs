
using LogicPOS.UI.Components.InputFields;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UpdateUniqueArticleModal
    {
        private SerialNumberField SerialNumberField { get; set; } = new SerialNumberField();

        protected override void AddValidatableFields()
        {
            SerialNumberField.TxtSerialNumber.IsRequired = true;
            ValidatableFields.Add(SerialNumberField);
        }
    }
}
