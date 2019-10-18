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

/*
 * TODO: performance (redrawing just a cell)
 * TODO: rubberbanding + scroll
 * TODO: implement use_align = false;
 */

using Gtk;
using System;
using System.Collections;

namespace Medsphere.Widgets
{
	public delegate void CairoCellRendererDataFunc (IconLayout view, ICairoCellRenderer renderer, TreeModel model, TreeIter iter);

	public enum LayoutMode
	{
		Horizontal,
		Vertical,
		Grid
	}

	public enum LayoutAffinity
	{
		Horizontal,
		Vertical
	}

	public class IconLayout : DrawingArea
	{
#region public events
		public event EventHandler SelectionChanged;
		public event ItemActivatedHandler ItemActivated;
#endregion

#region public properties
		public TreeModel Model {
			get { return model; }
			set {
				if (model != null) {
					model.RowChanged -= new RowChangedHandler (OnDataRowChanged);
					model.RowDeleted -= new RowDeletedHandler (OnDataRowDeleted);
					model.RowInserted -= new RowInsertedHandler (OnDataRowInserted);
					model.RowsReordered -= new RowsReorderedHandler (OnDataRowsReordered);
				}

				model = value;

				if (model != null) {
					model.RowChanged += new RowChangedHandler (OnDataRowChanged);
					model.RowDeleted += new RowDeletedHandler (OnDataRowDeleted);
					model.RowInserted += new RowInsertedHandler (OnDataRowInserted);
					model.RowsReordered += new RowsReorderedHandler (OnDataRowsReordered);
				}

				QueueResize ();
			}
		}

		public ICairoCellRenderer CellRenderer {
			get { return renderer; }
			set { 
				renderer = value;
				QueueResize ();
			}
		}

		public LayoutMode LayoutMode {
			get { return layout_mode; }
			set {
				layout_mode = value;
				QueueResize ();
			}
		}

		public LayoutAffinity LayoutAffinity {
			get { return affinity; }
			set {
				affinity = value;
				QueueResize ();
			}
		}

		public SelectionMode SelectionMode {
			get { return selection_mode; }
			set {
				selection_mode = value;

				switch (selection_mode) {
				case SelectionMode.None:
					focused_path = null;
					UnselectAll ();
					break;
				case SelectionMode.Browse:
				case SelectionMode.Single:
					SelectPath (selection_anchor);
					break;
				}
			}
		}

		public int CellWidth {
			get { return cell_size.Width; }
		}

		public int CellHeight {
			get { return cell_size.Height; }
		}

		public int Padding {
			get { return padding; }
			set {
				padding = value;
				QueueResize ();
			}
		}

		public CairoCellRendererDataFunc CellDataFunc {
			get { return data_func; }
			set {
				data_func = value;
				QueueResize ();
			}
		}
			
		public Adjustment Hadjustment {
			set { hadj = value; }
			get { return hadj; }
		}
	
		public Adjustment Vadjustment {
			set { vadj = value; }
			get { return vadj; }
		}

		public TreePath[] SelectedItems {
			get { return (TreePath[])selected_paths.ToArray (typeof (TreePath)); }
		}
#endregion

#region public methods
		public IconLayout ()
		{
		}

		public IconLayout (TreeModel model) : base ()
		{
			this.Model = model;
		}

		public void SetTileSizeOverride (int max_width, int max_height)
		{
			this.override_width = max_width;
			this.override_height = max_width;

			QueueResize ();
		}

		public void SelectAll ()
		{
			if (selection_mode == SelectionMode.None) {
				return;
			}

			selected_paths.Clear ();
			for (int i = 0; i < n_cells; i++)
			{
				selected_paths.Add (new TreePath (new int[] { i }));
			}

			if (SelectionChanged != null) {
				SelectionChanged (this, new EventArgs ());
			}

			QueueDraw ();
		}

		public void UnselectAll ()
		{
			if (selection_mode == SelectionMode.None) {
				return;
			}

			selected_paths.Clear ();
			if (SelectionChanged != null) {
				SelectionChanged (this, new EventArgs ());
			}

			QueueDraw ();
		}

		public void SelectPath (TreePath path)
		{
			if (path == null) {
				return;
			}

			if (selection_mode == SelectionMode.None) {
				return;
			}

			if (selected_paths.Contains (path)) {
				return;
			}

			if (selection_mode == SelectionMode.Single
			    || selection_mode == SelectionMode.Browse) {
				selected_paths.Clear ();
			}

			if (selection_mode == SelectionMode.Multiple
			    || selection_mode == SelectionMode.Browse) {
				focused_path = path;
			}
			 
			selection_anchor = path;
			selected_paths.Add (path);

			if (SelectionChanged != null) {
				SelectionChanged (this, new EventArgs ());
			}

			QueueDraw ();
		}

