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
using System.Reflection;

using Gdk;
using Gtk;

namespace Medsphere.Widgets
{
	public delegate void GridCellDataFunc (GridViewColumn c, CellRenderer r,
	                                       TreeModel m, TreeIter i);

	public class GridSelection
	{
		private GridView gv;

		private TreePath path = null;
		private GridViewColumn col = null;

		public bool HasSelection {
			get { return path != null && col != null; }
		}
		public event EventHandler Changed;

		public GridSelection (GridView g)
		{
			gv = g;

			gv.ColumnsChanged += new EventHandler (OnGridViewColumnsChanged);
		}

		public bool SelectCell (TreePath path, GridViewColumn col)
		{
			if (!gv.GetSelectable (path, col)) {
				return false;
			}

			this.path = path;
			this.col = col;

			if (Changed != null) {
				Changed (this, EventArgs.Empty);
			}

			return true;
		}

		public void Deselect ()
		{
			this.path = null;
			this.col = null;

			if (Changed != null) {
				Changed (this, EventArgs.Empty);
			}
		}

		public bool GetSelected (out TreePath path, out GridViewColumn col)
		{
			path = this.path;
			col = this.col;

			return this.HasSelection;
		}

		private void OnGridViewColumnsChanged (object o, EventArgs a)
		{
			if (path == null && col == null) {
				return;
			}

			if (Array.IndexOf (gv.Columns, col) < 0) {
				Deselect ();
			}
		}
	}

	public class GridViewColumn
	{
		public event EventHandler VisibilityChanged;

		private bool visible = true;

		private CellRenderer header_renderer, field_renderer;
		private Hashtable header_attrs, field_attrs;

		private GridCellDataFunc df = null;
		private GridCellDataFunc hdf = null;

		public GridViewColumn ()
		{

		}

		public GridViewColumn (CellRenderer r, params object[] a)
		{
			field_renderer = r;
			field_attrs = new Hashtable ();

			for (int x = 1; x < a.Length; x += 2) {
				field_attrs[a[x - 1]] = a[x];
			}
		}

		public bool Visible {
			get { return visible; }
			set {
				if (visible == value) {
					return;
				}

				visible = value;

				if (VisibilityChanged != null) {
					VisibilityChanged (this, EventArgs.Empty);
				}
			}
		}


		public CellRenderer FieldRenderer {
			get { return field_renderer; }
		}

		public CellRenderer HeaderRenderer {
			get { return header_renderer; }
		}

		public GridCellDataFunc CellDataFunc {
			set { df = value; }
		}

		public GridCellDataFunc HeaderCellDataFunc {
			set { hdf = value; }
		}

		public void SetHeaderRenderer (CellRenderer r, params object[] a)
		{
			header_renderer = r;
			header_attrs = new Hashtable ();

			for (int x = 1; x < a.Length; x += 2) {
				header_attrs[a[x - 1]] = a[x];
			}
		}

		public void CellSetCellData (TreeModel m, TreeIter i, bool h)
		{
			Hashtable attrs;
			CellRenderer r;
			PropertyInfo pi;
			object v;

			if (h && header_renderer != null) {
				r = header_renderer;
				attrs = header_attrs;
			} else {
				r = field_renderer;
				attrs = field_attrs;
			}

			foreach (object k in attrs.Keys) {
				pi = r.GetType ().GetProperty (k.ToString ());
				v = m.GetValue (i, (int)attrs[k]);

				pi.SetValue (r, v, null);
			}

			if (h && hdf != null) {
				hdf (this, r, m, i);
			} else if (df != null) {
				df (this, r, m, i);
			}
		}
	}

	public class GridView : Container
	{
		public enum ScrollbarSpan {
			HeaderGap,
			HeaderOverlap,
			Full
		}

		private class LinkedScrollbar
		{
			Scrollbar scrollbar;

			public LinkedScrollbar (Scrollbar sb, params Widget[] widgets)
			{
				scrollbar = sb;

				foreach (Widget w in widgets) {
					w.ScrollEvent +=
						new ScrollEventHandler (OnLinkedWidgetScrollEvent);
				}
			}

			/**
			 * the delta calculation is lifted from
			 * _gtk_range_get_wheel_delta (), which is unexposed c api
			 **/
			private void OnLinkedWidgetScrollEvent (object o,
			                                        ScrollEventArgs a)
			{
				if (scrollbar is VScrollbar &&
				    (a.Event.Direction == ScrollDirection.Left ||
				     a.Event.Direction == ScrollDirection.Right))
				{
					return;
				} else if (scrollbar is HScrollbar &&
				           (a.Event.Direction == ScrollDirection.Up ||
				            a.Event.Direction == ScrollDirection.Down))
				{
					return;
				}

				Adjustment adj = scrollbar.Adjustment;
				double delta = Math.Pow (adj.PageSize, 2.0 / 3.0);

				if (a.Event.Direction == ScrollDirection.Up ||
				    a.Event.Direction == ScrollDirection.Left)
				{
					adj.Value -= delta;
				} else {
					adj.Value = Math.Min (adj.Upper - adj.PageSize,
					                      adj.Value + delta);
				}
			}
		}

