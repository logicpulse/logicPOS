
using System;
using System.Windows.Forms;
namespace logicpos.reports
{
    public class ManualScrollPanel : Panel
    {
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ManualScrollPanel()
        {
            //this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            VScroll = true; //this can be only done in ctor
            HScroll = true;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            //here you can do something for the MouseWheel
            base.OnMouseWheel(e);
        }

        #region ScrollEventType to SB_* messages and SB_* messages to ScrollEventType

        private const int SB_LINEUP = 0;
        private const int SB_LINEDOWN = 1;
        private const int SB_PAGEUP = 2;
        private const int SB_PAGEDOWN = 3;
        private const int SB_THUMBPOSITION = 4;
        private const int SB_THUMBTRACK = 5;
        private const int SB_TOP = 6;
        private const int SB_BOTTOM = 7;
        private const int SB_ENDSCROLL = 8;

        private int getSBFromScrollEventType(ScrollEventType type)
        {
            int result = -1;
            switch (type)
            {
                case ScrollEventType.SmallDecrement:
                    result = SB_LINEUP;
                    break;
                case ScrollEventType.SmallIncrement:
                    result = SB_LINEDOWN;
                    break;
                case ScrollEventType.LargeDecrement:
                    result = SB_PAGEUP;
                    break;
                case ScrollEventType.LargeIncrement:
                    result = SB_PAGEDOWN;
                    break;
                case ScrollEventType.ThumbTrack:
                    result = SB_THUMBTRACK;
                    break;
                case ScrollEventType.First:
                    result = SB_TOP;
                    break;
                case ScrollEventType.Last:
                    result = SB_BOTTOM;
                    break;
                case ScrollEventType.ThumbPosition:
                    result = SB_THUMBPOSITION;
                    break;
                case ScrollEventType.EndScroll:
                    result = SB_ENDSCROLL;
                    break;
                default:
                    break;
            }
            return result;
        }

        private ScrollEventType getScrollEventType(System.IntPtr wParam)
        {
            ScrollEventType result = 0;
            switch (LoWord((int)wParam))
            {
                case SB_LINEUP:
                    result = ScrollEventType.SmallDecrement;
                    break;
                case SB_LINEDOWN:
                    result = ScrollEventType.SmallIncrement;
                    break;
                case SB_PAGEUP:
                    result = ScrollEventType.LargeDecrement;
                    break;
                case SB_PAGEDOWN:
                    result = ScrollEventType.LargeIncrement;
                    break;
                case SB_THUMBTRACK:
                    result = ScrollEventType.ThumbTrack;
                    break;
                case SB_TOP:
                    result = ScrollEventType.First;
                    break;
                case SB_BOTTOM:
                    result = ScrollEventType.Last;
                    break;
                case SB_THUMBPOSITION:
                    result = ScrollEventType.ThumbPosition;
                    break;
                case SB_ENDSCROLL:
                    result = ScrollEventType.EndScroll;
                    break;
                default:
                    result = ScrollEventType.EndScroll;
                    break;
            }
            return result;
        }

        #endregion

        #region WndProd override

        private const int WM_HSCROLL = 0x114;
        private const int WM_VSCROLL = 0x115;

