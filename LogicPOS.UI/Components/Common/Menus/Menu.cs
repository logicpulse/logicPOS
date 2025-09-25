using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Common.Menus;
using LogicPOS.UI.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace LogicPOS.UI.Components.Menus
{
    public abstract class Menu<TEntity> : Gtk.Table where TEntity : ApiEntity
    {
        public List<MenuButton<TEntity>> ButtonsCache { get; } = new List<MenuButton<TEntity>>();
        private readonly bool _toggleMode;
        private int TotalItems => Entities.Count;
        public int PageSize => (int)(_rows * _columns);
        public int CurrentPage { get; set; } = 1;
        public int TotalPages => (int)Math.Ceiling((float)TotalItems / PageSize);
        public CustomButton BtnPrevious { get; set; }
        public CustomButton BtnNext { get; set; }
        protected readonly uint _rows;
        protected readonly uint _columns;
        public bool SelectFirstOnReload { get; set; }
        public Window SourceWindow { get; }
        public TEntity SelectedEntity { get; set; }
        public CustomButton SelectedButton { get; set; }
        public List<TEntity> Entities { get; set; } = new List<TEntity>();
        public event Action<TEntity> OnEntitySelected;

        public Menu(uint rows,
                    uint columns,
                    CustomButton btnPrevious,
                    CustomButton btnNext,
                    Window sourceWindow,
                    bool toggleMode = true) : base(rows, columns, toggleMode)
        {
            _toggleMode = toggleMode;
            SourceWindow = sourceWindow;
            _rows = rows;
            _columns = columns;
            BtnPrevious = btnPrevious;
            BtnNext = btnNext;
            AddNavigationEventHandlers();
        }

        private void AddNavigationEventHandlers()
        {
            BtnPrevious.Clicked += BtnPrevious_Clicked;
            BtnNext.Clicked += BtnNext_Clicked;
        }

        protected abstract CustomButton CreateButtonForEntity(TEntity entity);

        public void UpdateUI()
        {
            Clear();
            PresentCurrentPage();
            UpdateNavigationButtons();
        }

        protected virtual void UpdateNavigationButtons()
        {
            if (CurrentPage == 1)
            {
                BtnPrevious.Sensitive = false;
            }
            else
            {
                BtnPrevious.Sensitive = true;
            }

            if (CurrentPage == TotalPages)
            {
                BtnNext.Sensitive = false;
            }
            else
            {
                if (TotalPages > 1)
                {
                    BtnNext.Sensitive = true;
                }
            }
        }

        protected virtual IEnumerable<TEntity> GetPage(int page)
        {
            return Entities.Skip((page - 1) * PageSize).Take(PageSize);
        }

        private MenuButton<TEntity> GetOrCreateMenuButtonForEntity(TEntity entity)
        {
            MenuButton<TEntity> menuButton = ButtonsCache.Where(mb => mb.Entity.Id == entity.Id).FirstOrDefault();

            if (menuButton != null)
            {
                return menuButton;
            }

            CustomButton button = CreateButtonForEntity(entity);
            button.Clicked += MenuButton_Clicked;
            menuButton = new MenuButton<TEntity>(entity, button);
            ButtonsCache.Add(menuButton);

            return menuButton;
        }

        private void PresentCurrentPage()
        {
            var entities = GetPage(CurrentPage);

            uint row = 0, column = 0;
            MenuButton<TEntity> FirstButton=null;
  
            foreach (var entity in entities)
            {
                var menuButton = GetOrCreateMenuButtonForEntity(entity);

                if (column == 0 && row == 0  && _toggleMode)
                {
                    FirstButton = menuButton;
                }

                this.Attach(menuButton.Button,
                            column,
                            column + 1,
                            row,
                            row + 1,
                            AttachOptions.Fill,
                            AttachOptions.Fill,
                            0,
                            0);

                if (column == this.NColumns - 1)
                {
                    ++row;
                    column = 0;
                }
                else
                {
                    ++column;
                }

            }
            if (SelectFirstOnReload && FirstButton!=null)
            {
                MenuButton_Clicked(FirstButton.Button, EventArgs.Empty);
            }
        }

        private void Clear()
        {
            int childrenLength = this.Children.Length;
            for (int i = 0; i < childrenLength; i++)
            {
                this.Remove(this.Children[0]);
            }
        }

        protected virtual void BtnPrevious_Clicked(object obj, EventArgs args)
        {
            if (CurrentPage <= 1) return;

            CurrentPage -= 1;
            UpdateUI();
        }

        protected virtual void BtnNext_Clicked(object obj, EventArgs args)
        {
            if (CurrentPage >= TotalPages) return;

            CurrentPage += 1;
            UpdateUI();
        }

        protected abstract void LoadEntities();

        protected virtual void MenuButton_Clicked(object sender, EventArgs e)
        {
            CustomButton button = (CustomButton)sender;

            if (_toggleMode && SelectedButton != null)
            {
                SelectedButton.Sensitive = true;
            }

            SelectedButton = button;

            if (_toggleMode)
            {
                SelectedButton.Sensitive = false;
            }

            SelectedEntity = ButtonsCache.Where(x => x.Button == SelectedButton).Select(x => x.Entity).FirstOrDefault();

            if (SelectedEntity != null)
            {
                OnEntitySelected?.Invoke(SelectedEntity);
            }
        }

        public virtual void Refresh()
        {
            LoadEntities();

            UpdateUI();
        }
    }
}