		public event EventHandler ColumnsChanged;
		public event EventHandler OrientationChanged;

		private ArrayList children = new ArrayList ();

		private TreeModel model;

		private Orientation orientation;

		private ArrayList columns = new ArrayList ();
		private ArrayList visible = new ArrayList ();

		private ArrayList widths = new ArrayList ();
		private ArrayList heights = new ArrayList ();

		/* column header span, row header span */
		private int c_span = 1,/* c_span2 = boring, */r_span = 1;
		/* the column currently being resized, or -1 */
		private int drag_col = -1;

		/**
		 * c = top left corner
		 * t = top
		 * l = left
		 * f = field
		 **/
		private Viewport tvp, lvp, fvp;
		private DrawingArea cda, tda, lda, fda;

		private Scrollbar hbar, vbar;
		/* XXX: is this what ScrolledWindow defaults to? */
		private PolicyType hbar_policy = PolicyType.Always,
		                   vbar_policy = PolicyType.Always;
		private ScrollbarSpan hbar_span = ScrollbarSpan.HeaderGap,
		                      vbar_span = ScrollbarSpan.HeaderGap;

		private GridSelection selection;
		private TreePath prev_sel_path;
		private GridViewColumn prev_sel_col;

		public GridView ()
		{
			CanFocus = true;
			WidgetFlags |= WidgetFlags.NoWindow;

			selection = new GridSelection (this);
			orientation = Orientation.Vertical;

			StyleSet += new StyleSetHandler (OnStyleSet);
			StateChanged += new StateChangedHandler (OnStateChanged);
			FocusInEvent += new FocusInEventHandler (OnFocusInEvent);
			FocusOutEvent += new FocusOutEventHandler (OnFocusOutEvent);
			ButtonPressEvent +=
				new ButtonPressEventHandler (OnButtonPressEvent);
			KeyPressEvent += new KeyPressEventHandler (OnKeyPressEvent);
			selection.Changed += new EventHandler (OnSelectionChanged);

			cda = new DrawingArea ();
			cda.ExposeEvent += new ExposeEventHandler (OnCDAExposeEvent);
			cda.AddEvents ((int)EventMask.PointerMotionMask);
			cda.MotionNotifyEvent +=
				new MotionNotifyEventHandler (OnYHeaderMotionNotifyEvent);
			cda.AddEvents ((int)EventMask.ButtonPressMask);
			cda.ButtonPressEvent +=
				new ButtonPressEventHandler (OnYHeaderButtonPressEvent);
			cda.AddEvents ((int)EventMask.ButtonReleaseMask);
			cda.ButtonReleaseEvent +=
				new ButtonReleaseEventHandler (OnYHeaderButtonReleaseEvent);
			cda.AddEvents ((int)EventMask.ScrollMask);
			Add (cda);

			tda = new DrawingArea ();
			tda.ExposeEvent += new ExposeEventHandler (OnTDAExposeEvent);
			tda.AddEvents ((int)EventMask.PointerMotionMask);
			tda.MotionNotifyEvent +=
				new MotionNotifyEventHandler (OnYHeaderMotionNotifyEvent);
			tda.AddEvents ((int)EventMask.ButtonPressMask);
			tda.ButtonPressEvent +=
				new ButtonPressEventHandler (OnYHeaderButtonPressEvent);
			tda.AddEvents ((int)EventMask.ButtonReleaseMask);
			tda.ButtonReleaseEvent +=
				new ButtonReleaseEventHandler (OnYHeaderButtonReleaseEvent);
			tda.AddEvents ((int)EventMask.ScrollMask);

			lda = new DrawingArea ();
			lda.ExposeEvent += new ExposeEventHandler (OnLDAExposeEvent);
			lda.AddEvents ((int)EventMask.ScrollMask);
			lda.ButtonPressEvent +=
				new ButtonPressEventHandler (OnLDAButtonPressEvent); 

			fvp = new Viewport ();
			fvp.ShadowType = ShadowType.None;

			fda = new DrawingArea ();
			fda.StyleSet += new StyleSetHandler (OnFDAStyleSet);
			fda.Realized += new EventHandler (OnFDARealized);
			fda.ExposeEvent += new ExposeEventHandler (OnFDAExposeEvent);
			fda.AddEvents ((int)EventMask.ButtonPressMask);
			fda.ButtonPressEvent +=
				new ButtonPressEventHandler (OnFDAButtonPressEvent);
			fda.AddEvents ((int)EventMask.ScrollMask);
			fvp.Add (fda);
			Add (fvp);

			tvp = new Viewport (fvp.Hadjustment, null);
			tvp.ShadowType = ShadowType.None;
			tvp.Add (tda);
			Add (tvp);

			lvp = new Viewport (null, fvp.Vadjustment);
			lvp.ShadowType = ShadowType.None;
			lvp.Add (lda);
			Add (lvp);

			hbar = new HScrollbar (fvp.Hadjustment);
			new LinkedScrollbar (hbar, tda, fda);
			Add (hbar);

			vbar = new VScrollbar (fvp.Vadjustment);
			new LinkedScrollbar (vbar, lda, fda);
			Add (vbar);
		}

