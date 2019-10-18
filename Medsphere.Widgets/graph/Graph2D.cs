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
	public class Graph2D : Graph
	{
		/* public properties */
		public bool DrawGridLines {
			get { return draw_grid_lines; }
			set {
				draw_grid_lines = value;
				
				QueueDraw ();
			}
		}

		public bool ShowTooltips {
			get { return show_tooltips; }
			set { show_tooltips = value; }
		}

		/* public methods */
		public Graph2D () : base ()
		{
		}

		public override void Clear ()
		{
			base.Clear ();

			axis_requisitions.Clear ();
		}

		/* protected methods */
		protected override void OnSizeAllocated (Gdk.Rectangle rect)
		{
			base.OnSizeAllocated (rect);
			
			axis_requisitions.Clear ();
			if (plots.Count == 0) {
				return;
			}

			foreach (IAxis axis in axes)
			{
				// Recalculate the range, since we need to
				// recalculate the pad after allocation has
				// changed.
				axis.SetRange (Plots, Allocation);

				// Get the size request for the widget based
				// upon the total space we have.  This is
				// critical in cases where axes might
				// dynamically add ticks based upon the total
				// area showing.  Then we will reduce the plot
				// area to accommodate, then send the axes a new
				// (smaller) allocation.
				if (axis.Visible) {
					axis_requisitions[axis] = axis.SizeRequest (Allocation);
				}
			}

			CalculateLayout ();
		}
		
		protected override void AddAxisListeners (IAxis axis)
		{
			axis.SizeRequested += new SizeRequestedHandler (OnAxisSizeRequested);
			axis.Changed += new EventHandler (OnAxisChanged);
		}

		protected override void RemoveAxisListeners (IAxis axis)
		{
			axis.SizeRequested -= new SizeRequestedHandler (OnAxisSizeRequested);
			axis.Changed -= new EventHandler (OnAxisChanged);
		}

		protected override void Redraw (Cairo.Context cr)
		{
			// no axes, or have we not been SizeAllocated yet?
			if (axis_requisitions.Count <= 0) {
				return;
			}

			// remember, Gdk.Rectangle is a ValueType.
			Gdk.Rectangle alloc = plot_alloc;
			
			// render the field
			style_prov.DrawField (cr, alloc.X, alloc.Y,
			                      alloc.Width, alloc.Height);

			Gdk.Rectangle inner_alloc = alloc;
			inner_alloc.X += style_prov.FieldLineThickness;
			inner_alloc.Y += style_prov.FieldLineThickness;
			inner_alloc.Width -= (style_prov.FieldLineThickness * 2);
			inner_alloc.Height -= (style_prov.FieldLineThickness * 2);

            
            // create an ArrayList of axes for each location
            ArrayList[] axes_by_location = new ArrayList[4];
			foreach (IAxis axis in axes)
			{
				if (!axis.Visible) {
					continue;
				}

				ArrayList list = axes_by_location[(int)axis.Location];
				if (list == null) {
					list = new ArrayList ();
				}

				list.Add (axis);

				axes_by_location[(int)axis.Location] = list;
			}

			for (int i = 0; i < 4; i++)
			{
				int x = 0, y = 0;

				AxisLocation l = (AxisLocation)i;
				switch (l) {
				case AxisLocation.Top:
					y = alloc.Y;
					break;
				case AxisLocation.Bottom:
					y = alloc.Y + alloc.Height;
					break;
				case AxisLocation.Left:
					x = alloc.X;
					break;
				case AxisLocation.Right:
					x = alloc.X + alloc.Width;
					break;
				}

				ArrayList list = axes_by_location[i];
				if (list == null) {
					continue;
				}
				
				foreach (IAxis axis in list)
				{
					if (!axis_requisitions.ContainsKey (axis)) {
						// hasn't been size requested.  we'll get it next redraw
						continue;
					}

					Requisition req = (Requisition)axis_requisitions[axis];
					
					Gdk.Rectangle a = new Gdk.Rectangle ();
					switch ((AxisLocation)i) {
					case AxisLocation.Top:
						a.X = alloc.X;
						a.Y = y - req.Height;
						a.Width = alloc.Width;
						a.Height = req.Height;

						y -= a.Height;
						break;
					case AxisLocation.Bottom:
						a.X = alloc.X;
						a.Y = y;
						a.Width = alloc.Width;
						a.Height = req.Height;

						y += a.Height;
						break;
					case AxisLocation.Left:
						a.X = x - req.Width;
						a.Y = alloc.Y;
						a.Width = req.Width;
						a.Height = alloc.Height;
						
						x -= a.Width;
						break;
					case AxisLocation.Right:
						a.X = x;
						a.Y = alloc.Y;
						a.Width = req.Width;
						a.Height = alloc.Height;
	
						x += a.Width;
						break;
					}

					if (a.Width > 0 && a.Height > 0) {
						cr.Rectangle (a.X, a.Y, a.Width, a.Height);
						cr.Clip ();

						axis.DrawTicks (a, cr);

						cr.ResetClip ();
					}

					if (draw_grid_lines) {
						cr.Rectangle (inner_alloc.X - 0.5f, inner_alloc.Y - 0.5f,
							      inner_alloc.Width + 0.5f, inner_alloc.Height + 0.5f);
						cr.Clip ();

						axis.DrawGridLines (inner_alloc, cr);
						
						cr.ResetClip ();
					}
				}
			}
			
			// clip drawing to plot_alloc
			cr.Rectangle (inner_alloc.X - 0.5f, inner_alloc.Y,
			              inner_alloc.Width, inner_alloc.Height);
			cr.Clip ();

			// draw all plots
			foreach (IPlot plot in plots)
			{
				plot.SizeAllocate (plot_alloc);
				plot.Draw (cr);
			}

			// draw selection rect
			if (selection_started) {
				style_prov.DrawSelectionRectangle (cr, sel_rect.X,
				                                   sel_rect.Y,
				                                   sel_rect.Width,
			                                           sel_rect.Height);
			}

			cr.ResetClip ();
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton ev)
		{
			base.OnButtonPressEvent (ev);
			
			if (ev.Button == 1 && plots.Count > 0) {
				sel_start.X = (int)ev.X;
				sel_start.Y = (int)ev.Y;

				sel_rect.X = sel_start.X;
				sel_rect.Y = sel_start.Y;

				selection_started = true;
				return true;
			} else if (ev.Button == 3) {
				foreach (IAxis axis in axes)
				{
					axis.Unzoom ();
				}

				QueueResize ();
			}

			return false;
		}

		protected override bool OnButtonReleaseEvent (Gdk.EventButton ev)
		{
			base.OnButtonReleaseEvent (ev);

			if (ev.Button == 1 && plots.Count > 0) {
				// User wants to zoom
				if (selection_started
				    && sel_rect.Width >= 10
				    && sel_rect.Height >= 10) {
					IAxis x_axis, y_axis;
					GetXAndYAxes (out x_axis, out y_axis);

					int x_min = Int32.MaxValue;
					int x_max = Int32.MinValue;
					int y_min = Int32.MaxValue;
					int y_max = Int32.MinValue;

					// translate sel_rect into grid coordinates
					if (x_axis != null) {
						int x1 = sel_rect.X - plot_alloc.X;
						int x2 = x1 + sel_rect.Width;

						x_min = Convert.ToInt32 ((x1 / (double)plot_alloc.Width) * GRID_MAX);
						x_max = Convert.ToInt32 ((x2 / (double)plot_alloc.Width) * GRID_MAX);
						
						RemoveAxisListeners (y_axis);
						x_axis.Zoom (x_min, x_max);
						AddAxisListeners (y_axis);
					}

					if (y_axis != null) {
						int y1 = sel_rect.Y - plot_alloc.Y;
						int y2 = y1 + sel_rect.Height;

						y_min = Convert.ToInt32 (-1 * GRID_MAX * ((y2 - plot_alloc.Height) / (double)plot_alloc.Height));
						y_max = Convert.ToInt32 (-1 * GRID_MAX * ((y1 - plot_alloc.Height) / (double)plot_alloc.Height));
						
						RemoveAxisListeners (y_axis);
						y_axis.Zoom (y_min, y_max);
						AddAxisListeners (y_axis);
					}

					// now, do it for the rest of the axes
					foreach (IAxis axis in axes)
					{
						if (axis == x_axis || axis == y_axis) {
							continue;
						}
							
						if (axis.Dimension == 0) {
							axis.Zoom (x_min, x_max);
						} else if (axis.Dimension == 1) {
							axis.Zoom (y_min, y_max);
						}
					}

					sel_rect = new Gdk.Rectangle ();
					selection_started = false;

					QueueResize ();
					return true;
				}

				sel_rect = new Gdk.Rectangle ();
				selection_started = false;

				// convert the clicked point to graph locations
				int x = ((int)ev.X - plot_alloc.X);
				int y = ((int)ev.Y - plot_alloc.Y);

				if ((x < 0 || x > plot_alloc.Width)
				    || (y < 0 || y > plot_alloc.Height)) {
					return true;
				}

				foreach (IPlot plot in plots)
				{
					plot.UnselectAll ();

					if (plot.CanFocus) {
						plot.ReleaseFocus ();
					}
				}

				for (int i = 0; i < plots.Count; i++)
				{
					IPlot plot = (IPlot)plots[i];

					focused_plot_index = i;
					if ((ev.State & Gdk.ModifierType.ControlMask)
						== Gdk.ModifierType.ControlMask)
					{
						if (plot.UnselectPoint (x, y)) {
							break;
						}

						continue;
					}

					if (plot.SelectPoint (x, y)) {
						break;
					}
				}

				QueueDraw ();
			}

			return true;
		}

		protected override bool OnMotionNotifyEvent (Gdk.EventMotion ev)
		{
			Gdk.ModifierType state;
			int x, y;

			if (show_tooltips) {
				if (tooltip_window != null
				    && tooltip_window.Visible) {
					StartHideTooltipDelay ();
				} else {
					StartShowTooltipDelay ();
				}
			}

			if (!selection_started) {
				return false;
			}

			if (ev.IsHint) {
				GdkWindow.GetPointer (out x, out y, out state);
			} else {
				x = (int)ev.X;
				y = (int)ev.Y;
				state = ev.State;
			}

			sel_rect.X = Math.Min (sel_start.X, x);
			sel_rect.Y = Math.Min (sel_start.Y, y);
			sel_rect.Width = Math.Abs (sel_start.X - x);
			sel_rect.Height = Math.Abs (sel_start.Y - y);

			sel_rect.Intersect (plot_alloc);

			QueueDraw ();

			return true;
		}

		protected override bool OnLeaveNotifyEvent (Gdk.EventCrossing evt)
		{
			base.OnLeaveNotifyEvent (evt);

			if (tooltip_window != null
			    && tooltip_window.Visible) {
				tooltip_window.Hide ();
			}
			
			cursor_in_widget = false;

			return false;
		}

		protected override bool OnEnterNotifyEvent (Gdk.EventCrossing evt)
		{
			base.OnEnterNotifyEvent (evt);

			cursor_in_widget = true;
	
			return false;
		}

		/* private fields */
		private const long GRID_MIN = 0;
		private const long GRID_MAX = 10000;

		private Hashtable axis_requisitions = new Hashtable ();

		private bool draw_grid_lines = true;
		private Gdk.Rectangle plot_alloc = new Gdk.Rectangle ();

		private bool selection_started = false;
		private Gdk.Rectangle sel_start = new Gdk.Rectangle ();
		private Gdk.Rectangle sel_rect = new Gdk.Rectangle ();

		private bool show_tooltips = true;
		private uint show_tooltips_id = 0;
		private Window tooltip_window = null;
		private uint hide_tooltips_id = 0;

		private const uint TOOLTIP_APPEAR_DELAY = 500;
		private const uint TOOLTIP_HIDE_DELAY = 200;
		private const int TOOLTIP_Y_OFFSET = 15;

		private bool cursor_in_widget = false;
		
		/* private method */
		private void StartShowTooltipDelay ()
		{
			if (show_tooltips_id > 0) {
				GLib.Source.Remove (show_tooltips_id);
			}

			show_tooltips_id = GLib.Timeout.Add (TOOLTIP_APPEAR_DELAY,
			                                     new GLib.TimeoutHandler (ShowTooltipTimeoutHandler));
		}

		private void StartHideTooltipDelay ()
		{
			// only start the hide timeout if we're not already
			// running one
			if (hide_tooltips_id > 0) {
				return;
			}

			hide_tooltips_id = GLib.Timeout.Add (TOOLTIP_HIDE_DELAY,
			                                     new GLib.TimeoutHandler (HideTooltipTimeoutHandler));
		}

		private bool ShowTooltipTimeoutHandler ()
		{
			show_tooltips_id = 0;
			if (!cursor_in_widget) {
				return false;
			}

			if (GdkWindow == null) {
				return false;
			}

			// pointer will be relative to the GdkWindow
			Gdk.ModifierType state;
			int x, y;
			GdkWindow.GetPointer (out x, out y, out state);

			int x1 = x - plot_alloc.X;
			int y1 = y - plot_alloc.Y;

			Widget tooltip_child = null;
			foreach (IPlot plot in plots)
			{
				if (plot.GetTooltipForPoint (x1, y1, out tooltip_child)) {
					break;
				}
			}

			if (tooltip_child == null) {
				return false;
			}

			if (tooltip_window == null) {
				tooltip_window = new Window (WindowType.Popup);
// Need Gtk+ 2.10 for this
//				tooltip_window.TypeHint = Gdk.WindowTypeHint.Tooltip;
				tooltip_window.AppPaintable = true;
				tooltip_window.Resizable = false;
				tooltip_window.Name = "gtk-tooltips";
				tooltip_window.BorderWidth = 4;
				tooltip_window.ExposeEvent += new ExposeEventHandler (OnTooltipExpose);
			}

			if (tooltip_window.Child != null) {
				tooltip_window.Remove (tooltip_window.Child);
			}

			tooltip_window.Add (tooltip_child);

			/*
			 * We can't just obtain the SizeRequest of
			 * tooltip_window because it isn't calculated until the
			 * container is laid out, so if the window's request is
			 * smaller than the child, we'll use the childs, and
			 * add some padding.  It'll be close.
			 */
			Requisition req = tooltip_window.SizeRequest ();
			Requisition child_req = tooltip_child.SizeRequest ();
			int w = req.Width < child_req.Width ? child_req.Width + 10 : req.Width;
			int h = req.Height < child_req.Height ? child_req.Height + 10 : req.Height;

			int pos_x, pos_y;
			GdkWindow.GetOrigin (out pos_x, out pos_y);

			x += pos_x;
			y += pos_y;

			// offset the tooltip window a bit
			x -= (w / 2);
			y += TOOLTIP_Y_OFFSET;

			int monitor = Screen.GetMonitorAtPoint (x, y);

			Gdk.Rectangle monitor_extents = Screen.GetMonitorGeometry (monitor);

			// Check left/right bounds
			int m_right = monitor_extents.X + monitor_extents.Width;
			if (x + w > m_right) {
				x = m_right - w;
			} else if (x < monitor_extents.X) {
				x = monitor_extents.X;
			}

			// Check top/bottom bounds
			int m_bottom = monitor_extents.Y + monitor_extents.Height;
			if (y + h > m_bottom) {
				// make sure you can actually see the point
				y = m_bottom - h - (TOOLTIP_Y_OFFSET * 2);
			} else if (y < monitor_extents.Y) {
				y = monitor_extents.Y;
			}

			// If we have a hide queued, stop it, as we don't want
			// our shown window to be immediately hidden
			if (hide_tooltips_id > 0) {
				GLib.Source.Remove (hide_tooltips_id);
				hide_tooltips_id = 0;
			}

			tooltip_window.Move (x, y);
			tooltip_window.ShowAll ();

			show_tooltips_id = 0;
			return false;
		}

		private bool HideTooltipTimeoutHandler ()
		{
			hide_tooltips_id = 0;
			if (tooltip_window == null
			    || !tooltip_window.Visible) {
				return false;
			}

			// pointer will be relative to the GdkWindow
			Gdk.ModifierType state;
			int x, y;
			GdkWindow.GetPointer (out x, out y, out state);

			int x1 = x - plot_alloc.X;
			int y1 = y - plot_alloc.Y;

			Widget tooltip_child = null;
			foreach (IPlot plot in plots)
			{
				if (plot.GetTooltipForPoint (x1, y1, out tooltip_child)) {
					break;
				}
			}

			if (tooltip_child == null) {
				// We're not over any point, so hide the window
				tooltip_window.Hide ();
				return false;
			} else {
				ShowTooltipTimeoutHandler ();
			}

			// Leave the window open
			return false;
		}

		[GLib.ConnectBefore]
		private void OnTooltipExpose (object o, ExposeEventArgs args)
		{
			Window w = (Window)o;

			Gtk.Style.PaintFlatBox (w.Style,
			                        w.GdkWindow,
			                        StateType.Normal,
			                        ShadowType.Out,
			                        args.Event.Area,
			                        w,
			                        "tooltip",
			                        0, 0,
			                        w.Requisition.Width,
			                        w.Requisition.Height);
		}

		private void GetXAndYAxes (out IAxis x_axis, out IAxis y_axis)
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

		private void OnAxisChanged (object o, EventArgs args)
		{
			CalculateLayout ();
			QueueDraw ();
		}

		private void CalculateLayout ()
		{
			int visible_axes = 0;
			foreach (IAxis axis in axes)
			{
				if (axis.Visible) {
					visible_axes++;
				}
			}

			if (axis_requisitions.Count != visible_axes) {
				// we've missed some, re-allocate
				QueueResize ();
				return;
			}

			int x = 0;
			int y = 0;
			int width = Allocation.Width;
			int height = Allocation.Height;

			// Calculate the plot area
			for (int i = 0; i < axes.Count; i++)
			{
				IAxis axis = (IAxis)axes[i];
				if (!axis.Visible) {
					continue;
				}

				Requisition req = (Requisition)axis_requisitions[axis];

				// negotiate plot region size based on the
				// axis label size
				switch (axis.Location) {
				case AxisLocation.Top:
					y += req.Height;
					height -= req.Height;
					break;
				case AxisLocation.Bottom:
					height -= req.Height;
					break;
				case AxisLocation.Left:
					x += req.Width;
					width -= req.Width;
					break;
				case AxisLocation.Right:
					width -= req.Width;
					break;
				}
			}

			if (width <= 0 || height <= 0) {
				return;
			}

			// calculate the axes new allocation

			// fixup the height of the axes along dimension 1's
			// first, calculating the total width along the way

			int dim_1_width = 0;
			foreach (DictionaryEntry entry in axis_requisitions)
			{
				IAxis axis = (IAxis)entry.Key;
				Requisition req = (Requisition)entry.Value;

				if (axis.Dimension == 1) {
					req.Height = height;
					dim_1_width += req.Width;
				}
			}

			// now, fixup the width for the 0 dimension axes
			foreach (DictionaryEntry entry in axis_requisitions)
			{
				IAxis axis = (IAxis)entry.Key;
				Requisition req = (Requisition)entry.Value;

				if (axis.Dimension == 0) {
					req.Width = width - dim_1_width;
				}
			}

			plot_alloc = new Gdk.Rectangle ();
			plot_alloc.X = x;
			plot_alloc.Y = y;
			plot_alloc.Width = width;
			plot_alloc.Height = height;
		}
		
		private void OnAxisSizeRequested (object o, SizeRequestedArgs args)
		{
			IAxis axis = (IAxis)o;
			if (!axis.Visible) {
				return;
			}

			axis_requisitions[axis] = args.Requisition;

			CalculateLayout ();
			QueueDraw ();
		}
	}
}
