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
	public class ReferenceRangePlot : IPlot
	{
		/* public events */
		public event EventHandler Changed;
		public virtual event EventHandler SelectionChanged;

        /* public properties */
		public string Name {
			get { return String.Empty; }
		}

		public bool FactorIntoAxisCalculation {
			get { return false; }
		}

		public IStyleProvider Style {
			set { style = value; }
		}

		public PlotColor Color {
			set {
				color = value;
				
				if (Changed != null) {
					Changed (this, new EventArgs ());
				}
			}
		}
		
		public bool ShowInLegend {
			get { return false; }
		}

		public bool HasSelection {
			get { return false; }
		}

		public bool CanFocus {
			get { return false; }
		}

		public bool HasFocus {
			get { return false; }
		}

		/* public methods */
		public ReferenceRangePlot (IComparable min, IComparable max, int dimension, PlotColor color)
		{
			this.ref_min = min;
			this.ref_max = max;
			this.ref_dimension = dimension;

			this.color = color;
		}

		public void SizeAllocate (Gdk.Rectangle allocation)
		{
			Allocation = allocation;
		}

		public void SetAxes (IAxis[] axes)
		{
			x_axis = y_axis = null;

			foreach (IAxis axis in axes)
			{
				switch (axis.Dimension) {
				case 0:
					x_axis = axis;
					break;
				case 1:
					y_axis = axis;
					break;
				}
			}
		}

		public void Draw (Cairo.Context cr)
		{
			if (Allocation.Width == 0
			    && Allocation.Height == 0) {
				return;
			}

			if (x_axis == null || y_axis == null
			    || (ref_min == null && ref_max == null)) {
				return;
			}

			// change when we support more than 2 dimensions
			if (ref_dimension < 0 || ref_dimension > 1) {
				return;
			}

			int plot_max_x = Allocation.X + Allocation.Width;
			int plot_max_y = Allocation.Y + Allocation.Height;
			if (ref_dimension == 0) {
				if (ref_min != null) {
					long min = GRID_MIN;
					if (ref_min.CompareTo (x_axis.MinValue) == 1) {
						min = x_axis.ValueToGridCoords (ref_min);
					}

					if (min > GRID_MAX) {
						min = GRID_MAX;
					}

					if (min > GRID_MIN) {
						style.DrawReferenceRectangle (cr, color,
									      Allocation.X, Allocation.Y,
									      GridXToRender (min), Allocation.Height);
					}
				}
				
				if (ref_max != null) {
					long max = GRID_MAX;
					if (ref_max.CompareTo (x_axis.MaxValue) == -1) {
						max = x_axis.ValueToGridCoords (ref_max);
					}

					if (max < GRID_MIN) {
						max = GRID_MIN;
					}

					if (max < GRID_MAX) {
						style.DrawReferenceRectangle (cr, color,
									      GridXToRender (max), Allocation.Y,
									      plot_max_x - GridXToRender (max), Allocation.Height);
					}
				}
			} else if (ref_dimension == 1) {
				if (ref_min != null) {
					long min = GRID_MIN;
					if (ref_min.CompareTo (y_axis.MinValue) == 1) {
						min = y_axis.ValueToGridCoords (ref_min);
					}

					if (min > GRID_MAX) {
						min = GRID_MAX;
					}

					if (min > GRID_MIN) {
						style.DrawReferenceRectangle (cr, color,
									      Allocation.X, GridYToRender (min),
									      Allocation.Width, plot_max_y - GridYToRender (min));
					}
				}
				
				if (ref_max != null) {
					long max = GRID_MAX;
					if (ref_max.CompareTo (y_axis.MaxValue) == -1) {
						max = y_axis.ValueToGridCoords (ref_max);
					}

					if (max < GRID_MIN) {
						max = GRID_MIN;
					}

					if (max < GRID_MAX) {
						style.DrawReferenceRectangle (cr, color,
									      Allocation.X, Allocation.Y,
									      Allocation.Width, GridYToRender (max));
					}
				}
			}
		}

		public void GetRange (int dimension, out IComparable min, out IComparable max)
		{
			min = max = null;
		}

		public bool SelectPoint (int x, int y)
		{
			return false;
		}

		public void SelectIter (TreeIter iter)
		{
			throw new NotSupportedException ();
		}

		public bool UnselectPoint (int x, int y)
		{
			return false;
		}

		public void UnselectAll ()
		{
		}

		public void GetGrabFocusRequest (out int x, out int y)
		{
			x = y = -1;
		}

		public void GrabFocus ()
		{
			throw new NotSupportedException ("ReferenceRangePlot does not support focus.");
		}

		public void ReleaseFocus ()
		{
			throw new NotSupportedException ("ReferenceRangePlot does not support focus.");
		}

		public void FocusNext ()
		{
			throw new NotSupportedException ("ReferenceRangePlot does not support focus.");
		}

		public void FocusPrev ()
		{
			throw new NotSupportedException ("ReferenceRangePlot does not support focus.");
		}

		public void FocusNearest (int x, int y)
		{
			throw new NotSupportedException ("ReferenceRangePlot does not support focus.");
		}

		public bool GetFocusedPoint (out int x, out int y)
		{
			x = y = -1;

			throw new NotSupportedException ("ReferenceRangePlot does not support focus.");
		}

		public void SelectFocusedPoint ()
		{
			throw new NotSupportedException ("ReferenceRangePlot does not support focus.");
		}

		public Requisition LegendSizeRequest (Orientation o, int spacing)
		{
			return Requisition.Zero;
		}

		public void DrawLegend (Cairo.Context cr, Gdk.Rectangle alloc,
		                        Orientation o, int spacing)
		{
			// don't draw anything to the legend
		}

		public bool GetTooltipForPoint (int x, int y, out Widget widget)
		{
			// no tooltip support

			widget = null;
			return false;
		}

		/* private fields */
		private Gdk.Rectangle Allocation = Gdk.Rectangle.Zero;
		private IStyleProvider style;

		private IAxis x_axis = null, y_axis = null;

		private PlotColor color;

		private const long GRID_MIN = 0;
		private const long GRID_MAX = 10000;

		private IComparable ref_min = null, ref_max = null;
		private int ref_dimension = 1;

		/* private methods */
		private int GridXToRender (long x)
		{
			return Convert.ToInt32 ((((double)x / GRID_MAX) * Allocation.Width) + Allocation.X);
		}

		private int GridYToRender (long y)
		{
			return Allocation.Y + Allocation.Height
			       - Convert.ToInt32 (((double)y / GRID_MAX) * Allocation.Height);
		}
	}
}
