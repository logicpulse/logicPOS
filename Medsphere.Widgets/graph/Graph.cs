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
	public delegate void PlotsChangedHandler (object o, IPlot[] plots);

	public abstract class Graph : DrawingArea
	{
		/* public events */
		public event EventHandler SelectionChanged;
		public event PlotsChangedHandler PlotsChanged;

		/* public properties */
		public int Padding {
			get { return padding; }
			set { padding = value; }
		}
		
		public IPlot[] Plots {
			get { return (IPlot[])plots.ToArray (typeof (IPlot)); }
		}

		public IAxis[] Axes {
			get { return (IAxis[])axes.ToArray (typeof (IAxis)); }
		}

		public bool HasSelection {
			get {
				foreach (IPlot plot in plots)
				{
					if (plot.HasSelection) {
						return true;
					}
				}
				return false;
			}
		}

		public IPlot[] SelectedPlots {
			get {
				ArrayList selected_plots = new ArrayList ();
				foreach (IPlot plot in plots)
				{
					if (plot.HasSelection) {
						selected_plots.Add (plot);
					}
				}

				return (IPlot[])selected_plots.ToArray (typeof (IPlot));
			}
		}

		public IStyleProvider StyleProvider {
			set { style_prov = value; }
		}

		/* public methods */
		public Graph (IStyleProvider style) : base ()
		{
			this.style_prov = style;
		}

		public Graph () : base ()
		{
			this.style_prov = new GtkStyleProvider (this.Style, this.State,
			                                        this.PangoContext);
		}

		/**
		 * Append an axis to the axes for this graph.  If there is
		 * already an axis for the provided AxisLocation, AppendAxis
		 * will place this axis on the outside of existing axes.
		 */ 
		public void AppendAxis (IAxis axis)
		{
			if (axis == null) {
				return;
			}

			axes.Add (axis);
			axis.Style = style_prov;
			AddAxisListeners (axis);

			QueueResize ();
		}

		/**
		 * Remove the axis from the graph.
		 *
		 * Undefined behaviors (read: crashing) will occur if you
		 * remove an axis that plots currently reference.  Remove the
		 * plot first, then remove the axis.
		 */ 
		public void RemoveAxis (IAxis axis)
		{
			int index = axes.IndexOf (axis);
			if (index < 0) {
				return;
			}

			axes.RemoveAt (index);
			RemoveAxisListeners (axis);

			QueueResize ();
		}

		/**
		 * Removes all axes and plots from the graph.
		 */
		public virtual void Clear ()
		{	
			foreach (IPlot plot in plots)
			{
				RemovePlotListeners (plot);
			}

			plots.Clear ();

			foreach (IAxis axis in axes)
			{
				RemoveAxisListeners (axis);	
			}

			axes.Clear ();

			focused_plot_index = -1;
			
			if (PlotsChanged != null) {
				PlotsChanged (this, Plots);
			}

			QueueResize ();
		}

		/**
		 * Add a plot to the graph.  This plot will use the specified
		 * axes when drawing.  The axes specified must have already
		 * been appended to the Graph's axes list.
	 	 */
		public void AddPlot (IPlot plot, params IAxis[] axes)
		{
			if (plot == null || axes == null || axes.Length == 0) {
				return;
			}

			plots.Add (plot);
			
			plot.SetAxes (axes);
			plot.Style = style_prov;
			AddPlotListeners (plot);

			if (PlotsChanged != null) {
				PlotsChanged (this, Plots);
			}

			QueueResize ();
		}

		public void RemovePlot (IPlot plot)
		{
			if (plot == null) {
				return;
			}

			int index = plots.IndexOf (plot);
			if (index < 0) {
				return;
			}

			RemovePlotListeners (plot);
			plots.RemoveAt (index);

			if (Allocation.X >= 0 && Allocation.Y >= 0) {
				foreach (IAxis axis in axes)
				{
					axis.SetRange (Plots, Allocation);
				}
			}

			if (PlotsChanged != null) {
				PlotsChanged (this, Plots);
			}

			QueueResize ();
		}

		public void ClearPlots ()
		{
			foreach (IPlot plot in plots)
			{
				RemovePlotListeners (plot);
			}

			plots.Clear ();

			focused_plot_index = -1;

			if (PlotsChanged != null) {
				PlotsChanged (this, Plots);
			}

			QueueResize ();
		}

		public void UnselectAll ()
		{
			foreach (IPlot plot in plots)
			{
				plot.UnselectAll ();
			}

			QueueDraw ();
		}

		public void Unzoom ()
		{
			foreach (IAxis axis in axes)
			{
				axis.Unzoom ();
			}
		}

		public void RecalculateAxisRange ()
		{
			foreach (IAxis axis in axes)
			{
				axis.RecalculateRange (Plots, Allocation);
			}
		}

		/* protected fields */
		protected ArrayList plots = new ArrayList ();
		protected ArrayList axes = new ArrayList ();

		protected int padding = 3;

		protected IStyleProvider style_prov;

		protected int focused_plot_index = -1;

		protected bool freeze_selection_changed = false;

		/* protected methods */
		protected abstract void Redraw (Cairo.Context cr);
		protected abstract void AddAxisListeners (IAxis axis);
		protected abstract void RemoveAxisListeners (IAxis axis);

		protected override void OnRealized ()
		{
			base.OnRealized ();

			CanFocus = true;

			AddEvents ((int)Gdk.EventMask.ExposureMask);
			AddEvents ((int)Gdk.EventMask.ScrollMask);
			AddEvents ((int)Gdk.EventMask.PointerMotionMask);
			AddEvents ((int)Gdk.EventMask.ButtonReleaseMask);
			AddEvents ((int)Gdk.EventMask.ButtonPressMask);
			AddEvents ((int)Gdk.EventMask.KeyPressMask);
			AddEvents ((int)Gdk.EventMask.EnterNotifyMask);
			AddEvents ((int)Gdk.EventMask.LeaveNotifyMask);
		}

		protected override void OnSizeRequested (ref Requisition req)
		{
			req.Width = req.Height = 200;
		}

		protected override bool OnExposeEvent (Gdk.EventExpose args)
		{
			using (Cairo.Context cr = Gdk.CairoHelper.Create (args.Window)) {
				// set a clip region for the expose event
				Gdk.Rectangle rect = args.Area;
				cr.Rectangle (rect.X, rect.Y, rect.Width, rect.Height);
				cr.Clip ();

				Redraw (cr);
			}
			
			return false;	
		}

		protected override bool OnKeyReleaseEvent (Gdk.EventKey ev)
		{
			// Remove this when we can use OnActivate ()
			if (ev.Key == Gdk.Key.KP_Space
			    || ev.Key == Gdk.Key.KP_Enter
			    || ev.Key == Gdk.Key.space
			    || ev.Key == Gdk.Key.ISO_Enter
			    || ev.Key == Gdk.Key.Return) {
				foreach (IPlot plot in plots)
				{
					plot.UnselectAll ();
				}

				foreach (IPlot plot in plots)
				{
					if (plot.HasFocus) {
						plot.SelectFocusedPoint ();
						break;
					}
				}

				return true;
			}

			return false;
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton ev)
		{
			base.OnButtonPressEvent (ev);

			// focus the widget
			if (!HasFocus) {
				GrabFocus ();
			}

			return true;
		}

		/* XXX: Uncomment when ximian bug #81343 is closed and usable.
		protected override void OnActivate ()
		{
			foreach (IPlot plot in plots)
			{
				plot.UnselectAll ();
			}

			foreach (IPlot plot in plots)
			{
				if (plot.HasFocus) {
					int x, y;
					plot.GetFocusedPoint (axes, out x, out y);
					plot.SelectPoint (axes, x, y);
					break;
				}
			}
		}
		*/

		protected override bool OnFocused (DirectionType dir)
		{
			if (plots.Count < 1) {
				return false;
			}

			if (!HasFocus) {
				GrabFocus ();
				return true;
			}

			int visits;
			int focused_x = -1, focused_y = -1;
			switch (dir) {
			case DirectionType.Left:
				((IPlot)plots[focused_plot_index]).FocusPrev ();
				return true;
			case DirectionType.Right:
				((IPlot)plots[focused_plot_index]).FocusNext ();
				return true;
			case DirectionType.Up:
				// Don't move selection if there is only one
				// plot
				if (plots.Count <= 1) {
					return true;
				}

				// get the currently focused point
				((IPlot)plots[focused_plot_index]).GetFocusedPoint (out focused_x,
				                                                    out focused_y);

				// Iterate through the plots in the zorder,
				// looking for the next plot which CanFocus.
				// Also, make sure we don't see plots twice, or
				// infinite loop.
				visits = 0;
				do {
					// move to the next plot in the z-order
					focused_plot_index--;
					if (focused_plot_index < 0) {
						focused_plot_index = plots.Count - 1;
					}

					visits++;					

					IPlot plot = (IPlot)plots[focused_plot_index];
					if (!plot.CanFocus) {
						continue;
					}

					// clear focus on all the plots
					foreach (IPlot p in plots)
					{
						if (p.HasFocus) {
							p.ReleaseFocus ();
						}
					}

					// focus the nearest to the last
					// focused point
					plot.FocusNearest (focused_x, focused_y);
					break;
				} while (visits <= plots.Count);

				return true;
			case DirectionType.Down:
				// Don't move selection if there is only one
				// plot
				if (plots.Count <= 1) {
					return true;
				}

				// get the currently focused point
				((IPlot)plots[focused_plot_index]).GetFocusedPoint (out focused_x,
				                                                    out focused_y);

				// Iterate through the plots in the zorder,
				// looking for the next plot which CanFocus.
				// Also, make sure we don't see plots more than
				// once, or worse, infinite loop.
				visits = 0;
				do {
					// move to the previous plot in the z-order
					focused_plot_index++;
					if (focused_plot_index >= plots.Count) {
						focused_plot_index = 0;
					}


					visits++;					

					IPlot plot = (IPlot)plots[focused_plot_index];
					if (!plot.CanFocus) {
						continue;
					}

					// clear focus on all the plots
					foreach (IPlot p in plots)
					{
						if (p.HasFocus) {
							p.ReleaseFocus ();
						}
					}

					// focus the nearest to the last
					// focused point
					plot.FocusNearest (focused_x, focused_y);
					break;
				} while (visits <= plots.Count);

				return true;
			case DirectionType.TabForward:
			default:
				// let focus jump to the next widget
				return false;
			}
		}

		protected override void OnFocusGrabbed ()
		{
			base.OnFocusGrabbed ();

			if (plots.Count < 1) {
				return;
			}

			// don't switch focus if any plot already has focus
			foreach (IPlot plot in plots)
			{
				if (plot.HasFocus) {
					return;
				}
			}

			// find the plot with the smallest x value, and focus it
			int min_plot = -1;
			int min_x = Int32.MaxValue;
			int min_y = Int32.MaxValue;
			for (int i = 0; i < plots.Count; i++)
			{
				IPlot plot = (IPlot)plots[i];
				if (!plot.CanFocus) {
					continue;
				}

				int x, y;
				plot.GetGrabFocusRequest (out x, out y);

				if ((min_x > x) || (min_x == x && min_y > y)) {
					min_x = x;
					min_y = y;
					min_plot = i;
				}
			}

			if (min_plot == -1) {
				return;
			}

			((IPlot)plots[min_plot]).GrabFocus ();
			focused_plot_index = min_plot;
		}

		protected override bool OnFocusInEvent (Gdk.EventFocus focus)
		{
			if (!focus.In) {
				return true;
			}

			GrabFocus ();

			return true;
		}

		protected override void OnStateChanged (StateType prev_state)
		{
			base.OnStateChanged (prev_state);

			style_prov.State = State;
		}

		protected override void OnStyleSet (Style prev_style)
		{
			base.OnStyleSet (prev_style);

			style_prov.Style = Style;
		}

		protected virtual void AddPlotListeners (IPlot plot)
		{
			plot.Changed += new EventHandler (OnPlotChanged);
			plot.SelectionChanged += new EventHandler (OnPlotSelectionChanged);

			if (plot is ITreeModelPlot) {
				((ITreeModelPlot)plot).ModelChanged += new EventHandler (OnModelChanged);
			}
		}

		protected virtual void RemovePlotListeners (IPlot plot)
		{
			plot.Changed -= new EventHandler (OnPlotChanged);
			plot.SelectionChanged -= new EventHandler (OnPlotSelectionChanged);

			if (plot is ITreeModelPlot) {
				((ITreeModelPlot)plot).ModelChanged -= new EventHandler (OnModelChanged);
			}
		}

		protected virtual void OnPlotSelectionChanged (object o, EventArgs args)
		{
			if (freeze_selection_changed) {
				return;
			}

			// proxy the event up
			if (SelectionChanged != null) {
				SelectionChanged (o, new EventArgs ());
			}
		}

		/* private methods */
		private void OnPlotChanged (object o, EventArgs args)
		{
			QueueDraw ();
		}

		private void OnModelChanged (object o, EventArgs args)
		{
			foreach (IAxis axis in axes)
			{
				axis.SetRange (Plots, Allocation);
			}

			QueueResize ();
		}
	}
}
