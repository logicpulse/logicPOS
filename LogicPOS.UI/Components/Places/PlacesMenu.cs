using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Places.GetAllPlaces;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Terminals;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Menus
{
    public class PlacesMenu : Gtk.Table
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        public int ScrollerHeight { get; set; } = 0;
        public int ButtonFontSize = Convert.ToInt16(AppSettings.Instance.fontPosBaseButtonSize);
        public int MaxCharsPerButtonLabel { get; set; } = AppSettings.Instance.posBaseButtonMaxCharsPerLabel;
        public string ButtonOverlay { get; set; } = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_overlay.png";
        public List<(Place Place, CustomButton Button)> Buttons { get; set; } = new List<(Place, CustomButton)>();
        public string ButtonImage { get; set; }
        public string ButtonLabel { get; set; }
        public bool ToggleMode { get; set; } = true;
        public Place InitialPlace { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public CustomButton BtnPrevious { get; set; }
        public CustomButton BtnNext { get; set; }
        public uint Rows { get; set; } = 5;
        public uint Columns { get; set; } = 1;
        public Size ButtonSize { get; set; } = AppSettings.Instance.sizePosTableButton;

        public Window SourceWindow { get; set; }
        public Place SelectedPlace { get; set; }
        public CustomButton SelectedButton { get; set; }

        public event Action<Place> PlaceSelected;

        public PlacesMenu(CustomButton btnPrevious, CustomButton btnNext) : base(5, 1, true)
        {
            BtnPrevious = btnPrevious;
            BtnNext = btnNext;
            AddEventHandlers();
            LoadEntities();
        }

        private void AddEventHandlers()
        {
            BtnPrevious.Clicked += BtnPrevious_Clicked;
            BtnNext.Clicked += BtnNext_Clicked;
        }

        public virtual CustomButton InitializeButton()
        {
            return new ImageButton(
                new ButtonSettings
                {
                    Name = "buttonFamilyId",
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

        public void LoadEntities()
        {
            if (AppSettings.Instance.useImageOverlay == false)
            {
                ButtonOverlay = null;
            }

            CurrentPage = 1;
            CustomButton currentButton = null;

            SelectedPlace = InitialPlace;

            if (Buttons.Count > 0)
            {
                Buttons.Clear();
            }

            IEnumerable<Place> places = Enumerable.Empty<Place>();
            var getPlacesResult = _mediator.Send(new GetAllPlacesQuery()).Result;

            if (getPlacesResult.IsError == false)
            {
                places = getPlacesResult.Value;
            }

            if(TerminalService.Terminal.PlaceId != null)
            {
                places = places.Where(t => t.Id == TerminalService.Terminal.PlaceId);
            }


            if (places.Any())
            {
                foreach (var place in places)
                {
                    if (SelectedPlace == null)
                    {
                        SelectedPlace = place;

                        if (InitialPlace == null)
                        {
                            InitialPlace = SelectedPlace;
                        }
                    }

                    ButtonLabel = place.Designation;

                    ButtonImage = null;

                    if (ButtonLabel.Length > MaxCharsPerButtonLabel)
                    {
                        ButtonLabel = ButtonLabel.Substring(0, MaxCharsPerButtonLabel) + ".";
                    }

                    currentButton = InitializeButton();
                    currentButton.Clicked += Button_Clicked;
                    Buttons.Add((place, currentButton));

                    if (place.Id == InitialPlace.Id)
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

            SelectedPlace = Buttons.Find(x => x.Button == SelectedButton).Place;

            PlaceSelected?.Invoke(SelectedPlace);
        }

        internal void Refresh()
        {
            Buttons = new List<(Place, CustomButton)>();
            LoadEntities();
        }
    }
}
