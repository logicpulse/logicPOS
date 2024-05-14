using System.Collections;

namespace LogicPOS.Settings
{
    public static class PathsSettings
    {
        public static Hashtable Paths { get; set; }

        public static string TempFolderLocation => Paths["temp"].ToString();
        public static string ImagesFolderLocation => Paths["images"].ToString();
    }
}
