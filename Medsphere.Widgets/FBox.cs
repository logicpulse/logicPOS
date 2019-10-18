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

using System;
using System.Collections;
using GLib;
using Gdk;
using Gtk;

namespace Medsphere.Widgets
{
	public class FBox : Container
	{
		private class FBoxChild : Container.ContainerChild
		{
			private int left_pad = 0;
			private bool newline = false;
			private bool expand = false;

			public FBoxChild (FBox p, Widget c, int l, bool e, bool n) :
				base (p, c)
			{
				left_pad = l;
				expand = e;
				newline = n;
			}

			public int LeftPadding
			{
				get { return left_pad; }
				set {
					if (left_pad == value) {
						return;
					}

					left_pad = value;
					parent.QueueResize ();
				}
			}

			public bool Newline
			{
				get { return newline; }
				set {
					if (newline == value) {
						return;
					}

					newline = value;
					parent.QueueResize ();
				}
			}

			public bool Expand
			{
				get { return expand; }
				set {
					if (expand == value) {
						return;
					}

					expand = value;
					parent.QueueResize ();
				}
			}
		}
		
		private class ReqCache
		{
			Hashtable cache = new Hashtable ();

			public ReqCache ()
			{

			}

			public Requisition GetSizeRequest (Widget w)
			{
				if (!cache.Contains (w)) {
					cache[w] = w.SizeRequest ();
				}

				return (Requisition)cache[w];
			}
		}

		int leading = 6;
		bool indent = false;
		ArrayList children = new ArrayList ();

		public FBox () : base ()
		{
			WidgetFlags |= WidgetFlags.NoWindow;
		}

		protected FBox (IntPtr ptr) : base (ptr)
		{
		}

		/**
		 * Pronounced "ledding." In typography, the vertical spacing between
		 * lines of type (between baselines). The name comes from the early
		 * hot-metal days of typesetting when the space was achieved with thin
		 * bars of lead.
		 **/
		public int Leading
		{
			get { return leading; }
			set {
				if (leading == value) {
					return;
				}

				leading = value;
				QueueResize ();
			}
		}

		/**
		 * false: each line starts at the far left of the FBox's allocation.
		 *                                       *
		 * [  widget 1][widget 2][     widget 3] *
		 * [widget 4 ][widget 5  ][widget 6]     *
		 *                                       *
		 * true: starting with the second line, each line will be indented by
		 *       an amount equal to the width of the first widget in the FBox.
		 *                                       *
		 * [  widget 1][widget 2][     widget 3] *
		 *             [widget 4 ][widget 5  ]   *
		 *             [widget 6]                *
		 *                                       *
		 **/
		public bool Indent
		{
			get { return indent; }
			set {
				if (indent == value) {
					return;
				}

				indent = value;
				QueueResize ();
			}
		}

		public override ContainerChild this[Widget w]
		{
			get {
				foreach (FBoxChild c in children) {
					if (c.Child == w) {
						return c;
					}
				}

				return null;
			}
		}

		public override GType ChildType ()
		{
			return Widget.GType;
		}

		public void Append (Widget w)
		{
			Append (w, 0, false, false);
		}

		public void Append (Widget w, int l, bool e, bool n)
		{
			children.Add (new FBoxChild (this, w, l, e, n));
			w.Parent = this;
			QueueResize ();
		}

		protected override void OnAdded (Widget w)
		{
			Append (w, 0, false, false);
		}

		protected override void OnRemoved (Widget w)
		{
			FBoxChild c = this[w] as FBoxChild;

			if (c != null) {
				c.Child.Unparent ();
				children.Remove (c);
				QueueResize ();
			}
		}

		protected override void ForAll (bool include_internals, Callback cb)
		{
			for (int i = 0; i < children.Count; i++)
			{
				cb (((FBoxChild)children[i]).Child);
			}
		}

		protected override void OnSizeAllocated (Rectangle a)
		{
			base.OnSizeAllocated (a);

			bool new_row = false;
			int x = a.X, y = a.Y, indent_w = 0, max_h = 0;
			int req_w = -1, req_h = -1;
			Requisition r;
			Rectangle ca = new Rectangle ();

			ArrayList row = null;
			ArrayList rows = new ArrayList ();

			ReqCache req_cache = new ReqCache ();

			foreach (FBoxChild c in children) {
				if (!c.Child.Visible) {
					continue;
				}

				r = req_cache.GetSizeRequest (c.Child);

				if (rows.Count == 0 || new_row || c.Newline || 
				    x + c.LeftPadding + r.Width > a.Right)
				{
					row = new ArrayList ();
					rows.Add (row);

					x = a.X + c.LeftPadding + r.Width;
				} else {
					x += c.LeftPadding + r.Width;
				}

				row.Add (c);

				new_row = c.Expand;
			}

			foreach (ArrayList current_row in rows) {
				foreach (FBoxChild c in current_row) {
					r = req_cache.GetSizeRequest (c.Child);

					max_h = Math.Max (max_h, r.Height);
				}

				x = a.X + indent_w;

				foreach (FBoxChild c in current_row) {
					if (x != a.X + indent_w) {
						x += c.LeftPadding;
					}

					r = req_cache.GetSizeRequest (c.Child);

					ca = new Rectangle (x, y, r.Width, max_h);
					if (c.Expand) {
						ca.Width = a.Right - x;
					}
					c.Child.Allocation = ca;

					if (indent &&
					    rows.IndexOf (current_row) == 0 &&
					    current_row.IndexOf (c) == 1)
					{
						indent_w = ca.X - a.X;
					}

					x += ca.Width;
					req_w = Math.Max (req_w, ca.Right - a.X);
				}

				y += max_h + leading;
				max_h = 0;
				req_h = Math.Max (req_h, ca.Bottom + leading - a.Y);
			}

			SetSizeRequest (req_w, req_h);
		}
	}
}
