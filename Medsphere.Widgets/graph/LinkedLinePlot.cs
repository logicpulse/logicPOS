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

/*
 * LinkedLinePlot: A plot which links the selection and focus of n LinePlots.
 * This is pretty useful when you want to graph blood pressure values, but
 * pretty useless otherwise.

 * XXX: This code is giant hack which relies on the fact that models
 * are sorted in the x direction to avoid highly costly comparisons.  Your
 * mileage may vary.
 */

using Gtk;
using System;
using System.Collections;

namespace Medsphere.Widgets
{
	public class LinkedLinePlot : IPlot
	{
		/* public events */
		public event EventHandler Changed;
		public event EventHandler ModelChanged;
		public event EventHandler SelectionChanged;

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

		public IStyleProvider Style {
			set {
				style = value;
				foreach (LinePlot plot in plots)
				{
					plot.Style = style;	
				}
			}
		}

		public bool ShowInLegend {
			get { return show_in_legend; }
			set { show_in_legend = value; }
		}

		public bool HasSelection {
			get { return has_selection; }
		}

		public bool CanFocus {
			get { return true; }
		}

		public bool HasFocus {
			get { return has_focus; }
		}

		public LinePlot[] Plots {
			get { return plots; }
		}

		/* public methods */
		public LinkedLinePlot (params LinePlot[] plots)
		{
			this.plots = plots;

			foreach (LinePlot plot in plots)
			{
				plot.ModelChanged += new EventHandler (OnModelChanged);
				plot.SelectionChanged += new EventHandler (OnPlotSelectionChanged);
			}
		}

		public void SizeAllocate (Gdk.Rectangle allocation)
		{
			Allocation = allocation;
		}

		public void SetAxes (IAxis[] axes)
		{
			this.axes = axes;

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

			if (x_axis == null || y_axis == null) {
				return;
			}

			int selection_x = -1;
			if (has_selection && selected_plots.Count > 0) {
				// get the x of the first focused point, and
				// then draw a line from top to bottom
				// connecting all focused points
				LinePlot first_plot = (LinePlot)selected_plots[0];

				TreeIter iter;
				TreeModel model;
				first_plot.GetSelected (out model, out iter);
				
				int x, y;
				GetPlotPoint (first_plot, iter, out x, out y);
				
				style.DrawLinkedSelectionLine (cr, x, Allocation.Y,
				                               x, Allocation.Y + Allocation.Height);
				selection_x = x;
			}

			if (has_focus && focused_plots.Count > 0) {
				// get the x of the first focused point, and
				// then draw a line from top to bottom
				// connecting all focused points
				LinePlot first_plot = (LinePlot)focused_plots[0];
				
				TreeIter iter;
				TreeModel model;
				first_plot.GetFocused (out model, out iter);

				int x, y;
				GetPlotPoint (first_plot, iter, out x, out y);

				if (x != selection_x) {
					style.DrawLinkedFocusLine (cr, x, Allocation.Y,
								   x, Allocation.Y + Allocation.Height);
				}
			}

			foreach (LinePlot plot in plots)
			{
				plot.SizeAllocate (Allocation);
				plot.SetAxes (axes);
				plot.Draw (cr);
			}
		}

		public void GetRange (int dimension, out IComparable min, out IComparable max)
		{
			min = max = null;
			foreach (LinePlot plot in plots)
			{
				IComparable plot_min, plot_max;
				plot.GetRange (dimension, out plot_min, out plot_max);
				
				if (min == null || plot_min.CompareTo (min) == -1) {
					min = plot_min;
				}

				if (max == null || plot_max.CompareTo (max) == 1) {
					max = plot_max;
				}
			}
		}

		public bool SelectPoint (int x, int y)
		{
			freeze_selection = true;

			foreach (LinePlot plot in plots)
			{
				plot.UnselectAll ();
			}

			LinePlot selected_plot = null;
			foreach (LinePlot plot in plots)
			{
				if (!plot.SelectPoint (x, y)) {
					continue;
				}

				selected_plot = plot;
				break;
			}

			bool ret = SelectPointsNear (selected_plot, x, y, true);

			freeze_selection = false;

			return ret;
		}

		public void SelectIter (TreeIter iter)
		{
			freeze_selection = true;

			foreach (LinePlot plot in plots)
			{
				plot.SelectIter (iter);
				if (plot.HasSelection) {
					int x, y;
					GetPlotPoint (plot, iter,
					              out x, out y);

					SelectPoint (x, y);

					if (SelectionChanged != null) {
						SelectionChanged (this, new EventArgs ());
					}

					break;
				}
			}

			freeze_selection = false;
		}

