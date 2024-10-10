using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Families.GetAllArticleFamilies;
using LogicPOS.Api.Features.Articles.Subfamilies.GetAllArticleSubfamilies;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Menus
{
    public class ArticleSubfamiliesMenu : Gtk.Table
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        public int ScrollerHeight { get; set; } = 0;
        public int ButtonFontSize = Convert.ToInt16(AppSettings.Instance.fontPosBaseButtonSize);
        public int MaxCharsPerButtonLabel { get; set; } = AppSettings.Instance.posBaseButtonMaxCharsPerLabel;
        public string ButtonOverlay { get; set; } = PathsSettings.ImagesFolderLocation + @"Buttons\Pos\button_overlay.png";
        public List<(ArticleSubfamily Subfamily, CustomButton Button)> Buttons { get; set; } = new List<(ArticleSubfamily, CustomButton)>();
        public string ButtonImage { get; set; }
        public string ButtonLabel { get; set; }
        public bool ToggleMode { get; set; } = true;
        public ArticleSubfamily InitialSubfamily { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public CustomButton BtnPrevious { get; set; }
        public CustomButton BtnNext { get; set; }
        public uint Rows { get; set; } = 1;
        public uint Columns { get; set; } = 7;
        public Size ButtonSize { get; set; } = new Size(176, 120);
        public Window SourceWindow { get; set; }
        public ArticleSubfamily SelectedSubfamily { get; set; }
        public CustomButton SelectedButton { get; set; }
        public ArticleFamiliesMenu FamiliesMenu { get; set; }
        public List<ArticleSubfamily> AllSubfamilies { get; set; }
        public event Action<ArticleSubfamily> SubfamilySelected;

        public ArticleSubfamiliesMenu(
            ArticleFamiliesMenu articleFamiliesMenu,
            CustomButton btnPrevious,
            CustomButton btnNext) : base(1, 7, true)
        {
            BtnPrevious = btnPrevious;
            BtnNext = btnNext;
            FamiliesMenu = articleFamiliesMenu;
            AddEventHandlers();
            LoadEntities();
        }

        private void AddEventHandlers()
        {
            BtnPrevious.Clicked += BtnPrevious_Clicked;
            BtnNext.Clicked += BtnNext_Clicked;
            FamiliesMenu.FamilySelected += FamiliesMenu_FamilySelected;
        }

        private void FamiliesMenu_FamilySelected(ArticleFamily family)
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

        public void LoadEntities()
        {
            if (AppSettings.Instance.useImageOverlay == false)
            {
                ButtonOverlay = null;
            }

            CurrentPage = 1;
            CustomButton currentButton = null;

            if (Buttons.Count > 0)
            {
                Buttons.Clear();
            }

            if (AllSubfamilies == null)
            {
                LoadAllSubfamilies();
            }

            var subfamilies = AllSubfamilies.Where(s => s.FamilyId == FamiliesMenu.SelectedFamily.Id).ToList();

            foreach (var subfamily in subfamilies)
            {
                if (SelectedSubfamily == null)
                {
                    SelectedSubfamily = subfamily;

                    if (InitialSubfamily == null)
                    {
                        InitialSubfamily = SelectedSubfamily;
                    }
                }

                ButtonLabel = subfamily.Button.Label ?? subfamily.Designation;
                ButtonImage = subfamily.Button.Image ?? "";

                if (ButtonLabel.Length > MaxCharsPerButtonLabel)
                {
                    ButtonLabel = ButtonLabel.Substring(0, MaxCharsPerButtonLabel) + ".";
                }

                currentButton = InitializeButton();
                currentButton.Clicked += Button_Clicked;
                Buttons.Add((subfamily, currentButton));
                currentButton.CurrentButtonId = subfamily.Id;

                if (subfamily.Id == InitialSubfamily.Id)
                {
                    if (ToggleMode)
                    {
                        currentButton.Sensitive = false;
                    }
                    SelectedButton = currentButton;
                }

            }

            if(subfamilies.Count == 0)
            {
                SelectedSubfamily = null;
                SubfamilySelected?.Invoke(SelectedSubfamily);
            }

            TotalItems = Buttons.Count;
            ItemsPerPage = Convert.ToInt16(Rows * Columns);
            TotalPages = (int)Math.Ceiling((float)TotalItems / (float)ItemsPerPage);

            Update();
        }

        private void LoadAllSubfamilies()
        {
            var subfamilies = _mediator.Send(new GetAllArticleSubfamiliesQuery()).Result;
            if (subfamilies.IsError == false)
            {
                AllSubfamilies = subfamilies.Value.ToList();
                return;
            }

            AllSubfamilies = new List<ArticleSubfamily>();
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

            SelectedSubfamily = Buttons.Find(x => x.Button == SelectedButton).Subfamily;
            SubfamilySelected?.Invoke(SelectedSubfamily);
        }

        internal void Refresh()
        {
            Buttons = new List<(ArticleSubfamily, CustomButton)>();
            LoadEntities();
        }
    }
}