		public TreeModel Model
		{
			get { return model; }
			set {
				if (model == value) {
					return;
				}

				selection.Deselect ();

				model = value;

				if (model != null) {
					model.RowChanged +=
						new RowChangedHandler (OnModelRowChanged);
					model.RowDeleted +=
						new RowDeletedHandler (OnModelRowDeleted);
					model.RowInserted +=
						new RowInsertedHandler (OnModelRowInserted);
					model.RowsReordered +=
						new RowsReorderedHandler (OnModelRowsReordered);
				}

				RebuildDimensions ();

				cda.QueueDraw ();
				tda.QueueDraw ();
				lda.QueueDraw ();
				fda.QueueDraw ();
			}
		}

		public GridViewColumn[] Columns
		{
			get {
				return
					(GridViewColumn[])columns.ToArray (typeof (GridViewColumn));
			}
		}

		public PolicyType HscrollbarPolicy {
			get { return hbar_policy; }
			set {
				if (hbar_policy == value) {
					return;
				}

				hbar_policy = value;

				QueueResize ();
			}
		}

		public PolicyType VscrollbarPolicy {
			get { return vbar_policy; }
			set {
				if (vbar_policy == value) {
					return;
				}

				vbar_policy = value;

				QueueResize ();
			}
		}

		public ScrollbarSpan HscrollbarSpan {
			get { return hbar_span; }
			set {
				if (hbar_span == value) {
					return;
				}

				hbar_span = value;

				QueueResize ();
			}
		}

		public ScrollbarSpan VscrollbarSpan {
			get { return vbar_span; }
			set {
				if (vbar_span == value) {
					return;
				}

				vbar_span = value;

				QueueResize ();
			}
		}

		public Orientation Orientation
		{
			get { return orientation; }
			set {
				if (orientation != value) {
					orientation = value;

					RebuildDimensions ();

					if (OrientationChanged != null) {
						OrientationChanged (this, EventArgs.Empty);
					}
				}
			}
		}

		public GridSelection Selection
		{
			get { return selection; }
		}

		public int NRowHeaders
		{
			get { return r_span; }
			set {
				if (value >= 0 && value <= columns.Count) {
					r_span = value;

					UpdateDrawingAreaSizeRequests ();
				}
			}
		}

		public int NColHeaders
		{
			get { return c_span; }
			set {
				TreeIter i;
				if (value >= 0 &&
				    (value == 0 || model.IterNthChild (out i, value - 1))) {
					c_span = value;

					UpdateDrawingAreaSizeRequests ();
				}
			}
		}

		public int AppendColumn (GridViewColumn col)
		{
			col.VisibilityChanged +=
				new EventHandler (OnColumnVisibilityChanged);

			if (col.Visible) {
				if (orientation == Orientation.Vertical) {
					widths.Add (0);
				} else {
					heights.Add (0);
				}

				visible.Add (col);

				TreeIter i;

				if (model != null && model.GetIterFirst (out i)) {
					do {
						TreePath path = model.GetPath (i);
						MeasureCell (path, col);
					} while (model.IterNext (ref i));
				}

				if (Visible) {
					UpdateDrawingAreaSizeRequests ();
				}
			}

			int ret = columns.Add (col);

			if (ColumnsChanged != null) {
				ColumnsChanged (this, EventArgs.Empty);
			}

			return ret;
		}

		public GridViewColumn AppendColumn (CellRenderer r, params object[] a)
		{
			GridViewColumn col = new GridViewColumn (r, a);
			AppendColumn (col);

			return col;
		}

		public GridViewColumn AppendColumn (CellRenderer r, GridCellDataFunc f)
		{
			return AppendColumnWithHeader (r, f, null, null);
		}

		public GridViewColumn AppendColumnWithHeader (CellRenderer r,
		                                              GridCellDataFunc f,
		                                              CellRenderer hr,
		                                              GridCellDataFunc hf)
		{
			GridViewColumn col = new GridViewColumn (r);
			col.CellDataFunc = f;
			if (hr != null) {
				col.SetHeaderRenderer (hr);
				col.HeaderCellDataFunc = hf;
			}
			AppendColumn (col);

			return col;
		}

