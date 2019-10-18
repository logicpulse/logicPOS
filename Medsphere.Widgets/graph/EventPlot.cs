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
	public class EventPlot : ITreeModelPlot
	{
		/* public events */
		public event EventHandler Changed;
		public event EventHandler ModelChanged;
		public event EventHandler SelectionChanged;
		public event PointActivatedHandler PointActivated;

		/* public properties */
		public string Name {
			get { return String.Empty; }
		}

		public bool FactorIntoAxisCalculation {
			get { return true; }
		}

		public IStyleProvider Style {
			set { style = value; }
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

		public bool ShowInLegend {
			get { return false; }
		}

		public PointTooltipDataFunc TooltipDataFunc {
			set { tooltip_data_func = value; }
		}

		public TreeModel Model {
			get { return model; }
			set { model = value; }
		}

		/* public methods */
		public EventPlot (TreeModel model, PlotColor color, PointShape shape)
		{
			this.model = model;
			this.color = color;
			this.shape = shape;
		}

		public void SizeAllocate (Gdk.Rectangle allocation)
		{
			Allocation = allocation;
		}

		public void SetAxes (IAxis[] axes)
		{
			x_axis = null;

			foreach (IAxis axis in axes)
			{
				switch (axis.Dimension) {
				case 0:
					x_axis = axis;
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

			if (x_axis == null || !x_axis.HasValidRange) {
				return;
			}

			TreeIter iter;
			if (!model.GetIterFirst (out iter)) {
				return;
			}

			do {
				IComparable x_val;
				if (!GetValue (iter, 0, out x_val)) {
					continue;
				}

				if (!x_axis.IsInRange (x_val)) {
					continue;
				}
				
				int x = GridXToRender (x_axis.ValueToGridCoords (x_val));
				
				style.DrawDashedLine (cr, color, x, Allocation.Y, x,
				                      Allocation.Y + Allocation.Height);
				
				style.DrawEventTag (cr, color, x, Allocation.Y + style.EventTagHeight,
				                    Orientation.Vertical);

				style.DrawPoint (cr, color, shape, x, Allocation.Y + 10, false);
			} while (model.IterNext (ref iter));
		}

		public void GetRange (int dimension, out IComparable min, out IComparable max)
		{
			min = max = null;

			if (dimension != 0) {
				return;
			}

			TreeIter iter;
			if (model.GetIterFirst (out iter)) {
				do {
					IComparable val;
					if (GetValue (iter, dimension, out val)) {
						if (min == null && max == null) {
							min = val;
							max = val;
							continue;
						}

						min = (val.CompareTo (min) == -1) ? val : min;
						max = (val.CompareTo (max) == 1) ? val : max;
					}
				} while (model.IterNext (ref iter));
			}
		}
	
		public bool GetTooltipForPoint (int x, int y, out Widget widget)
		{
			widget = null;
			if (tooltip_data_func == null) {
				return false;
			}

			if (y > style.EventTagHeight) {
				return false;
			}

			TreeIter iter;
			if (!model.GetIterFirst (out iter)) {
				return false;
			}

			bool found = false;
			do {
				IComparable x_val;
				if (!GetValue (iter, 0, out x_val)) {
					continue;
				}

				int plot_x = GridXToRender (x_axis.ValueToGridCoords (x_val));

				// x and y come back with reference to the
				// allocation, so transform our plot point to
				// that origin
				plot_x -= Allocation.X;

				int half_size = style.EventTagHalfWidth;
				if (x <= plot_x + half_size
				    && x >= plot_x - half_size) {
					found = true;
					break;
				}
			} while (model.IterNext (ref iter));

			if (!found) {
				return false;
			}

			widget = tooltip_data_func (this, model, iter);
			if (widget == null) {
				return false;
			}

			return true;
		}

		public bool SelectPoint (int x, int y)
		{
			return false;
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
			throw new NotSupportedException ("EventPlot does not support focus.");
		}
		
		public void ReleaseFocus ()
		{
			throw new NotSupportedException ("EventPlot does not support focus.");
		}
		
		public void FocusNext ()
		{
			throw new NotSupportedException ("EventPlot does not support focus.");
		}
		
		public void FocusPrev ()
		{
			throw new NotSupportedException ("EventPlot does not support focus.");
		}
		
		public void FocusNearest (int x, int y)
		{
			throw new NotSupportedException ("EventPlot does not support focus.");
		}
		
		public bool GetFocusedPoint (out int x, out int y)
		{
			throw new NotSupportedException ("EventPlot does not support focus.");
		}

		public void SelectFocusedPoint ()
		{
			throw new NotSupportedException ("EventPlot does not support focus.");
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

		public bool GetSelected (out TreeModel model, out TreeIter iter)
		{
			model = null;
			iter = TreeIter.Zero;
			return false;
		}

		public void SelectIter (TreeIter iter)
		{
		}

		public bool IterIsSelected (TreeIter iter)
		{
			return false;
		}

		public bool PathIsSelected (TreePath path)
		{
			return false;
		}

		public void SetValueDataFunc (int dimension, PlotValueDataFunc func, object data)
		{
			if (col_nums.ContainsKey (dimension)) {
				col_nums.Remove (dimension);
			}

			data_funcs[dimension] = func;
			cb_data[dimension] = data;
		}

		public void SetValueDataColumn (int dimension, int column)
		{
			if (column < 0 || column > model.NColumns) {
				throw new ArgumentException ("column cannot be smaller than 0 or greater than the number of columns in the model.");
			}

			if (data_funcs.ContainsKey (dimension)) {
				data_funcs.Remove (dimension);
			}

			col_nums[dimension] = column;
		}

		/* private fields */
		private Gdk.Rectangle Allocation = Gdk.Rectangle.Zero;
		private IStyleProvider style;

		private IAxis x_axis = null;

		private PlotColor color;
		private PointShape shape;

		private TreeModel model;

		private PointTooltipDataFunc tooltip_data_func = null;
		private Hashtable col_nums = new Hashtable ();
		private Hashtable data_funcs = new Hashtable ();
		private Hashtable cb_data = new Hashtable ();

		private const long GRID_MIN = 0;
		private const long GRID_MAX = 10000;

		/* private methods */
		internal bool GetValue (TreeIter iter, int dimension, out IComparable val)
		{
			val = null;

			if (model == null || iter.Equals (TreeIter.Zero)) {
				return false;
			}
			
			if (col_nums.ContainsKey (dimension)) {
				int col = (int)col_nums[dimension];

				val = (IComparable)model.GetValue (iter, col);
				return true;
			}

			if (data_funcs.ContainsKey (dimension)) {
				PlotValueDataFunc func = data_funcs[dimension] as PlotValueDataFunc;
				if (func == null) {
					return false;
				}

				val = func (this, dimension, model, iter, cb_data[dimension]);
				return true;
			}
			return false;
		}

		private int GridXToRender (long x)
		{
			return Convert.ToInt32 ((((double)x / GRID_MAX) * Allocation.Width) + Allocation.X);
		}
	}
}
