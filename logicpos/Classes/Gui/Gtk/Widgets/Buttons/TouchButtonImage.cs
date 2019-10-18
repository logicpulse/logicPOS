using Gtk;
using logicpos.App;
using System;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.Widgets.Buttons
{
    public class TouchButtonImage : TouchButtonBase
    {
        private const int _BUTTON_INNER_BORDER = 7;
        private const int _BUTTON_TEXT_OVERLAY_INNER_MARGIN = 0;
        private const int _BUTTON_TEXT_ALPHA_OVERLAY = 250;
        private bool _useCachedImages = Utils.UseCache();
        private bool _useVatAutocompletee = Utils.UseVatAutocomplete();
        private string _pathCache = FrameworkUtils.OSSlash(Convert.ToString(GlobalFramework.Path["cache"]));

        public Widget widget;

        public TouchButtonImage(String name)
            : base(name)
        {
        }

        public TouchButtonImage(String name, System.Drawing.Color color, String labelText, int fontSize, String image, String overlay, int width, int height)
            : base(name)
        {
            InitObject(name, color, labelText, fontSize, image, overlay, width, height);
            base.InitObject(name, color, widget, width, height);
        }

        public void InitObject(String name, System.Drawing.Color color, String labelText, int fontSize, String image, String overlay, int width, int height)
        {
            String stringImageFilename = _pathCache + name + ".png";
            System.Drawing.Bitmap bitmap;

            if (_useCachedImages && File.Exists(stringImageFilename))
            {
                //Load cached Thumbnail
                bitmap = new System.Drawing.Bitmap(stringImageFilename);
            }
            else
            {
                //createThumbnail
                bitmap = CreateThumbnail(name, color, labelText, fontSize, image, overlay, width, height);
            }

            //create pixBuf + gtkImage
            Gdk.Pixbuf pixBuf = Utils.ImageToPixbuf(bitmap);
            Image gtkImage = new Image(pixBuf);
            //free resources
            bitmap.Dispose();
            pixBuf.Dispose();

            widget = gtkImage;
        }

        private System.Drawing.Bitmap CreateThumbnail(String name, System.Drawing.Color color, String labelText, int fontSize, String image, String overlay, int width, int height)
        {
            bool debug = false;

            System.Drawing.Size targetImageSize = new System.Drawing.Size(width - (_BUTTON_INNER_BORDER * 2), height - (_BUTTON_INNER_BORDER * 2));
            String stringResolution = "";

            //Create a Working Bitmap
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(targetImageSize.Width, targetImageSize.Height);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
            //Required else creates Bold Fonts Bug.....q afinal apenas tinha a ver c o nivel de transparencia da imagem e nao do code, umas horas a procura deste bug improvável...
            graphics.Clear(color);

#pragma warning disable
            if (debug)
            {
                stringResolution = bitmap.HorizontalResolution + "x" + bitmap.VerticalResolution;
                bitmap.Save(FrameworkUtils.OSSlash(GlobalFramework.Path["temp"] + name + "_1_new_" + stringResolution + ".png"));
            };
#pragma warning restore

            //Render Image Over bitmap
            if (image != string.Empty)
            {
                if (File.Exists(image))
                {
                    try
                    {
                        System.Drawing.Image imageButton = new System.Drawing.Bitmap(image);
                        //_log.Debug(string.Format("CreateThumbnail(): imageButton: {0}x{1} resize to targetImageSize: {2}x{3}", imageButton.Width, imageButton.Height, targetImageSize.Width, targetImageSize.Height));

#pragma warning disable
                        if (debug)
                        {
                            stringResolution = imageButton.HorizontalResolution + "x" + imageButton.VerticalResolution;
                            imageButton.Save(FrameworkUtils.OSSlash(GlobalFramework.Path["temp"] + name + "_2_image_" + stringResolution + ".png"));
                        };
#pragma warning restore

                        imageButton = Utils.ResizeAndCrop(imageButton, targetImageSize);
                        System.Drawing.Graphics.FromImage(bitmap).DrawImage(imageButton, 0, 0);

#pragma warning disable
                        if (debug)
                        {
                            stringResolution = bitmap.HorizontalResolution + "x" + bitmap.VerticalResolution;
                            bitmap.Save(FrameworkUtils.OSSlash(GlobalFramework.Path["temp"] + name + "_3_image_resized_" + stringResolution + ".png"));
                        };
#pragma warning restore
                    }
                    catch (FileNotFoundException ex)
                    {
                        _log.Error(string.Format("CreateThumbnail(): [{0}]", ex.Message), ex);
                    }
                }
            }

            //Render Overlay Over bitmap
            if (overlay != string.Empty)
            {
                if (File.Exists(overlay))
                {
                    try
                    {
                        System.Drawing.Image imageOverlay = new System.Drawing.Bitmap(overlay);
                        //System.Drawing.Image imageOverlay = System.Drawing.Image.FromFile(overlay);

#pragma warning disable
                        if (debug)
                        {
                            stringResolution = imageOverlay.HorizontalResolution + "x" + imageOverlay.VerticalResolution;
                            imageOverlay.Save(FrameworkUtils.OSSlash(GlobalFramework.Path["temp"] + name + "_4_overlay_" + stringResolution + ".png"));
                        };
#pragma warning restore

                        imageOverlay = Utils.ResizeAndCrop(imageOverlay, targetImageSize);
                        System.Drawing.Graphics.FromImage(bitmap).DrawImage(imageOverlay, 0, 0);

#pragma warning disable
                        if (debug)
                        {
                            stringResolution = bitmap.HorizontalResolution + "x" + bitmap.VerticalResolution;
                            bitmap.Save(FrameworkUtils.OSSlash(GlobalFramework.Path["temp"] + name + "_5_resized_" + stringResolution + ".png"));
                        };
#pragma warning restore
                    }
                    catch (FileNotFoundException ex)
                    {
                        _log.Error(string.Format("CreateThumbnail(): [{0}]", ex.Message), ex);
                    }
                }
            }

            //Render Text Over bitmap
            if (labelText != string.Empty)
            {
                System.Drawing.Rectangle transpRectangle = new System.Drawing.Rectangle();
                transpRectangle.Width = targetImageSize.Width - (_BUTTON_TEXT_OVERLAY_INNER_MARGIN * 4);
                transpRectangle.Height = fontSize * 2;
                transpRectangle.X = _BUTTON_TEXT_OVERLAY_INNER_MARGIN * 2;
                transpRectangle.Y = targetImageSize.Height - transpRectangle.Height - (_BUTTON_TEXT_OVERLAY_INNER_MARGIN * 2);
                Utils.ImageTextOverlay(bitmap, labelText, transpRectangle, System.Drawing.Color.Black, "Arial", fontSize, _BUTTON_TEXT_ALPHA_OVERLAY);

#pragma warning disable
                if (debug) bitmap.Save(FrameworkUtils.OSSlash(GlobalFramework.Path["temp"] + @"touchbuttonImage6_" + stringResolution + "_textoverlay.png"));
#pragma warning restore
            }

            if (_useCachedImages && !File.Exists(_pathCache + name + ".png"))
            {
                try
                {
                    bitmap.Save(_pathCache + name + ".png");
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                }
            }

            return bitmap;
        }
    }
}
