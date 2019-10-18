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
	public class Legend : Misc
	{
		/* public properties */
		public Graph Graph {
			get { return graph; }
			set {
				if (graph != null) {
					graph.PlotsChanged -= new PlotsChangedHandler (OnPlotsChanged);
				}

				graph = value;
				graph.PlotsChanged += new PlotsChangedHandler (OnPlotsChanged);
			}
		}
		
		public Orientation Orientation {
			get { return orientation; }
			set {
				orientation = value;
				QueueResize ();
			}
		}

		public int Spacing {
			get { return spacing; }
			set {
				spacing = value;
				QueueResize ();
			}
		}

		/* public methods */
		public Legend () : base ()
		{
			this.style = new GtkStyleProvider (this.Style,
			                                   this.State,
			                                   this.PangoContext);
		}

		public Legend (Graph graph) : this ()
		{
			this.Graph = graph;
		}

		public Legend (Graph graph, IStyleProvider style) : base ()
		{
			this.Graph = graph;
			this.style = style;
		}

		/* protected methods */
		protected override void OnSizeRequested (ref Requisition req)
		{
			req = Requisition.Zero;
			
			if (graph == null || graph.Plots.Length == 0) {
				return;
			}

			int visible_plots = 0;
			if (Orientation == Orientation.Horizontal) {
				foreach (IPlot plot in graph.Plots)
				{
					if (plot.ShowInLegend) {
						Requisition plot_req = plot.LegendSizeRequest (orientation,
											       spacing);
						req.Width += plot_req.Width;
						req.Height = Math.Max (req.Height,
								       plot_req.Height);
						visible_plots++;
					}
				}

				req.Width += (Spacing * (visible_plots - 1));
			} else {
				foreach (IPlot plot in graph.Plots)
				{
					if (plot.ShowInLegend) {
						Requisition plot_req = plot.LegendSizeRequest (orientation,
											       spacing);
						req.Width = Math.Max (req.Width,
								      plot_req.Width);
						req.Height += plot_req.Height;
						visible_plots++;
					}
				}

				req.Height += (Spacing * (visible_plots - 1));
			}

			req.Width += (style.LegendBorderSize * 2) + (spacing * 2) + (Xpad * 2);
			req.Height += (style.LegendBorderSize * 2) + (spacing * 2) + (Ypad * 2);
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

		protected override void OnStateChanged (StateType prev_state)
		{
			base.OnStateChanged (prev_state);

			style.State = State;
			QueueDraw ();
		}

		protected override void OnStyleSet (Style prev_style)
		{
			base.OnStyleSet (prev_style);

			if (style != null) {
				style.Style = Style;
			}
		}

		/* private fields */
		private Graph graph = null;
		private IStyleProvider style = null;
		private Orientation orientation = Orientation.Vertical;
		private int spacing = 6;

		/* private methods */
		private void Redraw (Cairo.Context cr)
		{
			int x = Xpad + (int)(Xalign * (Allocation.Width - Requisition.Width - Xpad));
			int y = Ypad + (int)(Yalign * (Allocation.Height - Requisition.Height - Ypad));

			style.DrawLegendArea (cr, x, y,
			                      Requisition.Width - (Xpad * 2) - style.LegendBorderSize,
			                      Requisition.Height - (Ypad * 2) - style.LegendBorderSize);

			x += style.LegendBorderSize + spacing;
			y += style.LegendBorderSize + spacing;

			foreach (IPlot plot in graph.Plots)
			{
				if (!plot.ShowInLegend) {
					continue;
				}

				Requisition plot_req = plot.LegendSizeRequest (orientation,
				                                               spacing);

				Gdk.Rectangle alloc = new Gdk.Rectangle ();
				alloc.X = x;
				alloc.Y = y;
				alloc.Width = plot_req.Width;
				alloc.Height = plot_req.Height;

				plot.DrawLegend (cr, alloc, orientation,
				                 spacing);

				if (orientation == Orientation.Horizontal) {
					x += alloc.Width + spacing;
				} else {
					y += alloc.Height + spacing;
				}
			}
		}

		private void OnPlotsChanged (object o, IPlot[] plots)
		{
			QueueResize ();
		}
	}
}
