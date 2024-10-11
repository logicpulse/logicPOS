using Gtk;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Documents
{
    public static class DocumentsAlerts
    {
        public static void ShowIgnoredDocumentsAlert(Window parentWindow, params string[] documents)
        {
            string ignoredDocumentsMessage = string.Empty;
            if (documents.Length > 0)
            {
                for (int i = 0; i < documents.Length; i++)
                {
                    ignoredDocumentsMessage += $"{Environment.NewLine}{documents[i]}";
                }

                string infoMessage = string.Format(GeneralUtils.GetResourceByName("app_info_show_ignored_cancelled_documents"), ignoredDocumentsMessage);
                logicpos.Utils.ShowMessageBox(parentWindow, DialogFlags.Modal, new Size(600, 400), MessageType.Info, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_information"), infoMessage);
            }
        }
    }
}
