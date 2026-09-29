namespace LogicPOS.Api.Enums
{
    public enum ArticleSerialNumberStatus
    {
        None = 0,
        Available = 1,
        Sold = 2,
        Exchanged = 3,
        Returned = 4
    }

    public static class ArticleSerialNumberStatusExtensions
    {
        public static string ToFriendlyString(this ArticleSerialNumberStatus status)
        {
            switch (status)
            {
                case ArticleSerialNumberStatus.None:
                    return "Nenhum";
                case ArticleSerialNumberStatus.Available:
                    return "Disponível";
                case ArticleSerialNumberStatus.Sold:
                    return "Vendido";
                case ArticleSerialNumberStatus.Exchanged:
                    return "Trocado";
                case ArticleSerialNumberStatus.Returned:
                    return "Devolvido";
                default:
                    return status.ToString();
            }
        }
    }
}