using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace logicpos.shared.App
{
    /// <summary>
    /// This class is responsible for all POS custom regions definitions and operations.
    /// </summary>
    public class CustomRegion
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly CustomRegion GBR = new CustomRegion("GB", "en-GB", "en-GB-gbr");
        public static readonly CustomRegion USA = new CustomRegion("US", "en-US", "en-US-usa");
        public static readonly CustomRegion FRA = new CustomRegion("FR", "fr-FR", "fr-FR-fra");
        public static readonly CustomRegion BRA = new CustomRegion("BR", "pt-BR", "pt-BR-bra");
        public static readonly CustomRegion AGO = new CustomRegion("AO", "pt-PT", "pt-PT-ago");
        public static readonly CustomRegion MOZ = new CustomRegion("MZ", "pt-PT", "pt-PT-moz");
        public static readonly CustomRegion PRT = new CustomRegion("PT", "pt-PT", "pt-PT-prt");
        public static readonly CustomRegion ESP = new CustomRegion("ES", "es-ES", "es-ES-esp");

        public static IEnumerable<CustomRegion> Values
        {
            get
            {
                yield return GBR;
                yield return USA;
                yield return FRA;
                yield return AGO;
                yield return BRA;
                yield return MOZ;
                yield return PRT;
                yield return ESP;
            }
        }

        public string Region { get; private set; }
        public string BaseCulture { get; private set; }
        public string CustomCultureName { get; private set; }

        CustomRegion(string region, string baseCulture, string customCultureName)
        {
            Region = region;
            BaseCulture = baseCulture;
            CustomCultureName = customCultureName;
        }

        /// <summary>
        /// This static method creates and register a custom culture based on <see cref="Values"/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// These custom cultures are necessary to make POS platform "language dependency-free" for which either the country and/or the language is not currently supported by the .NET Framework (or Windows).
        /// </para>
        /// </remarks>
        public static void RegisterCustomRegion()
        {
            foreach (CustomRegion customRegion in Values)
            {
                string customCultureName = customRegion.CustomCultureName;

                try
                {
                    /* This call is necessary to erase some changes that would have be applied to a custom culture */
                    /* IN006018 and IN007009 */
                    //UnregisterCustomRegion(customCultureName);

                    /* If for some reason a custom culture has not been removed, we will not attempt to recreate/register it */
                    if (!customRegionExists(customCultureName))
                    {
                        var baseCultureInfo = new CultureInfo(customRegion.BaseCulture);
                        var regionInfo = new RegionInfo(customRegion.Region);

                        /* This block of code is responsible for POS custom region creation */
                        var customRegionBuilder = new CultureAndRegionInfoBuilder(customCultureName, CultureAndRegionModifiers.None);
                        customRegionBuilder.LoadDataFromCultureInfo(baseCultureInfo);
                        customRegionBuilder.LoadDataFromRegionInfo(regionInfo);
                        customRegionBuilder.Register();

                        _log.Debug(string.Format("Custom Culture [{0}] Created and Registered Successfully", customCultureName));
                    }
                }
                catch (Exception e)
                {
                    _log.Debug(string.Format("Error when Creating and Registering [{0}] Custom Culture: {1}", customCultureName, e));
                }
            }
        }

        /// <summary>
        /// This method checks whether a custom region exists.
        /// </summary>
        /// <param name="customCultureName"></param>
        /// <returns></returns>
        private static bool customRegionExists(string customCultureName)
        {
            //return CultureInfo.GetCultures(CultureTypes.UserCustomCulture).Contains(new CultureInfo(customCultureName));
            
            /* IN008017 */
            var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var containsAnyInAllCultures = allCultures.Any(localCulture => string.Equals(localCulture.Name, customCultureName, StringComparison.CurrentCultureIgnoreCase));

            /*
            var userCultures = CultureInfo.GetCultures(CultureTypes.UserCustomCulture);
            var customCulture = userCultures.Where(localCustomculture => localCustomculture.Name.Equals(customCultureName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            return customCulture != null;
            */
            return containsAnyInAllCultures;
        }

        /// <summary>
        /// This method is responsible for unregister a specific custom culture based on its name.
        /// </summary>
        /// <param name="customCultureName"></param>
        private static void UnregisterCustomRegion(string customCultureName)
        {
            try
            {
                CultureAndRegionInfoBuilder.Unregister(customCultureName);
                _log.Debug(string.Format("Custom Culture [{0}] Unregistered Successfully", customCultureName));
            }
            catch (Exception e)
            {
                _log.Debug(string.Format("Error when Unegistering [{0}] Custom Culture: {1}", customCultureName, e));
            }
        }
    }
}