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
	public enum Corner
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	public class PixbufCellRenderer : ICairoCellRenderer
	{
#region public properties
		public bool Visible {
			get { return visible; }
			set { visible = value; }
		}

		public Gdk.Pixbuf Pixbuf {
			set { pixbuf = value; }
		}

		public Gdk.Pixbuf EmblemPixbuf {
			set { emblem_pixbuf = value; }
		}

		public Corner EmblemCorner {
			set { emblem_corner = value; }
		}

		public bool EmblemVisible {
			get { return emblem_visible; }
			set { emblem_visible = value; }
		}
#endregion

#region public methods
		public void GetSize (out int width, out int height)
		{
			width = height = 0;
			if (!visible) {
				return;
			}

			if (pixbuf != null) {
				width = pixbuf.Width;
				height = pixbuf.Height;
			}

			if (emblem_pixbuf != null && emblem_visible) {
				width += (int)Math.Ceiling ((double) emblem_pixbuf.Width / 2);
				height += (int)Math.Ceiling ((double) emblem_pixbuf.Height / 2);
			}
		}
		
		public void Render (Cairo.Context cr,
		                    Widget widget,
		                    Gdk.Rectangle background_area,
		                    Gdk.Rectangle cell_area,
		                    CairoCellRendererState state)
		{
			if (!visible || pixbuf == null) {
				return;
			}

			int x = cell_area.X + (cell_area.Width / 2) - (pixbuf.Width / 2);
			int y = cell_area.Y + (cell_area.Height / 2) - (pixbuf.Height / 2);

			int w = pixbuf.Width;
			int h = pixbuf.Height;

			cr.Save ();

			Gdk.CairoHelper.SetSourcePixbuf (cr, pixbuf, x, y);
			cr.Rectangle (x, y, w, h);
			cr.Fill ();

			if ((state & CairoCellRendererState.Selected)
			    == CairoCellRendererState.Selected) {
				cr.Save ();

				//cr.Color = CairoHelper.GetCairoColorWithAlpha (widget.Style.Background (StateType.Selected), 0.4f);
                cr.SetSourceRGBA(CairoHelper.GetCairoColorWithAlpha(widget.Style.Background(StateType.Selected), 0.4f).R, CairoHelper.GetCairoColorWithAlpha(widget.Style.Background(StateType.Selected), 0.4f).G, CairoHelper.GetCairoColorWithAlpha(widget.Style.Background(StateType.Selected), 0.4f).B, CairoHelper.GetCairoColorWithAlpha(widget.Style.Background(StateType.Selected), 0.4f).A);

                cr.Rectangle (x, y, w, h);
				cr.Operator = Operator.Xor;
				cr.Fill ();

				cr.Restore ();
			}

			if ((state & CairoCellRendererState.Focused)
			    == CairoCellRendererState.Focused) {
				cr.Save ();

				cr.Rectangle ((int)x - 0.5f, y - 0.5f,
				              w + 1.5f, h + 1.0f);
				CairoHelper.PaintFocus (cr, widget.Style, StateType.Normal);

				cr.Restore ();
			}

			// draw the emblem over everything (with no selection
			// or focus outline
			if (emblem_pixbuf != null && emblem_visible) {
				switch (emblem_corner) {
				case Corner.TopLeft:
					x -= (emblem_pixbuf.Width / 2);
					y -= (emblem_pixbuf.Height / 2);
					break;
				case Corner.TopRight:
					x += w - (emblem_pixbuf.Width / 2);
					y -= (emblem_pixbuf.Height / 2);
					break;
				case Corner.BottomLeft:
					x -= (emblem_pixbuf.Width / 2);
					y += h - (emblem_pixbuf.Height / 2);
					break;
				case Corner.BottomRight:
					x += w - (emblem_pixbuf.Width / 2);
					y += h - (emblem_pixbuf.Height / 2);
					break;
				}

				Gdk.CairoHelper.SetSourcePixbuf (cr, emblem_pixbuf, x, y);
				cr.Rectangle (x, y, emblem_pixbuf.Width, emblem_pixbuf.Height);
				cr.Fill ();
			}

			cr.Restore ();
		}
#endregion

#region private fields
		private bool visible = true;
		private bool emblem_visible = true;
		private Gdk.Pixbuf pixbuf;
		private Gdk.Pixbuf emblem_pixbuf;
		private Corner emblem_corner;
#endregion
	}
}
