using LogicPOS.Settings;

namespace LogicPOS.Utility
{
    public static class PreferencesUtils
    {
        public static string GetPreferenceParameterFromToken(string pToken)
        {
            string result = (GeneralSettings.PreferenceParameters.ContainsKey(pToken.ToUpper()))
              ? GeneralSettings.PreferenceParameters[pToken.ToUpper()]
              : $"UNDEFINED [{pToken}]";

            return result;
        }
    }
}
