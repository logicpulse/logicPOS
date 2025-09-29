using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace LogicPOS.UI.Components.Articles
{
    public static class ButtonImageCache
    {
        private const int ImageWidth = 167;
        private const int ImageHeight = 106;
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
            catch (Exception ex)
            {

                return null;
            }

            return filePath;
        }

        public static void DeleteImage(Guid id, string extension)
        {
           var path = CreateImagePath(id, extension);
            File.Delete(path);
        }

        private static void SaveImage(string base64Image, string path)
        {
            using (var ms = new MemoryStream(Convert.FromBase64String(base64Image)))
            using (var image = Image.FromStream(ms))
            {
                if (image.Size.Width != ImageWidth || image.Size.Height != ImageHeight)
                {
                    using (var resized = ResizeImage(image))
                    {
                        resized.Save(path);
                    }
                }
                else
                {
                    image.Save(path);
                }
            }
        }

        private static Image ResizeImage(Image image)
        {
            var resized = new Bitmap(ImageWidth, ImageHeight);
            using (var g = Graphics.FromImage(resized))
            {
                g.DrawImage(image, 0, 0, ImageWidth, ImageHeight);
            }

            return resized;
        }

        public static string ImageToResizedBase64Image(string path)
        {
            byte[] imageBytes;
            string tempFile = Path.GetTempFileName();
            using (Image image = Image.FromFile(path))
            {
                var resizedImage = ResizeImage(image);
                resizedImage.Save(tempFile);
            }
            imageBytes = File.ReadAllBytes(tempFile);
            File.Delete(tempFile);
            return Convert.ToBase64String(imageBytes);
        }

        private static string CreateImagePath(Guid id, string extension)
        {
            return Path.Combine(FolderLocation, id.ToString().ToLower() + "." + extension.ToLower());
        }

        public static string GetImageLocation(Guid id, string extension)
        {
            var filePath = CreateImagePath(id, extension);
            return File.Exists(filePath) ? filePath : null;
        }
    }
}
