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

using Gdk;
using Gtk;
using Cairo;
using System;

namespace Medsphere.Widgets
{
	public interface ICairoCellRenderer
	{
		void GetSize (out int width, out int height);
		
		void Render (Cairo.Context cr,
		             Widget widget,
		             Gdk.Rectangle background_area,
		             Gdk.Rectangle cell_area,
		             CairoCellRendererState state);
	}

	[Flags]
	public enum CairoCellRendererState
	{
		None = 0,
		Selected = 2,
		Insensitive = 4,
		Focused = 8
	}
}
