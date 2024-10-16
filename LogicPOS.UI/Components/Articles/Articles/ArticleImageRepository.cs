using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Articles
{
    public static class ArticleImageRepository
    {
        private static Dictionary<Guid, string> _images = new Dictionary<Guid, string>();

        public static string AddBase64Image(Guid articleId, string content, string extension)
        {
            if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(extension) || extension.Length != 3)
            {
                return null;
            }

            var tempFile = System.IO.Path.GetTempFileName() + "." + extension;
            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(content);
            }
            catch (Exception)
            {
                return null;
            }

            System.IO.File.WriteAllBytes(tempFile, bytes);

            _images[articleId] = tempFile;

            return tempFile;
        }

        public static string GetImage(Guid articleId)
        {
            if (_images.ContainsKey(articleId))
            {
                return _images[articleId];
            }

            return null;
        }
    }
}
