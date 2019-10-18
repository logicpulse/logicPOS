/*
 * Copyright (c) 2007-2008 Medsphere Systems Corporation
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the
 * Free Software Foundation, Inc., 59 Temple Place - Suite 330,
 * Boston, MA 02111-1307, USA.
 */

using Gtk;
using Cairo;
using System;
using System.Collections;

namespace Medsphere.Widgets
{
	public class BoxCellRenderer : ICairoCellRenderer
	{
#region public properties
		public Orientation Orientation {
			get { return orientation; }
			set { orientation = value; }
		}

		public int Spacing {
			get { return spacing; }
			set { spacing = value; }
		}

		public ICairoCellRenderer[] Children {
			get {
				ICairoCellRenderer[] tmp = new ICairoCellRenderer[children.Count];
				for (int i = 0; i < tmp.Length; i++)
				{
					tmp[i] = ((BoxCellRendererChild)children[i]).Renderer;
				}

				return tmp;
			}
		}
#endregion

#region public methods
		public void PackStart (ICairoCellRenderer renderer)
		{
			children.Add (new BoxCellRendererChild (renderer));
		}

		public void GetSize (out int width, out int height)
		{
			width = height = 0;

			int child_w, child_h;
			foreach (BoxCellRendererChild child in children)
			{
				child.Renderer.GetSize (out child_w, out child_h);
				child.RequestedWidth = child_w;
				child.RequestedHeight = child_h;

				if (orientation == Orientation.Vertical) {
					width = Math.Max (child_w, width);
					height += child_h + spacing;
				} else {
					width += child_w + spacing;
					height = Math.Max (child_h, height);
				}
			}
			
			if (orientation == Orientation.Vertical) {
				height -= spacing;
			} else {
				width -= spacing;
			}
		}
		
		public void Render (Cairo.Context cr,
		                    Widget widget,
		                    Gdk.Rectangle background_area,
		                    Gdk.Rectangle cell_area,
		                    CairoCellRendererState state)
		{
			int x = cell_area.X, bg_x = cell_area.X - (spacing / 2);
			int y = cell_area.Y, bg_y = cell_area.Y - (spacing / 2);

			Gdk.Rectangle child_area = new Gdk.Rectangle ();
			Gdk.Rectangle bg_area = new Gdk.Rectangle ();

			foreach (BoxCellRendererChild child in children)
			{
				int child_w, child_h;
				child.Renderer.GetSize (out child_w, out child_h);

				child_area.X = x;
				child_area.Y = y;

				bg_area.X = bg_x;
				bg_area.Y = bg_y;
				if (orientation == Orientation.Vertical) {
					child_area.Width = cell_area.Width;
					child_area.Height = child_h;

					y += child_area.Height + spacing;

					bg_y = y - (spacing / 2);
				} else {
					child_area.Width = child_w;
					child_area.Height = cell_area.Height;

					x += child_area.Width + spacing;

					bg_x = x - (spacing / 2);
				}

				bg_area.Width = child_area.Width + spacing;
				bg_area.Height = child_area.Height + spacing;

				child.Renderer.Render (cr, widget, bg_area,
						       child_area, state);
			}
		}
#endregion

#region private fields
		private ArrayList children = new ArrayList ();
		private Orientation orientation = Orientation.Vertical;
		private int spacing = 4;
#endregion

#region private classes
		private class BoxCellRendererChild
		{
			public ICairoCellRenderer Renderer;
			public int RequestedWidth;
			public int RequestedHeight;

			public BoxCellRendererChild (ICairoCellRenderer r)
			{
				Renderer = r;
			}
		}
#endregion
	}
}