		public bool UnselectPoint (int x, int y)
		{
			freeze_selection = true;

			bool selected = false;
			foreach (LinePlot plot in plots)
			{
				if (plot.HasSelection
				    && plot.UnselectPoint (x, y)) {
					selected = true;
					break;
				}
			}

			freeze_selection = false;

			if (!selected) {
				return false;
			}
			
			if (SelectionChanged != null) {
				SelectionChanged (this, new EventArgs ());
			}

			return true;
		}

		public void UnselectAll ()
		{
			freeze_selection = true;

			foreach (LinePlot plot in selected_plots)
			{
				plot.UnselectAll ();
			}

			freeze_selection = false;
			
			selected_plots.Clear ();

			has_selection = false;
			if (SelectionChanged != null) {
				SelectionChanged (this, new EventArgs ());
			}
		}

		public void GetGrabFocusRequest (out int x, out int y)
		{
			x = Int32.MaxValue;
			y = Int32.MaxValue;

			foreach (LinePlot plot in plots)
			{
				int p_x, p_y;
				plot.GetGrabFocusRequest (out p_x, out p_y);
				
				x = p_x < x ? p_x : x;
				y = p_y < y ? p_y : y;
			}
		}

		public void GrabFocus ()
		{
			FocusFirst ();
		}

		public void ReleaseFocus ()
		{
			has_focus = false;
			
			foreach (LinePlot plot in plots)
			{
				plot.ReleaseFocus ();
			}

			if (Changed != null) {
				Changed (this, new EventArgs ());
			}
		}

		// XXX: this code assumes that the model is sorted
		public void FocusNext ()
		{
			if (!has_focus || focused_plots.Count == 0) {
				return;
			}
			
			// get the focused value for the first plot
			LinePlot focus_plot = (LinePlot)focused_plots[0];

			TreeModel model;
			TreeIter focus_iter;
			if (!focus_plot.GetFocused (out model, out focus_iter)) {
				return;
			}

			if (x_axis == null || y_axis == null) {
				return;
			}

			int focus_x, focus_y;
			GetPlotPoint (focus_plot, focus_iter,
			              out focus_x, out focus_y);

			// scour the rest of the plots, searching for values
			// with a small delta to min_x
			int min_delta = Int32.MaxValue;
			ArrayList min_delta_plots = new ArrayList ();
			ArrayList min_delta_iters = new ArrayList ();
			foreach (LinePlot plot in plots)
			{
				TreeIter iter;
				if (!plot.Model.GetIterFirst (out iter)) {
					break;
				}

				TreeModel plot_model;
				TreePath plot_focus_path = null;
				TreeIter plot_focus_iter = TreeIter.Zero;
				if (plot.HasFocus
				    && plot.GetFocused (out plot_model, out plot_focus_iter)) {
					plot_focus_path = plot.Model.GetPath (plot_focus_iter);
				}

				do {
					TreePath path = plot.Model.GetPath (iter);

					int x, y;
					if (!GetPlotPoint (plot, iter, out x, out y)) {
						continue;
					}

					// ensure that we always pick points
					// after the previously focused point,
					// if this plot had focus
					if (plot_focus_path != null
					    && path.Compare (plot_focus_path) != 1) {
						continue;
					}

					// continue walking until p_x >= x
					if (x < focus_x) {
						continue;
					}

					// now, we're at the point right of the
					// last focused point, so find the
					// minimal x delta
					int delta = x - focus_x;
					if (delta > min_delta) {
						continue;
					}

					if (delta < min_delta) {
						min_delta_plots.Clear ();
						min_delta_iters.Clear ();
					}

					min_delta = delta;
					min_delta_plots.Add (plot);
					min_delta_iters.Add (iter);
					break;
				} while (plot.Model.IterNext (ref iter));
			}

			// we must be at the end
			if (min_delta_plots.Count == 0) {
				FocusFirst ();
				return;
			}

			FocusIters (min_delta_plots, min_delta_iters);
		}

