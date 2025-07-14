using Gtk;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Logic.Others;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Application.Screen;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using Serilog;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Text.RegularExpressions;

namespace logicpos
{
    internal static class Utils
    {

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
                Log.Error(ex,"Exception");
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
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(pImage, 0, 0, pSize.Width, pSize.Height);
                graphics.Dispose();

                return bitmap;
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

                //Log.Debug(
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
                return bmpCrop;
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Exception");
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

            //Log.Debug(
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
                targetResizeImageWidth = (int)(iHelper1 * (float)iHelper2 / iHelper3);
                targetResizeImageHeight = pTargetImageHeight;
                //required PROTECTION, para nunca calacular abaixo da targetImageWight, senao qnd as imagens sao mais pequenas da problemas
                if (targetResizeImageWidth < pTargetImageWidth)
                {
                    targetResizeImageWidth = pTargetImageWidth;
                    targetResizeImageHeight = (int)(pImageHeight * (float)pTargetImageWidth / pImageWidth);
                }
            }
            else
            {
                iHelper1 = pImageHeight;
                iHelper2 = pTargetImageWidth;
                iHelper3 = pImageWidth;
                targetResizeImageWidth = pTargetImageWidth;
                targetResizeImageHeight = (int)(iHelper1 * (float)iHelper2 / iHelper3);
                //required PROTECTION, para nunca calacular abaixo da targetImageHeight, senao qnd as imagens sao mais pequenas da problemas
                if (targetResizeImageHeight < pTargetImageHeight)
                {
                    targetResizeImageWidth = (int)(pImageWidth * (float)pTargetImageHeight / pImageHeight);
                    targetResizeImageHeight = pTargetImageHeight;
                }
            };

            //Log.Debug(
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
                Log.Error(ex,"Exception");
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
            return string.Format("{0} : {1}", AppSettings.AppName, pTitle);
        }

      

        // Used to get the final Resolution for Render template, it uses some stuff from config and detected ScreenSize to get guest best resolution for themes

       

       

        public static Dialog CreateSplashScreen(string backupProcess = "")
        {
            string loadingImage = AppSettings.Paths.Images + @"Other\working.gif";

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
            if (LogicPOSApp.LoadingDialog != null)
            {
                LogicPOSApp.LoadingDialog.Destroy();
            }
        }


        public static string GetThemeFileLocation(string pFile)
        {
            return string.Format(@"{0}{1}\{2}", AppSettings.Paths.Themes, AppSettings.AppTheme, pFile);
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
                Log.Error(string.Format("Missing Theme[{0}] Image: [{1}]", AppSettings.AppTheme, fileImageBackground));
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
            bool useBaseDialogWindowMask = AppSettings.Instance.UseBaseDialogWindowMask;

            //if (GlobalApp.DialogPosKeyboard == null)
            //{
            //Chama teclado certo na janela de artigos
            switch (pMode)
            {
                case KeyboardMode.Alfa:
                case KeyboardMode.AlfaNumeric:
                    //On Create SourceWindow is always GlobalApp.WindowPos else if its a Dialog, when it is destroyed, in Memory Keyboard is Destroyed too, this way we keep it in Memory
                    LogicPOSApp.DialogPosKeyboard = new PosKeyboardDialog(POSWindow.Instance, DialogFlags.DestroyWithParent, KeyboardMode.AlfaNumeric, pInitialValue, pRegExRule);
                    break;

                case KeyboardMode.Numeric:
                    LogicPOSApp.DialogPosKeyboard = new PosKeyboardDialog(POSWindow.Instance, DialogFlags.DestroyWithParent, KeyboardMode.Numeric, pInitialValue, pRegExRule);
                    break;
                default: break;
            }
            //}
            //else
            //{
            //pInitialValue, pRegExRule
            LogicPOSApp.DialogPosKeyboard.Text = pInitialValue;
            LogicPOSApp.DialogPosKeyboard.Rule = pRegExRule;

            //Fix TransientFor, ALT+TABS
            if (useBaseDialogWindowMask)
            {
                LogicPOSApp.DialogPosKeyboard.TransientFor = LogicPOSApp.DialogPosKeyboard.WindowSettings.Mask;
                LogicPOSApp.DialogPosKeyboard.WindowSettings.Mask.TransientFor = parentWindow;
                LogicPOSApp.DialogPosKeyboard.WindowSettings.Mask.Show();
            }
            else
            {
                //Now we can change its TransientFor
                LogicPOSApp.DialogPosKeyboard.TransientFor = parentWindow;
            }
            //}

            //Always Start Validated, else Only Construct start Validated
            LogicPOSApp.DialogPosKeyboard.TextEntry.Validate();
            //Put Cursor in End
            LogicPOSApp.DialogPosKeyboard.TextEntry.Position = LogicPOSApp.DialogPosKeyboard.TextEntry.Text.Length;
            LogicPOSApp.DialogPosKeyboard.TextEntry.GrabFocus();
            int response = LogicPOSApp.DialogPosKeyboard.Run();
            string result;
            if (response == (int)ResponseType.Ok)
            {
                result = LogicPOSApp.DialogPosKeyboard.Text;
            }
            else
            {
                result = null;
            }

            LogicPOSApp.DialogPosKeyboard.TransientFor = POSWindow.Instance;

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
                    AppSettings.License.LicenseData.HardwareId =CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "HardwareId", "Empresa Demonstração"), true);
                    AppSettings.License.LicenseData.Company = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Company", "NIF Demonstração"), true);
                    AppSettings.License.LicenseData.FiscalNumber = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Nif", "Morada Demonstração"), true);
                    AppSettings.License.LicenseData.Address = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Address", "mail@demonstracao.tld"), true);
                    AppSettings.License.LicenseData.Email = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Email", string.Empty), true);
                    AppSettings.License.LicenseData.Telephone = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Telephone", "Telefone Demonstração"), true);
                    AppSettings.License.LicenseData.Reseller = CryptographyUtils.Decrypt(iNIFile.GetValue("Licence", "Reseller", "LogicPulse"), true);
                    //Test
                    if (pDebug)
                    {
                        Log.Debug(string.Format("{0}:{1}", "HardwareId", AppSettings.License.LicenseData.HardwareId));
                        Log.Debug(string.Format("{0}:{1}", "Company", AppSettings.License.LicenseData.Company));
                        Log.Debug(string.Format("{0}:{1}", "Nif", AppSettings.License.LicenseData.FiscalNumber));
                        Log.Debug(string.Format("{0}:{1}", "Address", AppSettings.License.LicenseData.Address));
                        Log.Debug(string.Format("{0}:{1}", "Email", AppSettings.License.LicenseData.Email));
                        Log.Debug(string.Format("{0}:{1}", "Telephone", AppSettings.License.LicenseData.Telephone));
                        Log.Debug(string.Format("{0}:{1}", "Reseller", AppSettings.License.LicenseData.Reseller));
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
                Log.Error(ex,"Exception");
            }

            return result;
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
                Log.Debug(string.Format("Port already in use: [{0}]", portName));
                return false;
            }
        }

    }
}