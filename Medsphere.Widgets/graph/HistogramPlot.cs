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
	public class HistogramPlot : ITreeModelPlot
	{
		/* public events */
		public event EventHandler Changed;
		public event EventHandler ModelChanged;
		public event EventHandler SelectionChanged;
		public event PointActivatedHandler PointActivated;

		/* public properties */
		public string Name {
			get { return name; }
			set {
				name = value;

				if (Changed != null) {
					Changed (this, new EventArgs ());
				}
			}
		}

		public bool FactorIntoAxisCalculation {
			get { return true; }
		}

		public TreeModel Model {
			get { return model; }
			set {
				if (model != null) {
					model.RowChanged -= new RowChangedHandler (OnDataRowChanged);
					model.RowDeleted -= new RowDeletedHandler (OnDataRowDeleted);
					model.RowInserted -= new RowInsertedHandler (OnDataRowInserted);
					model.RowsReordered -= new RowsReorderedHandler (OnDataRowsReordered);
				}

				model = value;
 				model.RowChanged += new RowChangedHandler (OnDataRowChanged);
				model.RowDeleted += new RowDeletedHandler (OnDataRowDeleted);
				model.RowInserted += new RowInsertedHandler (OnDataRowInserted);
				model.RowsReordered += new RowsReorderedHandler (OnDataRowsReordered);

				if (ModelChanged != null) {
					ModelChanged (this, new EventArgs ());
				}
			}
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

		public bool ShowValues {
			get { return show_values; }
			set {
				if (show_values != value) {
					show_values = value;

					if (Changed != null) {
						Changed (this, new EventArgs ());
					}
				}
			}
		}

		public int ShowValuesDimension {
			get { return show_values_dim; }
			set {
				if (show_values_dim != value) {
					show_values_dim = value;

					if (Changed != null) {
						Changed (this, new EventArgs ());
					}
				}
			}
		}

		public bool ShowInLegend {
			get { return show_in_legend; }
			set { show_in_legend = value; }
		}

		public bool HasSelection {
			get { return (selected_path != null); }
		}

		public bool CanFocus {
			get { return true; }
		}

		public bool HasFocus {
			get { return (focused_path != null); }
		}

		public PointTooltipDataFunc TooltipDataFunc {
			set { tooltip_data_func = value; }
		}

		/* public methods */
		public HistogramPlot (TreeModel model, PlotColor color)
		{
			Model = model;
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

			if ((x_axis == null || !x_axis.HasValidRange)
			    || (y_axis == null || !y_axis.HasValidRange)) {
				// we need both axes to be valid to draw anything
				return;
			}

			TreeIter iter;
			if (!model.GetIterFirst (out iter)) {
				return;
			}

			int selected_x = -1;
			int selected_y = -1;

			int total_height = (Allocation.Y + Allocation.Height);
			do {
				TreePath path = model.GetPath (iter);

				IComparable x_val, y_val;
				if (!GetValue (iter, 0, out x_val)
				    || !GetValue (iter, 1, out y_val)) {
					return;
				}

				// see where min_val would be plotted
				int x = GridXToRender (x_axis.ValueToGridCoords (x_val));
				int y = GridYToRender (y_axis.ValueToGridCoords (y_val));

				if (IsBarOutOfBounds (x, y)) {
					continue;
				}

				bool is_focused = (focused_path != null && focused_path.Compare (path) == 0);
				bool is_selected = (selected_path != null && selected_path.Compare (path) == 0);
	
				style.DrawFixedWidthBar (cr, color, x, y,
							 total_height - y,
							 is_focused);

				if (is_selected) {
					selected_x = x;
					selected_y = y;
				}
				
				if (!show_values) {
					continue;
				}

				string val = String.Empty;
				if (show_values_dim == 0) {
					val = x_val.ToString ();
				} else if (show_values_dim == 1) {
					val = y_val.ToString ();
				}

				if (val == String.Empty) {
					continue;
				}

				int val_w, unused;
				style.GetAxisLabelMetrics (val, out val_w, out unused);

				if (show_values) {
					style.DrawPointValue (cr, val,
					                      x - (val_w / 2), y);
				}
			} while (model.IterNext (ref iter));

			// draw the selected point above everything else
			if (selected_x != -1) {
				style.DrawSelectedFixedWidthBar (cr, color,
				                                 selected_x, selected_y,
				                                 total_height - selected_y);
			}
		}

		public void GetRange (int dimension, out IComparable min, out IComparable max)
		{
			if (dimension < 0 || dimension > 1) {
				throw new ArgumentException ("HistogramPlot only supports 2 dimensions.");
			}
			
			min = max = null;

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

		public bool SelectPoint (int x, int y)
		{
			return SelectPointHelper (x, y, true);
		}

		public void SelectIter (TreeIter iter)
		{
			selected_path = model.GetPath (iter);
			if (Changed != null) {
				Changed (this, new EventArgs ());
			}
		}

		public bool UnselectPoint (int x, int y)
		{
			return SelectPointHelper (x, y, false);
		}

		public void UnselectAll ()
		{
			selected_path = null;

			if (Changed != null) {
				Changed (this, new EventArgs ());
			}
		}

		public bool GetSelected (out TreeModel model, out TreeIter iter)
		{
			model = this.model;
			iter = TreeIter.Zero;

			if (selected_path == null) {
				return false;
			}

			if (!model.GetIter (out iter, selected_path)) {
				return false;
			}

			return true;
		}

		public bool IterIsSelected (TreeIter iter)
		{
			TreePath path = model.GetPath (iter);
			return PathIsSelected (path);
		}

		public bool PathIsSelected (TreePath path)
		{
			if (path != null) {
				return (selected_path != null
				        && path.Compare (selected_path) == 0);
			}

			return false;
		}

		public bool GetFocused (out TreeModel model, out TreeIter iter)
		{
			model = this.model;
			iter = TreeIter.Zero;

			if (focused_path == null) {
				return false;
			}

			if (!model.GetIter (out iter, focused_path)) {
				return false;
			}
			
			return true;
		}

		public void GetGrabFocusRequest (out int x, out int y)
		{
			x = y = -1;

			// Return the position of the first point plotted, with
			// the assumption this will be the leftmost point.
			TreeIter iter;
			if (model.GetIterFirst (out iter)) {
				GetPlotPoint (iter, out x, out y);
			}
		}

		public void GrabFocus ()
		{
			focused_path = null;

			// Focus the first point plotted, with the assumption
			// this will be the leftmost point.

			// should be revised if we write some crazy axis that
			// put things in non-standard locations, which is quite
			// possible.
			TreeIter iter;
			if (model.GetIterFirst (out iter)) {
				focused_path = Model.GetPath (iter);

				if (Changed != null) {
					Changed (this, new EventArgs ());
				}
			}
		}

		public void ReleaseFocus ()
		{
			if (focused_path == null) {
				return;
			}

			focused_path = null;

			if (Changed != null) {
				Changed (this, new EventArgs ());
			}
		}

		public void FocusNext ()
		{
			FocusHelper (true);
		}

		public void FocusPrev ()
		{
			FocusHelper (false);
		}

		public void FocusNearest (int x, int y)
		{
			TreeIter iter;
			if (!model.GetIterFirst (out iter)) {
				return;
			}

			if ((x_axis == null || !x_axis.HasValidRange)
			    || (y_axis == null || !y_axis.HasValidRange)) {
				return;
			}

			// find the point with the min distance to x (since the
			// user hit up/down)
			TreePath min_path = TreePath.NewFirst ();
			double min_distance = Double.MaxValue;
			do {
				IComparable x_val, y_val;
				if (!GetValue (iter, 0, out x_val)
				    || !GetValue (iter, 1, out y_val)) {
					return;
				}

				// we need it relative to the plot field, so subtract the X allocation.
				int plot_x = GridXToRender (x_axis.ValueToGridCoords (x_val)) - Allocation.X;

				double distance = Math.Abs (plot_x - x);
				if (min_distance > distance) {
					min_distance = distance;
					min_path = model.GetPath (iter);
				}
			} while (model.IterNext (ref iter));

			focused_path = min_path;
		}

		public bool GetFocusedPoint (out int x, out int y)
		{
			x = y = -1;
			if (focused_path == null) {
				return false;
			}

			TreeIter iter;
			if (!model.GetIter (out iter, focused_path)) {
				return false;
			}

			GetPlotPoint (iter, out x, out y);

			// we need it relative to the plot field, so subtract the X and Y.
			x -= Allocation.X;
			y -=  Allocation.Y;

			return true;
		}

		public void SelectFocusedPoint ()
		{
			bool fire_selection_changed = false;
			if (focused_path.Compare (selected_path) != 0) {
				fire_selection_changed = true;
			}

			selected_path = focused_path.Copy ();
			if (Changed != null) {
				Changed (this, new EventArgs ());
			}

			// fire SelectionChanged only if the selection has
			// changed
			if (fire_selection_changed
			    && SelectionChanged != null) {
				SelectionChanged (this, new EventArgs ());
			}
			
			// always fire PointActivated
			TreeIter iter;
			if (PointActivated != null
			    && model.GetIter (out iter, selected_path)) {
				PointActivated (this, model, iter);
			}
		}

		public Requisition LegendSizeRequest (Orientation o, int spacing)
		{
			Requisition req = Requisition.Zero;

			int width, height;
			style.GetLegendLabelMetrics (Name, out width,
			                             out height);

			req.Width = width + style.PointSize
			            + style.LegendSpacing;
			req.Height = Math.Max (height, style.PointSize);

			return req;
		}

		public void DrawLegend (Cairo.Context cr, Gdk.Rectangle alloc,
		                        Orientation o, int spacing)
		{
			int width, height;
			style.GetLegendLabelMetrics (Name, out width,
			                             out height);

			style.DrawPoint (cr, color, PointShape.Square,
			                 alloc.X + style.PointHalfSize,
			                 alloc.Y + style.PointHalfSize, false);

			style.DrawLegendLabel (cr, name,
			                       alloc.X + style.PointSize + style.LegendSpacing,
			                       alloc.Y + style.PointHalfSize - (height / 2));
		}

		public bool GetTooltipForPoint (int x, int y, out Widget widget)
		{
			widget = null;
			if (tooltip_data_func == null) {
				return false;
			}

			TreeIter iter;
			if (!GetPointNear (x, y, out iter)) {
				return false;
			}

			widget = tooltip_data_func (this, model, iter);
			if (widget == null) {
				return false;
			}

			return false;
		}

		/* private fields */
		private Gdk.Rectangle Allocation = Gdk.Rectangle.Zero;
		private IStyleProvider style;

		private IAxis x_axis = null, y_axis = null;

		private PlotColor color;

		private bool show_values = false;
		private int show_values_dim = 1;
		
		private string name;
		private TreeModel model;

		private TreePath selected_path = null;
		private TreePath focused_path = null;

		private Hashtable data_funcs = new Hashtable ();
		private Hashtable col_nums = new Hashtable ();
		private Hashtable cb_data = new Hashtable ();

		private const long GRID_MIN = 0;
		private const long GRID_MAX = 10000;

		private bool show_in_legend = true;

		private PointTooltipDataFunc tooltip_data_func = null;

		/* private methods */
		private bool SelectPointHelper (int x, int y, bool select)
		{
			TreeIter iter = TreeIter.Zero;
			if (!GetPointNear (x, y, out iter)) {
				return false;
			}

			TreePath path = model.GetPath (iter);

			// always focus
			focused_path = path.Copy ();

			if (select) {
				// fire SelectionChanged if the
				// selection has actually
				// changed
				if (selected_path == null
				    || path.Compare (selected_path) != 0) {
					selected_path = path.Copy ();
					if (SelectionChanged != null) {
						SelectionChanged (this, new EventArgs ());
					}
				}
			} else {
				// deselect if we previously
				// had a selection
				if (selected_path != null) {
					selected_path = null;
					if (SelectionChanged != null) {
						SelectionChanged (this, new EventArgs ());
					}
				}
			}

			// always fire PointActivated
			if (PointActivated != null) {
				PointActivated (this, model, iter);
			}

			return true;
		}

		private bool GetPointNear (int x, int y, out TreeIter closest_iter)
		{
			closest_iter = TreeIter.Zero;

			TreeIter iter;
			if (!Model.GetIterFirst (out iter)) {
				return false;
			}

			if ((x_axis == null || !x_axis.HasValidRange)
			    || (y_axis == null || !y_axis.HasValidRange)) {
				return false;
			}

			// iterate through all the points, finding out where we
			// would render them to see if it intersects with the
			// click point.
			do {
				IComparable x_val, y_val;
				if (!GetValue (iter, 0, out x_val)
				    || !GetValue (iter, 1, out y_val)) {
					continue;
				}

				int plot_x, plot_y;
				GetPlotPoint (iter, out plot_x, out plot_y);

				plot_x -= Allocation.X;
				plot_y -= Allocation.Y;

				int half_width = style.FixedBarWidth / 2;
				int baseline = (Allocation.Y + Allocation.Height);
				if ((x <= plot_x + half_width && x >= plot_x - half_width)
				    && (y <= baseline && y >= plot_y)) {
					closest_iter = iter;
					return true;
				}
			} while (Model.IterNext (ref iter));
			
			return false;
		}

		private void FocusHelper (bool move_next)
		{
			if (focused_path == null) {
				return;
			}
			
			TreeIter focused_iter;
			if (!model.GetIter (out focused_iter, focused_path)) {
				return;
			}

			int focus_x, focus_y;
			GetPlotPoint (focused_iter, out focus_x, out focus_y);

			// we can't just do focused_path.Prev () since there is
			// a disconnect between the order in the model and
			// where the axis has us place points.  besides, we
			// need to clamp the focus region to the zoom rect.
			TreeIter iter;
			if (!model.GetIterFirst (out iter)) {
				return;
			}

			int total_height = (Allocation.Y + Allocation.Height);

			TreePath min_path = null;
			int min_x_delta = Int32.MaxValue;
			int min_y_delta = Int32.MaxValue;
			do {
				TreePath path = model.GetPath (iter);

				// don't try to focus the same point again
				if (path.Compare (focused_path) == 0) {
					continue;
				}

				int x, y;
				GetPlotPoint (iter, out x, out y);

				// if its out of bounds, throw it out
				if (IsBarOutOfBounds (x, y)) {
					continue;
				}

				int x_delta = 0, y_delta = 0;
				if (move_next) {
					// find the bar with the smallest positive x
					// delta, and smallest y, or if there
					// is a tie, the one that's larger on the y
					// axis
					x_delta = x - focus_x;
					y_delta = (focus_x != x) ? total_height - y : focus_y - y;
				} else {
					// find the bar with the smallest positive x
					// delta, and largest y, or if there
					// is a tie, the one that's larger on the y
					// axis
					x_delta = focus_x - x;
					y_delta = (focus_x != x) ? y : y - focus_y;
				}

				// don't go backward, and don't pick a bigger
				// delta than we already have
				if (x_delta < 0
				    || x_delta > min_x_delta) {
					continue;
				}

				if (x_delta == min_x_delta
				    && y_delta >= min_y_delta) {
					continue;
				}

				// if we're at the same X value, make sure
				// we're moving forward in the y direction so
				// we don't get stuck in loops
				if (x_delta == 0
				    && y_delta < 0) {
					continue;
				}

				min_x_delta = x_delta;
				min_y_delta = y_delta;
				min_path = path;
			} while (model.IterNext (ref iter));

			if (min_path != null) {
				focused_path = min_path;
			} else {
				if (move_next) {
					focused_path = TreePath.NewFirst ();
				} else {
					focused_path = new TreePath (new int[] {
						model.IterNChildren () - 1
					});
				}
			}

			if (Changed != null) {
				Changed (this, new EventArgs ());
			}
		}

		private bool IsBarOutOfBounds (int x, int y)
		{
			int total_height = (Allocation.Y + Allocation.Height);
			int total_width = (Allocation.X + Allocation.Width);

			int half_w = (style.FixedBarWidth / 2);

			return ((x + half_w < Allocation.X)
			        || (x - half_w > total_width)
			        || (y > total_height));
		}

		private void GetPlotPoint (TreeIter iter, out int x, out int y)
		{
			x = y = -1;

			IComparable x_val, y_val;
			if (!GetValue (iter, 0, out x_val)
			    || !GetValue (iter, 1, out y_val)) {
				return;
			}
	
			// see where min_val would be plotted
			x = GridXToRender (x_axis.ValueToGridCoords (x_val));
			y = GridYToRender (y_axis.ValueToGridCoords (y_val));
		}

		private bool GetValue (TreeIter iter, int dimension, out IComparable val)
		{
			val = null;

			//if (model == null || iter.Equals (TreeIter.Zero)) {
			//	return false;
			//}
			
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

		private int GridYToRender (long y)
		{
			return Allocation.Y + Allocation.Height
			       - Convert.ToInt32 (((double)y / GRID_MAX) * Allocation.Height);
		}

		private void OnDataRowChanged (object o, RowChangedArgs args)
		{
			if (Changed != null) {
				ModelChanged (this, new EventArgs ());
			}
		}

		private void OnDataRowDeleted (object o, RowDeletedArgs args)
		{
			if (Changed != null) {
				ModelChanged (this, new EventArgs ());
			}
		}

		private void OnDataRowInserted (object o, RowInsertedArgs args)
		{
			if (Changed != null) {
				ModelChanged (this, new EventArgs ());
			}
		}

		private void OnDataRowsReordered (object o, RowsReorderedArgs args)
		{
			if (Changed != null) {
				ModelChanged (this, new EventArgs ());
			}
		}
	}
}