		public void FocusPrev ()
		{
			if (!has_focus || focused_plots.Count == 0) {
				return;
			}
			
			// get the focused value for the first plot
			LinePlot focus_plot = (LinePlot)focused_plots[0];

			TreeModel model;
			TreeIter focus_iter;
			if (!focus_plot.GetFocused (out model, out focus_iter)) {
				return;
			}

			if (x_axis == null || y_axis == null) {
				return;
			}

			int focus_x, focus_y;
			GetPlotPoint (focus_plot, focus_iter,
			              out focus_x, out focus_y);

			// scour the rest of the plots, searching for values
			// with a small (negative) delta to min_x
			int min_delta = Int32.MaxValue;
			ArrayList min_delta_plots = new ArrayList ();
			ArrayList min_delta_iters = new ArrayList ();
			foreach (LinePlot plot in plots)
			{
				TreeIter iter;
				if (!plot.Model.GetIterFirst (out iter)) {
					break;
				}

				TreeModel plot_model;
				TreePath plot_focus_path = null;
				TreeIter plot_focus_iter = TreeIter.Zero;
				if (plot.HasFocus
				    && plot.GetFocused (out plot_model, out plot_focus_iter)) {
					plot_focus_path = model.GetPath (plot_focus_iter);
				}

				TreeIter prev_iter = TreeIter.Zero;

				do {
					TreePath path = plot.Model.GetPath (iter);

					int x, y;
					if (!GetPlotPoint (plot, iter, out x, out y)) {
						continue;
					}

					// stop when we hit the currently
					// focused point, if we had focus
					if (plot_focus_path != null) {
						if (path.Compare (plot_focus_path) == 0) {
							break;
						}
					} else if (x >= focus_x) {
						// or stop when we've hit
						// focus_x, or the nearest
						// point to it
						break;
					}

					prev_iter = iter;
				} while (plot.Model.IterNext (ref iter));

				// point must have been the first one
				if (prev_iter.Equals (TreeIter.Zero)) {
					continue;
				}

				// now, we're at the point right before the
				// last focused point, so find the minimal x
				// delta
				int prev_x, prev_y;
				GetPlotPoint (plot, prev_iter,
				              out prev_x, out prev_y);

				int delta = focus_x - prev_x;
				if (delta > min_delta) {
					continue;
				}

				if (delta < min_delta) {
					min_delta_plots.Clear ();
					min_delta_iters.Clear ();
				}

				min_delta = delta;
				min_delta_plots.Add (plot);
				min_delta_iters.Add (prev_iter);
			}
			
			if (min_delta_plots.Count == 0) {
				FocusLast ();
				return;
			}

			FocusIters (min_delta_plots, min_delta_iters);
		}

		public void FocusNearest (int x, int y)
		{
			// find the point with the min delta to x
			int min_delta = Int32.MaxValue;
			ArrayList min_delta_plots = new ArrayList ();
			ArrayList min_delta_iters = new ArrayList ();
			foreach (LinePlot plot in plots)
			{
				TreeIter iter;
				if (!plot.Model.GetIterFirst (out iter)) {
					break;
				}

				do {
					int plot_x, plot_y;
					if (!GetPlotPoint (plot, iter,
					                   out plot_x, out plot_y)) {
						// reject if it's not visible
						continue;
					}

					int delta = Math.Abs (plot_x - x);
					if (delta > min_delta) {
						continue;
					}

					if (delta < min_delta) {
						min_delta_plots.Clear ();
						min_delta_iters.Clear ();
					}

					min_delta = delta;
					min_delta_plots.Add (plot);
					min_delta_iters.Add (iter);
				} while (plot.Model.IterNext (ref iter));
			}

			FocusIters (min_delta_plots, min_delta_iters);
		}

		public bool GetFocusedPoint (out int x, out int y)
		{
			x = y = -1;
			if (!has_focus) {
				return false;
			}

			if (focused_plots.Count < 1) {
				return false;
			}

			// arbitrarily get the first focus point
			return ((LinePlot)focused_plots[0]).GetFocusedPoint (out x, out y);
		}

		public void SelectFocusedPoint ()
		{
			if (!has_focus) {
				return;
			}

			freeze_selection = true;

			has_selection = true;

			foreach (LinePlot plot in plots)
			{
				if (plot.HasFocus) {
					plot.SelectFocusedPoint ();
				}
			}

			// copy the focused_plots into selected_plots
			selected_plots = focused_plots.Clone () as ArrayList;

			freeze_selection = false;
			if (SelectionChanged != null) {
				SelectionChanged (this, new EventArgs ());
			}
		}

