using Gtk;
using logicpos;
using LogicPOS.UI.Services;
using LogicPOS.UI.Settings;
using System;
using System.IO;

namespace LogicPOS.UI.Buttons
{
    public class ImageButton : CustomButton
    {
        private const int _BUTTON_INNER_BORDER = 7;
        private const int _BUTTON_TEXT_OVERLAY_INNER_MARGIN = 0;
        private const int _BUTTON_TEXT_ALPHA_OVERLAY = 250;

        public ImageButton(ButtonSettings settings)
            : base(settings)
        {
            _settings.Widget = CreateWidget();
            Initialize();
        }

        public Widget CreateWidget()
        {
            System.Drawing.Bitmap bitmap;

            bitmap = CreateThumbnail(_settings.Name,
                                     _settings.BackgroundColor,
                                     _settings.Text,
                                     _settings.FontSize,
                                     _settings.Image,
                                     _settings.Overlay,
                                     _settings.ButtonSize.Width,
                                     _settings.ButtonSize.Height);


            Gdk.Pixbuf pixBuf = Utils.ImageToPixbuf(bitmap);
            Image gtkImage = new Image(pixBuf);

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
            System.Drawing.Size targetImageSize = new System.Drawing.Size(width - _BUTTON_INNER_BORDER * 2, height - _BUTTON_INNER_BORDER * 2);

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(targetImageSize.Width, targetImageSize.Height);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
        
            graphics.Clear(color);

            if (image != string.Empty)
            {
                if (File.Exists(image))
                {

                    System.Drawing.Image imageButton = new System.Drawing.Bitmap(image);

                    imageButton = Utils.ResizeAndCrop(imageButton, targetImageSize);
                    System.Drawing.Graphics.FromImage(bitmap).DrawImage(imageButton, 0, 0);

                }
            }

            if (overlay != string.Empty)
            {
                if (File.Exists(overlay))
                {

                    System.Drawing.Image imageOverlay = new System.Drawing.Bitmap(overlay);

                    imageOverlay = Utils.ResizeAndCrop(imageOverlay, targetImageSize);
                    System.Drawing.Graphics.FromImage(bitmap).DrawImage(imageOverlay, 0, 0);


                }
            }

            if (labelText != string.Empty)
            {
                System.Drawing.Rectangle transpRectangle = new System.Drawing.Rectangle();
                transpRectangle.Width = targetImageSize.Width - _BUTTON_TEXT_OVERLAY_INNER_MARGIN * 4;
                transpRectangle.Height = fontSize * 2;
                transpRectangle.X = _BUTTON_TEXT_OVERLAY_INNER_MARGIN * 2;
                transpRectangle.Y = targetImageSize.Height - transpRectangle.Height - _BUTTON_TEXT_OVERLAY_INNER_MARGIN * 2;
                Utils.ImageTextOverlay(bitmap, labelText, transpRectangle, System.Drawing.Color.Black, "Arial", fontSize, _BUTTON_TEXT_ALPHA_OVERLAY);

            }

            return bitmap;
        }
    }
}