		public int RemoveColumn (GridViewColumn col) {
			if (!columns.Contains (col)) {
				throw new ArgumentException ();
			}

			if (col.Visible) {
				try {
					if (orientation == Orientation.Vertical) {
						widths.RemoveAt (visible.IndexOf (col));
					} else {
						heights.RemoveAt (visible.IndexOf (col));
					}
				} catch {}

				visible.Remove (col);
				UpdateDrawingAreaSizeRequests ();
			}

			columns.Remove (col);

			if (ColumnsChanged != null) {
				ColumnsChanged (this, EventArgs.Empty);
			}

			return columns.Count;
		}

		public bool GetSelectable (TreePath path, GridViewColumn col)
		{
			TreeIter i;

			return col != null &&
			       columns.Contains (col) &&
			       col.Visible &&
			       visible.IndexOf (col) >= r_span &&
			       path != null &&
			       path.Indices[0] >= c_span &&
			       model.GetIter (out i, path);
		}

		public void ScrollToCell (TreePath path, GridViewColumn col, bool a,
		                          float ra, float ca)
		{
			Adjustment adj;
			Widget widget;
			Rectangle rect = CellRect (path, col, out widget);

			if (widget == tda || widget == fda) {
				adj = fvp.Hadjustment;

				if (a) {
					adj.Value = Math.Min (adj.Upper - adj.PageSize,
					                      (rect.X + rect.Width / 2) -
										  adj.PageSize * ra);
				} else {
					if (rect.X < adj.Value ||
						rect.Width > fvp.Allocation.Width)
					{
						adj.Value -= adj.Value - rect.X;
					} else if (rect.Right >
					           fvp.Allocation.Width + adj.Value)
					{
						adj.Value = rect.Right - fvp.Allocation.Width;
					}
				}
			}

			if (widget == lda || widget == fda) {
				adj = fvp.Vadjustment;

				if (a) {
					adj.Value = Math.Min (adj.Upper - adj.PageSize,
					                      (rect.Y + rect.Height / 2) -
					                      adj.PageSize * ca);
				} else {
					if (rect.Y < adj.Value ||
						rect.Height > fvp.Allocation.Height)
					{
						adj.Value -= adj.Value - rect.Y;
					} else if (rect.Bottom >
							   fvp.Allocation.Height + adj.Value)
					{
						adj.Value = rect.Bottom - fvp.Allocation.Height;
					}
				}
			}
		}

		protected override void OnAdded (Widget w)
		{
			w.Parent = this;
			children.Add (w);
		}

		protected override void OnRemoved (Widget w)
		{
			if (w != null) {
				children.Remove (w);
				w.Unparent ();
			}
		}

		protected override void ForAll (bool i, Callback cb)
		{
			for (int k = 0; k < children.Count; k++)
			{
				cb (children[k] as Widget);
			}
		}

		protected override void OnSizeRequested (ref Requisition r)
		{
			/**
			 * XXX: this doesn't change when the visibility of a scrollbar
			 *      changes.
			 **/
			r.Width =
				LeftWidth +
				(XSpan < widths.Count ? (int)widths[XSpan] : 0) +
			    (int)BorderWidth +
				vbar.SizeRequest ().Width;
			r.Height =
				TopHeight +
				(YSpan < heights.Count ? (int)heights[YSpan] : 0) +
			    (int)BorderWidth +
				hbar.SizeRequest ().Height;

			base.OnSizeRequested (ref r);
		}

