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
	public delegate IComparable PlotValueDataFunc (ITreeModelPlot plot, int dimension, TreeModel model, TreeIter iter, object data);

	public delegate void PointActivatedHandler (ITreeModelPlot plot, TreeModel model, TreeIter iter);

	public delegate Widget PointTooltipDataFunc (ITreeModelPlot plot, TreeModel model, TreeIter iter);

	public interface ITreeModelPlot : IPlot
	{
		/**
		 * Raised when the model, or the data in the model has changed,
		 * requiring axis recalculation and redrawing.
		 */
		event EventHandler ModelChanged;

		/**
		 * Raised when a point is clicked or activated by the keyboard.
		 * Should be fired even if point is CTRL+Clicked.
		 */
		event PointActivatedHandler PointActivated;

		PointTooltipDataFunc TooltipDataFunc {
			set;
		}

		TreeModel Model {
			get;
			set;
		}

		bool GetSelected (out TreeModel model, out TreeIter iter);

		void SelectIter (TreeIter iter);

		bool IterIsSelected (TreeIter iter);

		bool PathIsSelected (TreePath path);
	}
}
