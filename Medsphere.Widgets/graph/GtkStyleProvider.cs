/* 
 * Medsphere.Widgets
 * Copyright (C) 2007 Medsphere Systems Corporation
 *
 * This library is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as published by the
 * Free Software Foundation; either version 2 of the License, or (at your
 * option) any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this library; if not, write to the Free Software Foundation,
 * Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 */

using Gtk;
using System;

namespace Medsphere.Widgets
{
	public class GtkStyleProvider : IStyleProvider
	{
		public int TickLineSize {
			get { return TICK_SIZE; }
		}

		public int FieldLineThickness {
			get { return FIELD_LINE_THICKNESS; }
		}

		public int PointHalfSize {
			get { return POINT_HALF_SIZE; }
		}

		public int PointSize {
			get { return POINT_SIZE; }
		}

		public int FixedBarWidth {
			get { return FIXED_BAR_WIDTH; }
		}

		public int LegendSpacing {
			get { return LEGEND_SPACING; }
		}

		public int LegendBorderSize {
			get { return LEGEND_BORDER_SIZE; } 
		}

		public int EventTagHeight {
			get { return EVENT_TAG_SEGMENT_HEIGHT * 3; } 
		}

		public int EventTagHalfWidth {
			get { return EVENT_TAG_HALF_WIDTH; } 
		}

		public StateType State {
			set { state = value; }
		}

		public Style Style {
			set { gtk_style = value; }
		}

		/* public methods */
		public GtkStyleProvider (Style style, StateType state, Pango.Context context)
		{
			this.gtk_style = style;
			this.state = state;
			this.context = context;

			this.context.ToString ();
		}