		public Requisition LegendSizeRequest (Orientation o, int spacing)
		{
			Requisition req = Requisition.Zero;

			if (o == Orientation.Horizontal) {
				foreach (LinePlot plot in plots)
				{
					Requisition plot_req = plot.LegendSizeRequest (o, spacing);

					req.Width += plot_req.Width;
					req.Height = Math.Max (req.Height,
					                       plot_req.Height);
				}

				req.Width += (spacing * (plots.Length - 1));
			} else {
				foreach (LinePlot plot in plots)
				{
					Requisition plot_req = plot.LegendSizeRequest (o, spacing);

					req.Width = Math.Max (req.Width,
					                      plot_req.Width);
					req.Height += plot_req.Height;
				}

				req.Height += (spacing * (plots.Length - 1));
			}

			return req;
		}

		public void DrawLegend (Cairo.Context cr, Gdk.Rectangle alloc,
		                        Orientation o, int spacing)
		{
			int x = alloc.X;
			int y = alloc.Y;

			foreach (LinePlot plot in plots)
			{
				Requisition req = plot.LegendSizeRequest (o,
				                                          spacing);

				Gdk.Rectangle a = new Gdk.Rectangle ();
				a.X = x;
				a.Y = y;
				a.Width = req.Width;
				a.Height = req.Height;

				plot.DrawLegend (cr, a, o, spacing);

				if (o == Orientation.Horizontal) {
					x += a.Width + spacing;
				} else {
					y += a.Height + spacing;
				}
			}
		}

		public bool GetTooltipForPoint (int x, int y, out Widget widget)
		{
			widget = null;

			foreach (LinePlot plot in plots)
			{
				if (plot.GetTooltipForPoint (x, y, out widget)) {
					return true;
				}
			}

			return false;
		}

		/* private fields */
		private Gdk.Rectangle Allocation = Gdk.Rectangle.Zero;
		private IStyleProvider style;

		private IAxis[] axes = new IAxis[0];
		private IAxis x_axis = null, y_axis = null;

		private string name;

		private LinePlot[] plots = new LinePlot[0];

		private bool has_focus = false;
		private bool has_selection = false;

		private ArrayList focused_plots = new ArrayList ();
		private ArrayList selected_plots = new ArrayList ();

		private const long GRID_MIN = 0;
		private const long GRID_MAX = 10000;

		private bool show_in_legend = true;

		private bool freeze_selection = false;
		
		/* private methods */
		private void FocusFirst ()
		{
			if (x_axis == null || y_axis == null) {
				return;
			}

			// find the closest value to the left (i.e.: smallest x
			// value)
			// if there are two or more equal x values, select all

			int min_x = Int32.MaxValue;
			ArrayList min_plots = new ArrayList ();
			ArrayList min_iters = new ArrayList ();
			foreach (LinePlot plot in plots)
			{
				TreeIter iter;
				if (!plot.Model.GetIterFirst (out iter)) {
					continue;
				}

				int x, y;
				if (!GetPlotPoint (plot, iter, out x, out y)) {
					// reject if it's not visible
					continue;
				}

				// trivially reject if it's bigger than min
				if (x > min_x) {
					continue;
				}
				
				if (x < min_x) {
					min_plots.Clear ();
					min_iters.Clear ();
				}

				min_x = x;
				min_plots.Add (plot);
				min_iters.Add (iter);
			}

			FocusIters (min_plots, min_iters);
		}

		private void FocusLast ()
		{
			if (x_axis == null || y_axis == null) {
				return;
			}

			// find the closest value to the right (i.e.: largest x
			// value)
			// if there are two or more equal x values, select all

			int max_x = Int32.MinValue;
			ArrayList max_plots = new ArrayList ();
			ArrayList max_iters = new ArrayList ();
			foreach (LinePlot plot in plots)
			{
				TreePath last_path = new TreePath (new int[] {
					plot.Model.IterNChildren () - 1
				});

				TreeIter iter;
				if (!plot.Model.GetIter (out iter, last_path)) {
					continue;
				}

				int x, y;
				if(!GetPlotPoint (plot, iter, out x, out y)) {
					// reject if it's not visible
					continue;
				}

				// trivially reject if it's less than max
				if (x < max_x) {
					continue;
				}
				
				if (x > max_x) {
					max_plots.Clear ();
					max_iters.Clear ();
				}

				max_x = x;
				max_plots.Add (plot);
				max_iters.Add (iter);
			}

			FocusIters (max_plots, max_iters);
		}

