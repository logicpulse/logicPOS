using Gtk;
using System;
using System.IO;

namespace logicpos.Classes.Gui.Gtk.Widgets.Buttons
{
    public class TouchButtonImageEvent : EventBox
    {
        //Log4Net
        protected log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Constants
        private const int _BUTTON_TEXT_OVERLAY_INNER_MARGIN = 10;
        private const int _BUTTON_TEXT_ALPHA_OVERLAY = 0;
        //Event Handlers
        public event ButtonPressEventHandler Clicked;

        //Active and Reference Image
        private Image _image;
        private System.Drawing.Image _imageDefault;
        private System.Drawing.Image _imageClicked;
        private Gdk.Pixbuf _pixbufDefault;
        private Gdk.Pixbuf _pixbufClicked;
        private string _fontName;
        private int _fontSize;

        public TouchButtonImageEvent(
          String pName,
          String pLabel,
          String pImage,
          String pImageClicked,
          int pPaddingLeft = 0,
          int pPaddingRight = 0,
          String pFontName = "Arial",
          int pFontSize = 20
        )
        {
            //Parameters
            Name = pName;
            _fontName = pFontName;
            _fontSize = pFontSize;
            //Default
            VisibleWindow = false;
            //Local Vars
            System.Drawing.Rectangle transpRectangle = new System.Drawing.Rectangle();

            //Load Images
            if (File.Exists(pImage))
            {
                //get Image
                _imageDefault = new System.Drawing.Bitmap(pImage);
                //Prepare Rectangle
                transpRectangle = new System.Drawing.Rectangle(
                  _BUTTON_TEXT_OVERLAY_INNER_MARGIN + pPaddingLeft,
                  _BUTTON_TEXT_OVERLAY_INNER_MARGIN,
                  _imageDefault.Width - (_BUTTON_TEXT_OVERLAY_INNER_MARGIN * 2) - pPaddingLeft - pPaddingRight,
                  _imageDefault.Height - (_BUTTON_TEXT_OVERLAY_INNER_MARGIN * 2)
                );
                //Final Image
                _imageDefault = Utils.ImageTextOverlay(_imageDefault, pLabel, transpRectangle, System.Drawing.Color.White, _fontName, _fontSize, _BUTTON_TEXT_ALPHA_OVERLAY);
                //Assign PixBuf
                _pixbufDefault = Utils.ImageToPixbuf(_imageDefault);
            }

            if (File.Exists(pImageClicked))
            {
                //get Image
                _imageClicked = new System.Drawing.Bitmap(pImageClicked);
                //Prepare Rectangle
                //Prepare Rectangle
                transpRectangle = new System.Drawing.Rectangle(
                  _BUTTON_TEXT_OVERLAY_INNER_MARGIN + pPaddingLeft,
                  _BUTTON_TEXT_OVERLAY_INNER_MARGIN,
                  _imageDefault.Width - (_BUTTON_TEXT_OVERLAY_INNER_MARGIN * 2) - pPaddingLeft - pPaddingRight,
                  _imageDefault.Height - (_BUTTON_TEXT_OVERLAY_INNER_MARGIN * 2)
                );        //Final Image
                _imageClicked = Utils.ImageTextOverlay(_imageClicked, pLabel, transpRectangle, System.Drawing.Color.White, _fontName, _fontSize, _BUTTON_TEXT_ALPHA_OVERLAY);
                //Assign PixBuf
                _pixbufClicked = Utils.ImageToPixbuf(_imageClicked);
            }

            if (_imageDefault != null)
            {
                _image = new Image(_pixbufDefault);
                Add(_image);
                SetSizeRequest(_image.WidthRequest, _image.HeightRequest);
            }

            //Events
            ButtonPressEvent += TouchButtonImageEvent_ButtonPressEvent;
            ButtonReleaseEvent += TouchButtonImageEvent_ButtonReleaseEvent;
        }

        void TouchButtonImageEvent_ButtonPressEvent(object o, ButtonPressEventArgs args)
        {
            _image.Pixbuf = _pixbufClicked;
            if (Clicked != null) Clicked(o, args);
        }

        void TouchButtonImageEvent_ButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
        {
            _image.Pixbuf = _pixbufDefault;
        }
    }
}
