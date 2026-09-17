namespace LogicPOS.UI.Components.InputFields.Validation
{
    public static class ValidationFunctions
    {
        public static bool IsValidDiscount(string discount) => decimal.Parse(discount) >= 0 && decimal.Parse(discount) <= 100;
    }
}
