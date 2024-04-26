using System.IO;
using System.Resources;

namespace logicpos.resources
{
    public static class CustomResources
    {
        public static ResXResourceSet ResourceSet { get; set; }

        private static string GetResourcesFileByName(string resourceName)
        {
            var directorySeparatorChar = Path.DirectorySeparatorChar;
            return "Resources" + directorySeparatorChar + "Localization" + directorySeparatorChar + resourceName;
        }

        public static string GetCustomResource(string language, string value)
        {
            if (ResourceSet == null)
            {
                if (language == "")
                {
                    language = System.Configuration.ConfigurationManager.AppSettings["customCultureResourceDefinition"].ToString();
                }

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

            string result;
            return (result = ResourceSet.GetString(value)) != null ? result : value;
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
