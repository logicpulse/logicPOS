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
	public class LabelAxis : IAxis
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

		public int BorderWidth {
			get { return border_width; }
			set {
				border_width = value;
								
				if (Changed != null) {
					Changed (this, new EventArgs ());
				}
			}
		}

		public bool ShowTicks {
			get { return false; }
			set { throw new NotSupportedException (); }
		}

		public bool ShowTickLabels {
			get { return false; }
			set { throw new NotSupportedException (); }
		}

		public bool ShowGridLines {
			get { return false; }
			set { throw new NotSupportedException (); }
		}

		public IComparable CalculatedMinValue {
			get { throw new NotSupportedException (); }
		}

		public IComparable CalculatedMaxValue {
			get { throw new NotSupportedException (); }
		}

		public IComparable MinValue {
			get { throw new NotSupportedException (); }
		}

		public IComparable MaxValue {
			get { throw new NotSupportedException (); }
		}

		public bool HasValidRange {
			get { return false; }
		}

		public IStyleProvider Style {
			set { style = value; }
		}

		public string Title {
			get { return title; }
			set { title = value; }
		}
#endregion

#region public methods
		public LabelAxis (int dimension, AxisLocation location, string title)
		{
			Dimension = dimension;
			Location = location;
			Title = title;
		}

		public Requisition SizeRequest (Gdk.Rectangle plot_size)
		{
			Requisition req = Requisition.Zero;
			if (title == String.Empty) {
				return req;
			}

			int title_width, title_height;
			style.GetAxisTitleMetrics (title, out title_width, out title_height);

			if (location == AxisLocation.Top
			    || location == AxisLocation.Bottom) {
				req.Width = title_width + (border_width * 2);
				req.Height = title_height + (border_width * 2);
			} else if (location == AxisLocation.Left
			           || location == AxisLocation.Right) {
				// we rotate the label to the side when we're showing on
				req.Width = title_height + (border_width * 2);
				req.Height = title_width + (border_width * 2);
			}

			return req;
		}

		public void DrawTicks (Gdk.Rectangle allocation, Cairo.Context cr)
		{
			if (title == String.Empty) {
				return;
			}

			int title_width, title_height;
			style.GetAxisTitleMetrics (title, out title_width, out title_height);

			if (location == AxisLocation.Top
			    || location == AxisLocation.Bottom) {
				style.DrawAxisTitle (cr, title,
						     allocation.X + (allocation.Width / 2) - (title_width / 2),
						     allocation.Y + (allocation.Height / 2) - (title_height / 2),
				                     Orientation.Horizontal);
			} else if (location == AxisLocation.Left
			           || location == AxisLocation.Right) {
				style.DrawAxisTitle (cr, title,
						     allocation.X + (allocation.Width / 2) - (title_height / 2),
						     allocation.Y + (allocation.Height / 2) - (title_width / 2),
				                     Orientation.Vertical);
			}
		}

		public void DrawGridLines (Gdk.Rectangle plot_alloc, Cairo.Context cr)
		{
			// Don't show grid lines
		}

		public virtual void SetRange (IPlot[] plots, Gdk.Rectangle allocation_hint)
		{
			// We don't care about the range
		}

		public long ValueToGridCoords (IComparable comp)
		{
			throw new NotSupportedException ();
		}

		public void GetGridClipRegion (out long min, out long max)
		{
			throw new NotSupportedException ();
		}

		public void Zoom (long grid_min, long grid_max)
		{
			// ignore
		}

		public void Unzoom ()
		{
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
			// ignore
		}

		public bool IsInRange (IComparable c)
		{
			return false;
		}
#endregion

#region private fields
		private int border_width = 8;
		private IStyleProvider style;
		private AxisLocation location;
		private bool visible = true;

		private int dimension = 0;
		private string title = String.Empty;
#endregion
	}
}
