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
	public interface IStyleProvider
	{
		int TickLineSize {
			get;
		}

		int FieldLineThickness {
			get;
		}

		int PointHalfSize {
			get;
		}

		int PointSize {
			get;
		}

		int FixedBarWidth {
			get;
		}

		int LegendSpacing {
			get;
		}

		int LegendBorderSize {
			get;
		}

		int EventTagHeight {
			get;
		}

		int EventTagHalfWidth {
			get;
		}

		StateType State {
			set;
		}

		Style Style {
			set;
		}

		void DrawField (Cairo.Context cr, int x, int y, int width, int height);

		void GetAxisLabelMetrics (string text, out int width, out int height);

		void GetAxisTitleMetrics (string text, out int width, out int height);

		void GetAxisUnitMetrics (string text, out int width, out int height);

		void DrawAxisTick (Cairo.Context cr, int x, int y, AxisLocation loc);

		void DrawAxisLabel (Cairo.Context cr, string text, int x, int y);

		void DrawAxisTitle (Cairo.Context cr, string text, int x, int y, Orientation orientation);

		void DrawAxisUnit (Cairo.Context cr, string text, int x, int y);

		void DrawGridLine (Cairo.Context cr, int x1, int y1, int x2, int y2);

		void DrawPoint (Cairo.Context cr, PlotColor color, PointShape shape, int x, int y, bool is_focused);

		void DrawSelectedPoint (Cairo.Context cr, PlotColor color, PointShape shape, int x, int y);
		
		void DrawPointValue (Cairo.Context cr, string val, int x, int y);

		void DrawLine (Cairo.Context cr, PlotColor color, int x1, int y1, int x2, int y2);

		void DrawDashedLine (Cairo.Context cr, PlotColor color, int x1, int y1, int x2, int y2);

		void DrawLinkedFocusLine (Cairo.Context cr, int x1, int y1, int x2, int y2);

		void DrawLinkedSelectionLine (Cairo.Context cr, int x1, int y1, int x2, int y2);

		void DrawFixedWidthBar (Cairo.Context cr, PlotColor color, int x, int y, int height, bool is_focused);

		void DrawSelectedFixedWidthBar (Cairo.Context cr, PlotColor color, int x, int y, int height);

		void DrawSelectionRectangle (Cairo.Context cr, int x, int y, int width, int height);

		void GetLegendLabelMetrics (string text, out int width, out int height);

		void DrawLegendLabel (Cairo.Context cr, string text, int x, int y);

		void DrawLegendArea (Cairo.Context cr, int x, int y, int width, int height);

		void DrawReferenceRectangle (Cairo.Context cr, PlotColor color,
		                             int x, int y, int width, int height);

		void DrawEventTag (Cairo.Context cr, PlotColor color,
		                   int x, int y, Orientation orientation);

		void DrawAxisBoundingBox (Cairo.Context cr, int x, int y,
		                          int width, int height, AxisLocation loc);
	}
}
