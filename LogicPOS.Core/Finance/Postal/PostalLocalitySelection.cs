using LogicPOS.Core.BackOffice;

namespace LogicPOS.Core.Finance.Postal;

public static class PostalLocalitySelection
{
    public static PostalLocalityMatch? ChooseForAutoFill(
        IReadOnlyList<PostalLocalityMatch> matches,
        string zip,
        bool explicitRequest)
    {
        if (matches.Count == 0)
        {
            return null;
        }

        var exact = matches
            .Where(item => string.Equals(item.ZipCode, zip, StringComparison.OrdinalIgnoreCase))
            .ToList();
        var streets = exact
            .Where(item => string.IsNullOrWhiteSpace(item.Street) == false)
            .GroupBy(item => item.Street, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToList();

        PostalLocalityMatch? match = streets.Count == 1
            ? streets[0]
            : exact.FirstOrDefault(item => string.IsNullOrWhiteSpace(item.Street)) ?? exact.FirstOrDefault();
        if (match is not null && streets.Count != 1 && explicitRequest == false)
        {
            match = new PostalLocalityMatch
            {
                ZipCode = match.ZipCode,
                Locality = match.Locality,
                City = match.City
            };
        }

        if (match is null && explicitRequest && matches.Count > 0
            && PortuguesePostalFormats.IsPrefixOnly(zip) == false)
        {
            match = matches[0];
        }

        return match;
    }

    public static bool ShouldOfferPicker(
        IReadOnlyList<PostalLocalityMatch> matches,
        string zip,
        bool explicitRequest)
        => explicitRequest
           && PortuguesePostalFormats.IsPrefixOnly(zip)
           && matches.Count > 1;
}