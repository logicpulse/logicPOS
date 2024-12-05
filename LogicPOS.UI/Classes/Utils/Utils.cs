using Gtk;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Logic.Others;
using LogicPOS.Modules;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace logicpos
{
    internal static class Utils
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Request Text Dialog

        //Helper Response Object
        public class ResponseText
        {
            public ResponseType ResponseType;
            public string Text;

            public ResponseText() { }
            public ResponseText(ResponseType pResponseType, string pText)
            {
                ResponseType = pResponseType;
                Text = pText;
            }
        }

        public static ResponseText GetInputText(Window parentWindow, DialogFlags pDialogFlags, string pWindowIcon, string pEntryLabel, string pDefaultValue, string pRule, bool pRequired)
        {
            return GetInputText(parentWindow, pDialogFlags, GeneralUtils.GetResourceByName("window_title_default_input_text_dialog"), pWindowIcon, pEntryLabel, pDefaultValue, pRule, pRequired);
        }

        public static ResponseText GetInputText(Window parentWindow, DialogFlags pDialogFlags, string pWindowTitle, string pWindowIcon, string pEntryLabel, string pDefaultValue, string pRule, bool pRequired)
        {
            ResponseText result = new ResponseText();
            result.ResponseType = ResponseType.Cancel;

            //Prepare Dialog
            PosInputTextDialog dialog = new PosInputTextDialog(parentWindow, pDialogFlags, pWindowTitle, pWindowIcon, pEntryLabel, pDefaultValue, pRule, pRequired);

            //Call Dialog
            try
            {
                result.ResponseType = (ResponseType)dialog.Run();
                result.Text = dialog.Value;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            finally
            {
                dialog.Destroy();
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Images 

        public static Gdk.Pixmap FileToPixmap(string pFilename)
        {
            if (pFilename != null && File.Exists(pFilename))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    Gdk.Pixbuf image = new Gdk.Pixbuf(pFilename);
                    Gdk.Pixmap pixmap, pixmap_mask;
                    image.RenderPixmapAndMask(out pixmap, out pixmap_mask, 255);
                    //this.Style.SetBgPixmap(StateType.Normal, pixmap);
                    return pixmap;
                }
            }
            else
            {
                return null;
            }
        }

        public static Gdk.Pixmap PixbufToPixmap(Gdk.Pixbuf pFilename)
        {
            if (pFilename != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    Gdk.Pixbuf image = pFilename;
                    Gdk.Pixmap pixmap, pixmap_mask;
                    image.RenderPixmapAndMask(out pixmap, out pixmap_mask, 255);
                    //this.Style.SetBgPixmap(StateType.Normal, pixmap);
                    return pixmap;
                }
            }
            else
            {
                return null;
            }
        }

        //Convert System.Drawing.Image to Gdk.Image
        public static Gdk.Pixbuf ImageToPixbuf(System.Drawing.Image pImage)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                pImage.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(stream);
                return pixbuf;
            }
        }

        //Returns a PixBuf from File Path
        public static Gdk.Pixbuf FileToPixBuf(string pFilename)
        {
            if (pFilename != null && File.Exists(pFilename))
            {
                var buffer = File.ReadAllBytes(pFilename);
                return new Gdk.Pixbuf(buffer);
            }
            else
            {
                return null;
            }
        }

        //Returns a Resized PixBuf from File Path - Usefull for FileChooserButtons
        public static Gdk.Pixbuf ResizeAndCropFileToPixBuf(string pFilename, Size pSize)
        {
            if (pFilename != null && File.Exists(pFilename))
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(pFilename);
                image = ResizeAndCrop(image, new System.Drawing.Size(pSize.Width, pSize.Height));
                return ImageToPixbuf(image);
            }
            else
            {
                return null;
            }
        }

        //ResizeImage
        public static System.Drawing.Image ResizeImage(System.Drawing.Image pImage, Size pSize)
        {
            try
            {
                //skip dont require processing
                if (pImage.Width == pSize.Width && pImage.Height == pSize.Height) return pImage;

                int sourceWidth = pImage.Width;
                int sourceHeight = pImage.Height;

                Bitmap bitmap = new Bitmap(pSize.Width, pSize.Height);
                Graphics graphics = Graphics.FromImage((System.Drawing.Image)bitmap);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(pImage, 0, 0, pSize.Width, pSize.Height);
                graphics.Dispose();

                return (System.Drawing.Image)bitmap;
            }
            catch
            {
                return pImage;
            }
        }

        //Crop
        public static System.Drawing.Image CropImage(System.Drawing.Image pImage, Rectangle pCropArea)
        {
            if (pImage.Width == pCropArea.Width && pImage.Height == pCropArea.Height)
                return pImage;

            try
            {
                Bitmap bmpImage = new Bitmap(pImage);

                //_logger.Debug(
                //  "CropImage():" +
                //  ": image.Width:" + pImage.Width +
                //  ", image.Height:" + pImage.Height +
                //  ", bmpImage.Width:" + bmpImage.Width +
                //  ", bmpImage.Height:" + bmpImage.Height +
                //  ", cropArea.X:" + pCropArea.X +
                //  ", cropArea.Y:" + pCropArea.Y +
                //  ", cropArea.Width:" + pCropArea.Width +
                //  ", cropArea.Height:" + pCropArea.Height
                //);

                Bitmap bmpCrop = bmpImage.Clone(pCropArea, bmpImage.PixelFormat);
                return (System.Drawing.Image)(bmpCrop);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return pImage;
            }
        }

        //Shared function ResizeAndCrop to reuse both ResizeImage and CropImage
        public static System.Drawing.Image ResizeAndCrop(System.Drawing.Image pImage, Size pSize)
        {
            //store target resize size
            Size resizeImageSize = new Size();
            int iCropOffsetX, iCropOffsetY;

            //find optimal ResizeSize
            resizeImageSize = ImageFindResizeSize(pImage.Width, pImage.Height, pSize.Width, pSize.Height);

            //ResizeImage
            pImage = ResizeImage(pImage, resizeImageSize);
            //drawingOverlay.Save(image + ".resized.png");
            //crop image
            iCropOffsetX = (resizeImageSize.Width - pSize.Width) / 2;
            iCropOffsetY = (resizeImageSize.Height - pSize.Height) / 2;
            Rectangle rectCropArea = new Rectangle(iCropOffsetX, iCropOffsetY, pSize.Width, pSize.Height);
            pImage = CropImage(pImage, rectCropArea);
            //drawingImage.Save(image + ".croped.png");

            //_logger.Debug(
            //  "ResizeAndCrop()" +
            //  ": resizeImageSize.Width:" + resizeImageSize.Width +
            //  ", resizeImageSize.Height:" + resizeImageSize.Height +
            //  ", iCropOffsetX:" + iCropOffsetX +
            //  ", iCropOffsetY:" + iCropOffsetY +
            //  ", Final image.Width:" + pImage.Width +
            //  ", Final image.Height:" + pImage.Height
            //);

            return pImage;
        }

        //Find Target Resize size based on Image Size and Target Size, used before Crop
        public static Size ImageFindResizeSize(int pImageWidth, int pImageHeight, int pTargetImageWidth, int pTargetImageHeight)
        {
            int targetResizeImageWidth, targetResizeImageHeight, iHelper1, iHelper2, iHelper3;

            if (pImageWidth > pImageHeight)
            {
                iHelper1 = pImageWidth;
                iHelper2 = pTargetImageHeight;
                iHelper3 = pImageHeight;
                targetResizeImageWidth = (int)((float)iHelper1 * (float)iHelper2 / (float)iHelper3);
                targetResizeImageHeight = pTargetImageHeight;
                //required PROTECTION, para nunca calacular abaixo da targetImageWight, senao qnd as imagens sao mais pequenas da problemas
                if (targetResizeImageWidth < pTargetImageWidth)
                {
                    targetResizeImageWidth = pTargetImageWidth;
                    targetResizeImageHeight = (int)((float)pImageHeight * (float)pTargetImageWidth / (float)pImageWidth);
                }
            }
            else
            {
                iHelper1 = pImageHeight;
                iHelper2 = pTargetImageWidth;
                iHelper3 = pImageWidth;
                targetResizeImageWidth = pTargetImageWidth;
                targetResizeImageHeight = (int)((float)iHelper1 * (float)iHelper2 / (float)iHelper3);
                //required PROTECTION, para nunca calacular abaixo da targetImageHeight, senao qnd as imagens sao mais pequenas da problemas
                if (targetResizeImageHeight < pTargetImageHeight)
                {
                    targetResizeImageWidth = (int)((float)pImageWidth * (float)pTargetImageHeight / (float)pImageHeight);
                    targetResizeImageHeight = pTargetImageHeight;
                }
            };

            //_logger.Debug(
            //  "ImageFindResizeSize()" +
            //  ": imageWidth=" + pImageWidth +
            //  ", imageHeight=" + pImageHeight +
            //  ", targetImageWidth:" + pTargetImageWidth +
            //  ", targetImageHeight:" + pTargetImageHeight +
            //  ", targetResizeImageWidth:" + targetResizeImageWidth +
            //  ", targetResizeImageHeight:" + targetResizeImageHeight +
            //  ", iHelper1:" + iHelper1 +
            //  ", iHelper2:" + iHelper2 +
            //  ", iHelper3:" + iHelper3
            //);

            return new Size(targetResizeImageWidth, targetResizeImageHeight);
        }

        //Render text over Image 
        public static System.Drawing.Image ImageTextOverlay(System.Drawing.Image pImage, string pLabel, Rectangle pTranspRectangle, Color pFontColor, string pFontName = "Tahoma", int pFontSize = 12, int pTransparentLevel = 128)
        {
            Graphics g = Graphics.FromImage(pImage);
            SolidBrush semiTransBrushWhite = new SolidBrush(Color.FromArgb(pTransparentLevel, 255, 255, 255));
            SolidBrush semiTransBrushBlack = new SolidBrush(Color.FromArgb(255, pFontColor.R, pFontColor.G, pFontColor.B));

            g.FillRectangle(semiTransBrushWhite, pTranspRectangle);
            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Center;
            strFormat.LineAlignment = StringAlignment.Center;
            Font font = new Font(pFontName, pFontSize, FontStyle.Regular);
            g.DrawString(pLabel, font, semiTransBrushBlack, pTranspRectangle, strFormat);

            return pImage;
        }

        public static Point StringToPosition(string position)
        {
            string[] splitted = position.Split(',');
            short x = Convert.ToInt16(splitted[0]);
            short y = Convert.ToInt16(splitted[1]);
            return new Point(x, y);
        }

        public static TableConfig StringToTableConfig(string pTableConfig)
        {
            string[] splitted = pTableConfig.Split(',');
            TableConfig resultTableConfig = new TableConfig();
            try
            {
                resultTableConfig = new TableConfig(Convert.ToUInt16(splitted[0]), Convert.ToUInt16(splitted[1]));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return resultTableConfig;
            }
            return resultTableConfig;
        }

        public static char UnicodeHexadecimalStringToChar(string pInput)
        {
            char result = (char)int.Parse(pInput, NumberStyles.HexNumber);
            return result;
        }


        //Check if is letter with RegEx, better than Char.IsLetter that returns º and ª too
        public static bool IsLetter(char input)
        {
            string pattern = @"^[a-zA-ZçÇÁáÉéÍóÚúÀàÈèÌìÒòÙùÃãÕõÂâÊêÎîÔôÛû]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(Convert.ToString(input));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Windows

        public static string GetWindowTitle(string pTitle)
        {
            //return string.Format("{0} {1} : {2}", SettingsApp.AppName, FrameworkUtils.ProductVersion, pTitle);
            return string.Format("{0} : {1}", LogicPOSSettings.AppName, pTitle);
        }

        /// <summary>
        /// This method is responsible for dinamic dialog title creation, based on option chosen by user on BackOffice action menu.
        /// Related to #IN009039.
        /// </summary>
        /// <param name="pTitle"></param>
        /// <param name="dialogMode"></param>
        /// <returns></returns>
        public static string GetWindowTitle(string dialogWindowTitle, logicpos.Classes.Enums.Dialogs.DialogMode dialogMode)
        {
            string action = string.Empty;

            switch (dialogMode)
            {
                case Classes.Enums.Dialogs.DialogMode.Insert:
                    action = GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_insert");
                    break;
                case Classes.Enums.Dialogs.DialogMode.Update:
                    action = GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_update");
                    break;
                case Classes.Enums.Dialogs.DialogMode.View:
                    action = GeneralUtils.GetResourceByName("widget_generictreeviewnavigator_view");
                    break;
                default:
                    break;
            }

            return string.Format("{0} :: {1} {2}", LogicPOSSettings.AppName, action, dialogWindowTitle); // FrameworkUtils.ProductVersion
        }

        public static Size GetScreenSize()
        {
            Size result = new Size();

            // Moke Window only to extract its Resolution
            Window window = new Window("");
            Gdk.Screen screen = window.Screen;
            Gdk.Rectangle monitorGeometry = screen.GetMonitorGeometry(AppSettings.Instance.appScreen);
            result = new Size(monitorGeometry.Width, monitorGeometry.Height);
            // CleanUp
            window.Dispose();
            screen.Dispose();

            return result;
        }

        // Used to get the final Resolution for Render template, it uses some stuff from config and detected ScreenSize to get guest best resolution for themes

        public static Size GetThemeScreenSize()
        {
            return GetThemeScreenSize(GetScreenSize());
        }

        public static Size GetThemeScreenSize(Size screenSize)
        {
            Size result = screenSize;

            /* IN008023: apply "1024x768" settings as default. */
            ScreenSize screenSizeEnum = GetSupportedScreenResolution(screenSize);
            /* Implemented resolution settings: 800,600 | 1024,768 | 1280,768 | 1366,768 | 1280,1024 | 1680,1050 | 1920,1080 | 1920,1280*/

            switch (screenSizeEnum)
            {
                // Implemented
                case ScreenSize.res800x600:
                case ScreenSize.res1024x768:
                case ScreenSize.res1280x768:
                case ScreenSize.res1280x1024:
                case ScreenSize.res1366x768:
                case ScreenSize.res1680x1050:
                case ScreenSize.res1920x1080:
                    // Use Detected Value
                    break;
                case ScreenSize.res1024x600:
                case ScreenSize.res1280x720:
                    // Override Default
                    result = new Size(800, 600);
                    break;
                case ScreenSize.res1152x864:
                    // Override Default
                    //result = new Size(1280, 1024);
                    result = new Size(1024, 768);
                    break;
                case ScreenSize.res1280x800:
                case ScreenSize.res1360x768:
                    // Override Default
                    result = new Size(1280, 768);
                    break;
                //// Override Default
                //result = new Size(1366, 768);
                //break;
                case ScreenSize.res1440x900:
                case ScreenSize.res1536x864:
                case ScreenSize.res1600x900:
                    // Override Default
                    //result = new Size(1680, 1050);
                    result = new Size(1366, 768);
                    break;
                case ScreenSize.res1920x1200:
                case ScreenSize.res1920x1280:
                /* IN009047 */
                case ScreenSize.res2160x1440:
                case ScreenSize.res2560x1080:
                case ScreenSize.res2560x1440:
                case ScreenSize.res3440x1440:
                case ScreenSize.res3840x2160:
                    // Override Default
                    result = new Size(1920, 1080);
                    break;
                default:
                    /* IN008023: apply "1024x768" settings as default. */
                    //Default 1024*768
                    result = new Size(1024, 768);
                    break;
            }
            return result;
        }



        /// <summary>
        /// Method responsible for screen resolution validation and if necessary, sets the default resolution settings.
        /// <para>See also "IN008023: apply "800x600" settings as default"</para>
        /// </summary>
        /// <param name="screenSize"></param>
        /// <returns></returns>
        private static ScreenSize GetSupportedScreenResolution(Size screenSize)
        {
            ScreenSize supportedScreenSizeEnum;
            string screenSizeForEnum = string.Format("res{0}x{1}", screenSize.Width, screenSize.Height);

            try
            {
                supportedScreenSizeEnum = (ScreenSize)Enum.Parse(typeof(ScreenSize), screenSizeForEnum, true);
            }
            catch (Exception ex)
            {
                _logger.Error("ScreenSize GetSupportedScreenResolution(Size screenSize) :: " + ex.Message, ex);

                LogicPOSAppContext.DialogThreadNotify.WakeupMain();

                var messageDialog = new CustomAlert(LoginWindow.Instance)
                    .WithMessage($"ShowUnsupportedResolutionErrorAlert{screenSize.Width}, {screenSize.Height}")
                    .WithSize(new Size())
                    .ShowAlert();

                //ShowUnsupportedResolutionErrorAlert(LoginWindow.Instance, screenSize.Width, screenSize.Height);
                supportedScreenSizeEnum = ScreenSize.resDefault;
            }
            return supportedScreenSizeEnum;
        }

        public static Dialog CreateSplashScreen(string backupProcess = "")
        {
            string loadingImage = PathsSettings.ImagesFolderLocation + @"Other\working.gif";

            Dialog dialog = new Dialog("Working",
                                       null,
                                       DialogFlags.Modal | DialogFlags.DestroyWithParent);

            dialog.WindowPosition = WindowPosition.Center;

            Label labelBoot;


            labelBoot = new Label(GeneralUtils.GetResourceByName("global_load"));

            if (backupProcess != string.Empty)
            {
                labelBoot = new Label(backupProcess);
            }

            labelBoot.ModifyFont(Pango.FontDescription.FromString("Trebuchet MS 10 Bold"));
            labelBoot.ModifyFg(StateType.Normal, Color.DarkSlateGray.ToGdkColor());
            dialog.Decorated = false;
            dialog.ActionArea.Destroy();
            dialog.Fullscreen();
            Gtk.Image imageWorking = new Gtk.Image(loadingImage);
            dialog.VBox.PackStart(imageWorking);
            dialog.VBox.PackStart(labelBoot);
            dialog.ShowAll();

            return dialog;
        }

        public static void NotifyLoadingIsDone()
        {
            if (LogicPOSAppContext.LoadingDialog != null)
            {
                LogicPOSAppContext.LoadingDialog.Destroy();
            }
        }

        public static void ThreadStart(Window sourceWindow,
                                       Thread thread,
                                       string backupProcess)
        {
            LogicPOSAppContext.DialogThreadNotify = new ThreadNotify(new ReadyEvent(NotifyLoadingIsDone));
            thread.Start();

            if (sourceWindow != null)
            {
                LogicPOSAppContext.LoadingDialog = CreateSplashScreen(backupProcess);

                LogicPOSAppContext.LoadingDialog.Run();
            }
        }




        public static string GetThemeFileLocation(string pFile)
        {
            string pathThemes = PathsSettings.Paths["themes"].ToString();
            /* IN008024 */
            return string.Format(@"{0}{1}\{2}", pathThemes, GeneralSettings.AppTheme, pFile);
        }

        public static Gtk.Style GetThemeStyleBackground(string pFile)
        {
            /* IN008024 */
            string fileImageBackground = GetThemeFileLocation(pFile);

            if (fileImageBackground != null && File.Exists(fileImageBackground))
            {
                Gdk.Pixmap pixmap = FileToPixmap(fileImageBackground);

                if (pixmap != null)
                {
                    Gtk.Style style = new Style();
                    style.SetBgPixmap(StateType.Normal, pixmap);
                    return style;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                _logger.Error(string.Format("Missing Theme[{0}] Image: [{1}]", GeneralSettings.AppTheme, fileImageBackground));
                return null;
            }
        }

        public static Gtk.Style GetImageBackgroundDashboard(Gdk.Pixbuf image)
        {

            if (image == null)
            {
                return null;
            }

            Gdk.Pixmap pixmap = PixbufToPixmap(image);

            if (pixmap != null)
            {
                Gtk.Style style = new Style();
                style.SetBgPixmap(StateType.Normal, pixmap);
                return style;
            }
            else
            {
                return null;
            }
        }

        public static void ShowChangeLog(Window parentWindow)
        {
            WebClient wc = new WebClient();
            byte[] raw = wc.DownloadData("http://box.logicpulse.pt/files/changelogs/pos.txt");
           
            System.Text.Encoding iso = System.Text.Encoding.GetEncoding("ISO-8859-1");
            System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
            byte[] isoBytes = System.Text.Encoding.Convert(utf8, iso, raw);
            var message = iso.GetString(isoBytes);

            CustomAlerts.Information(parentWindow)
                        .WithSize(new Size(700, 480))
                        .WithTitleResource("change_log")
                        .WithMessage(message)
                        .ShowAlert();
        }

        public static string GetVirtualKeyBoardInput(Window parentWindow, KeyboardMode pMode, string pInitialValue, string pRegExRule)
        {
            bool useBaseDialogWindowMask = AppSettings.Instance.useBaseDialogWindowMask;

            //if (GlobalApp.DialogPosKeyboard == null)
            //{
            //Chama teclado certo na janela de artigos
            switch (pMode)
            {
                case KeyboardMode.Alfa:
                case KeyboardMode.AlfaNumeric:
                    //On Create SourceWindow is always GlobalApp.WindowPos else if its a Dialog, when it is destroyed, in Memory Keyboard is Destroyed too, this way we keep it in Memory
                    LogicPOSAppContext.DialogPosKeyboard = new PosKeyboardDialog(POSWindow.Instance, DialogFlags.DestroyWithParent, KeyboardMode.AlfaNumeric, pInitialValue, pRegExRule);
                    break;

                case KeyboardMode.Numeric:
                    LogicPOSAppContext.DialogPosKeyboard = new PosKeyboardDialog(POSWindow.Instance, DialogFlags.DestroyWithParent, KeyboardMode.Numeric, pInitialValue, pRegExRule);
                    break;
                default: break;
            }
            //}
            //else
            //{
            //pInitialValue, pRegExRule
            LogicPOSAppContext.DialogPosKeyboard.Text = pInitialValue;
            LogicPOSAppContext.DialogPosKeyboard.Rule = pRegExRule;

            //Fix TransientFor, ALT+TABS
            if (useBaseDialogWindowMask)
            {
                LogicPOSAppContext.DialogPosKeyboard.TransientFor = LogicPOSAppContext.DialogPosKeyboard.WindowSettings.Mask;
                LogicPOSAppContext.DialogPosKeyboard.WindowSettings.Mask.TransientFor = parentWindow;
                LogicPOSAppContext.DialogPosKeyboard.WindowSettings.Mask.Show();
            }
            else
            {
                //Now we can change its TransientFor
                LogicPOSAppContext.DialogPosKeyboard.TransientFor = parentWindow;
            }
            //}

            //Always Start Validated, else Only Construct start Validated
            LogicPOSAppContext.DialogPosKeyboard.TextEntry.Validate();
            //Put Cursor in End
            LogicPOSAppContext.DialogPosKeyboard.TextEntry.Position = LogicPOSAppContext.DialogPosKeyboard.TextEntry.Text.Length;
            LogicPOSAppContext.DialogPosKeyboard.TextEntry.GrabFocus();
            int response = LogicPOSAppContext.DialogPosKeyboard.Run();
            string result;
            if (response == (int)ResponseType.Ok)
            {
                result = LogicPOSAppContext.DialogPosKeyboard.Text;
            }
            else
            {
                result = null;
            }

            LogicPOSAppContext.DialogPosKeyboard.TransientFor = POSWindow.Instance;

            return result;
        }

        public static bool AssignLicence(string pFileName, bool pDebug = false)
        {
            bool result = false;

            try
            {
                StreamReader sr = null;
                try
                {
                    IniFileParser iNIFile = new IniFileParser(pFileName);

                    //Load
                    LicenseSettings.LicenseHardwareId = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "HardwareId", "Empresa Demonstração"), true);
                    LicenseSettings.LicenseCompany = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Company", "NIF Demonstração"), true);
                    LicenseSettings.LicenseNif = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Nif", "Morada Demonstração"), true);
                    LicenseSettings.LicenseAddress = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Address", "mail@demonstracao.tld"), true);
                    LicenseSettings.LicenseEmail = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Email", string.Empty), true);
                    LicenseSettings.LicenseTelephone = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Telephone", "Telefone Demonstração"), true);
                    LicenseSettings.LicenseReseller = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Reseller", "LogicPulse"), true);
                    //Test
                    if (pDebug)
                    {
                        _logger.Debug(string.Format("{0}:{1}", "HardwareId", LicenseSettings.LicenseHardwareId));
                        _logger.Debug(string.Format("{0}:{1}", "Company", LicenseSettings.LicenseCompany));
                        _logger.Debug(string.Format("{0}:{1}", "Nif", LicenseSettings.LicenseNif));
                        _logger.Debug(string.Format("{0}:{1}", "Address", LicenseSettings.LicenseAddress));
                        _logger.Debug(string.Format("{0}:{1}", "Email", LicenseSettings.LicenseEmail));
                        _logger.Debug(string.Format("{0}:{1}", "Telephone", LicenseSettings.LicenseTelephone));
                        _logger.Debug(string.Format("{0}:{1}", "Reseller", LicenseSettings.LicenseReseller));
                    }
                    iNIFile.Flush();

                    result = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (sr != null) sr.Close();
                    sr = null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        public static void StartReportsMenuFromBackOffice(Window parentWindow)
        {
            //tchial0: start Reports Menu
        }

        public static void OpenArticleStockDialog(Window parentWindow)
        {

            if (LicenseSettings.LicenseModuleStocks && ModulesSettings.StockManagementModule != null)
            {
                //tchial0: Open Stock Management
            }
            else if (PreferenceParametersService.CheckStocksMessage && !LicenseSettings.LicenseModuleStocks)
            {
                var messageDialog = new CustomAlert(parentWindow)
                    .WithMessageType(MessageType.Warning)
                    .WithButtonsType(ButtonsType.OkCancel)
                    .WithTitleResource("global_warning")
                    .WithMessageResource("global_warning_acquire_module_stocks")
                    .ShowAlert();


                if (messageDialog == ResponseType.Ok)
                {
                    Process.Start("https://logic-pos.com/");
                }

                var documentsMenu = new DocumentsMenuModal(parentWindow);
                documentsMenu.Run();
                documentsMenu.Destroy();

            }
            else
            {
                var addStockModal = new AddStockModal(parentWindow);
                addStockModal.Run();
                addStockModal.Destroy();
            }

        }

        public static bool IsPortOpen(string portName)
        {
            var port = new SerialPort(portName);
            try
            {
                port.Open();
                port.Close();
                return true;
            }
            catch (Exception)
            {
                _logger.Debug(string.Format("Port already in use: [{0}]", portName));
                return false;
            }
        }

    }
}