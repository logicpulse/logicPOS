using System.Resources;

namespace LogicPOS.Globalization
{
    public static class CultureResources
    {
        public static ResXResourceSet ResourceSet { get; set; }

        private static string GetResourcesFileByName(string resourceName)
        {
            return $"Resources\\Localization\\{resourceName}";
        }

        private static string GetDefaultLanguage()
        {
            return System.Configuration.ConfigurationManager.AppSettings["customCultureResourceDefinition"].ToString();
        }

        private static void UpdateResourceSetByLanguage(string language)
        {
            switch (language)
            {
                case "en-GB":
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.en-GB.resx"));
                    break;
                case "en-US":
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.en-US.resx"));
                    break;
                case "fr-FR":
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.fr-FR.resx"));
                    break;
                case "pt-BR":
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.pt-BR.resx"));
                    break;
                case "pt-AO":
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.pt-AO.resx"));
                    break;
                case "pt-MZ":
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.pt-MZ.resx"));
                    break;
                case "pt-PT":
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.pt-PT.resx"));
                    break;
                case "es-ES":
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.es-ES.resx"));
                    break;
                default:
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.resx"));
                    break;
            }
        }

        public static string GetResourceByLanguage(string language, string resourceName)
        {
            if (ResourceSet == null)
            {
                if (language == "")
                {
                    language = GetDefaultLanguage();
                }

                UpdateResourceSetByLanguage(language);
            }

            string resourceValue = ResourceSet.GetString(resourceName);

            if (resourceValue == null)
            {
                throw new System.Exception($"Resource {resourceName} not found in {language} language.");
            }

            return resourceValue;
        }

        public static void UpdateLanguage(string language)
        {
            switch (language)
            {
                case "en-GB":
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.en-GB.resx"));
                    break;
                case "en-US":
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.en-US.resx"));
                    break;
                case "fr-FR":
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.fr-FR.resx"));
                    break;
                case "pt-BR":
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.pt-BR.resx"));
                    break;
                case "pt-AO":
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.pt-AO.resx"));
                    break;
                case "pt-MZ":
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.pt-MZ.resx"));
                    break;
                case "pt-PT":
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.pt-PT.resx"));
                    break;
                case "es-ES":
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.es-ES.resx"));
                    break;
                default:
                    ResourceSet = new ResXResourceSet(GetResourcesFileByName("Resx.resx"));
                    break;
            }
        }
    }
}
