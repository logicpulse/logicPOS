using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Enums;
using LogicPOS.Api.Features.Tables.GetAllTables;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Table = LogicPOS.Api.Entities.Table;

namespace LogicPOS.UI.Components.Menus
{
    public class TablesMenu : Gtk.Table
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        public int ScrollerHeight { get; set; } = 0;
        public int ButtonFontSize = Convert.ToInt16(AppSettings.Instance.fontPosBaseButtonSize);
        public int MaxCharsPerButtonLabel { get; set; } = AppSettings.Instance.posBaseButtonMaxCharsPerLabel;
        public string ButtonOverlay { get; set; } = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_overlay.png";
        public List<(Table Table, CustomButton Button)> Buttons { get; set; } = new List<(Table, CustomButton)>();
        public string ButtonImage { get; set; }
        public string ButtonLabel { get; set; }
        public bool ToggleMode { get; set; } = true;
        public Table InitialTable { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public CustomButton BtnPrevious { get; set; }
        public CustomButton BtnNext { get; set; }
        public uint Rows { get; set; } = 5;
        public uint Columns { get; set; } = 4;
        public Size ButtonSize { get; set; } = AppSettings.Instance.sizePosTableButton;
        public Table SelectedTable { get; set; }
        public CustomButton SelectedButton { get; set; }
        public event Action<Table> TableSelected;
        public List<Table> AllTables { get; set; }
        public PlacesMenu PlacesMenu { get; }
        public TableStatus? Filter { get; private set; } = null;

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

        public virtual CustomButton InitializeButton(Table table)
        {
            return new TableButton(table);

        }

        public void Update()
        {
            RemoveOldButtons();
            PresentButtons();
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

        private void PresentButtons()
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
            var tables = TablesService.GetAllTables();
            AllTables = tables.ToList();
        }

        public void LoadEntities()
        {
            CurrentPage = 1;
            CustomButton currentButton = null;

            SelectedTable = (InitialTable != null) ? InitialTable : null;

            if (Buttons.Count > 0)
            {
                Buttons.Clear();
            }

            LoadAllTables();

            IEnumerable<Table> tables = Enumerable.Empty<Table>();
            tables = AllTables;

            if (PlacesMenu.SelectedPlace != null)
            {
                tables = tables.Where(x => x.PlaceId == PlacesMenu.SelectedPlace.Id).ToList();
            }

            if(Filter != null)
            {
                tables = tables.Where(x => x.Status == Filter).ToList();
            }

            if (tables.Any())
            {
                foreach (var table in tables)
                {
                    ButtonLabel = table.Designation;

                    ButtonImage = null;

                    if (ButtonLabel.Length > MaxCharsPerButtonLabel)
                    {
                        ButtonLabel = ButtonLabel.Substring(0, MaxCharsPerButtonLabel) + ".";
                    }

                    currentButton = InitializeButton(table);
                    currentButton.Clicked += Button_Clicked;
                    Buttons.Add((table, currentButton));
                    currentButton.CurrentButtonId = table.Id;

                    if (SaleContext.CurrentTable != null && table.Id == SaleContext.CurrentTable.Id)
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
            }

            Update();
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

        public void Refresh(TableStatus? tableStatus = null)
        {
            Buttons = new List<(Table, CustomButton)>();
            Filter = tableStatus;
            LoadEntities();
        }
    }
}
