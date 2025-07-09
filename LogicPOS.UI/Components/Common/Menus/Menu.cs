using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Menus
{
    public abstract class Menu<TEntity> : Gtk.Table
    {
        private int ButtonFontSize { get; } = Convert.ToInt16(AppSettings.Instance.FontPosBaseButtonSize);
        public int MaxCharsPerButtonLabel { get; set; } = AppSettings.Instance.PosBaseButtonMaxCharsPerLabel;
        private readonly string _buttonOverlay;
        public List<(TEntity Entity, CustomButton Button)> Buttons { get; set; } = new List<(TEntity, CustomButton)>();
        public string ButtonImage { get; set; }
        public string ButtonLabel { get; set; }
        private readonly bool _toggleMode;
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public CustomButton BtnPrevious { get; set; }
        public CustomButton BtnNext { get; set; }

        protected readonly uint _rows;
        protected readonly uint _columns;
        protected readonly Size _buttonSize;
        private readonly string _buttonName;
        public Window SourceWindow { get; }
        public TEntity SelectedEntity { get; set; }
        public CustomButton SelectedButton { get; set; }
        public List<TEntity> Entities { get; set; } = new List<TEntity>();
        public event Action<TEntity> OnEntitySelected;

        public Menu(uint rows,
                    uint columns,
                    Size buttonSize,
                    string buttonName,
                    CustomButton btnPrevious,
                    CustomButton btnNext,
                    Window sourceWindow,
                    bool toggleMode = true) : base(rows, columns, true)
        {
            _buttonName = buttonName;
            _toggleMode = toggleMode;
            SourceWindow = sourceWindow;
            _buttonSize = buttonSize;
            _rows = rows;
            _columns = columns;

            _buttonOverlay = (AppSettings.Instance.UseImageOverlay) ? AppSettings.Paths.Images + @"Buttons\Pos\button_overlay.png" : null;

            BtnPrevious = btnPrevious;
            BtnNext = btnNext;
            BtnPrevious.Clicked += BtnPrevious_Clicked;
            BtnNext.Clicked += BtnNext_Clicked;
        }

        protected virtual CustomButton CreateMenuButton(TEntity entity)
        {
            return new ImageButton(
                new ButtonSettings
                {
                    Name = _buttonName,
                    Text = ButtonLabel,
                    FontSize = ButtonFontSize,
                    Image = ButtonImage,
                    Overlay = _buttonOverlay,
                    ButtonSize = _buttonSize,
                });
        }

        public void UpdateUI()
        {
            RemoveOldPage();
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
                if (TotalPages > 1) BtnNext.Sensitive = true;
            }
        }

        private void PresentCurrentPage()
        {
            if (Buttons.Count <= 0)
            {
                return;
            }

            uint currentRow = 0, currentColumn = 0;
            int startItem = (CurrentPage * ItemsPerPage) - ItemsPerPage;
            int endItem = startItem + ItemsPerPage - 1;
            for (int i = startItem; i <= endItem; i++)
            {
                if (i < TotalItems)
                {
                    this.Attach(Buttons[i].Button, currentColumn, currentColumn + 1, currentRow, currentRow + 1, AttachOptions.Fill, AttachOptions.Fill, 0, 0);
                }

                if (currentColumn == this.NColumns - 1)
                {
                    ++currentRow;
                    currentColumn = 0;
                }
                else
                {
                    ++currentColumn;
                }
            }

        }

        private void RemoveOldPage()
        {
            int childrenLength = this.Children.Length;
            for (int i = 0; i < childrenLength; i++)
            {
                this.Remove(this.Children[0]);
            }
        }

        protected virtual void BtnPrevious_Clicked(object obj, EventArgs args)
        {
            CurrentPage -= 1;
            UpdateUI();
        }

        protected virtual void BtnNext_Clicked(object obj, EventArgs args)
        {
            CurrentPage += 1;
            UpdateUI();
        }

        protected abstract string GetButtonLabel(TEntity entity);

        protected abstract string GetButtonImage(TEntity entity);

        public virtual void ListEntities(IEnumerable<TEntity> entities)
        {
            if (entities == null || entities.Any() == false)
            {
                return;
            }


            Buttons.Clear();

            SelectedEntity = default;

            foreach (var entity in entities)
            {
                ButtonLabel = GetButtonLabel(entity);
                ButtonImage = GetButtonImage(entity);

                if (ButtonLabel.Length > MaxCharsPerButtonLabel)
                {
                    ButtonLabel = ButtonLabel.Substring(0, MaxCharsPerButtonLabel) + ".";
                }

                var menuButton = CreateMenuButton(entity);
                menuButton.Clicked += MenuButton_Clicked;
                Buttons.Add((entity, menuButton));

                if (_toggleMode && SelectedEntity == null)
                {
                    SelectedEntity = entity;
                    SelectedButton = menuButton;
                    SelectedButton.Sensitive = false;
                }
            }

            SetPagination();

            UpdateUI();
        }

        protected virtual void SetPagination()
        {
            CurrentPage = 1;
            TotalItems = Buttons.Count;
            ItemsPerPage = Convert.ToInt16(_rows * _columns);
            TotalPages = (int)Math.Ceiling(TotalItems / (float)ItemsPerPage);
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

            SelectedEntity = Buttons.Find(x => x.Button == SelectedButton).Entity;
            OnEntitySelected?.Invoke(SelectedEntity);
        }

        protected abstract IEnumerable<TEntity> FilterEntities(IEnumerable<TEntity> entities);

        public virtual void Refresh()
        {
            Buttons.Clear();
            LoadEntities();
            var filteredEntities = FilterEntities(Entities);
            ListEntities(filteredEntities);

            if (SelectedEntity != null)
            {
                OnEntitySelected?.Invoke(SelectedEntity);
            }
        }
    }
}
