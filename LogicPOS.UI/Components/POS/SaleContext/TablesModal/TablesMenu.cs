using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Subfamilies.GetAllArticleSubfamilies;
using LogicPOS.Api.Features.Places.GetAllPlaces;
using LogicPOS.Api.Features.Tables.GetAllTables;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.POS;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Menus
{
    public class TablesMenu : Gtk.Table
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        public int ScrollerHeight { get; set; } = 0;
        public int ButtonFontSize = Convert.ToInt16(AppSettings.Instance.fontPosBaseButtonSize);
        public int MaxCharsPerButtonLabel { get; set; } = AppSettings.Instance.posBaseButtonMaxCharsPerLabel;
        public string ButtonOverlay { get; set; } = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_overlay.png";
        public List<(Api.Entities.Table Table, CustomButton Button)> Buttons { get; set; } = new List<(Api.Entities.Table, CustomButton)>();
        public string ButtonImage { get; set; }
        public string ButtonLabel { get; set; }
        public bool ToggleMode { get; set; } = true;
        public Api.Entities.Table InitialTable { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public CustomButton BtnPrevious { get; set; }
        public CustomButton BtnNext { get; set; }
        public uint Rows { get; set; } = 5;
        public uint Columns { get; set; } = 4;
        public Size ButtonSize { get; set; } = AppSettings.Instance.sizePosTableButton;
        public Api.Entities.Table SelectedTable { get; set; }
        public CustomButton SelectedButton { get; set; }
        public event Action<Api.Entities.Table> TableSelected;
        public List<Api.Entities.Table> AllTables{ get; set; }
        public PlacesMenu PlacesMenu { get;  }

        public TablesMenu(CustomButton btnPrevious, CustomButton btnNext, PlacesMenu palcesMenu) : base(5, 4, true)
        {
            BtnPrevious = btnPrevious;
            BtnNext = btnNext;
            PlacesMenu = palcesMenu;
            AddEventHandlers();
            LoadEntities();
        }

        private void AddEventHandlers()
        {
            BtnPrevious.Clicked += BtnPrevious_Clicked;
            BtnNext.Clicked += BtnNext_Clicked;
            PlacesMenu.PlaceSelected += PlacesMenu_PlaceSelected;
        }

        private void PlacesMenu_PlaceSelected(Place place)
        {
            Refresh();
        }

        public virtual CustomButton InitializeButton()
        {
            return new ImageButton(
                new ButtonSettings
                {
                    Text = ButtonLabel,
                    FontSize = ButtonFontSize,
                    Image = ButtonImage,
                    Overlay = ButtonOverlay,
                    ButtonSize = ButtonSize
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

        private void LoadAllTables()
        {
            var tables = _mediator.Send(new GetAllTablesQuery()).Result;
            
            if (tables.IsError == false)
            {
                AllTables = tables.Value.ToList();
                return;
            }

            AllTables = new List<Api.Entities.Table>();
        }

        public void LoadEntities()
        {
            if (AppSettings.Instance.useImageOverlay == false)
            {
                ButtonOverlay = null;
            }

            CurrentPage = 1;
            CustomButton currentButton = null;

            SelectedTable = (InitialTable != null) ? InitialTable : null;

            if (Buttons.Count > 0)
            {
                Buttons.Clear();
            }

            if (AllTables == null)
            {
                LoadAllTables();
            }

            var tables = AllTables.Where(x => x.PlaceId == PlacesMenu.SelectedPlace.Id).ToList();

            if (tables.Count > 0)
            {
                foreach (var table in tables)
                {
                    ButtonLabel = table.Designation;

                    ButtonImage = null;

                    if (ButtonLabel.Length > MaxCharsPerButtonLabel)
                    {
                        ButtonLabel = ButtonLabel.Substring(0, MaxCharsPerButtonLabel) + ".";
                    }

                    currentButton = InitializeButton();
                    currentButton.Clicked += Button_Clicked;
                    Buttons.Add((table, currentButton));
                    currentButton.CurrentButtonId = table.Id;

                    if (SaleContext.CurrentTable != null &&  table.Id == SaleContext.CurrentTable.Id)
                    {
                        if (ToggleMode)
                        {
                            currentButton.Sensitive = false;
                        }
                        SelectedButton = currentButton;
                    }

                }

                TotalItems = Buttons.Count;
                ItemsPerPage = Convert.ToInt16(Rows * Columns);
                TotalPages = (int)Math.Ceiling((float)TotalItems / (float)ItemsPerPage);
                Update();

            }
            else
            {

                Update();
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            CustomButton button = (CustomButton)sender;

            if (ToggleMode && SelectedButton != null)
            {
                SelectedButton.Sensitive = true;
            }

            SelectedButton = button;

            if (ToggleMode)
            {
                SelectedButton.Sensitive = false;
            }

            SelectedTable = Buttons.Find(x => x.Button == SelectedButton).Table;

            TableSelected?.Invoke(SelectedTable);
        }

        internal void Refresh()
        {
            Buttons = new List<(Api.Entities.Table, CustomButton)>();
            LoadEntities();
        }
    }
}