        protected override void WndProc(ref Message msg)
        {
          try
          {

            _log.Debug("Enter WndProc");

            if (msg.HWnd == this.Handle)
            {
              switch (msg.Msg)
              {

                case WM_VSCROLL:
                  if (msg.LParam != IntPtr.Zero)
                  {
                    break; //do the base.WndProc
                  }
                  try
                  {
                    int oldVscrollPos = VerticalScroll.Value;
                    int newVscrollPos = oldVscrollPos;
                    ScrollEventType type = getScrollEventType(msg.WParam);
                    switch (type)
                    {
                      case ScrollEventType.SmallDecrement:
                        newVscrollPos = oldVscrollPos - VerticalScroll.SmallChange;
                        break;
                      case ScrollEventType.SmallIncrement:
                        newVscrollPos = oldVscrollPos + VerticalScroll.SmallChange;
                        break;
                      case ScrollEventType.LargeDecrement:
                        newVscrollPos = oldVscrollPos - VerticalScroll.LargeChange;
                        break;
                      case ScrollEventType.LargeIncrement:
                        newVscrollPos = oldVscrollPos + VerticalScroll.LargeChange;
                        break;
                      case ScrollEventType.First:
                        newVscrollPos = VerticalScroll.Minimum;
                        break;
                      case ScrollEventType.Last:
                        newVscrollPos = VerticalScroll.Maximum;
                        break;
                      case ScrollEventType.ThumbTrack:
                        newVscrollPos = HiWord((int)msg.WParam);
                        break;
                      case ScrollEventType.ThumbPosition:
                        newVscrollPos = HiWord((int)msg.WParam);
                        break;
                    }
                    if (newVscrollPos < VerticalScroll.Minimum)
                    {
                      newVscrollPos = VerticalScroll.Minimum;
                    }
                    if (newVscrollPos > VerticalScroll.Maximum)
                    {
                      newVscrollPos = VerticalScroll.Maximum;
                    }
                    if (oldVscrollPos != newVscrollPos)
                    {
                      VerticalScroll.Value = newVscrollPos;
                      if (type != ScrollEventType.EndScroll)
                      {
                        ScrollEventArgs arg = new ScrollEventArgs(type, oldVscrollPos, newVscrollPos, ScrollOrientation.VerticalScroll);
                        this.OnScroll(arg);
                        Invalidate();
                      }
                    }
                  }
                  catch (Exception ex)
                  {
                    _log.Error(ex.Message, ex);
                  }
                  return;

                case WM_HSCROLL:
                  if (msg.LParam != IntPtr.Zero)
                  {
                    break; //do the base.WndProc
                  }
                  try
                  {
                    int oldHscrollPos = HorizontalScroll.Value;
                    int newHscrollPos = oldHscrollPos;
                    ScrollEventType type = getScrollEventType(msg.WParam);
                    switch (type)
                    {
                      case ScrollEventType.SmallDecrement:
                        newHscrollPos = oldHscrollPos - HorizontalScroll.SmallChange;
                        break;
                      case ScrollEventType.SmallIncrement:
                        newHscrollPos = oldHscrollPos + HorizontalScroll.SmallChange;
                        break;
                      case ScrollEventType.LargeDecrement:
                        newHscrollPos = oldHscrollPos - HorizontalScroll.LargeChange;
                        break;
                      case ScrollEventType.LargeIncrement:
                        newHscrollPos = oldHscrollPos + HorizontalScroll.LargeChange;
                        break;
                      case ScrollEventType.First:
                        newHscrollPos = HorizontalScroll.Minimum;
                        break;
                      case ScrollEventType.Last:
                        newHscrollPos = HorizontalScroll.Maximum;
                        break;
                      case ScrollEventType.ThumbTrack:
                        newHscrollPos = HiWord((int)msg.WParam);
                        break;
                      case ScrollEventType.ThumbPosition:
                        newHscrollPos = HiWord((int)msg.WParam);
                        break;
                    }
                    if (newHscrollPos < HorizontalScroll.Minimum)
                    {
                      newHscrollPos = HorizontalScroll.Minimum;
                    }
                    if (newHscrollPos > HorizontalScroll.Maximum)
                    {
                      newHscrollPos = HorizontalScroll.Maximum;
                    }
                    if (oldHscrollPos != newHscrollPos)
                    {
                      HorizontalScroll.Value = newHscrollPos;
                      ScrollEventArgs arg = new ScrollEventArgs(type, oldHscrollPos, newHscrollPos, ScrollOrientation.HorizontalScroll);
                      if (type != ScrollEventType.EndScroll)
                      {
                        this.OnScroll(arg);
                        Invalidate();
                      }
                    }
                  }
                  catch (Exception ex)
                  {
                    _log.Error(ex.Message, ex);
                  }
                  return;

                default:
                  break;
              }
            }
            base.WndProc(ref msg);
          }
          catch(Exception exx)
          {
            _log.Error("WndProc: " + exx.Message, exx);
          }
            _log.Debug("Leave WndProc");
        }

        #endregion

        #region API32 functions

        static int MakeLong(int LoWord, int HiWord)
        {
            return (HiWord << 16) | (LoWord & 0xffff);
        }

        static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }

        static int HiWord(int number)
        {
            if ((number & 0x80000000) == 0x80000000)
                return (number >> 16);
            else
                return (number >> 16) & 0xffff;
        }

        static int LoWord(int number)
        {
            return number & 0xffff;
        }

        #endregion
    }
}