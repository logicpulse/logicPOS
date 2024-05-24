using LogicPOS.Settings;
using System.IO;
using System.Security;
using System.Security.Permissions;

namespace LogicPOS.Utility
{
    public static class PathsUtils
    {
        public static string GetImageLocationRelativeToImagesFolder(string imageLocation)
        {
            return Path.Combine(PathsSettings.ImagesFolderLocation, imageLocation);
        }

        public static bool HasWritePermissionOnPath(string path)
        {
            var writePermission = new FileIOPermission(FileIOPermissionAccess.Write, path);

            if (SecurityManager.IsGranted(writePermission) == false)
            {
                return false;
            }

            return true;
        }
    }
}
