namespace LogicPOS.Globalization
{
    public static class ResourcesUtility
    {
        public static string GetDocumentCopyNameByNumber(
            string culture, 
            int number)
        {
            return CultureResources.GetResourceByLanguage(culture, $"global_print_copy_title{number}");
        }
    }
}