		public void DrawField (Cairo.Context cr, int x, int y, int width, int height)
		{
			cr.Rectangle (x + DEFUZZ, y + DEFUZZ, width - 1.0f, height - 1.0f);

            cr.SetSourceRGBA(CairoHelper.GetCairoColor(gtk_style.Base(state)).R, CairoHelper.GetCairoColor(gtk_style.Base(state)).G, CairoHelper.GetCairoColor(gtk_style.Base(state)).B, CairoHelper.GetCairoColor(gtk_style.Base(state)).A);

            //cr.Color = CairoHelper.GetCairoColor (gtk_style.Base (state));
			cr.FillPreserve ();

            cr.SetSourceRGBA(CairoHelper.GetCairoColor(gtk_style.Foreground(state)).R, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).G, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).B, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).A);

            //cr.Color = CairoHelper.GetCairoColor (gtk_style.Foreground (state));

            cr.LineWidth = FIELD_LINE_THICKNESS;
			cr.Stroke ();
		}

		public void GetAxisLabelMetrics (string text, out int width, out int height)
		{
			GetTextMetrics (AXIS_LABEL_FONT_SIZE, text,
			                out width, out height);
		}

		public void GetAxisTitleMetrics (string text, out int width, out int height)
		{
			GetTextMetrics (AXIS_TITLE_FONT_SIZE, text,
			                out width, out height);
		}

		public void GetAxisUnitMetrics (string text, out int width, out int height)
		{
			GetTextMetrics (AXIS_UNIT_FONT_SIZE, text,
			                out width, out height);
		}

		public void DrawAxisTick (Cairo.Context cr, int x, int y, AxisLocation loc)
		{
			//cr.Color = CairoHelper.GetCairoColor (gtk_style.Foreground (state));
            cr.SetSourceRGBA(CairoHelper.GetCairoColor(gtk_style.Foreground(state)).R, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).G, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).B, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).A);

            cr.LineWidth = 1.0f;

			switch (loc) {
			case AxisLocation.Left:
				cr.MoveTo (x, y + DEFUZZ);
				cr.LineTo (x + TICK_SIZE, y + DEFUZZ);	
				break;
			case AxisLocation.Right:
				cr.MoveTo (x, y + DEFUZZ);
				cr.LineTo (x + TICK_SIZE, y + DEFUZZ);
				break;
			case AxisLocation.Top:
				cr.MoveTo (x + DEFUZZ, y);
				cr.LineTo (x + DEFUZZ, y - TICK_SIZE);
				break;
			case AxisLocation.Bottom:
				cr.MoveTo (x + DEFUZZ, y);
				cr.LineTo (x + DEFUZZ, y + TICK_SIZE);
				break;
			}

			cr.Stroke ();
		}

		public void DrawAxisLabel (Cairo.Context cr, string text, int x, int y)
		{
			DrawText (cr, AXIS_LABEL_FONT_SIZE, text, x, y);
		}

		public void DrawAxisTitle (Cairo.Context cr, string text, int x, int y,
		                           Orientation orientation)
		{
			cr.Save ();

			if (orientation == Orientation.Vertical) {
				cr.Translate (x, y);
				cr.Rotate (Math.PI/2);
				cr.SetFontSize (AXIS_TITLE_FONT_SIZE);
                cr.SetSourceRGBA(CairoHelper.GetCairoColor(gtk_style.Foreground(state)).R, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).G, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).B, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).A);

                //cr.Color = CairoHelper.GetCairoColor (gtk_style.Foreground (state));
                cr.ShowText (text);
			} else {
				DrawText (cr, AXIS_TITLE_FONT_SIZE, text, x, y);
			}

			cr.Restore ();
		}

		public void DrawAxisUnit (Cairo.Context cr, string text, int x, int y)
		{
			DrawText (cr, AXIS_UNIT_FONT_SIZE, text, x, y);
		}

		public void DrawGridLine (Cairo.Context cr, int x1, int y1, int x2, int y2)
		{
			if (x1 == x2) {
				// vertical line
				cr.MoveTo (x1 + DEFUZZ, y1);
				cr.LineTo (x2 + DEFUZZ, y2);
			} else if (y1 == y2) {
				// horizontal line
				cr.MoveTo (x1, y1 + DEFUZZ);
				cr.LineTo (x2, y2 + DEFUZZ);
			} else {
				cr.MoveTo (x1 + DEFUZZ, y1 + DEFUZZ);
				cr.LineTo (x2 + DEFUZZ, y2 + DEFUZZ);
			}

            cr.SetSourceRGBA(CairoHelper.GetCairoColor(gtk_style.Background(state)).R, CairoHelper.GetCairoColor(gtk_style.Background(state)).G, CairoHelper.GetCairoColor(gtk_style.Background(state)).B, CairoHelper.GetCairoColor(gtk_style.Background(state)).A);


            //cr.Color = CairoHelper.GetCairoColor (gtk_style.Background (state));
            cr.LineWidth = 1.0f;
			cr.Stroke ();
		}

		public void DrawPoint (Cairo.Context cr, PlotColor color,
		                       PointShape type, int x, int y, bool is_focused)
		{
			DrawPointGlyph (cr, type, x, y, POINT_SIZE);

			//cr.Color = colors[(int)color];
            cr.SetSourceRGBA(colors[(int)color].R, colors[(int)color].G, colors[(int)color].B, colors[(int)color].A);
            cr.FillPreserve ();
			cr.Stroke ();

			if (is_focused) {
				float size = POINT_SIZE + FOCUS_BORDER;
				DrawFocusRect (cr, x - (size / 2), y - (size / 2),
				               size, size);
			}
		}

		public void DrawSelectedPoint (Cairo.Context cr, PlotColor color,
		                               PointShape type, int x, int y)
		{
			DrawPointGlyph (cr, type, x, y, POINT_SIZE);

            //cr.Color = CairoHelper.GetCairoColor (gtk_style.Base (state));
            cr.SetSourceRGBA(CairoHelper.GetCairoColor(gtk_style.Base(state)).R, CairoHelper.GetCairoColor(gtk_style.Base(state)).G, CairoHelper.GetCairoColor(gtk_style.Base(state)).B, CairoHelper.GetCairoColor(gtk_style.Base(state)).A);

            cr.FillPreserve ();

			//cr.Color = colors[(int)color];
            cr.SetSourceRGBA(colors[(int)color].R, colors[(int)color].G, colors[(int)color].B, colors[(int)color].A);
            
            cr.LineWidth = 2.0f;
			cr.Stroke ();
		}

		public void DrawPointValue (Cairo.Context cr, string val, int x, int y)
		{
			int total_width, total_height;
			GetTextMetrics (VALUE_LABEL_FONT_SIZE, val,
			                out total_width, out total_height);

			DrawText (cr, VALUE_LABEL_FONT_SIZE, val, x,
			          y - total_height - LEADING);
		}

		public void DrawLine (Cairo.Context cr, PlotColor color, int x1, int y1, int x2, int y2)
		{
			cr.MoveTo (x1 + DEFUZZ, y1 + DEFUZZ);
			cr.LineTo (x2 + DEFUZZ, y2 + DEFUZZ);

            //cr.Color = colors[(int)color];
            cr.SetSourceRGBA(colors[(int)color].R, colors[(int)color].G, colors[(int)color].B, colors[(int)color].A);

            cr.LineWidth = 1.0f;
			cr.Stroke ();
		}

		public void DrawDashedLine (Cairo.Context cr, PlotColor color, int x1, int y1, int x2, int y2)
		{
			cr.Save ();

			cr.MoveTo (x1 + DEFUZZ, y1 + DEFUZZ);
			cr.LineTo (x2 + DEFUZZ, y2 + DEFUZZ);

            //cr.Color = colors[(int)color];
            cr.SetSourceRGBA(colors[(int)color].R, colors[(int)color].G, colors[(int)color].B, colors[(int)color].A);

            cr.LineWidth = 1.0f;
			cr.SetDash (standard_dash, 0);
			cr.Stroke ();

			cr.Restore ();
		}

		public void DrawLinkedFocusLine (Cairo.Context cr, int x1, int y1, int x2, int y2)
		{
			cr.Save ();

			cr.MoveTo (x1 + DEFUZZ, y1);
			cr.LineTo (x2 + DEFUZZ, y2);

            cr.SetSourceRGBA(CairoHelper.GetCairoColor(gtk_style.Foreground(state)).R, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).G, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).B, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).A);

            //cr.Color = CairoHelper.GetCairoColorWithAlpha (gtk_style.Foreground (state), 0.4f);

            cr.SetDash (linked_dash, 0);
			cr.LineWidth = 1.0f;
			cr.Stroke ();

			cr.Restore ();
		}

		public void DrawLinkedSelectionLine (Cairo.Context cr, int x1, int y1, int x2, int y2)
		{
			cr.Save ();

			cr.MoveTo (x1 + DEFUZZ, y1);
			cr.LineTo (x2 + DEFUZZ, y2);

            cr.SetSourceRGBA(CairoHelper.GetCairoColor(gtk_style.Foreground(state)).R, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).G, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).B, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).A);
            //cr.Color = CairoHelper.GetCairoColorWithAlpha (gtk_style.Foreground (state), 0.4f);

            cr.LineWidth = 1.0f;
			cr.Stroke ();

			cr.Restore ();
		}

		public void DrawBar (Cairo.Context cr, PlotColor color, int x, int y, int width, int height, bool is_focused)
		{
			// Draw the fill
			cr.Rectangle (x, y, width, height);
			Cairo.Color fill = colors[(int)color];
			fill.A = 0.6;
            //cr.Color = fill;
            cr.SetSourceRGBA(fill.R, fill.G, fill.B, fill.A);
            cr.Fill ();

			// Stroke just the top, left and right sides
			cr.MoveTo (x - DEFUZZ, y + height - DEFUZZ);
			cr.RelLineTo (0, -height);
			cr.RelLineTo (width, 0);
			cr.RelLineTo (0, height);

            // decrease saturation by 40%
            //cr.Color = CairoHelper.Darken (colors[(int)color], 0.4f);
            cr.SetSourceRGBA(CairoHelper.Darken(colors[(int)color], 0.4f).R, CairoHelper.Darken(colors[(int)color], 0.4f).G, CairoHelper.Darken(colors[(int)color], 0.4f).B, CairoHelper.Darken(colors[(int)color], 0.4f).A);

            cr.LineWidth = 1.0f;

			cr.Stroke ();

			if (is_focused) {
				DrawFocusRect (cr, x - FOCUS_BORDER, y - FOCUS_BORDER,
				               width + (FOCUS_BORDER * 2), height + FOCUS_BORDER);
			}
		}

		public void DrawSelectedBar (Cairo.Context cr, PlotColor color, int x, int y, int width, int height)
		{
			// Draw the fill
			cr.Rectangle (x, y, width, height);
            cr.SetSourceRGBA(CairoHelper.GetCairoColor(gtk_style.Base(state)).R, CairoHelper.GetCairoColor(gtk_style.Base(state)).G, CairoHelper.GetCairoColor(gtk_style.Base(state)).B, CairoHelper.GetCairoColor(gtk_style.Base(state)).A);
            //cr.Color = CairoHelper.GetCairoColor (gtk_style.Base (state));
            cr.Fill ();

			// Stroke just the top, left and right sides
			cr.MoveTo (x, y + height);
			cr.RelLineTo (0, -height);
			cr.RelLineTo (width, 0);
			cr.RelLineTo (0, height);
			
			// darken by 40%
			//cr.Color = CairoHelper.Darken (colors[(int)color], 0.4f);

            cr.SetSourceRGBA(CairoHelper.Darken(colors[(int)color], 0.4f).R, CairoHelper.Darken(colors[(int)color], 0.4f).G, CairoHelper.Darken(colors[(int)color], 0.4f).B, CairoHelper.Darken(colors[(int)color], 0.4f).A);

            cr.LineWidth = 2.0f;

			cr.Stroke ();
		}

		public void DrawFixedWidthBar (Cairo.Context cr, PlotColor color, int x, int y, int height, bool is_focused)
		{
			DrawBar (cr, color, x - (FIXED_BAR_WIDTH / 2), y,
			         FIXED_BAR_WIDTH, height, is_focused);
		}

		public void DrawSelectedFixedWidthBar (Cairo.Context cr, PlotColor color, int x, int y, int height)
		{
			DrawSelectedBar (cr, color, x - (FIXED_BAR_WIDTH / 2), y,
			                 FIXED_BAR_WIDTH, height);
		}
		
		public void DrawSelectionRectangle (Cairo.Context cr, int x, int y, int width, int height)
		{
			cr.Rectangle (x + DEFUZZ, y + DEFUZZ,
			              width, height);

            //cr.Color = CairoHelper.GetCairoColorWithAlpha (gtk_style.Foreground (state), 0.1f);
            cr.SetSourceRGBA(CairoHelper.GetCairoColorWithAlpha(gtk_style.Foreground(state), 0.1f).R, CairoHelper.GetCairoColorWithAlpha(gtk_style.Foreground(state), 0.1f).G, CairoHelper.GetCairoColorWithAlpha(gtk_style.Foreground(state), 0.1f).B, CairoHelper.GetCairoColorWithAlpha(gtk_style.Foreground(state), 0.1f).A);
            cr.FillPreserve ();

            //cr.Color = CairoHelper.GetCairoColorWithAlpha (gtk_style.Foreground (state), 0.7f);
            cr.SetSourceRGBA(CairoHelper.GetCairoColorWithAlpha(gtk_style.Foreground(state), 0.7f).R, CairoHelper.GetCairoColorWithAlpha(gtk_style.Foreground(state), 0.7f).G, CairoHelper.GetCairoColorWithAlpha(gtk_style.Foreground(state), 0.7f).B, CairoHelper.GetCairoColorWithAlpha(gtk_style.Foreground(state), 0.7f).A);

            cr.LineWidth = 1.0f;
			cr.Stroke ();
		}

		public void GetLegendLabelMetrics (string text, out int width, out int height)
		{
			GetTextMetrics (LEGEND_LABEL_FONT_SIZE, text,
			                out width, out height);
		}
		
		public void DrawLegendLabel (Cairo.Context cr, string text, int x, int y)
		{
			DrawText (cr, LEGEND_LABEL_FONT_SIZE, text, x, y);
		}

		public void DrawLegendArea (Cairo.Context cr, int x, int y, int width, int height)
		{
			// draw the legend shadow offset a few pixels
			cr.Rectangle (x + DEFUZZ + 2, y + DEFUZZ + 2,
			              width + 2, height + 2);
			//cr.Color = CairoHelper.GetCairoColor (gtk_style.Background (state));
            cr.SetSourceRGBA(CairoHelper.GetCairoColor(gtk_style.Background(state)).R, CairoHelper.GetCairoColor(gtk_style.Background(state)).G, CairoHelper.GetCairoColor(gtk_style.Background(state)).B, CairoHelper.GetCairoColor(gtk_style.Background(state)).A);

            cr.Fill ();

			cr.Rectangle (x + DEFUZZ, y + DEFUZZ,
			              width, height);
			//cr.Color = CairoHelper.GetCairoColor (gtk_style.Base (state));
            cr.SetSourceRGBA(CairoHelper.GetCairoColor(gtk_style.Base(state)).R, CairoHelper.GetCairoColor(gtk_style.Base(state)).G, CairoHelper.GetCairoColor(gtk_style.Base(state)).B, CairoHelper.GetCairoColor(gtk_style.Base(state)).A);

            cr.FillPreserve ();

			//cr.Color = CairoHelper.GetCairoColor (gtk_style.Foreground (state));
            cr.SetSourceRGBA(CairoHelper.GetCairoColor(gtk_style.Foreground(state)).R, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).G, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).B, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).A);
            cr.LineWidth = 1.0f;
			cr.Stroke ();
		}

		public void DrawReferenceRectangle (Cairo.Context cr, PlotColor color,
		                                    int x, int y, int width, int height)
		{
			cr.Rectangle (x, y, width, height);

			Cairo.Color c = colors[(int)color];
			c.A = 0.4f;
			//cr.Color = c;
            cr.SetSourceRGBA(c.R, c.G, c.B, c.A);


            cr.Fill ();
		}

		public void DrawEventTag (Cairo.Context cr, PlotColor color,
		                          int x, int y, Orientation orientation)
		{
			cr.MoveTo (x + DEFUZZ, y);

			if (orientation == Orientation.Vertical) {
				cr.RelLineTo (-1 * EVENT_TAG_HALF_WIDTH, -1 * EVENT_TAG_SEGMENT_HEIGHT);
				cr.RelLineTo (0, -2 * EVENT_TAG_SEGMENT_HEIGHT);
				cr.RelLineTo (EVENT_TAG_HALF_WIDTH * 2, 0);
				cr.RelLineTo (0, 2 * EVENT_TAG_SEGMENT_HEIGHT);
			} else {
				cr.RelLineTo (EVENT_TAG_SEGMENT_HEIGHT, -1 * EVENT_TAG_HALF_WIDTH);
				cr.RelLineTo (EVENT_TAG_SEGMENT_HEIGHT * 2, 0);
				cr.RelLineTo (0, EVENT_TAG_HALF_WIDTH * 2);
				cr.RelLineTo (-2 * EVENT_TAG_SEGMENT_HEIGHT, 0);
			}

			cr.ClosePath ();
			//cr.Color = CairoHelper.GetCairoColor (gtk_style.Base (state));
            cr.SetSourceRGBA(CairoHelper.GetCairoColor(gtk_style.Base(state)).R, CairoHelper.GetCairoColor(gtk_style.Base(state)).G, CairoHelper.GetCairoColor(gtk_style.Base(state)).B, CairoHelper.GetCairoColor(gtk_style.Base(state)).A);

            cr.FillPreserve ();

			cr.LineWidth = 1.0f;
			//cr.Color = colors[(int)color];
            cr.SetSourceRGBA(colors[(int)color].R, colors[(int)color].G, colors[(int)color].B, colors[(int)color].A);

            cr.Stroke ();
		}

		public void DrawAxisBoundingBox (Cairo.Context cr, int x, int y,
		                                 int width, int height, AxisLocation loc)
		{
			cr.MoveTo (x, y);
			if (loc == AxisLocation.Bottom) {
				cr.RelMoveTo (width, 0);
			} else {
				cr.RelLineTo (width, 0);
			}
			
			if (loc == AxisLocation.Left) {
				cr.RelMoveTo (0, height);
			} else {
				cr.RelLineTo (0, height);
			}
			
			if (loc == AxisLocation.Top) {
				cr.RelMoveTo (-width, 0);
			} else {
				cr.RelLineTo (-width, 0);
			}

			if (loc == AxisLocation.Right) {
				cr.RelMoveTo (0, -height);
			} else {
				cr.RelLineTo (0, -height);
			}

			//cr.Color = CairoHelper.GetCairoColor (gtk_style.Foreground (state));
            cr.SetSourceRGBA(CairoHelper.GetCairoColor(gtk_style.Foreground(state)).R, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).G, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).B, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).A);

            cr.LineWidth = 2.0f;
			cr.Stroke ();
		}

		/* private fields */
		private Gtk.Style gtk_style;
		private StateType state;
		private Pango.Context context;

		private Cairo.Color[] colors = new Cairo.Color[] {
			new Cairo.Color (0.64f, 0.00f, 0.00f),	// Scarlet Red 2
			new Cairo.Color (0.20f, 0.40f, 0.64f),	// Sky Blue 2
			new Cairo.Color (0.45f, 0.82f, 0.09f),	// Chameleon 2
			new Cairo.Color (0.96f, 0.47f, 0.00f),	// Orange 2
			new Cairo.Color (0.46f, 0.31f, 0.48f),	// Plum 2
			new Cairo.Color (0.93f, 0.83f, 0.00f),	// Butter 2
			new Cairo.Color (0.76f, 0.49f, 0.01f),	// Chocolate 2
			new Cairo.Color (0.64f, 0.00f, 0.00f),	// Scarlet Red 3
			new Cairo.Color (0.13f, 0.29f, 0.53f),	// Sky Blue 3
			new Cairo.Color (0.31f, 0.60f, 0.02f),	// Chameleon 3
			new Cairo.Color (0.81f, 0.36f, 0.00f),	// Orange 3
			new Cairo.Color (0.36f, 0.21f, 0.40f),	// Plum 3
			new Cairo.Color (0.76f, 0.63f, 0.00f),	// Butter 3
			new Cairo.Color (0.56f, 0.35f, 0.01f), 	// Chocolate 3
			new Cairo.Color (0.94f, 0.16f, 0.16f),	// Scarlet Red 1
			new Cairo.Color (0.45f, 0.62f, 0.81f),	// Sky Blue 1
			new Cairo.Color (0.54f, 0.89f, 0.20f),	// Chameleon 1
			new Cairo.Color (0.99f, 0.69f, 0.24f),	// Orange 1
			new Cairo.Color (0.68f, 0.50f, 0.66f),	// Plum 1
			new Cairo.Color (0.99f, 0.91f, 0.30f),	// Butter 1
			new Cairo.Color (0.91f, 0.73f, 0.43f), 	// Chocolate 1
			new Cairo.Color (0.0f, 0.0f, 0.0f),	// Black
			new Cairo.Color (1.0f, 1.0f, 1.0f)	// White
		};

		private double[] linked_dash = new double[] {
			4.0, 4.0
		};

		private double[] standard_dash = new double[] {
			8.0, 8.0
		};

		private const float DEFUZZ = 0.5f;

		private const int POINT_SIZE = 8;
		private const int POINT_HALF_SIZE = 4;

		private const int FOCUS_BORDER = 3;

		private const int TICK_SIZE = 5;

		private const int FIELD_LINE_THICKNESS = 1;

		private const int LEADING = 8;

		private const int FIXED_BAR_WIDTH = 15;

		private const int LEGEND_SPACING = 3;
		private const int LEGEND_BORDER_SIZE = 3;

		private const int AXIS_LABEL_FONT_SIZE = 10;
		private const int VALUE_LABEL_FONT_SIZE = 10;
		private const int LEGEND_LABEL_FONT_SIZE = 10;

		private const int AXIS_UNIT_FONT_SIZE = 12;
		private const int AXIS_TITLE_FONT_SIZE = 16;

		private const int EVENT_TAG_HALF_WIDTH = 10;
		private const int EVENT_TAG_SEGMENT_HEIGHT = 8;

		/* private methods */
		private void DrawPointGlyph (Cairo.Context cr, PointShape type, float x, float y, float side_size)
		{
			float half_size = side_size / 2;
	
			switch (type) {
			case PointShape.Square:
				cr.Rectangle (x - half_size, y - half_size,
					      side_size, side_size);
				break;
			case PointShape.Circle:
				cr.MoveTo (x + half_size, y);
				cr.Arc (x, y, half_size, 0, 2 * Math.PI);
				cr.ClosePath ();
				break;
			case PointShape.Diamond:
				cr.MoveTo (x - half_size, y);
				cr.LineTo (x, y - half_size);
				cr.LineTo (x + half_size, y);
				cr.LineTo (x, y + half_size);
				cr.ClosePath ();
				break;
			case PointShape.Triangle:
				cr.MoveTo (x - half_size, y + half_size);
				cr.LineTo (x, y - half_size);
				cr.LineTo (x + half_size, y + half_size);
				cr.ClosePath ();
				break;
			case PointShape.X:
				cr.MoveTo (x - half_size, y - half_size);
				cr.LineTo (x + half_size, y + half_size);
				cr.MoveTo (x + half_size, y - half_size);
				cr.LineTo (x - half_size, y + half_size);
				break;
			}
		}

		private void DrawFocusRect (Cairo.Context cr, float x, float y, float width, float height)
		{
			cr.Save ();

			cr.Rectangle (x, y, width, height);
			CairoHelper.PaintFocus (cr, gtk_style, state);

			cr.Restore ();
		}

		private void GetTextMetrics (int size, string text, out int width, out int height)
		{
			/* XXX: Uncomment when we can safely use Gtk# 2.10
			// TODO: Cache layouts
			Pango.Layout label = new Pango.Layout (context);
			label.SetMarkup (String.Format ("<span weight='small'>{0}</span>",
			                                GLib.Markup.EscapeText (text)));
			label.GetPixelSize (out width, out height);
			*/

			CairoHelper.GetToyTextMetrics (size, text, out width, out height);
		}

		public void DrawText (Cairo.Context cr, int size, string text, int x, int y)
		{
			/* XXX: Uncomment when we can safely use Gtk# 2.10
			Pango.Layout label = Pango.CairoHelper.CreateLayout (cr);
			label.FontDescription = context.FontDescription;
			label.SetMarkup (String.Format ("<span size='small'>{0}</span>",
			                                GLib.Markup.EscapeText (text)));

			Gdk.CairoHelper.SetSourceColor (cr, gtk_style.Foreground (state));
			cr.MoveTo (x, y);
			Pango.CairoHelper.ShowLayout (cr, label);
			*/

			//cr.Color = CairoHelper.GetCairoColor (gtk_style.Foreground (state));
            cr.SetSourceRGBA(CairoHelper.GetCairoColor(gtk_style.Foreground(state)).R, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).G, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).B, CairoHelper.GetCairoColor(gtk_style.Foreground(state)).A);

            CairoHelper.PaintToyText (cr, size, text, x, y, false, 0);
		}
	}
}
