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
	public sealed class CairoHelper
	{
#region public static fields
		public static double[] FocusDash = new double[] {
			1.0, 1.0
		};
#endregion

#region public methods
		// More versatile than Gdk.CairoHelper.SetSourceColor
		public static Cairo.Color GetCairoColor (Gdk.Color color)
		{
			return new Cairo.Color (color.Red / 65535.0,
			                        color.Green / 65535.0,
			                        color.Blue / 65535.0);
		}

		public static Cairo.Color GetCairoColorWithAlpha (Gdk.Color color, float alpha)
		{
			Cairo.Color c = GetCairoColor (color);
			c.A = alpha;
			return c;
		}


		public static Cairo.Color Darken (Cairo.Color c, float percentage)
		{
			float scale = 1 - percentage;
			return new Cairo.Color (c.R * scale,
			                        c.G * scale,
			                        c.B * scale);
		}

		public static void PaintFocus (Cairo.Context cr, Style style, StateType state)
		{
            cr.SetSourceRGBA(GetCairoColorWithAlpha(style.Foreground(state), 0.7f).R, GetCairoColorWithAlpha(style.Foreground(state), 0.7f).G, GetCairoColorWithAlpha(style.Foreground(state), 0.7f).B, GetCairoColorWithAlpha(style.Foreground(state), 0.7f).A) ;
            //cr.Color = GetCairoColorWithAlpha (style.Foreground (state), 0.7f);
			cr.LineCap = LineCap.Butt;
			cr.LineJoin = LineJoin.Miter;
			cr.SetDash (FocusDash, 1.5f);
			cr.Antialias = Antialias.None;
			cr.LineWidth = 1.0f;
			cr.Stroke ();
            

        }

		public static void RoundedRectangle (Cairo.Context cr, float x, float y,
		                                     float w, float h, float r)
		{
			cr.MoveTo (x + r, y);
			cr.LineTo (x + w - r, y);
			cr.CurveTo (x + w, y, x + w, y, x + w, y + r);
			cr.LineTo (x + w, y + h - r);
			cr.CurveTo (x + w, y + h, x + w, y + h, x + w - r, y + h);
			cr.LineTo (x + r, y + h);
			cr.CurveTo (x, y + h, x, y + h, x, y + h - r);
			cr.LineTo (x, y + r);
			cr.CurveTo (x, y, x, y, x + r, y);
		}
		
		/*
		 * Obtains the dimensions of text rendered at size with Cairo's
		 * Toy text API.  This is solely useful when trying to get text
		 * metrics without the assistance of Pango.CairoHelper.
		 */ 
		public static void GetToyTextMetrics (int size, string text,
		                                      out int width, out int height)
		{
			width = height = 0;

			if (text == null) {
				return;
			}

			// Create a Cairo.Context expressly for finding out the
			// size of given text, since we're not (and shouldn't
			// be) passed a Cairo.Context.
			using (Cairo.ImageSurface s = new Cairo.ImageSurface (Cairo.Format.A1,
			                                                      300, 300))
			{
				using (Cairo.Context cr = new Cairo.Context (s)) {
					// XXX: ugh, Cairo's toy text api
					// doesn't do any text layout
					string[] lines = text.Split ('\n');
					foreach (string line in lines)
					{
						cr.SetFontSize (size);

						Cairo.TextExtents te = cr.TextExtents (line);

						width = Math.Max ((int)(te.Width + te.XBearing), width);
						height += (int)te.Height;
					}
					height += LEADING * (lines.Length - 1);
				}
			}
		}

		/*
		 * Paints text to the surface with Cairo's Toy text API.  Toy
		 * text API.  This is solely useful when trying to draw
		 * text without the assistance of Pango.CairoHelper.
		 */ 
		public static void PaintToyText (Cairo.Context cr, int size,
		                                 string text, int x, int y,
		                                 bool centered, int hint_width)
		{
			if (text == null) {
				return;
			}

			cr.SetFontSize (size);

			// Do manual text layout since Cairo's toy text api is
			// just that, a toy.
			string[] lines = text.Split ('\n');
			foreach (string line in lines)
			{
				// Cairo's text origin is at the bottom left
				// corner of the text rect
				Cairo.TextExtents te = cr.TextExtents (line);

				int line_x = centered
					? (int)(x + (hint_width / 2) - ((te.Width + te.XBearing) / 2))
					: x;

				// Center the text
				cr.MoveTo (line_x, y - (te.Height + te.YBearing) + te.Height);
				cr.ShowText (line);

				y += (int)te.Height + LEADING;
			}
		}
#endregion

#region private fields
		private const int LEADING = 8;
		private const float DEFUZZ = 0.5f;
#endregion
	}
}
