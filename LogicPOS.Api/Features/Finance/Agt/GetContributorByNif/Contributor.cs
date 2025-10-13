namespace LogicPOS.Api.Features.Finance.Agt.GetContributorByNif
{
    public class Contributor
    {
        public string Nif { get; set; }
        public string Province { get; set; }
        public string Municipe { get; set; }
        public string Locality { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string Name { get; set; }
        public string CollectiveName { get; set; }
        public string CommercialDesignation { get; set; }
        public string SocialCapital { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public string GetCity()
        {
            if (string.IsNullOrWhiteSpace(Province))
            {
                return Municipe;
            }

            return Province;
        }

        public string GetLocality()
        {
            if (string.IsNullOrWhiteSpace(Locality))
            {
                return GetAddress();
            }

            return Locality;
        }

        public string GetAddress()
        {
            if (string.IsNullOrWhiteSpace(Address))
            {
                return Street;
            }

            return Address;
        }

        public string GetCustomerName()
        {
            if (string.IsNullOrWhiteSpace(Name) == false)
            {
                return Name;
            }

            if (string.IsNullOrWhiteSpace(CommercialDesignation) == false)
            {
                return CommercialDesignation;
            }

            return CollectiveName;

        }
    }
}