		/**
		 * we need to call QueueDraw () on each of the drawing areas to avoid
		 * some nasty unpainted rect artifacts on win32
		 **/
		protected override void OnSizeAllocated (Rectangle a)
		{
			base.OnSizeAllocated (a);

			Requisition hbar_r = hbar.SizeRequest (),
			            vbar_r = vbar.SizeRequest ();

			/**
			 * tmp variables to store the scrollbar visibilities, as flipping
			 * the .Visible property can cause infinite OnSizeAllocated() loop
			 **/
			bool hbar_v, vbar_v;

			hbar_v = hbar_policy == PolicyType.Always ||
			         (hbar_policy == PolicyType.Automatic &&
			          LeftWidth + FieldWidth + BorderWidth > a.Width);

			vbar_v = vbar_policy == PolicyType.Always ||
			         (vbar_policy == PolicyType.Automatic &&
			          TopHeight + FieldHeight + BorderWidth +
			          (hbar_v ? hbar_r.Height : 0) >
			           a.Height);

			if (!vbar_v) {
				vbar_r = Requisition.Zero;
			} else {
				/**
				 * the horizontal space used to show the vbar may now
				 * cause the hbar to be needed
				 **/
				hbar_v = hbar_policy == PolicyType.Always ||
				         (hbar_policy == PolicyType.Automatic &&
				          LeftWidth + FieldWidth + BorderWidth + vbar_r.Width >
				          a.Width);
			}

			if (!hbar_v) {
				hbar_r = Requisition.Zero;
			}

			hbar.Visible = hbar_v;
			vbar.Visible = vbar_v;

			Rectangle wa;

			wa = new Rectangle (a.X, a.Y, LeftWidth, TopHeight);
			cda.Allocation = wa;
			cda.QueueDraw ();

			wa = new Rectangle (cda.Allocation.Right, a.Y,
			                    a.Width - LeftWidth -
			                    (int)BorderWidth -
			                    (vbar_span != ScrollbarSpan.HeaderOverlap ?
			                     vbar_r.Width : 0), TopHeight);
			tvp.Allocation = wa;
			tvp.QueueDraw ();

			wa = new Rectangle (a.X, cda.Allocation.Bottom,
			                    LeftWidth,
			                    a.Height - TopHeight -
			                    (int)BorderWidth -
			                    (hbar_span != ScrollbarSpan.HeaderOverlap ?
			                     hbar_r.Height : 0));
			lvp.Allocation = wa;
			lvp.QueueDraw ();

			wa = new Rectangle (a.X + LeftWidth, a.Y + TopHeight,
			                    a.Width - LeftWidth - (int)BorderWidth -
			                    vbar_r.Width,
			                    a.Height - TopHeight - (int)BorderWidth -
			                    hbar_r.Height);
			fvp.Allocation = wa;
			fvp.QueueDraw ();

			int gap;

			if (hbar.Visible) {
				gap = hbar_span == ScrollbarSpan.Full ? 0 : LeftWidth;
				wa = new Rectangle (a.X + gap, a.Bottom - hbar_r.Height,
				                    a.Width - gap - vbar_r.Width,
				                    hbar_r.Height);
				hbar.Allocation = wa;
			}

			if (vbar.Visible) {
				gap = vbar_span == ScrollbarSpan.Full ? 0 : TopHeight;
				wa = new Rectangle (a.Right - vbar_r.Width, a.Y + gap,
				                    vbar_r.Width,
				                    a.Height - gap - hbar_r.Height);
				vbar.Allocation = wa;
			}

		}

		private int XSpan
		{
			get {
				return orientation == Orientation.Vertical ? r_span : c_span;
			}
		}

		private int YSpan
		{
			get {
				return orientation == Orientation.Vertical ? c_span : r_span;
			}
		}

		private int LeftWidth
		{
			get {
				int w = 0;

				for (int x = 0; x < XSpan && x < widths.Count; x++) {
					w += (int)widths[x];
				}

				return w;
			}
		}

		private int TopHeight
		{
			get {
				int h = 0;

				for (int y = 0; y < YSpan && y < heights.Count; y++) {
					h += (int)heights[y];
				}

				return h;
			}
		}

		private int FieldWidth
		{
			get {
				int w = 0;

				for (int x = XSpan; x < widths.Count; x++) {
					w += (int)widths[x];
				}

				return w;
			}
		}

		private int FieldHeight
		{
			get {
				int h = 0;

				for (int y = YSpan; y < heights.Count; y++) {
					h += (int)heights[y];
				}

				return h;
			}
		}

		private void OnModelRowChanged (object o, RowChangedArgs a)
		{
			foreach (GridViewColumn col in visible) {
				// We don't just use the a.Path directly,
				// as for some bloody reason the a.Path.Depth
				// gets corrupted on win32 randomly resulting 
				// in depths of 10923461, and things go boom.
				TreePath p = model.GetPath (a.Iter);
				MeasureCell (p, col);
				InvalidateCellRect (p, col);
			}

			UpdateDrawingAreaSizeRequests ();
		}

		private void OnModelRowDeleted (object o, RowDeletedArgs a)
		{
			if (orientation == Orientation.Vertical) {
				heights.RemoveAt (a.Path.Indices[0]);
			} else {
				widths.RemoveAt (a.Path.Indices[0]);
			}

			UpdateDrawingAreaSizeRequests ();
		}

		private void OnModelRowInserted (object o, RowInsertedArgs a)
		{
			if (orientation == Orientation.Vertical) {
				heights.Insert (a.Path.Indices[0], 0);
			} else {
				widths.Insert (a.Path.Indices[0], 0);
			}
		}

		private void OnModelRowsReordered (object o, RowsReorderedArgs a)
		{
			// XXX
		}

		private void OnStyleSet (object o, StyleSetArgs a)
		{
			RebuildDimensions ();
		}

