/*
 * Copyright (c) 2007-2008 Medsphere Systems Corporation
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

using Gtk;
using Cairo;
using System;

namespace Medsphere.Widgets
{
	public class TextCellRenderer : ICairoCellRenderer
	{
#region public properties
		public bool Visible {
			get { return visible; }
			set { visible = value; }
		}

		public string Text {
			set { text = value; }
		}

		public int FontSize {
			set { font_size = value; }
		}

		public Gdk.Color FontColor {
			set { font_color = value; } 
		}
#endregion

#region public methods
		public void GetSize (out int width, out int height)
		{
			width = height = 0;
			if (!visible) {
				return;
			}

			CairoHelper.GetToyTextMetrics (font_size, text,
			                               out width, out height);
			width += SELECTION_PAD * 2;
			height += SELECTION_PAD * 2;
		}
		
		public void Render (Cairo.Context cr,
		                    Widget widget,
		                    Gdk.Rectangle background_area,
		                    Gdk.Rectangle cell_area,
		                    CairoCellRendererState state)
		{
			if (!visible) {
				return;
			}

			int width, height;
			CairoHelper.GetToyTextMetrics (font_size, text,
			                               out width, out height);

			// pad the requested size with the selection outline
			width += SELECTION_PAD * 2;
			height += SELECTION_PAD * 2;

			int x = cell_area.X + (cell_area.Width / 2) - (width / 2) + SELECTION_PAD;
			int y = cell_area.Y + SELECTION_PAD;

			cr.Save ();

			Gtk.StateType state_type = StateType.Normal;
			if ((state & CairoCellRendererState.Selected)
			    == CairoCellRendererState.Selected) {
				state_type = StateType.Selected;

				CairoHelper.RoundedRectangle (cr, x - SELECTION_PAD, y - SELECTION_PAD,
				                              width, height, 10);
                //cr.Color = CairoHelper.GetCairoColor (widget.Style.Background (state_type));
                cr.SetSourceRGBA(CairoHelper.GetCairoColor(widget.Style.Background(state_type)).R, CairoHelper.GetCairoColor(widget.Style.Background(state_type)).G, CairoHelper.GetCairoColor(widget.Style.Background(state_type)).B, CairoHelper.GetCairoColor(widget.Style.Background(state_type)).A);

                cr.Fill ();
			}

			if (font_color.Equal (Gdk.Color.Zero)) {
				//cr.Color = CairoHelper.GetCairoColor (widget.Style.Text (state_type));
                cr.SetSourceRGBA(CairoHelper.GetCairoColor(widget.Style.Text(state_type)).R, CairoHelper.GetCairoColor(widget.Style.Text(state_type)).G, CairoHelper.GetCairoColor(widget.Style.Text(state_type)).B, CairoHelper.GetCairoColor(widget.Style.Text(state_type)).A);

            }
            else {
				//cr.Color = CairoHelper.GetCairoColor (font_color);
                cr.SetSourceRGBA(CairoHelper.GetCairoColor(font_color).R, CairoHelper.GetCairoColor(font_color).G, CairoHelper.GetCairoColor(font_color).B, CairoHelper.GetCairoColor(font_color).A);

            }

            CairoHelper.PaintToyText (cr, font_size, text, x, y, true,
			                          width - (SELECTION_PAD * 2));

			if ((state & CairoCellRendererState.Focused)
			    == CairoCellRendererState.Focused) {
				cr.Rectangle (x - SELECTION_PAD, y - SELECTION_PAD, width, height);
				CairoHelper.PaintFocus (cr, widget.Style, StateType.Normal);
			}

			cr.Restore ();
		}
#endregion

#region private fields
		private bool visible = true;
		private string text;
		private int font_size = 12;
		private Gdk.Color font_color = Gdk.Color.Zero;
		private const int SELECTION_PAD = 4;
#endregion
	}
}
