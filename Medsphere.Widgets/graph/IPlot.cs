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
using System.Collections;

namespace Medsphere.Widgets
{
	public interface IPlot
	{
		/**
		 * Indicates when the plot needs to be redrawn due to
		 * change in #Name, #FactorIntoAxisCalculation, etc.
		 */
		event EventHandler Changed;

		/**
		 * Raised when point's selection changes.
		 */ 
		event EventHandler SelectionChanged;

		/**
		 * The name of this plot as to be displayed on a legend.
		 */
		string Name {
			get;
		}

		/**
		 * Whether or not this plot should be featured in axis range
		 * calculation.  Most consumers should set this to true, but
		 * plots that simply draw reference lines or reference ranges
		 * should set this to false, since they don't really have
		 * values to speak of.
		 */
		bool FactorIntoAxisCalculation {
			get;
		}

		/**
		 * The style provider to be used for drawing this plot's lines
		 * and points.
		 */
		IStyleProvider Style {
			set;
		}

		bool HasSelection {
			get;
		}

		bool CanFocus {
			get;
		}

		bool HasFocus {
			get;
		}

		bool ShowInLegend {
			get;
		}

		void SizeAllocate (Gdk.Rectangle allocation);

		void SetAxes (IAxis[] axes);

		/**
		 * Draws the plot within the allocation, using the provided
		 * Cairo.Context and axes.
		 */
		void Draw (Cairo.Context cr);

		/**
		 * Returns the range of values for each dimension by comparing
		 * the value of each row of the internal model.  DataColumns
		 * and DataFuncs should be consumed by this method.
		 */
		void GetRange (int dimension, out IComparable min, out IComparable max);
	
		/**
		 * Called when the mouse is hovering over the graph.  If this
		 * plot has something interesting at that location, it should
		 * return true and set widget.  Otherwise, it should return
		 * false.  Implementers should provide a data func to allow
		 * users to specify widget based on the hovered point.
		 */
		bool GetTooltipForPoint (int x, int y, out Widget widget);

		bool SelectPoint (int x, int y);

		bool UnselectPoint (int x, int y);

		void UnselectAll ();

		/**
		 * Returns the x and y for the top-leftmost point in this plot.
		 * When the Surface gets an initial GrabFocus (), it holds a
		 * deathmatch to see which plot has the point closest to (0,0).
		 * If the plot wins, it calls GrabFocus () on the plot.
		 */
		void GetGrabFocusRequest (out int x, out int y);

		/**
		 * Focus the top leftmost point graphed.  A dotted focus line
		 * should be drawn around the point area indicating it is
		 * focused.
		 */
		void GrabFocus ();
		
		/**
		 * Unsets the plot's focus.
		 */
		void ReleaseFocus ();
		
		/**
		 * Focus the next point on the graph.  If the user is already
		 * at the last point, focus should wrap around to the first
		 * point.
		 */
		void FocusNext ();
		
		/**
		 * Focuses the previous point on the graph.  If the user is
		 * already at the first point, focus should wrap around to the
		 * last point.
		 */
		void FocusPrev ();
		
		/**
		 * Focuses the nearest point to the given x and y coordinates,
		 * relative to the plot field.  This is used when jumping
		 * between plots with the Up/Down arrow keys.
		 */
		void FocusNearest (int x, int y);
		
		/**
		 * Gets the x and y for the focused point, relative to the plot
		 * field.
		 */
		bool GetFocusedPoint (out int x, out int y);

		/**
		 * Selects the currently focused point.  If none are focused,
		 * does not change current focus or selection.
		 */
		void SelectFocusedPoint ();

		Requisition LegendSizeRequest (Orientation o, int spacing);

		void DrawLegend (Cairo.Context cr, Gdk.Rectangle alloc,
		                 Orientation o, int spacing);
	}
}
