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
	public interface IAxis
	{
		/**
		 * Indicates when the axis needs to be redrawn due to change in
		 * #Location, #ShowTicks, #ShowTickLabels or min/max
		 * values.
		 */
		event EventHandler Changed;

		/**
		 * Raised when the axis wants to notify the Surface that it's
		 * size request has changed, often when the range changes.
		 */
		event SizeRequestedHandler SizeRequested;

		/**
		 * Specifies the 0-based geometric dimension that this axis
		 * applies to.  For 2D plots, this number should be between 0
		 * and 1, inclusive.
		 */
		int Dimension {
			get;
			set;
		}

		/**
		 * Specifies the side of the plot where the axis will be drawn
		 * in 2D space.
		 */
		AxisLocation Location {
			get;
			set;
		}

		/**
		 * Specifies whether this axis will be drawn to the screen.
		 */
		bool Visible {
			get;
			set;
		}

		/**
		 * Specifies if tick marks at large intervals should be drawn
		 * along the axis at #Location.  If false, #ShowTickLabels is
		 * also assumed to be false.
		 */
		bool ShowTicks {
			get;
			set;
		}

		/**
		 * Specifies if tick labels at large intervals should be drawn
		 * along the axis at #Location.
		 */
		bool ShowTickLabels {
			get;
			set;
		}

		/**
		 * Specifies if the axis should draw grid lines at large tick
		 * intervals.
		 */
		bool ShowGridLines {
			get;
			set;
		}

		/**
		 * The lower bound of values to be shown on this graph.  If
		 * there is no zoom, this will be the same as
		 * #CalculatedMinValue.
		 */
		IComparable MinValue {
			get;
		}

		/**
		 * The upper bound of values to be shown on this graph.  If
		 * there is no zoom, this will be the same as
		 * #CalculatedMaxValue.
		 */
		IComparable MaxValue {
			get;
		}

		/**
		 * The absolute lower bound of values in the all of the plots.
		 * This value will be the same regardless of zoom.
		 */
		IComparable CalculatedMinValue {
			get;
		}

		/**
		 * The absolute upper bound of values in the all of the plots.
		 * This value will be the same regardless of zoom.
		 */
		IComparable CalculatedMaxValue {
			get;
		}

		/**
		 * Returns true if the associated plots have returned a valid
		 * range, false otherwise.  This is primarily used to tell
		 * whether calling #ValueToGridCoords will return a real value.
		 */
		bool HasValidRange {
			get;
		}

		/**
		 * The style provider to be used for drawing this axis's ticks
		 * and labels.
		 */
		IStyleProvider Style {
			set;
		}

		/**
		 * Obtains the preferred size of the axis ticks + labels.  The
		 * Surface uses this information to decide what size to
		 * eventually give the various axes and plot field during their
		 * respective Draw methods.
		 */
		Requisition SizeRequest (Gdk.Rectangle plot_size);

		/**
		 * Calculates the range (min anx max values) for each of the
		 * provided IPlots, and sets the internal range of the Axis.
		 *
		 * This method may be called multiple times per instanciation
		 * to support linked axes, so it is expected to always store
		 * the smallest and largest value seen during it's lifetime.
		 *
		 * In most cases, the axis should pad (subtract/add a pad value
		 * from min/max) these values to give extra room to the plots.
		 * However, the real values should still be returned from
		 * #CalculatedMinValue and #CalculatedMaxValue.
		 */
		void SetRange (IPlot[] plots, Gdk.Rectangle allocation_hint);

		/**
		 * Converts value from data model to grid coordinate system
		 * (0-10000) for later graphing.
		 */
		long ValueToGridCoords (IComparable val);

		/**
		 * Sets min and max to the displayed (zoom, etc) min and max
		 * mapped to the Grid coordinate system.
		 */
		void GetGridClipRegion (out long min, out long max);

		/**
		 * Draws tick marks along the axis inside allocation, if
		 * #ShowTicks is true.  If #ShowTickLabels is true, this will
		 * also draw labels at tick points along the axis.
		 */
		void DrawTicks (Gdk.Rectangle allocation, Cairo.Context cr);

		/**
		 * Draws grid lines inside of plot_alloc.
		 */
		void DrawGridLines (Gdk.Rectangle plot_alloc, Cairo.Context cr);

		/**
		 * Zooms the range of values displayed for this axis.
		 * The axis should map these grid coordinates to a rectangle in
		 * the value space, limiting the visible range to the selected
		 * rectangle.  This should be reflected in the new
		 * MinValue/MaxValue combination, and should fire a Changed
		 * event if the range was altered.
		 */
		void Zoom (long grid_min, long grid_max);

		/**
		 * Alters the min/max values to the calculated min/max values
		 * to return to a normal zoom.
		 */
		void Unzoom ();

		/**
		 * Raises the SizeRequested event, with new_req as the provided
		 * requisiton.  This is mainly used for the AxisSizeGroup, and
		 * has little usefulness outside of that.
		 */
		void FireSizeRequested (Requisition new_req);

		void RecalculateRange (IPlot[] plots, Gdk.Rectangle allocation_hint);

		/**
		 * Returns true if c is between MinValue and MaxValue
		 * inclusive, false otherwise.
		 */
		bool IsInRange (IComparable c);
	}
}
