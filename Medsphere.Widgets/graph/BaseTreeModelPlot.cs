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
	public abstract class BaseTreeModelPlot : ITreeModelPlot
	{
		/* public events */
		public virtual event EventHandler Changed;
		public virtual event EventHandler ModelChanged;
		public virtual event EventHandler SelectionChanged;
		public virtual event PointActivatedHandler PointActivated;

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

		public bool ShowInLegend {
			get { return show_in_legend; }
			set { show_in_legend = value; }
		}

		public abstract bool HasSelection {
			get;
		}

		public abstract bool CanFocus {
			get;
		}

		public abstract bool HasFocus {
			get;
		}

		public abstract PointTooltipDataFunc TooltipDataFunc {
			set;
		}

		/* public methods */
		public BaseTreeModelPlot (TreeModel model)
		{
			Model = model;
		}

		public void SizeAllocate (Gdk.Rectangle allocation)
		{
			Allocation = allocation;
		}

		/**
		 * Sets the DataFunc for a given dimension.  If a DataColumn
		 * was previously set for this dimension, it is unset.
		 *
		 * The ValueDataFunc is similar to TreeCellDataFunc in Gtk's
		 * TreeView.  When the plot needs to request the value of a
		 * given row, it will call this method, providing the TreeIter,
		 * the Model, the Plot and the dimension, expecting in return,
		 * an IComparable value of a type that corresponds to what the
		 * plot expects.
		 */
		public void SetValueDataFunc (int dimension, PlotValueDataFunc func, object data)
		{
			if (col_nums.ContainsKey (dimension)) {
				col_nums.Remove (dimension);
			}

			data_funcs[dimension] = func;
			cb_data[dimension] = data;
		}

		/**
		 * Sets the DataColumn for a given dimension.  If a DataFunc
		 * was previously set for this dimension, it is unset.
		 */
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

		public abstract void SetAxes (IAxis[] axes);
		public abstract void Draw (Cairo.Context cr);
		public abstract void GetRange (int dimension, out IComparable min, out IComparable max);
		public abstract bool SelectPoint (int x, int y);
		public abstract void SelectIter (TreeIter iter);
		public abstract void FocusIter (TreeIter iter);
		public abstract bool UnselectPoint (int x, int y);
		public abstract void UnselectAll ();
		public abstract bool GetSelected (out TreeModel model, out TreeIter iter);
		public abstract bool IterIsSelected (TreeIter iter);
		public abstract bool PathIsSelected (TreePath path);
		public abstract bool GetFocused (out TreeModel model, out TreeIter iter);
		public abstract void GetGrabFocusRequest (out int x, out int y);
		public abstract void GrabFocus ();
		public abstract void ReleaseFocus ();
		public abstract void FocusNext ();
		public abstract void FocusPrev ();
		public abstract void FocusNearest (int x, int y);
		public abstract bool GetFocusedPoint (out int x, out int y);
		public abstract void SelectFocusedPoint ();
		public abstract Requisition LegendSizeRequest (Orientation o, int spacing);
		public abstract void DrawLegend (Cairo.Context cr, Gdk.Rectangle alloc,
		                                 Orientation o, int spacing);
		public abstract bool GetTooltipForPoint (int x, int y, out Widget widget);

		/* protected classes */
		protected class Point : ICloneable
		{
			public long X;
			public long Y;
			
			public IComparable XValue;
			public IComparable YValue;

			public bool IsClipped;
			public bool IsSelected;
			public bool IsFocused;
			public bool IsAGap;

			public Point ()
			{
			}

			public Point (long x, long y, IComparable xval, IComparable yval)
			{
				X = x;
				Y = y;
				XValue = xval;
				YValue = yval;
				IsClipped = false;
			}

			public override string ToString ()
			{
				return "(" + X + ", " + Y + ")";
			}

			public object Clone ()
			{
				return MemberwiseClone ();
			}
		}

		protected class PointXAxisIComparer : IComparer
		{
			public int Compare (object a, object b)
			{
				Point p_a = a as Point;
				Point p_b = b as Point;
				if (p_a == null && p_b == null) {
					return 0;
				}

				return p_a.X.CompareTo (p_b.X);
			}
		}

		/* protected fields */
		protected Gdk.Rectangle Allocation = Gdk.Rectangle.Zero;
		protected IStyleProvider style; 

		protected bool show_values = false;
		protected int show_values_dim = 1;

		protected string name;
		protected TreeModel model;

		protected Hashtable data_funcs = new Hashtable ();
		protected Hashtable col_nums = new Hashtable ();
		protected Hashtable cb_data = new Hashtable ();

		protected const long GRID_MIN = 0;
		protected const long GRID_MAX = 10000;
		
		protected bool show_in_legend = true;

		/* protected methods */
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

		// Returns true if point lies in the viewing plane, false otherwise
		protected bool ClipSegment (IAxis x_axis, IAxis y_axis,
		                            ref Point a, ref Point b)
		{
			long x_min, x_max;
			x_axis.GetGridClipRegion (out x_min, out x_max);

			long y_min, y_max;
			y_axis.GetGridClipRegion (out y_min, out y_max);

			// Use the Cohen-Sutherland algorithm to clip points
			// described http://www.cim.mcgill.ca/~langer/557B/Jan16.html
			int outcode_a, outcode_b;
			while (true)
			{
				outcode_a = Convert.ToInt32 (a.Y > y_max) << 3;
				outcode_a |= Convert.ToInt32 (a.Y < y_min) << 2;
				outcode_a |= Convert.ToInt32 (a.X > x_max) << 1;
				outcode_a |= Convert.ToInt32 (a.X < x_min);

				outcode_b = Convert.ToInt32 (b.Y > y_max) << 3;
				outcode_b |= Convert.ToInt32 (b.Y < y_min) << 2;
				outcode_b |= Convert.ToInt32 (b.X > x_max) << 1;
				outcode_b |= Convert.ToInt32 (b.X < x_min);

				// Trivially accept edge if bitwise OR of outcodes is 0000
				if ((outcode_a | outcode_b) == 0) {
					return true;
				}

				// Trivially reject edge if the bitwise AND of the two outcomes is other than 0000
				if ((outcode_a & outcode_b) != 0) {
					return false;
				}

				// Now, we know that at least one of the points needs to be clipped.
				Point p = a, o = b;
				int current_outcode = outcode_a;

				if (outcode_a == 0) {
					p = b;
					o = a;
					current_outcode = outcode_b;
				}

				double m = (b.Y - a.Y) / ((double)b.X - a.X);
		
				p.IsClipped = true;	

				// handle vertical lines specially, since
				// Cohen-Sutherland doesn't specifically
				// discuss them

				bool is_vertical = (m == Double.PositiveInfinity);
				if ((current_outcode & 8) == 8) {			// Above
					p.Y = y_max;
					p.X = (is_vertical) ? p.X : Convert.ToInt64 (((p.Y - o.Y) / m) + o.X);
				} else if ((current_outcode & 4) == 4) {		// Below
					p.Y = y_min;
					p.X = (is_vertical) ? p.X : Convert.ToInt64 (((p.Y - o.Y) / m) + o.X);
				} else if ((current_outcode & 2) == 2) {		// Right
					p.X = x_max;
					p.Y = (is_vertical) ? p.Y : Convert.ToInt64 (((p.X - o.X) * m) + o.Y);
				} else if ((current_outcode & 1) == 1) {		// Left
					p.X = x_min;
					p.Y = (is_vertical) ? p.Y : Convert.ToInt64 (((p.X - o.X) * m) + o.Y);
				}

				if (outcode_a != 0) {
					a = p;
				} else {
					b = p;
				}
			}
		}

		protected int GridXToRender (long x)
		{
			return Convert.ToInt32 ((((double)x / GRID_MAX) * Allocation.Width) + Allocation.X);
		}

		protected int GridYToRender (long y)
		{
			return Allocation.Y + Allocation.Height
			       - Convert.ToInt32 (((double)y / GRID_MAX) * Allocation.Height);
		}
 
		protected long RenderXToGrid (int x)
		{
			return Convert.ToInt64 (GRID_MAX * (((double)x - Allocation.X) / Allocation.Width));
		}

		/* private methods */
		private void OnDataRowChanged (object o, RowChangedArgs args)
		{
			if (ModelChanged != null) {
				ModelChanged (this, new EventArgs ());
			}
		}

		private void OnDataRowDeleted (object o, RowDeletedArgs args)
		{
			if (ModelChanged != null) {
				ModelChanged (this, new EventArgs ());
			}
		}

		private void OnDataRowInserted (object o, RowInsertedArgs args)
		{
			if (ModelChanged != null) {
				ModelChanged (this, new EventArgs ());
			}
		}

		private void OnDataRowsReordered (object o, RowsReorderedArgs args)
		{
			if (ModelChanged != null) {
				ModelChanged (this, new EventArgs ());
			}
		}
	}
}
