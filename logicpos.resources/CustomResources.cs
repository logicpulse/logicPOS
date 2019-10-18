using System.IO;
using System.Resources;

namespace logicpos.resources
{
    public class CustomResources
    {
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ResXResourceSet rsxr;

        public static string GetCustomResources(string language, string value)
        {
            try
            {
                string result = value;

                if (rsxr == null)
                {
                    if (language == "")
                    {
                        language = System.Configuration.ConfigurationManager.AppSettings["customCultureResourceDefinition"].ToString();
                    }

                    switch (language)
                    {
                        case "en-GB":
                            rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.en-GB.resx");
                            break;
                        case "en-US":
                            rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.en-US.resx");
                            break;
                        case "fr-FR":
                            rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.fr-FR.resx");
                            break;
                        case "pt-BR":
                            rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.pt-BR.resx");
                            break;
                        case "pt-AO":
                            rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.pt-AO.resx");
                            break;
                        case "pt-MZ":
                            rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.pt-MZ.resx");
                            break;
                        case "pt-PT":
                            rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.pt-PT.resx");
                            break;
                        case "es-ES":
                            rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.es-ES.resx");
                            break;
                        default:
                            rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.resx");
                            break;
                    }
                }

                if ((result = rsxr.GetString(value)) != null)
                {
                    return result;
                }
            }
            catch (System.Exception ex)
            {
                _log.Error("", ex);
            }
            return value;
        }
		//IN009296 BackOffice - Mudar a língua da aplicação
        public static void UpdateLanguage(string language)
        {
            try
            {

                switch (language)
                {
                    case "en-GB":
                        rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.en-GB.resx");
                        break;
                    case "en-US":
                        rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.en-US.resx");
                        break;
                    case "fr-FR":
                        rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.fr-FR.resx");
                        break;
                    case "pt-BR":
                        rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.pt-BR.resx");
                        break;
                    case "pt-AO":
                        rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.pt-AO.resx");
                        break;
                    case "pt-MZ":
                        rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.pt-MZ.resx");
                        break;
                    case "pt-PT":
                        rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.pt-PT.resx");
                        break;
                    case "es-ES":
                        rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.es-ES.resx");
                        break;
                    default:
                        rsxr = new ResXResourceSet("Resources" + Path.DirectorySeparatorChar + "Localization" + Path.DirectorySeparatorChar + "Resx.resx");
                        break;
                }



            }
            catch (System.Exception ex)
            {
                _log.Error("", ex);
            }

        }
    }
}
