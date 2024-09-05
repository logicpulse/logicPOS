namespace LogicPOS.UI.Components.InputFields
{
    public interface IValidatableField
    {
        string FieldName { get; }
        bool IsValid();
    }
}
