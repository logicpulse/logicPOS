using LogicPOS.Settings;
using System.IO;

namespace LogicPOS.Utility
{
    public static class PathsUtils
    {
        public static string GetImageLocationRelativeToImagesFolder(string imageLocation)
        {
            return Path.Combine(PathsSettings.ImagesFolderLocation, imageLocation);
        }
    }
}
