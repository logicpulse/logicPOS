using System;
using System.Drawing;
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
            if (string.IsNullOrWhiteSpace(base64Image) || string.IsNullOrWhiteSpace(extension))
            {
                return null;
            }

            var filePath = CreateImagePath(id, extension);

            try
            {
                SaveImage(base64Image, filePath);
            }
            catch (Exception)
            {
                return null;
            }

            return filePath;
        }

        private static void SaveImage(string base64Image, string path)
        {
            using (var ms = new MemoryStream(Convert.FromBase64String(base64Image)))
            using (var image = Image.FromStream(ms))
            {
                int width = 167;
                int height = 106;
                if (image.Size.Width != width || image.Size.Height != height)
                {


                    using (var resized = new Bitmap(width, height))
                    using (var g = Graphics.FromImage(resized))
                    {
                        g.DrawImage(image, 0, 0, width, height);
                        resized.Save(path);
                    }
                }
                else
                {
                    image.Save(path);
                }
            }
        }

        private static string CreateImagePath(Guid id, string extension)
        {
            return Path.Combine(FolderLocation, id.ToString().ToLower() + "." + extension.ToLower());
        }

        public static string GetImagePath(Guid id, string extension)
        {
            var filePath = CreateImagePath(id, extension);
            return File.Exists(filePath) ? filePath : null;
        }
    }
}
