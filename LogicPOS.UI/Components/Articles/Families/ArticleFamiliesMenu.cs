﻿using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Families.GetAllArticleFamilies;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Menus
{
    public class ArticleFamiliesMenu : Gtk.Table
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        public int ScrollerHeight { get; set; } = 0;
        public int ButtonFontSize = Convert.ToInt16(AppSettings.Instance.fontPosBaseButtonSize);
        public int MaxCharsPerButtonLabel { get; set; } = AppSettings.Instance.posBaseButtonMaxCharsPerLabel;
        public string ButtonOverlay { get; set; } = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_overlay.png";
        public List<(ArticleFamily Family,CustomButton Button)> Buttons { get; set; } = new List<(ArticleFamily,CustomButton)>();
        public string ButtonImage { get; set; }
        public string ButtonLabel { get; set; }
        public bool ToggleMode { get; set; } = true;
        public ArticleFamily InitialFamily { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public CustomButton BtnPrevious { get; set; }
        public CustomButton BtnNext { get; set; }
        public uint Rows { get; set; } = 6;
        public uint Columns { get; set; } = 1;
        public Size ButtonSize { get; set; } = new Size(176, 120);

        public Window SourceWindow { get; set; }
        public ArticleFamily SelectedFamily { get; set; }
        public CustomButton SelectedButton { get; set; }
        public string Order { get; set; }

        public event Action<ArticleFamily> FamilySelected;

        public ArticleFamiliesMenu(CustomButton btnPrevious, CustomButton btnNext) : base(6, 1, true)
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

            SelectedFamily = (InitialFamily != null) ? InitialFamily : null;

            if (Buttons.Count > 0)
            {
                Buttons.Clear();
            }

            var getFamiliesResult = _mediator.Send(new GetAllArticleFamiliesQuery()).Result;
            var families = new List<ArticleFamily>();

            if (getFamiliesResult.IsError == false)
            {
                families.AddRange(getFamiliesResult.Value);
            }

            if (families.Count > 0)
            {
                foreach (var family in families)
                {
                    if (SelectedFamily == null)
                    {
                        SelectedFamily = family;

                        if (InitialFamily == null)
                        {
                            InitialFamily = SelectedFamily;
                        }
                    }

                    ButtonLabel = family.Button.Label ?? family.Designation;
                    ButtonImage = family.Button.Image ?? "";

                    if (ButtonLabel.Length > MaxCharsPerButtonLabel)
                    {
                        ButtonLabel = ButtonLabel.Substring(0, MaxCharsPerButtonLabel) + ".";
                    }

                    currentButton = InitializeButton();
                    currentButton.Clicked += Button_Clicked;
                    Buttons.Add((family,currentButton));
                    currentButton.CurrentButtonId = family.Id;

                    if (family.Id == InitialFamily.Id)
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

            SelectedFamily = Buttons.Find(x => x.Button == SelectedButton).Family;

            FamilySelected?.Invoke(SelectedFamily);
        }

        internal void Refresh()
        {
            Buttons = new List<(ArticleFamily, CustomButton)>();
            LoadEntities();
        }
    }
}
