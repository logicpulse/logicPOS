using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

/// <summary>
/// Auxiliary Código-postal → Localidade/Cidade for a País. The catalog is the JSON under db_seeds, not this table.
/// </summary>
public class PostalLocality : Entity
{
    public Guid CountryId { get; set; }

    public Country? Country { get; set; }

    public string ZipCode { get; set; } = string.Empty;

    /// <summary>Street line (Rua, Avenida, …). Empty when the row is only the postal code.</summary>
    public string? Street { get; set; }

    public string Locality { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;
}
