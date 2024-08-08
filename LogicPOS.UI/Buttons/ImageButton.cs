using Gtk;
using logicpos;
using LogicPOS.Settings;
using System;
using System.IO;

namespace LogicPOS.UI.Buttons
{
    public class ImageButton : CustomButton
    {
        private const int _BUTTON_INNER_BORDER = 7;
        private const int _BUTTON_TEXT_OVERLAY_INNER_MARGIN = 0;
        private const int _BUTTON_TEXT_ALPHA_OVERLAY = 250;
        private readonly bool _useCachedImages = Utils.UseCache();
        private readonly bool _useVatAutocompletee = Utils.UseVatAutocomplete();
        private readonly string _pathCache = Convert.ToString(PathsSettings.Paths["cache"]);

        public ImageButton(ButtonSettings settings)
            : base(settings)
        {
            _settings.Widget = CreateWidget();
            Initialize();
        }

        public Widget CreateWidget()
        {
            string stringImageFilename = _pathCache + _settings.Name + ".png";
            System.Drawing.Bitmap bitmap;

            if (_useCachedImages && File.Exists(stringImageFilename))
            {
                //Load cached Thumbnail
                bitmap = new System.Drawing.Bitmap(stringImageFilename);
            }
            else
            {
                //createThumbnail
                bitmap = CreateThumbnail(_settings.Name,
                                         _settings.BackgroundColor,
                                         _settings.Text,
                                         _settings.FontSize,
                                         _settings.Image,
                                         _settings.Overlay,
                                         _settings.ButtonSize.Width,
                                         _settings.ButtonSize.Height);
            }

            //create pixBuf + gtkImage
            Gdk.Pixbuf pixBuf = Utils.ImageToPixbuf(bitmap);
            Image gtkImage = new Image(pixBuf);
            //free resources
            bitmap.Dispose();
            pixBuf.Dispose();

            return gtkImage;
        }

        private System.Drawing.Bitmap CreateThumbnail(string name,
                                                      System.Drawing.Color color,
                                                      string labelText,
                                                      int fontSize,
                                                      string image,
                                                      string overlay,
                                                      int width,
                                                      int height)
        {
            bool debug = false;

            System.Drawing.Size targetImageSize = new System.Drawing.Size(width - _BUTTON_INNER_BORDER * 2, height - _BUTTON_INNER_BORDER * 2);
            string stringResolution = "";

            //Create a Working Bitmap
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(targetImageSize.Width, targetImageSize.Height);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
            //Required else creates Bold Fonts Bug.....q afinal apenas tinha a ver c o nivel de transparencia da imagem e nao do code, umas horas a procura deste bug improvável...
            graphics.Clear(color);


            if (debug)
            {
                stringResolution = bitmap.HorizontalResolution + "x" + bitmap.VerticalResolution;
                bitmap.Save(PathsSettings.TempFolderLocation + name + "_1_new_" + stringResolution + ".png");
            };

            //Render Image Over bitmap
            if (image != string.Empty)
            {
                if (File.Exists(image))
                {

                    System.Drawing.Image imageButton = new System.Drawing.Bitmap(image);
                    //_logger.Debug(string.Format("CreateThumbnail(): imageButton: {0}x{1} resize to targetImageSize: {2}x{3}", imageButton.Width, imageButton.Height, targetImageSize.Width, targetImageSize.Height));

                    if (debug)
                    {
                        stringResolution = imageButton.HorizontalResolution + "x" + imageButton.VerticalResolution;
                        imageButton.Save(PathsSettings.TempFolderLocation + name + "_2_image_" + stringResolution + ".png");
                    };


                    imageButton = Utils.ResizeAndCrop(imageButton, targetImageSize);
                    System.Drawing.Graphics.FromImage(bitmap).DrawImage(imageButton, 0, 0);


                    if (debug)
                    {
                        stringResolution = bitmap.HorizontalResolution + "x" + bitmap.VerticalResolution;
                        bitmap.Save(PathsSettings.TempFolderLocation + name + "_3_image_resized_" + stringResolution + ".png");
                    };

                }
            }

            //Render Overlay Over bitmap
            if (overlay != string.Empty)
            {
                if (File.Exists(overlay))
                {

                    System.Drawing.Image imageOverlay = new System.Drawing.Bitmap(overlay);
                    //System.Drawing.Image imageOverlay = System.Drawing.Image.FromFile(overlay);

                    if (debug)
                    {
                        stringResolution = imageOverlay.HorizontalResolution + "x" + imageOverlay.VerticalResolution;
                        imageOverlay.Save(PathsSettings.TempFolderLocation + name + "_4_overlay_" + stringResolution + ".png");
                    };

                    imageOverlay = Utils.ResizeAndCrop(imageOverlay, targetImageSize);
                    System.Drawing.Graphics.FromImage(bitmap).DrawImage(imageOverlay, 0, 0);

                    if (debug)
                    {
                        stringResolution = bitmap.HorizontalResolution + "x" + bitmap.VerticalResolution;
                        bitmap.Save(PathsSettings.TempFolderLocation + name + "_5_resized_" + stringResolution + ".png");
                    };

                }
            }

            //Render Text Over bitmap
            if (labelText != string.Empty)
            {
                System.Drawing.Rectangle transpRectangle = new System.Drawing.Rectangle();
                transpRectangle.Width = targetImageSize.Width - _BUTTON_TEXT_OVERLAY_INNER_MARGIN * 4;
                transpRectangle.Height = fontSize * 2;
                transpRectangle.X = _BUTTON_TEXT_OVERLAY_INNER_MARGIN * 2;
                transpRectangle.Y = targetImageSize.Height - transpRectangle.Height - _BUTTON_TEXT_OVERLAY_INNER_MARGIN * 2;
                Utils.ImageTextOverlay(bitmap, labelText, transpRectangle, System.Drawing.Color.Black, "Arial", fontSize, _BUTTON_TEXT_ALPHA_OVERLAY);


                if (debug) bitmap.Save(PathsSettings.TempFolderLocation + @"touchbuttonImage6_" + stringResolution + "_textoverlay.png");

            }

            if (_useCachedImages && !File.Exists(_pathCache + name + ".png"))
            {
                bitmap.Save(_pathCache + name + ".png");
            }

            return bitmap;
        }
    }
}
