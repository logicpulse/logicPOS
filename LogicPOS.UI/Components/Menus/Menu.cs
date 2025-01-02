using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Menus
{
    public abstract class Menu<TEntity> : Gtk.Table
    {
        private int ButtonFontSize { get; } = Convert.ToInt16(AppSettings.Instance.fontPosBaseButtonSize);
        public int MaxCharsPerButtonLabel { get; set; } = AppSettings.Instance.posBaseButtonMaxCharsPerLabel;
        public string ButtonOverlay { get; set; } = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_overlay.png";
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
        public CustomButton SelectedButton { get; private set; }
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

            BtnPrevious = btnPrevious;
            BtnNext = btnNext;
            BtnPrevious.Clicked += BtnPrevious_Clicked;
            BtnNext.Clicked += BtnNext_Clicked;
        }

        protected virtual CustomButton CreateMenuButton()
        {
            return new ImageButton(
                new ButtonSettings
                {
                    Name = _buttonName,
                    Text = ButtonLabel,
                    FontSize = ButtonFontSize,
                    Image = ButtonImage,
                    Overlay = ButtonOverlay,
                    ButtonSize = _buttonSize,
                });
        }

        public void Update()
        {
            RemoveOldButtons();
            AddItems();
            UpdateButtonsState();
        }

        private void UpdateButtonsState()
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

        private void AddItems()
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

        private void RemoveOldButtons()
        {
            int childrenLength = this.Children.Length;
            for (int i = 0; i < childrenLength; i++)
            {
                this.Remove(this.Children[0]);
            }
        }

        private void BtnPrevious_Clicked(object obj, EventArgs args)
        {
            CurrentPage -= 1;
            Update();
        }

        private void BtnNext_Clicked(object obj, EventArgs args)
        {
            CurrentPage += 1;
            Update();
        }

        protected abstract string GetButtonLabel(TEntity entity);

        protected abstract string GetButtonImage(TEntity entity);

        protected virtual IEnumerable<TEntity> GetFilteredEntities()
        {
            return Entities;
        }

        public virtual void PresentEntities(Predicate<IEnumerable<TEntity>> filter = null)
        {
            if (AppSettings.Instance.useImageOverlay == false)
            {
                ButtonOverlay = null;
            }

            CurrentPage = 1;

            Buttons.Clear();

            if (Entities == null || Entities.Count == 0)
            {
                LoadEntities();
            }

            var filteredEntities = GetFilteredEntities();

            foreach (var entity in filteredEntities)
            {
                ButtonLabel = GetButtonLabel(entity);
                ButtonImage = GetButtonImage(entity);

                if (ButtonLabel.Length > MaxCharsPerButtonLabel)
                {
                    ButtonLabel = ButtonLabel.Substring(0, MaxCharsPerButtonLabel) + ".";
                }

                var menuButton = CreateMenuButton();
                menuButton.Clicked += MenuButton_Clicked;
                Buttons.Add((entity, menuButton));
            }

            TotalItems = Buttons.Count;
            ItemsPerPage = Convert.ToInt16(_rows * _columns);
            TotalPages = (int)Math.Ceiling((float)TotalItems / (float)ItemsPerPage);

            Update();
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

        public virtual void Refresh()
        {
            Buttons.Clear();
            PresentEntities();
        }
    }
}
