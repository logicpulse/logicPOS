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
	public class DateTimeAxis : IAxis
	{
		/* public events */
		public event EventHandler Changed;
		public event SizeRequestedHandler SizeRequested;

		/* public properties */
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

		/* public methods */
		public DateTimeAxis (int dimension, AxisLocation location)
		{
			Dimension = dimension;
			Location = location;
		}

		public Requisition SizeRequest (Gdk.Rectangle plot_size)
		{
			Requisition req = Requisition.Zero;
			if (!show_ticks) {
				return req;
			}

			// no data in plots
			if (min == DateTime.MaxValue
			    && max == DateTime.MinValue) {
				return req;
			}

			if (!show_tick_labels) {
				if (location == AxisLocation.Top || location == AxisLocation.Bottom) {
					req.Height = style.TickLineSize + padding;
					req.Width = plot_size.Width;
				} else if (location == AxisLocation.Left || location == AxisLocation.Right) {
					req.Height = plot_size.Height;
					req.Width = style.TickLineSize + padding;
				}
				return req;
			}

			DateTime start = real_min;
            TimeSpan interval = new TimeSpan(DateTime.DaysInMonth(start.Year, start.Month), 0, 0, 0);

            //GetTickStartAndInterval (plot_size, out start, out interval);

            for (DateTime tick = start; tick < max; tick += interval)
			{
				int label_width, label_height;
				style.GetAxisLabelMetrics (GetTickLabelName (tick),
				                           out label_width, out label_height);

				if (location == AxisLocation.Top || location == AxisLocation.Bottom)
                {
					req.Width = plot_size.Width;

					// pick the tallest label
					req.Height = Math.Max (req.Height, label_height + (padding * 2) + style.TickLineSize);
				} else if (location == AxisLocation.Left || location == AxisLocation.Right)
                {
					// pick the widest
					req.Width = Math.Max (req.Width, label_width + (padding * 2) + style.TickLineSize);
					req.Height = plot_size.Height;
				}

                interval = new TimeSpan(DateTime.DaysInMonth(tick.Year, tick.Month), 0, 0, 0);

            }

			return req;
		}

		public void DrawTicks (Gdk.Rectangle allocation, Cairo.Context cr)
		{
			if (!show_ticks) {
				return;
			}

			// no data in plots
			if (min == DateTime.MaxValue
			    && max == DateTime.MinValue) {
				return;
			}

            // recalculate the tick interval, since our available
            // width probably has changed since SizeRequest
            DateTime start = real_min;
            TimeSpan interval = new TimeSpan(DateTime.DaysInMonth(start.Year, start.Month), 0, 0, 0);


            //GetTickStartAndInterval (allocation, out start, out interval);

            for (DateTime tick = start; tick < max; tick += interval)
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

					style.DrawAxisTick (cr, x, y, location);
				} else if (location == AxisLocation.Right) {
					x = allocation.X;
					y = allocation.Y + allocation.Height
					    - Convert.ToInt32 (((double)ValueToGridCoords (tick) / GRID_MAX)
					                       * allocation.Height);

					style.DrawAxisTick (cr, x, y, location);
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

                interval = new TimeSpan(DateTime.DaysInMonth(tick.Year, tick.Month), 0, 0, 0);
            }
		}

		public void DrawGridLines (Gdk.Rectangle plot_alloc, Cairo.Context cr)
		{
			if (!show_grid_lines) {
				return;
			}

			if (min == DateTime.MaxValue
			    && max == DateTime.MinValue) {
				DrawFakeGridLines (plot_alloc, cr);
				return;
			}

			DateTime start;
			TimeSpan interval;
			GetTickStartAndInterval (plot_alloc, out start, out interval);

			for (DateTime tick = start; tick < max; tick += interval)
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

		public void SetRange (IPlot[] plots, Gdk.Rectangle allocation_hint)
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

				if (comp_min == null || comp_max == null) {
					continue;
				}

				if (!(comp_min is DateTime)
				    || !(comp_max is DateTime)) {
					throw new ArgumentException ("Plot must return DateTime values for a DateTimeAxis.");
				}

				DateTime d_min = (DateTime)comp_min;
				DateTime d_max = (DateTime)comp_max;

				real_min = (d_min < real_min) ? d_min : real_min;
				real_max = (d_max > real_max) ? d_max : real_max;
			}

			if (is_zoomed) {
				return;
			}

			min = real_min;
			max = real_max;

			if (min == DateTime.MaxValue
			    && max == DateTime.MinValue) {
				// plots must have returned null for range
				return;
			}

			if (max == min) {
				min -= new TimeSpan (1, 0, 0, 0);
				max += new TimeSpan (1, 0, 0, 0);
			}

			// pad the max and min values by a decent amount of
			// characters, so plot labels can always be seen
			int em_width, em_height;
			style.GetAxisLabelMetrics ("M", out em_width, out em_height);

			long pad = 0;
			if (dimension == 0) {
				pad = (long)(((double)(em_width + 24) / allocation_hint.Width)
				             * (max - min).Ticks);
			} else if (dimension == 1) {
				pad = (long)(((double)(em_height + 24) / allocation_hint.Height)
				             * (max - min).Ticks);
			}

			min -= new TimeSpan (pad);
			max += new TimeSpan (pad);

			normal_min = min;
			normal_max = max;
		}

		public long ValueToGridCoords (IComparable comp)
		{
			if (!(comp is DateTime)) {
				throw new ArgumentException ("ValueToGridCoords must be called with a DateTime for a DateTimeAxis.");
			}

			DateTime val = (DateTime)comp;
			return Convert.ToInt64 ((((double)(val - min).Ticks) / (max - min).Ticks) * GRID_MAX);
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

			DateTime temp_min = new DateTime (Convert.ToInt64 (((grid_min / (double)GRID_MAX) * (max - min).Ticks) + min.Ticks));
			DateTime temp_max = new DateTime (Convert.ToInt64 (((grid_max / (double)GRID_MAX) * (max - min).Ticks) + min.Ticks));

			// prevent insane zooming
			if ((temp_max - temp_min).TotalMinutes <= 1) {
				return;
			}

			min = temp_min;
			max = temp_max;
			is_zoomed = true;

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
				SizeRequestedArgs a = new SizeRequestedArgs ();
				a.Requisition = new_req;

				SizeRequested (this, a);
			}
		}

		public void RecalculateRange (IPlot[] plots, Gdk.Rectangle allocation_hint)
		{
			is_zoomed = false;
			real_min = DateTime.MaxValue;
			real_max = DateTime.MinValue;

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

		/* private fields */
		private int padding = 5;
		private IStyleProvider style;
		private AxisLocation location;
		private bool visible = true;

		private bool show_ticks = true;
		private bool show_tick_labels = true;
		private bool show_grid_lines = true;

		private int dimension = 0;
		private DateTime real_min = DateTime.MaxValue;
		private DateTime real_max = DateTime.MinValue;

		private bool is_zoomed = false;
		private DateTime min = DateTime.MaxValue;
		private DateTime max = DateTime.MinValue;

		// real_min/max + padding
		private DateTime normal_min = DateTime.MaxValue;
		private DateTime normal_max = DateTime.MinValue;

		private const long GRID_MIN = 0;
		private const long GRID_MAX = 10000;

		private const int FAKE_GRID_INTERVAL = 50;

		/* private methods */
		private TimeSpan DetermineLargeTickInterval (int length, TimeSpan range)
		{
			TimeSpan interval;

			// hopefully no label should be wider or taller than
			// the max date time value
			int w, h;
			style.GetAxisLabelMetrics (GetTickLabelName (DateTime.MaxValue),
			                           out w, out h);

			w += Padding;
			interval = new TimeSpan ((long)(range.Ticks / ((double)length / w)));

			// drop the seconds out of the interval
			interval = new TimeSpan (interval.Days, interval.Hours,
			                         interval.Minutes + 1, 0);

			return interval;
		}
		
		private void GetTickStartAndInterval (Gdk.Rectangle alloc,
		                                      out DateTime start,
		                                      out TimeSpan interval)
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
			interval = DetermineLargeTickInterval (side_size, (max - min));

			// figure out where to start
			start = min + interval;
		}

		// if you change this, change it in LinearAxis.cs
		private void DrawFakeGridLines (Gdk.Rectangle plot_alloc, Cairo.Context cr)
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

		private string GetTickLabelName (DateTime time)
		{
            return String.Format ("{0}", time.ToString("MM"));
            
        }
	}
}
