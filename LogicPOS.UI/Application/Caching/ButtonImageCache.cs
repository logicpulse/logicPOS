using System;
using System.IO;

namespace LogicPOS.UI.Components.Articles
{
    public static class ButtonImageCache
    {
        private static string FolderLocation => "Cache\\ButtonImages\\";

        static ButtonImageCache()
        {
            if (!Directory.Exists(FolderLocation))
            {
                Directory.CreateDirectory(FolderLocation);
            }
        }

        public static string AddBase64Image(Guid id, string base64Image, string extension)
        {
            if (string.IsNullOrWhiteSpace(base64Image) || string.IsNullOrWhiteSpace(extension) || extension.Length != 3)
            {
                return null;
            }

            var filePath = GetImageFileName(id, extension);

            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(base64Image);
            }
            catch (Exception)
            {
                return null;
            }

            System.IO.File.WriteAllBytes(filePath, bytes);

            return filePath;
        }

        private static string GetImageFileName(Guid id, string extension)
        {
            return Path.Combine(FolderLocation, id.ToString().ToLower() + "." + extension.ToLower());
        }

        public static string GetImagePath(Guid id, string extension)
        {
            var filePath = GetImageFileName(id, extension);
            return File.Exists(filePath) ? filePath : null;
        }
    }
}