		private void OnStateChanged (object o, StateChangedArgs a)
		{
			Rectangle ir;

			ir = new Rectangle (0, 0,
			                    cda.Allocation.Width, cda.Allocation.Height);
			cda.GdkWindow.InvalidateRect (ir, true);

			ir = new Rectangle (0, 0,
			                    tda.Allocation.Width, tda.Allocation.Height);
			tda.GdkWindow.InvalidateRect (ir, true);

			ir = new Rectangle (0, 0,
			                    lda.Allocation.Width, lda.Allocation.Height);
			lda.GdkWindow.InvalidateRect (ir, true);

			ir = new Rectangle (0, 0,
			                    fda.Allocation.Width, fda.Allocation.Height);
			fda.GdkWindow.InvalidateRect (ir, true);
		}

		private void OnFocusInEvent (object o, FocusInEventArgs a)
		{
			TreePath spath;
			GridViewColumn scol;

			if (selection.GetSelected (out spath, out scol)) {
				InvalidateCellRect (spath, scol);
			}
		}

		private void OnFocusOutEvent (object o, FocusOutEventArgs a)
		{
			TreePath spath;
			GridViewColumn scol;

			if (selection.GetSelected (out spath, out scol)) {
				InvalidateCellRect (spath, scol);
			}
		}

		private void OnButtonPressEvent (object o, ButtonPressEventArgs a)
		{
			GrabFocus ();
		}

		private void OnKeyPressEvent (object o, KeyPressEventArgs a)
		{
			TreePath spath;
			GridViewColumn scol;

			if (!selection.GetSelected (out spath, out scol)) {
				return;
			}

			int x, y;

			TranslateCoords (spath, scol, out x, out y);

			switch (a.Event.Key) {
				case Gdk.Key.Up:
					y -= 1;
					break;
				case Gdk.Key.Down:
					y += 1;
					break;
				case Gdk.Key.Left:
					x -= 1;
					break;
				case Gdk.Key.Right:
					x += 1;
					break;
				default:
					a.RetVal = false;
					return;
			}

			a.RetVal = true;

			if (TranslateCoords (x, y, out spath, out scol) &&
			    selection.SelectCell (spath, scol))
			{
				ScrollToCell (spath, scol, false, 0, 0);
			}
		}

		private void OnSelectionChanged (object o, EventArgs a)
		{
			if (model != null && GetSelectable (prev_sel_path, prev_sel_col)) {
				InvalidateCellRect (prev_sel_path, prev_sel_col);
			}

			TreePath spath;
			GridViewColumn scol;

			if (selection.GetSelected (out spath, out scol)) {
				InvalidateCellRect (spath, scol);
			}

			prev_sel_path = spath;
			prev_sel_col = scol;
		}

		private void OnCDAExposeEvent (object o, ExposeEventArgs a)
		{
			if (model == null) {
				return;
			}

			GridViewColumn col;
			TreePath path;

			for (int x = 0; x < XSpan; x++) {
				for (int y = 0; y < YSpan; y++) {
					if (TranslateCoords (x, y, out path, out col)) {
						DrawCell (path, col, a.Event.Area);
					}
				}
			}
		}

		private void OnTDAExposeEvent (object o, ExposeEventArgs a)
		{
			if (model == null) {
				return;
			}

			GridViewColumn col;
			TreePath path;

			for (int x = XSpan; x < widths.Count; x++) {
				for (int y = 0; y < YSpan; y++) {
					if (TranslateCoords (x, y, out path, out col)) {
						DrawCell (path, col, a.Event.Area);
					}
				}
			}
		}

		private void OnLDAExposeEvent (object o, ExposeEventArgs a)
		{
			if (model == null) {
				return;
			}

			GridViewColumn col;
			TreePath path;

			for (int x = 0; x < XSpan; x++) {
				for (int y = YSpan; y < heights.Count; y++) {
					if (TranslateCoords (x, y, out path, out col)) {
						DrawCell (path, col, a.Event.Area);
					}
				}
			}
		}

		private void OnFDAExposeEvent (object o, ExposeEventArgs a)
		{
			if (model == null) {
				return;
			}

			GridViewColumn col;
			TreePath path;

			for (int x = XSpan; x < widths.Count; x++) {
				for (int y = YSpan; y < heights.Count; y++) {
					if (TranslateCoords (x, y, out path, out col)) {
						DrawCell (path, col, a.Event.Area);
					}
				}
			}
		}

		private void OnYHeaderMotionNotifyEvent (object o,
		                                         MotionNotifyEventArgs a)
		{
			int x = (int)a.Event.X;

			if (o == tda) {
				x += LeftWidth;
			}

			(o as Widget).GdkWindow.Cursor =
				drag_col >= 0 || DragX (x) >= 0 ?
				new Cursor (CursorType.SbHDoubleArrow) : null;

			if (drag_col >= 0 && x > ColumnX (drag_col) + 12) {
				int prev_width = (int)widths[drag_col];
				widths[drag_col] = x - ColumnX (drag_col);

				/**
				 * make sure that we don't cause the grid's size request
				 * to increase
				 **/
				if (drag_col <= XSpan &&
				    ColumnX (XSpan) + (int)widths[XSpan] +
				    (XSpan + 1 < widths.Count ? (int)widths[XSpan + 1] : 0) +
				    BorderWidth +
				    (vbar.Visible ? vbar.SizeRequest ().Width : 0) + 6 >=
				    Allocation.Width)
				{
					widths[drag_col] = prev_width;
					return;
				}

				UpdateDrawingAreaSizeRequests ();
			}
		}