		private void FocusIters (ArrayList focus_plots, ArrayList focus_iters)
		{
			if (focus_plots.Count == 0
			    || focus_plots.Count != focus_iters.Count) {
				return;
			}

			// release focus on the plots
			foreach (LinePlot plot in plots)
			{
				plot.ReleaseFocus ();
			}

			focused_plots.Clear ();
			for (int i = 0; i < focus_plots.Count; i++)
			{
				LinePlot p = (LinePlot)focus_plots[i];
				p.FocusIter ((TreeIter)focus_iters[i]);
				focused_plots.Add (p);
			}

			has_focus = true;
			if (Changed != null) {
				Changed (this, new EventArgs ());
			}
		}

		private bool SelectPointsNear (LinePlot selected_plot, int x, int y, bool fuzzy_x)
		{
			freeze_selection = true;

			if (selected_plot == null) {
				selected_plots.Clear ();

				freeze_selection = false;
				if (SelectionChanged != null) {
					SelectionChanged (this, new EventArgs ());
				}

				return false;
			}

			selected_plots.Clear ();
			selected_plots.Add (selected_plot);
			
			// select the rest of the points along the x axis
			foreach (LinePlot plot in plots)
			{
				if (plot == selected_plot) {
					continue;
				}

				TreeIter iter;
				if (!plot.Model.GetIterFirst (out iter)) {
					continue;
				}

				// select the closest point to y that has the
				// same x value
				int min_y_delta = Int32.MaxValue;
				TreeIter min_y_iter = TreeIter.Zero;

				do {
					int plot_x, plot_y;
					if (!GetPlotPoint (plot, iter,
					                   out plot_x, out plot_y)) {
						// reject if it's not visible
						continue;
					}
					
					// x and y come back with reference to
					// the allocation, so transform our
					// plot point to that origin
					plot_x -= Allocation.X;
					plot_y -= Allocation.Y;

					if (fuzzy_x) {
						int half_size = style.PointHalfSize;
						if (x > plot_x + half_size
						    || x < plot_x - half_size) {
							continue;
						}
					} else {
						if (x != plot_x) {
							continue;
						}
					}

					int delta = Math.Abs (plot_y - y);
					if (delta < min_y_delta) {
						min_y_delta = delta;
						min_y_iter = iter;
					}
				} while (plot.Model.IterNext (ref iter));

				if (min_y_delta == Int32.MaxValue) {
					continue;
				}

				plot.SelectIter (min_y_iter);
				plot.FocusIter (min_y_iter);
				selected_plots.Add (plot);
			}

			has_selection = true;
			has_focus = true;

			if (Changed != null) {
				Changed (this, new EventArgs ());
			}

			freeze_selection = false;
			if (SelectionChanged != null) {
				SelectionChanged (this, new EventArgs ());
			}

			return true;
		}

		/*
		 * Returns true if the iter provided is within the visible
		 * rectangle, false otherwise.  If it returns false, the values
		 * of x and y will be -1.
		 */ 
		private bool GetPlotPoint (LinePlot plot, TreeIter iter, out int x, out int y)
		{
			x = y = -1;

			IComparable x_val, y_val;
			if (!plot.GetValue (iter, 0, out x_val)
			    || !plot.GetValue (iter, 1, out y_val)) {
				return false;
			}

			if (x_axis == null || y_axis == null) {
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

		private int GridXToRender (long x)
		{
			return Convert.ToInt32 ((((double)x / GRID_MAX) * Allocation.Width) + Allocation.X);
		}

		private int GridYToRender (long y)
		{
			return Allocation.Y + Allocation.Height
			       - Convert.ToInt32 (((double)y / GRID_MAX) * Allocation.Height);
		}

		private void OnModelChanged (object o, EventArgs args)
		{
			// proxy the event up
			if (ModelChanged != null) {
				ModelChanged (this, new EventArgs ());
			}
		}

		private void OnPlotSelectionChanged (object o, EventArgs args)
		{
			if (freeze_selection) {
				return;
			}

			LinePlot p = o as LinePlot;
			if (p == null) {
				return;
			}

			TreeIter iter;
			TreeModel model;
			if (!p.GetSelected (out model, out iter)) {
				UnselectAll ();
				return;
			}

			int x, y;
			GetPlotPoint (p, iter, out x, out y);

			x -= Allocation.X;
			y -= Allocation.Y;

			SelectPointsNear (p, x, y, false);
		}
	}
}
