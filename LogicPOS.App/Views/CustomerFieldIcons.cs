namespace LogicPOS.App.Views;

/// <summary>
/// Canonical field-key → icon path for customer editors (Novo cliente / BackOffice Clientes).
/// Prefer these over generic menu/action glyphs so Morada is never an eye, etc.
/// </summary>
public static class CustomerFieldIcons
{
    private const string Root = "avares://LogicPOS.App/Assets/Images/";
    private const string Transport = Root + "Documents/Transport/";
    private const string Articles = Root + "Articles/";
    private const string Documents = Root + "Documents/";
    private const string Menu = Root + "BackOffice/Menu/";

    public const string Name = Transport + "cliente_b.png";
    public const string FiscalNumber = Transport + "empresa_b.png";
    public const string CardNumber = Articles + "icon_card_g.svg";
    public const string Discount = Articles + "icon_coins_b.svg";
    public const string Address = Transport + "morada_b.png";
    public const string Locality = Transport + "distrito_b.png";
    public const string ZipCode = Transport + "codigo_postal_b.png";
    public const string City = Transport + "cidade_b.png";
    public const string Country = Transport + "pais_b.png";
    public const string Phone = Articles + "icon_phone_b.svg";
    public const string Email = Documents + "icon_doc_email_b.png";
    public const string CustomerType = Menu + "menu_clientes_tipo_clientes_b.svg";
    public const string PriceType = Articles + "icon_tag_b.svg";
    public const string Notes = Articles + "icon_note_g.svg";

    public static readonly IReadOnlyDictionary<string, string> ByKey =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Name"] = Name,
            ["FiscalNumber"] = FiscalNumber,
            ["CardNumber"] = CardNumber,
            ["Discount"] = Discount,
            ["Address"] = Address,
            ["Locality"] = Locality,
            ["ZipCode"] = ZipCode,
            ["City"] = City,
            ["CountryId"] = Country,
            ["Country"] = Country,
            ["Phone"] = Phone,
            ["MobilePhone"] = Phone,
            ["Email"] = Email,
            ["CustomerTypeId"] = CustomerType,
            ["PriceTypeId"] = PriceType,
            ["Notes"] = Notes,
            ["Note"] = Notes
        };

    public static string? ForKey(string? key)
        => string.IsNullOrWhiteSpace(key) == false && ByKey.TryGetValue(key, out var path)
            ? path
            : null;
}
