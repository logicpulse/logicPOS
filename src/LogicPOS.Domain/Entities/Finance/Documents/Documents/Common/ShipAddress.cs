using System.ComponentModel.DataAnnotations.Schema;

namespace LogicPOS.Domain.ValueObjects;

[ComplexType]
public sealed class ShipAddress
{
    public string? DeliveryID { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string? WarehouseID { get; set; }
    public string? LocationID { get; set; }
    public string? BuildingNumber { get; set; }
    public string? StreetName { get; set; }
    public string? AddressDetail { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Region { get; set; }
    public string? Country { get; set; }

    public bool HasRequiredTransportFields(bool requirePostalCode)
    {
        if (string.IsNullOrWhiteSpace(AddressDetail))
        {
            return false;
        }

        if (requirePostalCode && string.IsNullOrWhiteSpace(PostalCode))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(City))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(Country))
        {
            return false;
        }

        if (DeliveryDate is null || DeliveryDate == default)
        {
            return false;
        }

        return true;
    }

    public static bool HasCompleteTransportData(
        ShipAddress? shipTo,
        ShipAddress? shipFrom,
        bool requirePostalCode)
    {
        if (shipTo is null || shipFrom is null)
        {
            return false;
        }

        if (!shipTo.HasRequiredTransportFields(requirePostalCode)
            || !shipFrom.HasRequiredTransportFields(requirePostalCode))
        {
            return false;
        }

        return shipTo.DeliveryDate!.Value >= shipFrom.DeliveryDate!.Value;
    }
}