		private void OnYHeaderButtonPressEvent (object o,
		                                        ButtonPressEventArgs a)
		{
			int x = (int)a.Event.X;

			if (o == tda) {
				x += LeftWidth;
			}

			drag_col = DragX (x);
			
			// make the headers eat clicks, so that when you
			// override ButtonPress in classes using GridView, you
			// don't get spurious clicks.
			a.RetVal = true;
		}

		private void OnYHeaderButtonReleaseEvent (object o,
		                                          ButtonReleaseEventArgs a)
		{
			drag_col = -1;
		}

		private void OnFDAStyleSet (object o, StyleSetArgs a)
		{
			if (fda.IsRealized) {
				fda.GdkWindow.Background = fda.Style.BaseColors[(int)fda.State];
			}
		}

		private void OnFDARealized (object o, EventArgs a)
		{
			fda.GdkWindow.Background = fda.Style.BaseColors[(int)fda.State];
		}

		private void OnFDAButtonPressEvent (object o, ButtonPressEventArgs a)
		{
			if (a.Event.Y > FieldHeight) {
				return;
			}

			int x = XSpan, y = YSpan;

			while (x < widths.Count &&
			       (int)a.Event.X > ColumnX (x) - LeftWidth)
			{
				x++;
			}
			while ((int)a.Event.Y > RowY (y) - TopHeight) {
				y++;
			}

			GridViewColumn col, scol;
			TreePath path, spath;

			TranslateCoords (x - 1, y - 1, out path, out col);
			selection.GetSelected (out spath, out scol);

			if (spath != null && path.Compare (spath) == 0 && col == scol &&
			    (a.Event.State & ModifierType.ControlMask) == ModifierType.ControlMask)
			{
				selection.Deselect ();
			} else {
				selection.SelectCell (path, col);
				ScrollToCell (path, col, false, 0, 0);
			}
		}

		// make the headers eat clicks, so that when you override
		// ButtonPress in classes using GridView, you don't get
		// spurious clicks.
		private void OnLDAButtonPressEvent (object o, ButtonPressEventArgs a)
		{
			a.RetVal = true;
		}

		private void OnColumnVisibilityChanged (object o, EventArgs a)
		{
			int vc;
			GridViewColumn changed_col = o as GridViewColumn;
			ArrayList dimensions = orientation == Orientation.Vertical ?
			                       widths : heights;

			if (changed_col.Visible) {
				visible.Clear ();

				foreach (GridViewColumn col in columns) {
					if (col.Visible) {
						visible.Add (col);
					}
				}

				vc = visible.IndexOf (changed_col);
				dimensions.Insert (vc, 0);

				TreeIter i;

				if (model != null && model.GetIterFirst (out i)) {
					do {
						TreePath path = model.GetPath (i);
						MeasureCell (path, changed_col);
					} while (model.IterNext (ref i));
				}
			} else {
				vc = visible.IndexOf (changed_col);

				if (dimensions.Count > vc) { 
					dimensions.RemoveAt (vc);
				}

				visible.Remove (changed_col);
			}

			UpdateDrawingAreaSizeRequests ();
		}

		private void TranslateCoords (TreePath path, GridViewColumn col,
		                              out int x, out int y)
		{
			if (orientation == Orientation.Vertical) {
				x = visible.IndexOf (col);
				y = path.Indices[0];
			} else {
				x = path.Indices[0];
				y = visible.IndexOf (col);
			}
		}

		private bool TranslateCoords (int x, int y,
		                              out TreePath path, out GridViewColumn col)
		{
			int c = orientation == Orientation.Vertical ? x : y;
			int r = orientation == Orientation.Vertical ? y : x;
			TreeIter i;

			if (c >= visible.Count || !model.IterNthChild (out i, r)) {
				col = null;
				path = null;

				return false;
			} else {
				col = visible[c] as GridViewColumn;
				path = new TreePath (r.ToString ());

				return true;
			}
		}
		
		private void UpdateDrawingAreaSizeRequests ()
		{
			cda.SetSizeRequest (LeftWidth, TopHeight);
			tda.SetSizeRequest (FieldWidth, TopHeight);
			lda.SetSizeRequest (LeftWidth, FieldHeight);
			fda.SetSizeRequest (FieldWidth, FieldHeight);
		}