		public void UnselectPath (TreePath path)
		{
			if (path == null) {
				return;
			}

			if (selection_mode == SelectionMode.None) {
				return;
			}

			int index = selected_paths.IndexOf (path);
			if (index < 0) {
				return;
			}

			selected_paths.RemoveAt (index);
			
			if (selection_anchor == path) {
				selection_anchor = null;
			}

			if (SelectionChanged != null) {
				SelectionChanged (this, new EventArgs ());
			}

			QueueDraw ();
		}

		public TreePath GetPathAtPos (int x, int y)
		{
			if (hadj != null) {
				x += (int)hadj.Value;
			}

			if (vadj != null) {
				y += (int)vadj.Value;
			}
			
			int r, c;
			GetRowAndColAtCoords (x, y, out r, out c);
			
			TreePath path;
			if (!GetPathAtRowAndCol (r, c, out path)) {
				return null;
			}

			return path;
		}

		public bool PathIsSelected (TreePath path)
		{
			return selected_paths.Contains (path);
		}

		public void ScrollToPath (TreePath path)
		{
			ScrollToPath (path, false, 0f, 0f);
		}

		public void ScrollToPath (TreePath path, float row_align, float col_align)
		{
			ScrollToPath (path, true, row_align, col_align);
		}
#endregion

#region protected methods
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
			AddEvents ((int)Gdk.EventMask.FocusChangeMask);
		}

		protected override void OnSizeRequested (ref Gtk.Requisition req)
		{
			cell_size = CalculateMaxCellSize ();
			
			// request at least 1 cell size
			req.Height = cell_size.Height + (padding * 2);
			req.Width = cell_size.Width + (padding * 2);
		}

