using System.Resources;

namespace LogicPOS.Globalization
{
    public class LocalizedString
    {
        public static LocalizedString Instance { get; set; }

        private readonly string _culture;
        private ResXResourceSet ResourceSet { get; set; }

        public LocalizedString(string culture)
        {
            _culture = culture;
            ResourceSet = GetResourceSetByLanguage(_culture);
        }

        public string this[string key]
        {
            get
            {
                return ResourceSet.GetString(key) ?? key;
            }
        }

        private static ResXResourceSet GetResourceSetByLanguage(string language)
        {
            switch (language)
            {
                case "en-GB":
                    return new ResXResourceSet(GetResourcesFileByName("Resx.en-GB.resx"));
                case "en-US":
                    return new ResXResourceSet(GetResourcesFileByName("Resx.en-US.resx"));
                case "fr-FR":
                    return new ResXResourceSet(GetResourcesFileByName("Resx.fr-FR.resx"));
                case "pt-BR":
                    return new ResXResourceSet(GetResourcesFileByName("Resx.pt-BR.resx"));
                case "pt-AO":
                    return new ResXResourceSet(GetResourcesFileByName("Resx.pt-AO.resx"));
                case "pt-MZ":
                    return new ResXResourceSet(GetResourcesFileByName("Resx.pt-MZ.resx"));
                case "pt-PT":
                    return new ResXResourceSet(GetResourcesFileByName("Resx.pt-PT.resx"));
                case "es-ES":
                    return new ResXResourceSet(GetResourcesFileByName("Resx.es-ES.resx"));
                default:
                    return new ResXResourceSet(GetResourcesFileByName("Resx.resx"));
            }
        }

        private static string GetResourcesFileByName(string resourceName)
        {
            return $"Localization\\{resourceName}";
        }
    }
}
