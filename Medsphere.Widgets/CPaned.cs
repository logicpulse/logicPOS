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

/**
 * CollapsePaned
 *
 * CPaned is a helper class for vpaneds with an expander child
 **/

/**
 * XXX: snap_lock is not being correctly set when the paned is moved using the
 *      keyboard.
 * XXX: this code only works when the expander is paned.Child2. it could be made
 *      to work for either Child1/Child2 with some added logic to decide whether
 *      to add/subtract, use min/max, etc.
 **/

using System;
using GLib;
using Gtk;

namespace Medsphere.Widgets
{
	public class CPaned
	{
		public Paned Paned {
			get { return paned; }
		}

		protected Paned paned;
		protected Expander expander;

		protected bool size_alloc_lock = false;
		protected bool snap_lock = false;

		protected double stored_pos = 0.5;

		public CPaned (Paned p)
		{
			if (!(p is VPaned) || !(p.Child2 is Expander)) {
				Console.WriteLine ("CPaned: this ain't gonna fly");
				return;
			}

			paned = p;
			expander = p.Child2 as Expander;

			paned.SizeAllocated +=
				new SizeAllocatedHandler (OnPanedSizeAllocated);
			paned.AcceptPosition +=
				new AcceptPositionHandler (OnPanedAcceptPosition);
			paned.ButtonPressEvent +=
				new ButtonPressEventHandler (OnPanedButtonPressEvent);
			paned.ButtonReleaseEvent +=
				new ButtonReleaseEventHandler (OnPanedButtonReleaseEvent);
			paned.AddNotification (
				"position", new NotifyHandler (OnPanedPositionChanged));

			expander.AddNotification ("expanded", new GLib.NotifyHandler (OnExpandedChanged));
		}

		public CPaned (Paned p, double initial_pos) : this (p)
		{
			stored_pos = initial_pos;
		}

		private void OnPanedSizeAllocated (object o, SizeAllocatedArgs a)
		{
			if (expander.Expanded) {
				stored_pos =
					(double)paned.Position / (double)paned.Allocation.Height;
			}

			if (size_alloc_lock) {
				size_alloc_lock = false;;
				return;
			}

			Snap ();
		}

		private void OnPanedAcceptPosition (object o, AcceptPositionArgs a)
		{
			Snap ();
		}

		[GLib.ConnectBefore]
		private void OnPanedButtonPressEvent (object o, ButtonPressEventArgs a)
		{
			snap_lock = true;
		}

		[GLib.ConnectBefore]
		private void OnPanedButtonReleaseEvent (object o,
		                                        ButtonReleaseEventArgs a)
		{
			snap_lock = false;
			Snap ();
		}

		private void OnPanedPositionChanged (object o, NotifyArgs a)
		{
			if (!snap_lock) {
				return;
			}

			size_alloc_lock = true;

			int ec_reqh = expander.Child.SizeRequest ().Height;
			bool e = paned.Position + ec_reqh <
			         paned.MaxPosition + (expander.Expanded ? ec_reqh : 0);

			if (expander.Expanded != e) {
				expander.Expanded = e;
			}

			if (e) {
				stored_pos =
					(double)paned.Position / (double)paned.Allocation.Height;
			}
		}

		private void OnExpandedChanged (object o, GLib.NotifyArgs a)
		{
			if (expander.Expanded) {
				size_alloc_lock = true;
				paned.Position = (int)(paned.Allocation.Height * stored_pos);
			} else {
				int e_reqh = expander.SizeRequest ().Height;
				int ec_reqh = expander.Child.SizeRequest ().Height;

				int height = (e_reqh - ec_reqh);

				expander.SetSizeRequest (-1, (height >= -1) ? height : -1);
				Snap ();
				expander.SetSizeRequest (-1, -1);
			}
		}

		/**
		 * OH SNAP
		 * http://baz.medsphere.com/~brad/Pelican%20Oh%20Snap.JPG
		 **/
		private void Snap ()
		{
			if (expander.Expanded || snap_lock) {
				return;
			}

			size_alloc_lock = true;
			paned.Position = paned.Allocation.Height;
		}
	}
}
