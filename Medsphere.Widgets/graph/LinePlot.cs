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
	public delegate bool LinePlotPointGapFunc (TreeModel model, TreeIter iter);

	public class LinePlot : BaseTreeModelPlot
	{
		/* public events */
		public override event EventHandler Changed;
		public override event EventHandler ModelChanged;
		public override event EventHandler SelectionChanged;
		public override event PointActivatedHandler PointActivated;

		/* public properties */
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

		public bool DrawPoints {
			get { return draw_points; }
			set { 
				if (draw_points != value) {
					draw_points = value;
					
					if (Changed != null) {
						Changed (this, new EventArgs ());
					}
				}
			}
		}

		public override bool HasSelection {
			get { return (selected_path != null); }
		}

		public override bool CanFocus {
			get { return draw_points; }
		}

		public override bool HasFocus {
			get { return (focused_path != null); }
		}

		public override PointTooltipDataFunc TooltipDataFunc {
			set { tooltip_data_func = value; }
		}

		/* public fields */
		public LinePlotPointGapFunc PointGapFunc = null;

		/* public methods */
		public LinePlot (TreeModel model, PlotColor color, PointShape shape)
			: base (model)
		{
			this.color = color;
			this.shape = shape;
		}

		public override void SetAxes (IAxis[] axes)
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

		public override void Draw (Cairo.Context cr)
		{
			if (Allocation.Width == 0
			    && Allocation.Height == 0) {
				return;
			}

			if ((x_axis == null || !x_axis.HasValidRange)
			    || (y_axis == null || !y_axis.HasValidRange)) {
				return;
			}

			// Find which points are visible
			Point[] points;
			FindPointsInRange (x_axis, y_axis, out points);

			Point selected_point = null;

			ArrayList gap_points = new ArrayList ();

			int x = 0, y = 0;

			// Plot the points, and connecting vertices
			Point prev = null;
			foreach (Point p in points)
			{
				if (prev != null &&
				    prev.X == p.X && prev.Y == p.Y) {
					continue;
				}

				if (p.IsAGap) {
					gap_points.Add (p);
					continue;
				}

				// When a gap is requested for an iter, collect
				// all the adjoining gaps and draw them all at
				// once
				if (gap_points.Count > 0) {
					DrawGapLines (cr, gap_points, prev, p);

					gap_points.Clear ();
				} else if (prev != null) {
					style.DrawLine (cr, color,
							GridXToRender (prev.X),
							GridYToRender (prev.Y),
							GridXToRender (p.X),
							GridYToRender (p.Y));
				}

				prev = p;

				if (p.IsClipped || !draw_points) {
					continue;
				}

				style.DrawPoint (cr, color, shape,
						 GridXToRender (p.X),
						 GridYToRender (p.Y),
				                 p.IsFocused);

				if (p.IsSelected) {
					selected_point = p;
				}

				if (!show_values) {
					continue;
				}

				string val = String.Empty;
				if (show_values_dim == 0) {
					val = p.XValue.ToString ();
				} else if (show_values_dim == 1) {
					val = p.YValue.ToString ();
				}

				if (val == String.Empty) {
					continue;
				}

				int val_w, val_h;
				style.GetAxisLabelMetrics (val, out val_w, out val_h);

				x = GridXToRender (p.X) - (val_w / 2);
				y = GridYToRender (p.Y);
				if (x <= Allocation.X) {
					x = Allocation.X + 5;
				} else if (x + val_w >= Allocation.X + Allocation.Width) {
					x = Allocation.X + Allocation.Width - val_w - 5;
				}

				style.DrawPointValue (cr, val, x, y);
			}
			
			// handle gap points that don't follow a "real" point
			if (gap_points.Count > 0) {
				DrawGapLines (cr, gap_points, prev, null);

				gap_points.Clear ();
			}

			// draw the selected point above everything else
			if (selected_point != null) {
				style.DrawSelectedPoint (cr, color, shape,
							 GridXToRender (selected_point.X),
							 GridYToRender (selected_point.Y));
			}
		}

		public override void GetRange (int dimension, out IComparable min, out IComparable max)
		{
			if (dimension < 0 || dimension > 1) {
				throw new ArgumentException ("LinePlot only supports 2 dimensions.");
			}
			
			min = max = null;

			TreeIter iter;
			if (model.GetIterFirst (out iter)) {
				do {
					if (PointGapFunc != null
					    && PointGapFunc (model, iter)) {
						continue;
					}

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

		public override bool SelectPoint (int x, int y)
		{
			return SelectPointHelper (x, y, true);
		}

		public override void SelectIter (TreeIter iter)
		{
			TreePath path = model.GetPath (iter);
			if (selected_path != null
			    && path.Compare (selected_path) == 0) {
				return;
			}

			selected_path = path;
			if (Changed != null) {
				Changed (this, new EventArgs ());
			}

			if (SelectionChanged != null) {
				SelectionChanged (this, new EventArgs ());
			}
		}

		public override void FocusIter (TreeIter iter)
		{
			if (!CanFocus) {
				return;
			}

			TreePath path = model.GetPath (iter);
			if (focused_path != null
			    && path.Compare (focused_path) == 0) {
				return;
			}

			focused_path = path;
			if (Changed != null) {
				Changed (this, new EventArgs ());
			}
		}

		public override bool UnselectPoint (int x, int y)
		{
			return SelectPointHelper (x, y, false);
		}

		public override void UnselectAll ()
		{
			selected_path = null;

			if (SelectionChanged != null) {
				SelectionChanged (this, new EventArgs ());
			}
		}

		public override bool GetSelected (out TreeModel model, out TreeIter iter)
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

		public override bool IterIsSelected (TreeIter iter)
		{
			TreePath path = model.GetPath (iter);
			return PathIsSelected (path);
		}

		public override bool PathIsSelected (TreePath path)
		{
			if (path != null && selected_path != null) {
				return path.Compare (selected_path) == 0;
			}

			return false;
		}

		public override bool GetFocused (out TreeModel model, out TreeIter iter)
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

		public override void GetGrabFocusRequest (out int x, out int y)
		{
			x = y = -1;
			
			if (!CanFocus) {
				return;
			}

			// Return the position of the first point plotted, with
			// the assumption this will be the leftmost point.
			TreeIter iter;
			if (model.GetIterFirst (out iter)) {
				GetPlotPoint (iter, out x, out y);
			}
		}

		public override void GrabFocus ()
		{
			focused_path = null;
			
			if (!CanFocus) {
				return;
			}

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

		public override void ReleaseFocus ()
		{
			if (focused_path == null) {
				return;
			}

			focused_path = null;

			if (Changed != null) {
				Changed (this, new EventArgs ());
			}
		}

		public override void FocusNext ()
		{
			if (!CanFocus) {
				return;
			}

			FocusHelper (true);
		}

		public override void FocusPrev ()
		{
			if (!CanFocus) {
				return;
			}

			FocusHelper (false);
		}

		public override void FocusNearest (int x, int y)
		{
			if (!CanFocus) {
				return;
			}

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

				// we need it relative to the plot field, so
				// subtract the X allocation.
				int plot_x = GridXToRender (x_axis.ValueToGridCoords (x_val)) - Allocation.X;

				double distance = Math.Abs (plot_x - x);
				if (min_distance > distance) {
					min_distance = distance;
					min_path = model.GetPath (iter);
				}
			} while (model.IterNext (ref iter));

			focused_path = min_path;
		}

		public override bool GetFocusedPoint (out int x, out int y)
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

			// we need it relative to the plot field, so subtract
			// the X and Y.
			x -= Allocation.X;
			y -= Allocation.Y;

			return true;
		}

		public override void SelectFocusedPoint ()
		{
			if (focused_path == null) {
				return;
			}

			bool fire_selection_changed = false;
			if (selected_path == null
			    || focused_path.Compare (selected_path) != 0) {
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

		public override Requisition LegendSizeRequest (Orientation o, int spacing)
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

		public override void DrawLegend (Cairo.Context cr, Gdk.Rectangle alloc,
		                        Orientation o, int spacing)
		{
			int width, height;
			style.GetLegendLabelMetrics (Name, out width,
			                             out height);

			style.DrawPoint (cr, color, shape,
			                 alloc.X + style.PointHalfSize,
			                 alloc.Y + style.PointHalfSize, false);

			style.DrawLegendLabel (cr, name,
			                       alloc.X + style.PointSize + style.LegendSpacing,
			                       alloc.Y + style.PointHalfSize - (height / 2));
		}

		public override bool GetTooltipForPoint (int x, int y, out Widget widget)
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

			return true;
		}

		/* private fields */
		private IAxis x_axis = null, y_axis = null;

		private PlotColor color;
		private PointShape shape;

		private bool draw_points = true;
		
		private TreePath selected_path = null;
		private TreePath focused_path = null;

		private const int GAP_PAD = 2;

		private PointTooltipDataFunc tooltip_data_func = null;

		/* private methods */
		private void DrawGapLines (Cairo.Context cr, ArrayList gap_points,
		                           Point prev, Point p)
		{
			Point gap_point = null;
			double m = 0;
			long prev_gap_x = 0;

			int x, y;

			// only draw the line going from the previous point to
			// the gap if the gap isn't the first point in the
			// graph
			if (prev != null) {
				gap_point = (Point)gap_points[0];
				
				if (p != null) {
					m = (p.X != prev.X) ? ((double)p.Y - prev.Y) / ((double)p.X - prev.X) : 1;
				}
				
				x = GridXToRender (gap_point.X) - style.PointHalfSize - GAP_PAD;
				y = GridYToRender (Convert.ToInt64 (((RenderXToGrid (x) - (double)prev.X) * m) + prev.Y));

				style.DrawLine (cr, color, x, y - 8, x, y + 8);

				if (p == null) {
					style.DrawDashedLine (cr, color,
							      GridXToRender (prev.X),
							      GridYToRender (prev.Y),
							      x, y);
				} else {
					style.DrawLine (cr, color,
							GridXToRender (prev.X),
							GridYToRender (prev.Y),
							x, y);
				}

				prev_gap_x = gap_point.X;
			}

			Point s = (p == null) ? prev : p;

			// if prev and p are both null, don't draw anything
			if (s == null) {
				return;
			}

			// draw any sequential gaps, drawing two-ended lines
			for (int i = 1; i < gap_points.Count; i++)
			{
				gap_point = (Point)gap_points[i];

				int x1 = GridXToRender (gap_point.X) - style.PointHalfSize - GAP_PAD;
				int y1 = GridYToRender (Convert.ToInt64 (((RenderXToGrid (x1) - (double)s.X) * m) + s.Y));
				int x2 = GridXToRender (prev_gap_x) + style.PointHalfSize + GAP_PAD;
				int y2 = GridYToRender (Convert.ToInt64 (((RenderXToGrid (x2) - (double)s.X) * m) + s.Y));

				style.DrawDashedLine (cr, color, x1, y1, x2, y2);
			
				style.DrawLine (cr, color, x1, y1 - 8, x1, y1 + 8);
				style.DrawLine (cr, color, x2, y2 - 8, x2, y2 + 8);

				prev_gap_x = gap_point.X;
			}

			if (p != null) {
				// draw the line going from the gap to the next real
				// point
				gap_point = (Point)gap_points[gap_points.Count - 1];

				x = GridXToRender (gap_point.X) + style.PointHalfSize + GAP_PAD;
				y = GridYToRender (Convert.ToInt64 (((RenderXToGrid (x) - (double)p.X) * m) + p.Y));

				style.DrawLine (cr, color, x, y - 8, x, y + 8);

				// draw a dotted line if the gap is the first point in
				// the graph
				if (prev != null) {
					style.DrawLine (cr, color, x, y,
							GridXToRender (p.X),
							GridYToRender (p.Y));
				} else {
					style.DrawDashedLine (cr, color, x, y,
					                      GridXToRender (p.X),
					                      GridYToRender (p.Y));
				}
			}
		}

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
				// fire SelectionChanged if the selection has
				// actually changed
				if (selected_path == null
				    || path.Compare (selected_path) != 0) {
					selected_path = path.Copy ();
					if (SelectionChanged != null) {
						SelectionChanged (this, new EventArgs ());
					}
				}
			} else {
				// deselect if we previously had a selection
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

			if (!draw_points) {
				return false;
			}

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
				int plot_x, plot_y;
				if (!GetPlotPoint (iter, out plot_x, out plot_y)) {
					// value isn't currently being shown
					continue;
				}

				// x and y come back with reference to the
				// allocation, so transform our plot point to
				// that origin
				plot_x -= Allocation.X;
				plot_y -= Allocation.Y;

				int half_size = style.PointHalfSize;
				if ((x <= plot_x + half_size && x >= plot_x - half_size)
				    && (y <= plot_y + half_size && y >= plot_y - half_size))
				{
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
				if (!GetPlotPoint (iter, out x, out y)) {
					// value isn't being shown
					continue;
				}

				// if its out of bounds, throw it out
				if ((x < Allocation.X || x > Allocation.X + Allocation.Width)
				    || (y < Allocation.Y || y > Allocation.Y + Allocation.Height)) {
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

		/*
		 * Returns true if the iter provided is within the visible
		 * rectangle, false otherwise.  If it returns false, the values
		 * of x and y will be -1.
		 */ 
		private bool GetPlotPoint (TreeIter iter, out int x, out int y)
		{
			x = y = -1;

			IComparable x_val, y_val;
			if (!GetValue (iter, 0, out x_val)
			    || !GetValue (iter, 1, out y_val)) {
				return false;
			}

			if ((x_axis == null || !x_axis.HasValidRange)
			    || (y_axis == null || !y_axis.HasValidRange)) {
				return false;
			}

			if (x_axis.IsInRange (x_val)
			    && y_axis.IsInRange (y_val)) {
				x = GridXToRender (x_axis.ValueToGridCoords (x_val));
				y = GridYToRender (y_axis.ValueToGridCoords (y_val));
				return true;
			}

			return false;
		}

		private void FindPointsInRange (IAxis x_axis, IAxis y_axis, out Point[] points)
		{
			points = new Point[0];

			if ((x_axis == null || !x_axis.HasValidRange)
			    || (y_axis == null || !y_axis.HasValidRange)) {
				// we need both axes to be valid to draw anything
				return;
			}

			ArrayList points_list = new ArrayList ();

			TreeIter iter;
			if (!model.GetIterFirst (out iter)) {
				return;
			}

			Point prev_point = null;
			do {
				IComparable x_val, y_val;
				if (!GetValue (iter, 0, out x_val)
				    || !GetValue (iter, 1, out y_val)) {
					continue;
				}

				// don't clip a gapped point
				if (PointGapFunc != null
				    && PointGapFunc (model, iter)) {
					if (x_axis.IsInRange (x_val)) {
						Point p = new Point (x_axis.ValueToGridCoords (x_val),
								     y_axis.ValueToGridCoords (y_val),
								     x_val, y_val);
						p.IsAGap = true;
						points_list.Add (p);
					}
					continue;
				}

				// create a line segment between the previous
				// point and the current point and see if the
				// segment intersects the viewing plane
				Point b = new Point (x_axis.ValueToGridCoords (x_val),
						     y_axis.ValueToGridCoords (y_val),
						     x_val, y_val);
				Point a = new Point (b.X, b.Y, b.XValue, b.YValue);

				if (prev_point != null) {
					a = prev_point;
				}

				// copy b before it becomes clipped
				prev_point = b.Clone () as Point;

				TreePath path = model.GetPath (iter);
				if (selected_path != null
				    && selected_path.Compare (path) == 0) {
					b.IsSelected = true;
				}

				if (focused_path != null
				    && focused_path.Compare (path) == 0) {
					b.IsFocused = true;
				}

				if (ClipSegment (x_axis, y_axis, ref a, ref b)) {
					if (a != b && a.IsClipped) {
						points_list.Add (a);
					}

					points_list.Add (b);
				}
			} while (model.IterNext (ref iter));

			// Sort this list on the x axis
			points_list.Sort (new PointXAxisIComparer ());

			points = (Point[])points_list.ToArray (typeof (Point));
		}

	}
}
