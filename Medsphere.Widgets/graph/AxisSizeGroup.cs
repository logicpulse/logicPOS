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
	public class AxisSizeGroup
	{
		public SizeGroupMode Mode {
			get { return mode; }
			set {
				mode = value;

				RecalculateLayout ();
			}
		}

		public AxisSizeGroup (SizeGroupMode mode)
		{
			this.mode = mode;
		}
		
		public void AddAxis (IAxis axis, Widget parent)
		{
			axes.Add (axis);
			axis.SizeRequested += new SizeRequestedHandler (OnAxesSizeRequested);

			parent.SizeAllocated += new SizeAllocatedHandler (OnParentSizeAllocated);
			axes_parents[axis] = parent;

			RecalculateLayout ();
		}

		/* private fields */
		private Hashtable axes_parents = new Hashtable ();
		private ArrayList axes = new ArrayList ();
		private SizeGroupMode mode = SizeGroupMode.None;

		/* private methods */
		private void RecalculateLayout ()
		{
			if (mode == SizeGroupMode.None) {
				return;
			}

			Hashtable requisitions = new Hashtable ();

			// find the max requisition in the desired direction
			int horiz_max = -1;
			int vert_max = -1;
			foreach (IAxis axis in axes)
			{
				Widget parent = (Widget)axes_parents[axis];
				if (parent.Allocation.X == -1
				    || parent.Allocation.Y == -1) {
					// wait until all the parents have
					// recieved their allocation
					return;
				}

				Requisition req = axis.SizeRequest (parent.Allocation);
				requisitions[axis] = req;

				if (mode == SizeGroupMode.Horizontal
				    || mode == SizeGroupMode.Both) {
					horiz_max = Math.Max (req.Width, horiz_max);
				}

				if (mode == SizeGroupMode.Vertical
				    || mode == SizeGroupMode.Both) {
					vert_max = Math.Max (req.Height, vert_max);
				}
			}

			if (horiz_max == -1 && vert_max == -1) {
				return;
			}

			// apply the new requisition to all the axes
			foreach (IAxis axis in axes)
			{
				Requisition req = (Requisition)requisitions[axis];
					
				if (mode == SizeGroupMode.Horizontal
				    || mode == SizeGroupMode.Both) {
					req.Width = horiz_max;
				}

				if (mode == SizeGroupMode.Vertical
				    || mode == SizeGroupMode.Both) {
					req.Width = vert_max;
				}

				// avoid looping
				axis.SizeRequested -= new SizeRequestedHandler (OnAxesSizeRequested);
				axis.FireSizeRequested (req);
				axis.SizeRequested += new SizeRequestedHandler (OnAxesSizeRequested);
			}
		}

		private void OnAxesSizeRequested (object o, SizeRequestedArgs args)
		{
			RecalculateLayout ();
		}	

		private void OnParentSizeAllocated (object o, SizeAllocatedArgs args)
		{
			RecalculateLayout ();
		}
	}
}
