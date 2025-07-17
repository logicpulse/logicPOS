using Gtk;
using System.Drawing;
using System.IO;
using LogicPOS.Utility;
using LogicPOS.UI.Dialogs;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Application;
using LogicPOS.UI.Settings;

namespace LogicPOS.UI.Components.Pickers
{
    internal class FilePicker : BaseDialog
    {
        private FileFilter Filter { get; set; }
        private FileChooserAction PickerAction { get; set; }
        private Fixed Body { get; set; }
        public FileChooserWidget FileChooser { get; set; }


        public FilePicker(Window sourceWindow,
                          DialogFlags flags,
                          FileFilter filter,
                          FileChooserAction chooserAction,
                          string title = null)
            : base(sourceWindow, flags)
        {
            //Parameters
            Filter = filter;
            PickerAction = chooserAction;

            //Init Local Vars
            string windowTitle = string.Format("{0} {1}", GeneralUtils.GetResourceByName("window_title_dialog_filepicker"),title);
            WindowSettings.Size = new Size(700, 473);
            string fileDefaultWindowIcon = AppSettings.Paths.Images + @"Icons\Windows\icon_window_select_record.png";

            Body = new Fixed();

            InitUI();

            IconButtonWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
            IconButtonWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);

            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(buttonOk, ResponseType.Ok),
                new ActionAreaButton(buttonCancel, ResponseType.Cancel)
            };

            Initialize(this, flags, fileDefaultWindowIcon, windowTitle, WindowSettings.Size, Body, actionAreaButtons);
        }

        private void InitUI()
        {
            Pango.FontDescription fontDescription = Pango.FontDescription.FromString(AppSettings.Instance.FontEntryBoxValue);

            FileChooser = new FileChooserWidget(PickerAction, "none");
            if (Filter != null) FileChooser.Filter = Filter;
            FileChooser.SetSizeRequest(WindowSettings.Size.Width - 13, WindowSettings.Size.Height - 120);
            Body.Put(FileChooser, 0, 0);
        }

        public static FileFilter GetFileFilterImages()
        {
            FileFilter filter = new FileFilter();
            filter.Name = "PNG and JPEG images";
            filter.AddMimeType("image/png");
            filter.AddPattern("*.png");
            filter.AddMimeType("image/jpeg");
            filter.AddPattern("*.jpg");
            return filter;
        }

        public static FileFilter GetFileFilterBMPImages()
        {
            FileFilter filter = new FileFilter();
            filter.Name = "BMP, PNG and JPEG images";
            filter.AddMimeType("image/png");
            filter.AddPattern("*.png");
            filter.AddMimeType("image/jpeg");
            filter.AddPattern("*.jpg");
            filter.AddMimeType("image/bmp");
            filter.AddPattern("*.bmp");
            return filter;
        }

        public static FileFilter GetFileFilterAll()
        {
            FileFilter filter = new FileFilter();

            filter.Name = "All";
            filter.AddPattern("*.*");

            return filter;
        }


        public static FileFilter GetFileFilterPDF()
        {
            FileFilter filter = new FileFilter();

            filter.Name = "PDF Files";
            filter.AddMimeType("application/pdf");
            filter.AddPattern("*.pdf");

            return filter;
        }

        public static FileFilter GetFileFilterImportExport()
        {

            FileFilter filter = new FileFilter();

            filter.Name = "Import/export xls";
            filter.AddMimeType("application/octet-stream");
            filter.AddPattern("*.xls");
            filter.AddPattern("*.xlsx");
            return filter;
        }

        public static string GetSaveFilePath(Window sourceWindow, string title, string defaultFileName)
        {
            FilePicker picker = new FilePicker(sourceWindow,
                                               DialogFlags.DestroyWithParent,
                                               GetFileFilterAll(),
                                               FileChooserAction.Save,
                                               title);

            picker.FileChooser.SelectMultiple = false;
            picker.FileChooser.CurrentName = defaultFileName;

            var result = (ResponseType)picker.Run();

            if (result != ResponseType.Ok || Directory.Exists(picker.FileChooser.Filename))
            {
                picker.Destroy();
                return null;
            }

            string backupFileDestination = picker.FileChooser.Filename;
            picker.Destroy();

            return backupFileDestination;
        }

        public static string GetOpenFilePath(Window sourceWindow,
                                             string title,
                                             FileFilter filter)
        {
            FilePicker picker = new FilePicker(sourceWindow,
                                               DialogFlags.DestroyWithParent,
                                               filter,
                                               FileChooserAction.Open,
                                               title);

            picker.FileChooser.SelectMultiple = false;

            var result = (ResponseType)picker.Run();
            if (result != ResponseType.Ok)
            {
                picker.Destroy();
                return null;
            }

            string path = picker.FileChooser.Filename;
            picker.Destroy();
            return path;
        }
    }
}
