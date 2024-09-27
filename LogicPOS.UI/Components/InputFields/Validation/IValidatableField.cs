namespace LogicPOS.UI.Components.InputFields.Validation
{
    public interface IValidatableField
    {
        string FieldName { get; }
        bool IsValid();

    }
}