		protected override void OnSizeAllocated (Gdk.Rectangle rect)
		{
			base.OnSizeAllocated (rect);

			// calculate how many rows and columns we want
			rows = cols = 0;
			if (model == null) {
				ResetAdjustments ();
				return;
			}

			n_cells = model.IterNChildren ();
			if (n_cells == 0) {
				ResetAdjustments ();
				return;
			}
			
			// find out how may cells per row we can fit in our
			// allocation
			switch (layout_mode) {
			case LayoutMode.Horizontal:
				cols = n_cells;
				rows = 1;
				break;
			case LayoutMode.Vertical:
				cols = 1;
				rows = n_cells;
				break;
			case LayoutMode.Grid:
				int max_rows = (int)(Allocation.Height / (cell_size.Height + padding));
				max_rows = (max_rows <= 0) ? 1 : max_rows;

				int max_cols = (int)(Allocation.Width / (cell_size.Width + padding));
				max_cols = (max_cols <= 0) ? 1 : max_cols;

				if (affinity == LayoutAffinity.Horizontal) {
					cols = Math.Max ((int)Math.Ceiling ((double)n_cells / max_rows), max_cols);
				} else {
					cols = Math.Min (max_cols, n_cells);
				}

				rows = (int)Math.Ceiling (n_cells / (double)cols);
				break;
			}

			if (hadj != null) {
				hadj.Upper = padding + cols * (cell_size.Width + padding);
				hadj.StepIncrement = 25;
				hadj.PageSize = Allocation.Width;
				hadj.Change ();
			}

			if (vadj != null) {
				vadj.Upper = padding + rows * (cell_size.Height + padding);
				vadj.StepIncrement = 25;
				vadj.PageSize = Allocation.Height;
				vadj.Change ();
			}

			if (hadj != null && hadj.Value > hadj.Upper - hadj.PageSize) {
				hadj.Value = Math.Max (0, hadj.Upper - hadj.PageSize);
				hadj.ChangeValue ();
			}

			if (vadj != null && vadj.Value > vadj.Upper - vadj.PageSize) {
				vadj.Value = Math.Max (0, vadj.Upper - vadj.PageSize);
				vadj.ChangeValue ();
			}

			if (scroll_to_path != null) {
				ScrollToPath (scroll_to_path, scroll_to_use_align,
				              scroll_to_row_align, scroll_to_col_align);
				scroll_to_path = null;
			}
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

		protected override void OnSetScrollAdjustments (Adjustment h, Adjustment v)
		{
			if (hadj != null) {
				hadj.ValueChanged -= new EventHandler (OnAdjustmentChanged);
			}

			if (vadj != null) {
				vadj.ValueChanged -= new EventHandler (OnAdjustmentChanged);
			}

			hadj = h;
			if (hadj != null) {
				hadj.ValueChanged += new EventHandler (OnAdjustmentChanged);
			}

			vadj = v;
			if (vadj != null) {
				vadj.ValueChanged += new EventHandler (OnAdjustmentChanged);
			}
		}

		protected override bool OnFocused (DirectionType dir)
		{
			base.OnFocused (dir);

			if (dir == DirectionType.TabForward) {
				// let focus jump to the next widget
				return false;
			}

			if (dir == DirectionType.TabBackward
			    || focused_path == null) {
				GrabFocus ();
				return true;
			}

			if (!HasFocus) {
				GrabFocus ();
			}

			return true;
		}

		protected override void OnFocusGrabbed ()
		{
			base.OnFocusGrabbed ();

			if (model == null) {
				return;
			}

			// don't support focus when we aren't
			// allowing selection
			if (selection_mode == SelectionMode.None
			    || focused_path != null) {
				QueueDraw ();
				return;
			}

			TreeIter iter;
			if (!model.IterNthChild (out iter, 0)) {
				return;
			}

			focused_path = model.GetPath (iter);

			if (selection_mode == SelectionMode.Browse) {
				SelectPath (focused_path);
			}

			// TODO: Constrain this to only the previous focus and
			// the current focus
			QueueDraw ();
		}
		
		protected override bool OnFocusOutEvent (Gdk.EventFocus focus)
		{
			// TODO: redraw only the focused cell
			QueueDraw ();
			return true;
		}

		protected override bool OnKeyPressEvent (Gdk.EventKey ev)
		{
			if (model == null) {
				return false;
			}

			int r, c;
			TreePath tmp;

			// Lame workaround for GtkBinding not being bound
			switch (ev.Key) {
			// Activate keycodes
			case Gdk.Key.space:
			case Gdk.Key.Return:
			case Gdk.Key.ISO_Enter:
			case Gdk.Key.KP_Enter:
				// Remove this when we can use OnActivate ()
				if (focused_path == null) {
					return false;
				}

				if ((ev.State & Gdk.ModifierType.ControlMask) == 0) {
					UnselectAll ();
				}

				if (selected_paths.Contains (focused_path)) {
					UnselectPath (focused_path);
				} else {
					SelectPath (focused_path);
				}

				if (ItemActivated != null) {
					ItemActivatedArgs activated_args = new ItemActivatedArgs ();
					activated_args.Args = new object[] { focused_path };
					ItemActivated (this, activated_args);
				}

				return true;
			case Gdk.Key.a:
				// if control down, select all
				if (selection_mode == SelectionMode.Multiple
				    && (ev.State & Gdk.ModifierType.ControlMask) > 0) {
					SelectAll ();
				}
				return true;
			case Gdk.Key.A:
				// if control down and shift down, unselect all
				if (selection_mode == SelectionMode.Multiple
				    && (ev.State & Gdk.ModifierType.ControlMask) > 0
				    && (ev.State & Gdk.ModifierType.ShiftMask) > 0) {
					UnselectAll ();
				}
				return true;
			case Gdk.Key.Up:
			case Gdk.Key.KP_Up:
				// Move focus or selection up
				if (layout_mode == LayoutMode.Vertical
				    || layout_mode == LayoutMode.Grid) {
					// find out the currently focused r and c
					GetRowAndColForPath (focused_path, out r, out c);
					
					// decrement the row by 1
					if (r > 0) {
						r--;
						
						// find the path at new r, c
						if (GetPathAtRowAndCol (r, c, out tmp)) {
							focused_path = tmp;
						}
					}
				}
				break;
			case Gdk.Key.Down:
			case Gdk.Key.KP_Down:
				// move down
				if (layout_mode == LayoutMode.Vertical
				    || layout_mode == LayoutMode.Grid) {
					// find out the currently focused r and c
					GetRowAndColForPath (focused_path, out r, out c);
					
					// increment the row by 1
					r++;
					
					// find the path at new r, c
					if (GetPathAtRowAndCol (r, c, out tmp)) {
						focused_path = tmp;
					}
				}
				break;
			case Gdk.Key.Left:
			case Gdk.Key.KP_Left:
				// move left
				if (layout_mode == LayoutMode.Horizontal
				    || layout_mode == LayoutMode.Grid) {
					tmp = focused_path.Copy ();

					// don't wrap around
					if (tmp.Prev ()) {
						focused_path = tmp;
					}
				}
				break;
			case Gdk.Key.Right:
			case Gdk.Key.KP_Right:
				// move right
				if (layout_mode == LayoutMode.Horizontal
				    || layout_mode == LayoutMode.Grid) {
					tmp = focused_path.Copy ();
					tmp.Next ();

					if (PathIsValid (tmp)) {
						focused_path = tmp;
					}
				}
				break;
			case Gdk.Key.Home:
			case Gdk.Key.KP_Home:
				// select and focus the first item, dropping
				// current selection
				tmp = TreePath.NewFirst ();
				
				// verify that the path is valid
				if (PathIsValid (tmp)) {
					selected_paths.Clear ();
					focused_path = tmp;
					SelectPath (focused_path);
				}
				return true;
			case Gdk.Key.End:
			case Gdk.Key.KP_End:
				// select and focus the first item, dropping
				// current selection
				tmp = new TreePath (new int[] { n_cells - 1 });
				
				// verify that the path is valid
				if (PathIsValid (tmp)) {
					selected_paths.Clear ();
					focused_path = tmp;
					SelectPath (focused_path);
				}
				return true;
			}

			if (selection_mode == SelectionMode.Multiple
			    && (ev.State & Gdk.ModifierType.ShiftMask) > 0) {
				selected_paths.Clear ();
				SelectAllBetween (selection_anchor, focused_path);
				return true;
			}

			if (selection_mode == SelectionMode.Browse) {
				SelectPath (focused_path);
			} else {
				// TODO: Constrain this to only the previous focus and
				QueueDraw ();
			}

			return true;
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton ev)
		{
			base.OnButtonPressEvent (ev);
			
			if (ev.Button != 1) {
				return false;
			}

			if (model == null) {
				return false;
			}

			if (!HasFocus) {
				GrabFocus ();
			}

			// adjust to total canvas coordinates
			int x = (int)(ev.X + hadj.Value);
			int y = (int)(ev.Y + vadj.Value);

			if (x <= padding || y <= padding) {
				return false;
			}

			if (cell_size == Gdk.Rectangle.Zero) {
				// we must be empty, or not have been size
				// requested yet
				return false;
			}
			
			int r, c;
			bool hit;
			hit = GetRowAndColAtCoords (x, y, out r, out c);

			TreePath path;
			if (!hit || !GetPathAtRowAndCol (r, c, out path)) {
				if (selection_mode == SelectionMode.Multiple) {
					// Rubberbanding
					sel_start.X = x;
					sel_start.Y = y;

					sel_rect.X = sel_start.X;
					sel_rect.Y = sel_start.Y;
					sel_rect.Width = sel_rect.Height = 0;

					have_rubberband_selection = true;
					
					// Save a copy of the selected_paths so
					// we know what we started with when we
					// start rubberbanding
					pre_rubberbanded_selection = (ArrayList)selected_paths.Clone ();

					return true;
				}
				
				if ((ev.State & Gdk.ModifierType.ControlMask) > 0) {
					// If we have control pressed down,
					// don't punish the user by killing
					// their selection
					return false;
				}
				
				UnselectAll ();
				return false;
			}

			if (selection_mode == SelectionMode.Multiple
			    && (ev.State & Gdk.ModifierType.ShiftMask) > 0) {
				if (selection_anchor == null) {
					selection_anchor = path;
				} else {
					UnselectAll ();
					SelectAllBetween (selection_anchor, path);
					return true;
				}
			} else {
				// do we need to deselect?
				if ((ev.State & Gdk.ModifierType.ControlMask) > 0
				    && (selection_mode == SelectionMode.Multiple
				        || selection_mode == SelectionMode.Single)) {
					if (selected_paths.Contains (path)) {
						UnselectPath (path);
						return true;
					}
				}
			}

			if (selection_mode == SelectionMode.Multiple
			    && !selected_paths.Contains (path)
			    && (ev.State & Gdk.ModifierType.ControlMask) == 0) {
				UnselectAll ();
			}

			if (ev.Type == Gdk.EventType.TwoButtonPress
			    && ItemActivated != null) {
				ItemActivatedArgs activated_args = new ItemActivatedArgs ();
				activated_args.Args = new object[] { path };
				ItemActivated (this, activated_args);
			}

			// SelectPath will handle multiple selection vs browse
			// vs single for us
			SelectPath (path);

			return true;
		}

		protected override bool OnMotionNotifyEvent (Gdk.EventMotion ev)
		{
			if (!have_rubberband_selection) {
				return false;
			}

			int x, y;
			Gdk.ModifierType state;
			if (ev.IsHint) {
				GdkWindow.GetPointer (out x, out y, out state);
			} else {
				x = (int)ev.X;
				y = (int)ev.Y;
				state = ev.State;
			}

			if (hadj != null) {
				x += (int)hadj.Value;
			}

			if (vadj != null) {
				y += (int)vadj.Value;
			}

			sel_rect.X = Math.Min (sel_start.X, x);
			sel_rect.Y = Math.Min (sel_start.Y, y);
			sel_rect.Width = Math.Abs (sel_start.X - x);
			sel_rect.Height = Math.Abs (sel_start.Y - y);

			ArrayList tmp;
			if ((ev.State & Gdk.ModifierType.ControlMask) > 0) {
				tmp = (ArrayList)pre_rubberbanded_selection.Clone ();
			} else {
				tmp = new ArrayList ();
			}

			for (int i = 0; i < n_cells; i++)
			{
				GetCellPosition (i, out x, out y);

				cell_size.X = x;
				cell_size.Y = y;

				if (!cell_size.IntersectsWith (sel_rect)) {
					continue;
				}
				
				TreePath new_path = new TreePath (new int[] { i });
				if ((ev.State & Gdk.ModifierType.ControlMask) > 0) {
					// If control is down and we highlight
					// something that was selected before we
					// started rubberbanding, deselect it
					int index = tmp.IndexOf (new_path);
					if (index >= 0) {
						tmp.RemoveAt (index);
						continue;
					}
				}

				tmp.Add (new_path);
			}

			// find out if selection has changed since last time
			bool dirty = false;
			if (selected_paths.Count != tmp.Count) {
				dirty = true;
			} else {
				for (int i = 0; i < selected_paths.Count; i++)
				{
					if (!((TreePath)selected_paths[i]).Equals ((TreePath)tmp[i])) {
						dirty = true;
						break;
					}
				}
			}

			selected_paths = tmp;

			if (dirty && SelectionChanged != null) {
				SelectionChanged (this, new EventArgs ());
			}

			QueueDraw ();
			
			return true;
		}

		protected override bool OnButtonReleaseEvent (Gdk.EventButton ev)
		{
			base.OnButtonReleaseEvent (ev);

			if (ev.Button == 1 && have_rubberband_selection) {
				have_rubberband_selection = false;

				if (sel_rect.Width == 0 && sel_rect.Height == 0
				    && (ev.State & Gdk.ModifierType.ControlMask) == 0) {
					UnselectAll ();
				}

				QueueDraw ();
			}

			return true;
		}
#endregion

#region private fields
		private TreeModel model;
		private ICairoCellRenderer renderer;
		private LayoutMode layout_mode = LayoutMode.Grid;
		private LayoutAffinity affinity = LayoutAffinity.Horizontal;
		private CairoCellRendererDataFunc data_func;
		private int padding = 8;

		private Gdk.Rectangle cell_size = Gdk.Rectangle.Zero;
		private int override_width = -1;
		private int override_height = -1;

		private int rows = 0, cols = 0, n_cells = 0;

		private Adjustment hadj, vadj = null;

		private SelectionMode selection_mode = SelectionMode.Single;
		private ArrayList selected_paths = new ArrayList ();
		private TreePath selection_anchor;
		private TreePath focused_path;

		// rubberbanding
		private bool have_rubberband_selection = false;
		private Gdk.Rectangle sel_start = new Gdk.Rectangle ();
		private Gdk.Rectangle sel_rect = new Gdk.Rectangle ();
		private ArrayList pre_rubberbanded_selection;

		// scroll to
		private TreePath scroll_to_path = null;
		private bool scroll_to_use_align = false;
		private float scroll_to_row_align;
		private float scroll_to_col_align;
#endregion

#region private methods
		private void Redraw (Cairo.Context cr)
		{
			// Clear the background
			cr.Rectangle (0, 0, Allocation.Width, Allocation.Height);
			Gdk.CairoHelper.SetSourceColor (cr, Style.Base (State));
			cr.Fill ();

			if (model == null) {
				if (hadj != null) {
					hadj.Upper = hadj.Lower = 0;
					hadj.Change ();
				}
				
				if (vadj != null) {
					vadj.Upper = 0;
					vadj.Change ();
				}
				return;
			}

			if (rows == 0 || cols == 0) {
				return;
			}

			Gdk.Rectangle background_area = cell_size;
			background_area.Width += padding;
			background_area.Height += padding;

			TreeIter iter;
			if (model.GetIterFirst (out iter)) {
				do {
					TreePath path = model.GetPath (iter);

					int x, y;
					GetCellPosition (path.Indices[0], out x, out y);

					if (hadj != null
					   && (x + cell_size.Width < hadj.Value
					       || x > hadj.Value + hadj.PageSize)) {
						continue;
					}

					if (vadj != null
					    && (y + cell_size.Height < vadj.Value
					        || y > vadj.Value + vadj.PageSize)) {
						continue;
					}

					if (data_func != null) {
						data_func (this, renderer, model, iter);
					}

					cell_size.X = x;
					cell_size.Y = y;
					
					if (hadj != null) {
						cell_size.X -= (int)hadj.Value;
					}
					
					if (vadj != null) {
						cell_size.Y -= (int)vadj.Value;
					}

					background_area.X = cell_size.X - (padding / 2);
					background_area.Y = cell_size.Y - (padding / 2);

					cr.Rectangle (background_area.X, background_area.Y,
					              background_area.Width, background_area.Height);
					cr.Clip ();
					
					renderer.Render (cr, this, background_area,
					                 cell_size, GetCellState (path));

					cr.ResetClip ();
				} while (model.IterNext (ref iter));
			}

			if (have_rubberband_selection) {
				int hadj_val = (hadj != null) ? (int)hadj.Value : 0;
				int vadj_val = (vadj != null) ? (int)vadj.Value : 0;

				cr.Rectangle (sel_rect.X - hadj_val + 0.5f, sel_rect.Y - vadj_val + 0.5f,
				              sel_rect.Width, sel_rect.Height);

				Cairo.Color sel_cairo_color = CairoHelper.GetCairoColor (Style.Background (StateType.Selected));



				//cr.Color = sel_cairo_color;
                cr.SetSourceRGBA(sel_cairo_color.R, sel_cairo_color.G, sel_cairo_color.B, sel_cairo_color.A);

                cr.LineWidth = 1.0f;
				cr.StrokePreserve ();

				sel_cairo_color.A = 0.3f;
				//cr.Color = sel_cairo_color;
                cr.SetSourceRGBA(sel_cairo_color.R, sel_cairo_color.G, sel_cairo_color.B, sel_cairo_color.A);

                cr.Fill ();
			}
		}

		private void ResetAdjustments ()
		{
			if (hadj != null) {
				hadj.SetBounds (0, 0, 0, 0, 0);
				hadj.Value = 0;
				hadj.Change ();
				hadj.ChangeValue ();
			}				

			if (vadj != null) {
				vadj.SetBounds (0, 0, 0, 0, 0);
				vadj.Value = 0;
				vadj.Change ();
				vadj.ChangeValue ();
			}
		}
		
		private bool GetPathAtRowAndCol (int r, int c, out TreePath path)
		{
			path = null;
			if (r > (rows - 1) || r < 0
			    || c > (cols - 1) || c < 0) {
				return false;
			}

			int index = (r * cols) + c;
			if (index >= n_cells) {
				return false;
			}
			
			if (model == null) {
				return false;
			}

			TreeIter iter;
			if (!model.IterNthChild (out iter, index)) {
				return false;
			}

			path = model.GetPath (iter);

			return true;
		}

		private void GetRowAndColForPath (TreePath path, out int r, out int c)
		{
			r = c = -1;
			if (path == null || path.Indices.Length < 1) {
				return;
			}

			if (cols <= 0) {
				return;
			}

			int index = path.Indices[0];
			c = (index % cols);
			r = (int)(index / cols);
		}

		private bool GetRowAndColAtCoords (int x, int y, out int r, out int c)
		{
			bool hit = true;
			r = c = 0;

			int cell_x, cell_y;
			GetCellPosition (0, out cell_x, out cell_y);

			// trivial x bounds check
			if (layout_mode == LayoutMode.Vertical
			    && (x < cell_x || x > (cell_x + cell_size.Width))) {
				hit = false;
			}

			// trivial y bounds check
			if (layout_mode == LayoutMode.Horizontal
			    && (y < cell_y || y > (cell_y + cell_size.Height))) {
				hit = false;
			}

			if (layout_mode == LayoutMode.Vertical
			    || layout_mode == LayoutMode.Grid) {
				// see if hit landed inside of the cell, or
				// inside of the padding after the cell
				int h_and_padding = cell_size.Height + padding;
				if (((y - padding) % h_and_padding) > cell_size.Height) {
					hit = false;
				}

				r = (y - padding) / h_and_padding;
			}

			if (layout_mode == LayoutMode.Horizontal
			    || layout_mode == LayoutMode.Grid) {
				// see if hit landed inside of the cell, or
				// inside of the padding after the cell
				int w_and_padding = cell_size.Width + padding;
				if (((x - padding) % w_and_padding) > cell_size.Width) {
					hit = false;
				}
				
				c = (x - padding) / w_and_padding;
			}

			return hit;
		}

		private void GetCellPosition (int index, out int x, out int y)
		{
			x = y = 0;
			if (cols <= 0) {
				return;
			}

			x = (padding + (index % cols) * (cell_size.Width + padding));
			y = (padding + (int)(index / cols) * (cell_size.Height + padding));

			if (layout_mode == LayoutMode.Horizontal) {
				// center tiles horizontally in horizontal mode
				y = Math.Max (y, (Allocation.Height / 2) - (cell_size.Height / 2));
			} else if (layout_mode == LayoutMode.Vertical) {
				// center tiles vertically in vertical mode
				x = Math.Max (x, (Allocation.Width / 2) - (cell_size.Width / 2));
			}
		}

		private void SelectAllBetween (TreePath anchor, TreePath cursor)
		{
			int anchor_r, anchor_c; 
			GetRowAndColForPath (anchor, out anchor_r, out anchor_c);

			int cursor_r, cursor_c;
			GetRowAndColForPath (cursor, out cursor_r, out cursor_c);

			int min_c = Math.Min (anchor_c, cursor_c);
			int max_c = Math.Max (anchor_c, cursor_c);

			int min_r = Math.Min (anchor_r, cursor_r);
			int max_r = Math.Max (anchor_r, cursor_r);

			TreePath path;
			for (int r = min_r; r <= max_r; r++)
			{
				for (int c = min_c; c <= max_c; c++)
				{
					if (GetPathAtRowAndCol (r, c, out path)) {
						selected_paths.Add (path);
					}
				}
			}

			selection_anchor = anchor;

			if (SelectionChanged != null) {
				SelectionChanged (this, new EventArgs ());
			}

			QueueDraw ();
		}

		private CairoCellRendererState GetCellState (TreePath path)
		{
			CairoCellRendererState state = CairoCellRendererState.None;
			if (selected_paths.Contains (path)) {
				state |= CairoCellRendererState.Selected;
			}

			if (HasFocus
			    && focused_path != null
			    && focused_path.Equals (path)) {
				state |= CairoCellRendererState.Focused;
			}
			
			return state;
		}

		private Gdk.Rectangle CalculateMaxCellSize ()
		{
			Gdk.Rectangle rect = Gdk.Rectangle.Zero;
			
			if (override_width > -1 && override_height > -1) {
				rect.Width = override_width;
				rect.Height = override_height;
				return rect;
			}

			if (model == null) {
				return rect;
			}

			TreeIter iter;
			if (model.GetIterFirst (out iter)) {
				do {
					if (data_func != null) {
						data_func (this, renderer, model, iter);
					}

					int w, h;
					renderer.GetSize (out w, out h);

					rect.Width = Math.Max (w, rect.Width);
					rect.Height = Math.Max (h, rect.Height);
				} while (model.IterNext (ref iter));
			}
			
			// catch if only one override is set
			if (override_width > -1) {
				rect.Width = override_width;
			}

			if (override_height > -1) {	
				rect.Height = override_height;
			}

			return rect;
		}
		
		private bool PathIsValid (TreePath path)
		{
			return PathIsValid (path, n_cells);
		}

		private bool PathIsValid (TreePath path, int n_cells)
		{
			if (model == null || path == null
			    || path.Indices.Length == 0) {
				return false;
			}

			return (path.Indices[0] >= 0
			        && path.Indices[0] < n_cells);
		}

		// TODO: implement use_align = false properly
		private void ScrollToPath (TreePath path, bool use_align,
		                           float row_align, float col_align)
		{
			if (model == null || path == null
			    || row_align < 0 || row_align > 1
			    || col_align < 0 || col_align > 1) {
				return;
			}

			if (!IsRealized || path.Indices[0] >= n_cells) {
				// we haven't seen this cell yet, so queue it
				// up and wait for an allocate
				scroll_to_path = path;
				scroll_to_use_align = use_align;
				scroll_to_row_align = row_align;
				scroll_to_col_align = col_align;
				return;
			}

			int x, y;
			GetCellPosition (path.Indices[0], out x, out y);

			if (vadj != null && (y < vadj.Value
			    || (y + cell_size.Height) >= (vadj.Value + vadj.PageSize))) {
				int y_mod = Math.Max (0, (int)((vadj.PageSize - cell_size.Height) * row_align));
				vadj.Value = Clamp (y - y_mod, vadj.Lower,
				                    vadj.Upper - vadj.PageSize);
				vadj.ChangeValue ();
			}

			if (hadj != null && (x < hadj.Value
			    || (x + cell_size.Width) >= (hadj.Value + hadj.PageSize))) {
				int x_mod = Math.Max (0, (int)((hadj.PageSize - cell_size.Width) * col_align));
				hadj.Value = Clamp (x - x_mod, hadj.Lower,
						    hadj.Upper - hadj.PageSize);
				hadj.ChangeValue ();
			}
		}

		[GLib.ConnectBeforeAttribute]
		private void OnAdjustmentChanged (object o, EventArgs args)
		{
			QueueDraw ();
		}

		private void OnDataRowChanged (object o, RowChangedArgs args)
		{
			// This will:
			// - invalidate our model row
			// - re-lay cells out
			QueueResize ();
		}
		
		private void OnDataRowDeleted (object o, RowDeletedArgs args)
		{
			if (model == null || args.Path == null) {
				return;
			}

			bool sel_paths_changed = false;

			// Don't update the real n_cells, as doing this will
			// throw off ScrollToPath if called before SizeAllocate
			// is run
			int n_cells = model.IterNChildren ();

			for (int i = 0; i < selected_paths.Count; i++)
			{
				TreePath path = (TreePath)selected_paths[i];

				int cmp = path.Compare (args.Path);
				if (cmp == 0) {
					selected_paths.RemoveAt (i);
					i--;
					sel_paths_changed = true;
					continue;
				}

				// decrement each path that follows the one we
				// just deleted
				if (cmp > 0) {
					path.Prev ();
					selected_paths[i] = path;
					continue;
				}
			}

			if (sel_paths_changed && SelectionChanged != null) {
				SelectionChanged (this, new EventArgs ());
			}

			if (focused_path != null
			    && focused_path.Equals (args.Path)) {
				focused_path = focused_path.Copy ();

				// try to advance the focus forward
				focused_path.Next ();
				
				if (!PathIsValid (focused_path, n_cells)
				    && !focused_path.Prev ()) {
					focused_path = null;
				}
			}

			if (selection_anchor != null
			    && selection_anchor.Equals (args.Path)) {
				selection_anchor = null;
			}

			QueueResize ();
		}
		
		private void OnDataRowInserted (object o, RowInsertedArgs args)
		{
			if (model == null || args.Path == null) {
				return;
			}

			// Don't update the real n_cells, as doing this will
			// throw off ScrollToPath if called before SizeAllocate
			// is run
			int n_cells = model.IterNChildren ();

			// see if there are any selected paths with the same
			// path or after the inserted path
			TreePath path;
			for (int i = 0; i < selected_paths.Count; i++)
			{
				path = (TreePath)selected_paths[i];
				if (path.Compare (args.Path) < 0) {
					continue;
				}

				path.Next ();

				// This case isn't very likely:
				// if the path isn't valid, we're at the end,
				// so just dump the paths
				if (!PathIsValid (path, n_cells)) {
					selected_paths.RemoveAt (i);
					i--;
					continue;
				}

				selected_paths[i] = path;
			}

			QueueResize ();
		}
		
		private void OnDataRowsReordered (object o, RowsReorderedArgs args)
		{
			int[] new_child_order = (int[])args.Args[2];
			if (model == null || new_child_order.Length == 0) {
				return;
			}
			
			// iterate through the selected paths, mapping from the
			// old index (in the path) to its new index
			for (int i = 0; i < selected_paths.Count; i++)
			{
				TreePath path = (TreePath)selected_paths[i];
				for (int new_index = 0; new_index < new_child_order.Length; new_index++)
				{
					int old_index = new_child_order[new_index];
					if (path.Indices[0] == old_index) {
						path = new TreePath (new int[] { new_index });
						break;
					}
				}
				
				selected_paths[i] = path;
			}

			QueueResize ();
		}

		private double Clamp (double val, double lower, double upper)
		{
			if (val > upper) {
				return upper;
			}
			
			if (val < lower) {
				return lower;
			}
			
			return val;
		}
#endregion
	}
}
