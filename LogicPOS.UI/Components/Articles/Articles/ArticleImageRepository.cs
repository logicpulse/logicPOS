using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Articles
{
    public static class ArticleImageRepository
    {
        private static Dictionary<Guid, string> _images = new Dictionary<Guid, string>();

        public static string AddBase64Image(Guid articleId, string content, string extension)
        {
            if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(extension))
            {
                return null;
            }

            var tempFile = System.IO.Path.GetTempFileName() + "." + extension;
            System.IO.File.WriteAllBytes(tempFile, Convert.FromBase64String(content));

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