		private void MeasureCell (TreePath path, GridViewColumn col)
		{
			int w, h, x, y, x_offset, y_offset;
			TreeIter i;
			bool header = path.Indices[0] < c_span;
			Rectangle rect = new Rectangle ();
			CellRenderer renderer = header && col.HeaderRenderer != null ?
			                        col.HeaderRenderer : col.FieldRenderer;

			model.GetIter (out i, path);
			col.CellSetCellData (model, i, header);
			renderer.GetSize (this, ref rect, out x_offset, out y_offset,
			                  out w, out h);
			TranslateCoords (path, col, out x, out y);

			widths[x] = Math.Max ((int)widths[x], w + 20);
			heights[y] = Math.Max ((int)heights[y], h + 4);
		}

		private void RebuildDimensions ()
		{
			widths.Clear ();
			heights.Clear ();

			int x, y;
			TreeIter i;
			TreePath path;

			if (model != null && model.GetIterFirst (out i)) {
				do {
					foreach (GridViewColumn col in visible) {
						path = model.GetPath (i);
						TranslateCoords (path, col, out x, out y);

						if (x == widths.Count) {
							widths.Add (0);
						}
						if (y == heights.Count) {
							heights.Add (0);
						}

						MeasureCell (path, col);
					}
				} while (model.IterNext (ref i));
			}

			UpdateDrawingAreaSizeRequests ();
		}

		private Rectangle CellRect (TreePath path, GridViewColumn col,
		                            out Widget widget)
		{
			int x, y;
			TranslateCoords (path, col, out x, out y);

			Rectangle rect = new Rectangle (ColumnX (x), RowY (y),
			                                (int)widths[x], (int)heights[y]);

			if (x < XSpan && y < YSpan) {
				widget = cda;
				return rect;
			} else if (x < XSpan) {
				widget = lda;
				rect.Y -= TopHeight;
			} else if (y < YSpan) {
				widget = tda;
				rect.X -= LeftWidth;
			} else {
				widget = fda;
				rect.X -= LeftWidth;
				rect.Y -= TopHeight;
			}

			/* stretch the rightmost cell to fill the Allocation */
			if (x == widths.Count - 1) {
				rect.Width = fda.Allocation.Right - rect.X;
			}

			return rect;
		}

		private void InvalidateCellRect (TreePath path, GridViewColumn col) {
			Widget widget;
			Rectangle rect = CellRect (path, col, out widget);

			if (widget != null && widget.GdkWindow != null) {
				widget.GdkWindow.InvalidateRect (rect, true);
			}
		}

		private int ColumnX (int x)
		{
			int ret = 0;
			for (--x; x >= 0 && x < widths.Count; x--) {
				ret += (int)widths[x];
			}

			return ret;
		}

		private int RowY (int y)
		{
			int ret = 0;
			for (--y; y >= 0 && y < heights.Count; y--) {
				ret += (int)heights[y];
			}

			return ret;
		}

		private int DragX (int x)
		{
			for (int xc = 1; xc < widths.Count; xc++) {
				int distance = x - ColumnX (xc);

				if (distance > -3 && distance < 3 &&
				    (xc != XSpan || distance < 0))
				{
					return xc - 1;
				}
			}

			return -1;
		}

		private void DrawCell (TreePath path, GridViewColumn col,
		                       Rectangle clip)
		{
			Widget w;

			Rectangle rect = CellRect (path, col, out w);
			if (!clip.Intersect (rect, out clip)) {
				return;
			}

			CellRendererState crs = 0;

			if (w != fda) {
				w.GdkWindow.DrawRectangle (w.Style.MidGC (w.State), true,
				                           rect);
			} else {
				TreePath spath;
				GridViewColumn scol;
				selection.GetSelected (out spath, out scol);

				if (spath != null && path.Compare (spath) == 0 && col == scol) {
					crs = CellRendererState.Selected;
					Style.PaintFlatBox (w.Style, w.GdkWindow,
					                    StateType.Selected, ShadowType.None,
					                    clip, this, "cell_odd", rect.X, rect.Y,
					                    rect.Width, rect.Height);
				}
			}

			w.GdkWindow.DrawLine (w.Style.BackgroundGC (w.State),
			                      rect.X, rect.Bottom - 1,
			                      rect.Right, rect.Bottom - 1);
			w.GdkWindow.DrawLine (w.Style.BackgroundGC (w.State),
			                      rect.Right - 1, rect.Y,
			                      rect.Right - 1, rect.Bottom);

			TreeIter i;
			bool header = path.Indices[0] < c_span;
			CellRenderer renderer = header && col.HeaderRenderer != null ?
			                        col.HeaderRenderer : col.FieldRenderer;
			model.GetIter (out i, path);
			col.CellSetCellData (model, i, header);
			renderer.Render (w.GdkWindow, this, rect, rect, clip, crs);
		}
	}
}
