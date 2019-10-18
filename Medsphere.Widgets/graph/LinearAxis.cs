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
	public delegate double LinearAxisTickTransformFunc (double input);

	public class LinearAxis : IAxis
	{
#region public events
		public event EventHandler Changed;
		public event SizeRequestedHandler SizeRequested;
#endregion

#region public properties
		public int Dimension {
			get { return dimension; }
			set {
				dimension = value;
				
				if (Changed != null) {
					Changed (this, new EventArgs ());
				}
			}
		}

		public AxisLocation Location {
			get { return location; }
			set {
				if (dimension == 0
				    && (value == AxisLocation.Left
				        || value == AxisLocation.Right)) {
					throw new AxisLocationException ();
				} else if (dimension == 1
				           && (value == AxisLocation.Top
				               || value == AxisLocation.Bottom)) {
					throw new AxisLocationException ();
				}

				location = value;

				if (Changed != null) {
					Changed (this, new EventArgs ());
				}
			}
		}

		public bool Visible {
			get { return visible; }
			set {
				visible = value;
				
				if (Changed != null) {
					Changed (this, new EventArgs ());
				}
			}
		}

		public int Padding {
			get { return padding; }
			set {
				padding = value;
								
				if (Changed != null) {
					Changed (this, new EventArgs ());
				}
			}
		}

		public bool ShowTicks {
			get { return show_ticks; }
			set {
				show_ticks = value;
								
				if (Changed != null) {
					Changed (this, new EventArgs ());
				}
			}
		}

		public bool ShowTickLabels {
			get { return show_tick_labels; }
			set {
				show_tick_labels = value;
				
				if (Changed != null) {
					Changed (this, new EventArgs ());
				}
			}
		}

		public bool ShowGridLines {
			get { return show_grid_lines; }
			set {
				show_grid_lines = value;

				if (Changed != null) {
					Changed (this, new EventArgs ());
				}
			}
		}

		public IComparable CalculatedMinValue {
			get { return real_min; }
		}

		public IComparable CalculatedMaxValue {
			get { return real_max; }
		}

		public IComparable MinValue {
			get { return min; }
		}

		public IComparable MaxValue {
			get { return max; }
		}

		public bool HasValidRange {
			get { return (min < max); }
		}

		public IStyleProvider Style {
			set { style = value; }
		}
#endregion

#region public fields
		/**
		 * Transforms the ticks from the plot's data space into another
		 * number space.
		 *
		 * NOTE: Any transform performed on the ticks will
		 * correspondingly change the Plot's range, so it is
		 * recommended to not use transformed axes as the default axis
		 * for plots.
		 *
		 * A good example of where this could be used would be a graph
		 * with centimeter data values that should have both an inch
		 * and centimeter axis.  The inch axis would use a centimeter
		 * to inch conversion in its #TickTransformFunc, and would show
		 * values in inches on the graph.
		 */
		public LinearAxisTickTransformFunc TickTransformFunc;
#endregion

#region public methods
		public LinearAxis (int dimension, AxisLocation location)
		{
			Dimension = dimension;
			Location = location;
		}

		public virtual Requisition SizeRequest (Gdk.Rectangle plot_size)
		{
			Requisition req = Requisition.Zero;
			if (!show_ticks) {
				return req;
			}

			// no data in plots
			if (min == Double.PositiveInfinity
			    && max == Double.NegativeInfinity) {
				return req;
			}

			if (!show_tick_labels) {
				if (location == AxisLocation.Top
				    || location == AxisLocation.Bottom) {
					req.Height = style.TickLineSize + padding;
					req.Width = plot_size.Width;
				} else if (location == AxisLocation.Left
				           || location == AxisLocation.Right) {
					req.Height = plot_size.Height;
					req.Width = style.TickLineSize + padding;
				}
				return req;
			}

			double interval, start;
			GetTickStartAndInterval (plot_size, out start, out interval);

			sig_figs = Math.Max (GetSignificantFigures (start),
			                     GetSignificantFigures (interval)) + 2;
	
			for (double tick = start; tick < max; tick += interval)
			{
				int label_width, label_height;
				style.GetAxisLabelMetrics (GetTickLabelName (tick),
				                           out label_width, out label_height);

				if (location == AxisLocation.Top
				    || location == AxisLocation.Bottom) {
					req.Width = plot_size.Width;
					
					// pick the tallest label
					req.Height = Math.Max (req.Height,
							       label_height + (padding * 2)
					                       + style.TickLineSize);
				} else if (location == AxisLocation.Left
				           || location == AxisLocation.Right) {
					// pick the widest label
					req.Width = Math.Max (req.Width,
							      label_width + (padding * 2)
					                      + style.TickLineSize);
					req.Height = plot_size.Height;
				}
			}

			return req;
		}

		public virtual void DrawTicks (Gdk.Rectangle allocation, Cairo.Context cr)
		{
			if (!show_ticks) {
				return;
			}
			
			// no data in plots
			if (min == Double.PositiveInfinity
			    && max == Double.NegativeInfinity) {
				return;
			}

			// recalculate the tick interval, since our available
			// width probably has changed since SizeRequest
			double interval, start;
			GetTickStartAndInterval (allocation, out start, out interval);

			sig_figs = Math.Max (GetSignificantFigures (start),
			                     GetSignificantFigures (interval)) + 2;

			for (double tick = start; tick < max; tick += interval)
			{
				int x = 0, y = 0;
				if (location == AxisLocation.Top) {
					x = Convert.ToInt32 ((((double)ValueToGridCoords (tick) / GRID_MAX)
					                     * allocation.Width) + allocation.X);
					y = allocation.Y + allocation.Height;

					style.DrawAxisTick (cr, x, y, location);
				} else if (location == AxisLocation.Bottom) {
					x = Convert.ToInt32 ((((double)ValueToGridCoords (tick) / GRID_MAX)
					                     * allocation.Width) + allocation.X);
					y = allocation.Y;

					style.DrawAxisTick (cr, x, y, location);
				} else if (location == AxisLocation.Left) {
					x = allocation.X + allocation.Width - style.TickLineSize;
					y = allocation.Y + allocation.Height
					    - Convert.ToInt32 (((double)ValueToGridCoords (tick) / GRID_MAX)
					                       * allocation.Height);

					style.DrawAxisTick (cr, x, y, AxisLocation.Left);
				} else if (location == AxisLocation.Right) {
					x = allocation.X;
					y = allocation.Y + allocation.Height
					    - Convert.ToInt32 (((double)ValueToGridCoords (tick) / GRID_MAX)
					                       * allocation.Height);

					style.DrawAxisTick (cr, x, y, AxisLocation.Left);
				}

				if (!show_tick_labels) {
					continue;
				}

				int width, height;
				style.GetAxisLabelMetrics (GetTickLabelName (tick),
							   out width,
							   out height);

				int label_x = 0, label_y = 0;
				if (location == AxisLocation.Top) {
					label_x = x - (width / 2);
					label_y = y - style.TickLineSize - height;
				} else if (location == AxisLocation.Bottom) {
					label_x = x - (width / 2);
					label_y = y + style.TickLineSize + padding;
				} else if (location == AxisLocation.Left) {
					label_x = x - padding - width;
					label_y = y - (height / 2);
				} else if (location == AxisLocation.Right) {
					label_x = x + style.TickLineSize + padding;
					label_y = y - (height / 2);
				}

				// Don't draw an axis label if it won't
				// completely fit in the allocation
				if (allocation.Contains (label_x, label_y)
				    && allocation.Contains (label_x + width, label_y + height)) {
					style.DrawAxisLabel (cr, GetTickLabelName (tick),
							     label_x, label_y);
				}
			}
		}

		public virtual void DrawGridLines (Gdk.Rectangle plot_alloc, Cairo.Context cr)
		{
			if (!show_grid_lines) {
				return;
			}

			if (min == Double.PositiveInfinity
			    && max == Double.NegativeInfinity) {
				DrawFakeGridLines (plot_alloc, cr);
				return;
			}
			
			double interval, start;
			GetTickStartAndInterval (plot_alloc, out start, out interval);

			for (double tick = start; tick < max; tick += interval)
			{
				int x1, y1, x2, y2;
				if (dimension == 0) {
					x1 = Convert.ToInt32 ((((double)ValueToGridCoords (tick) / GRID_MAX)
					                      * plot_alloc.Width) + plot_alloc.X);
					y1 = plot_alloc.Y;

					x2 = x1;
					y2 = y1 + plot_alloc.Height;
				} else {
					x1 = plot_alloc.X;
					y1 = plot_alloc.Y + plot_alloc.Height
					     - Convert.ToInt32 (((double)ValueToGridCoords (tick) / GRID_MAX)
					                        * plot_alloc.Height);

					x2 = x1 + plot_alloc.Width;
					y2 = y1;
				}

				style.DrawGridLine (cr, x1, y1, x2, y2);
			}
		}

		public virtual void SetRange (IPlot[] plots, Gdk.Rectangle allocation_hint)
		{
			if (plots.Length == 0) {
				return;
			}

			foreach (IPlot plot in plots)
			{
				if (!plot.FactorIntoAxisCalculation) {
					continue;
				}

				IComparable comp_min, comp_max;
				plot.GetRange (Dimension, out comp_min, out comp_max);

				if (comp_max == null || comp_max == null) {
					continue;
				}

				if (!(comp_min is double)
				    || !(comp_max is double)) {
					throw new ArgumentException ("Plot must return double values for a LinearAxis.");
				}

				double d_min = (double)comp_min;
				double d_max = (double)comp_max;

				real_min = (d_min < real_min) ? d_min : real_min;
				real_max = (d_max > real_max) ? d_max : real_max;
			}

			if (is_zoomed) {
				return;
			}

			if (TickTransformFunc != null) {
				min = TickTransformFunc (real_min);
				max = TickTransformFunc (real_max);
			} else {
				min = real_min;
				max = real_max;
			}

			if (min == Double.PositiveInfinity
			    && max == Double.NegativeInfinity) {
				// plots must have returned null for range

				// this means that either there is no data currently
				// or the plot simply doesn't have values for
				// this particular dimension
				return;
			}

			if (max == min) {
				if (min == 0) {
					min = 0;
					max = 1;
				} else {
					min -= min * .1;
					max += max * .1;
				}
			}

			// if min == max, min < 0
			if (min > max) {
				double tmp = min;
				min = max;
				max = tmp;
			}

			// pad the max and min values by a decent amount of
			// characters, so plot labels can always be seen
			int em_width, em_height;
			style.GetAxisLabelMetrics ("M", out em_width, out em_height);

			double pad = 0;
			if (dimension == 0) {
				pad = Math.Round (((double)(em_width + 24) / allocation_hint.Width)
				                  * (max - min), 2);
			} else if (dimension == 1) {
				pad = Math.Round (((double)(em_height + 24) / allocation_hint.Height)
				                  * (max - min), 2);
			}

			min -= pad;
			max += pad;

			normal_min = min;
			normal_max = max;
		}

		public long ValueToGridCoords (IComparable comp)
		{
			if (!(comp is double)) {
				throw new ArgumentException ("ValueToGridCoords must be called with a double for a LinearAxis.");
			}

			double val = (double)comp;
			return Convert.ToInt64 (((val - min) / (max - min)) * GRID_MAX);
		}

		public void GetGridClipRegion (out long min, out long max)
		{
			min = GRID_MIN;
			max = GRID_MAX;
		}

		public void Zoom (long grid_min, long grid_max)
		{
			// SetRange () hasn't been called yet
			if (min > max) {
				return;
			}

			double temp_min = (min + ((double)grid_min / GRID_MAX) * (max - min));
			double temp_max = (min + ((double)grid_max / GRID_MAX) * (max - min));

			if ((temp_max - temp_min) < 0.9E-8) {
				return;
			}

			is_zoomed = true;

			min = temp_min;
			max = temp_max;

			// Fire changed event so that other graphs which use
			// this axis can update
			if (Changed != null) {
				Changed (this, new EventArgs ());
			}
		}

		public void Unzoom ()
		{
			// SetRange () hasn't been called yet
			if (min > max) {
				return;
			}

			min = normal_min;
			max = normal_max;
			is_zoomed = false;

			// Fire changed event so that other graphs which use
			// this axis can update
			if (Changed != null) {
				Changed (this, new EventArgs ());
			}
		}

		public void FireSizeRequested (Requisition new_req)
		{
			if (SizeRequested != null) {
				SizeRequestedArgs args = new SizeRequestedArgs ();
				args.Args = new object[] { new_req };

				SizeRequested (this, args);
			}
		}

		public void RecalculateRange (IPlot[] plots, Gdk.Rectangle allocation_hint)
		{
			is_zoomed = false;
			real_min = Double.PositiveInfinity;
			real_max = Double.NegativeInfinity;

			SetRange (plots, allocation_hint);

			if (Changed != null) {
				Changed (this, new EventArgs ());
			}
		}

		public bool IsInRange (IComparable c)
		{
			/*
			 * This comparison must be inclusive of max and min
			 * (max < min), so the following truth table applies:
			 * 
			 * min.CT (c) | max.CT (c) | result
			 * --------------------------------
			 *     -1     |    -1      |    0
			 *      0     |     0      |    N/A
			 *      1     |     1      |    0
			 *     -1     |     0      |    N/A
			 *      0     |     1      |    N/A
			 *      1     |    -1      |    1
			 *     -1     |     1      |    N/A
			 *      0     |    -1      |    1
			 *      1     |     0      |    1
			 */

			return min.CompareTo (c) != max.CompareTo (c);
		}
#endregion

#region protected fields
		protected int padding = 3;
		protected IStyleProvider style;
		protected AxisLocation location;
		protected bool visible = true;

		protected bool show_ticks = true;
		protected bool show_tick_labels = true;
		protected bool show_grid_lines = true;

		protected int dimension = 0;
		protected double real_min = Double.PositiveInfinity;
		protected double real_max = Double.NegativeInfinity;

		protected bool is_zoomed = false;
		protected int sig_figs = 0;
		protected double min = Double.PositiveInfinity;
		protected double max = Double.NegativeInfinity;

		// real_min/max + padding
		protected double normal_min = Double.PositiveInfinity;
		protected double normal_max = Double.NegativeInfinity;

		protected const long GRID_MIN = 0;
		protected const long GRID_MAX = 10000;

		protected const int FAKE_GRID_INTERVAL = 50;

		protected double[] mantissas = {
			1.0, 2.0, 5.0
		};
#endregion

#region protected methods
		protected double DetermineLargeTickInterval (int length, double min, double range)
		{
			double interval = Math.Abs ((25.0f / length) * range);
			double exponent = Math.Floor (Math.Log10 (interval));
			double mantissa = Math.Pow (10.0, Math.Log10 (interval) - exponent);

			int index = -1;
			for (int i = 0; i < mantissas.Length; i++)
			{
				if (mantissa < mantissas[i]) {
					index = i;
					break;
				}
			}

			if (index == -1) {
				// round up to the next largest exponent
				index = 0;
				exponent += 1.0;
			}

			return Math.Pow (10.0, exponent) * mantissas[index];
		}
		
		protected void GetTickStartAndInterval (Gdk.Rectangle alloc,
		                                        out double start,
		                                        out double interval)
		{
			// XXX: This algorithm depends on the size of the plot
			// area so it add more ticks for wider plots.  Because
			// of this, it needs to know the size of the opposite
			// side (in a 2D plot).  This needs to be rethought for
			// any 3D plotting.
			int side_size = (dimension == 0) ? alloc.Width
			                                 : alloc.Height;

			// recalculate the tick interval, since our available
			// width probably has changed
			interval = DetermineLargeTickInterval (side_size, min,
			                                       (max - min));

			// figure out where to start
			// we want the smallest multiple of interval that is
			// greater than min, and use a decimal so we don't get
			// nasty precision errors
			start = (double)(((long)(min / interval) + 1) * (decimal)interval);
		}

		// if you change this, change it in DateTimeAxis.cs
		protected void DrawFakeGridLines (Gdk.Rectangle plot_alloc, Cairo.Context cr)
		{
			int x1, x2, y1, y2;
			if (dimension == 0) {
				x1 = plot_alloc.X + FAKE_GRID_INTERVAL;
				x2 = x1;

				y1 = plot_alloc.Y;
				y2 = plot_alloc.Y + plot_alloc.Height;

				do {
					style.DrawGridLine (cr, x1, y1, x2, y2);

					x1 += FAKE_GRID_INTERVAL;
					x2 = x1;
				} while (x1 < plot_alloc.X + plot_alloc.Width);
			} else if (dimension == 1) {
				x1 = plot_alloc.X;
				x2 = plot_alloc.X + plot_alloc.Width;

				y1 = plot_alloc.Y + FAKE_GRID_INTERVAL;
				y2 = y1;

				do {
					style.DrawGridLine (cr, x1, y1, x2, y2);

					y1 += FAKE_GRID_INTERVAL;
					y2 = y1;
				} while (y1 < plot_alloc.Y + plot_alloc.Height);
			}
		}

		protected string GetTickLabelName (double val)
		{
			return RoundToSignificantFigure (val, sig_figs + 2).ToString ();
		}

		protected static int GetSignificantFigures (double val)
		{
			// XXX: Ugh, I hate to do this with strings

			// After stripping leading 0s, and dropping the decimal
			// point, everything else will be significant.
			return (val.ToString ()).TrimStart ('0').Replace (".", String.Empty).Length;
		}

		protected static double RoundToSignificantFigure (double val,
		                                                  int sig_figures)
		{
			// Some handy short circuits
			if (sig_figures <= 0 || val == 0) {
				return val;
			}

			bool inverted = false;

			if (val < 0) {
				val *= -1;
				inverted = true;
			}

			// Math.Round doesn't support digits > 15
			if (sig_figures > 15) {
				sig_figures = 15;
			}

			/*
			 * So here's the idea:
			 *
			 * Take the log10 of the number, plus one for
			 * non-negative results, to get the number of digits
			 * in to the left of the decimal:
			 *
			 * log10(12387.273) = 4
			 *
			 * Since 4 >= 0 -> 5
			 *
			 * Next, we divide by 10^5 to move the decimal point
			 * to the front of our number:
			 *
			 * 12387.273 / 10^5 = 12387.273/100000 = .12387273
			 *
			 * We can now do regular rounding with a certain
			 * precision, then multiply by our 10^5 to get back to
			 * the right spot:
			 *
			 * round(.12387273, 3) = .124
			 * .124 * 10^5 = .124 * 100000 = 12400
			 *
			 * This works properly for numbers like 0.0002341 too.
			 * Why it works is left as an exercise for the reader.
			 */
			int log = (int)Math.Log10 (val);
			if (log >= 0) {
				log++;
			}

			double power = (double)Math.Pow (10, log);
			double ret = Math.Round (val/power, sig_figures)*power;
			if (inverted) {
				ret *= -1;
			}

			return ret;
		}
#endregion
	}
}
