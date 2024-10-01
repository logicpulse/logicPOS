using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals.Common
{
    public class ModalTabsNavigator : VBox
    {
        public Color HotButtonBackgroundColor = AppSettings.Instance.colorPagePadHotButtonBackground;
        public Size BtnNavigatorSize => AppSettings.Instance.sizePagesPadNavigatorButton;
        public Size BtnNavigatorIconSize => AppSettings.Instance.sizePagesPadNavigatorButtonIcon;
        public string BtnPreviousIcon => $"{PathsSettings.ImagesFolderLocation}{@"Icons/icon_pos_pagepad_prev.png"}";
        public string BtnNextIcon => $"{PathsSettings.ImagesFolderLocation}{@"Icons/icon_pos_pagepad_next.png"}";
        public string BtnNavigatorFont => AppSettings.Instance.fontPagePadNavigatorButton;

        public HBox Component { get; set; }
        public IconButtonWithText BtnPrevious { get; private set; }
        public IconButtonWithText BtnNext { get; private set; }
        public List<ModalTab> Tabs { get; set; }
        public int CurrentPageIndex { get; set; } = 0;
        public ModalTab ActivePage { get; set; }
        public event EventHandler PageChanged;

        public ModalTabsNavigator(params ModalTab[] pages)
        {
            Tabs = new List<ModalTab>(pages);
            Build();
        }

        private void Build()
        {
            InitializeButtons();
            AddEventHandlers();

            HBox navigatorButtons = new HBox(true, 0);

            int i = 0;
            foreach (ModalTab page in Tabs)
            {
                i++;
                page.BtnNavigator = new IconButtonWithText(
                    new ButtonSettings
                    {
                        Name = page.PageName,
                        Text = page.PageName,
                        Font = BtnNavigatorFont,
                        FontColor = Color.White,
                        Icon = page.PageIcon,
                        IconSize = BtnNavigatorIconSize,
                        ButtonSize = BtnNavigatorSize,
                        BackgroundColor = HotButtonBackgroundColor
                    });

                page.BtnNavigator.Sensitive = (i == 1);

                if ((i == 1))
                {
                    page.BtnNavigator.ModifyBg(
                    StateType.Normal,
                    HotButtonBackgroundColor.ToGdkColor());
                }


                navigatorButtons.PackStart(page.BtnNavigator, true, true, 2);
            }

            Component = new HBox(false, 0);
            Component.PackStart(BtnPrevious, false, false, 2);
            Component.PackStart(navigatorButtons, true, true, 2);
            Component.PackStart(BtnNext, false, false, 2);
            PackStart(Component, false, false, 2);

            ActivePage = Tabs[0];
            PackStart(ActivePage);
        }

        private void InitializeButtons()
        {
            BtnPrevious = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "buttonPrev",
                    BackgroundColor = HotButtonBackgroundColor,
                    Text = GeneralUtils.GetResourceByName("pos_button_label_prev_pages_toolbar"),
                    Font = BtnNavigatorFont,
                    FontColor = Color.White,
                    Icon = BtnPreviousIcon,
                    IconSize = BtnNavigatorIconSize,
                    ButtonSize = BtnNavigatorSize
                })
            { Sensitive = false };

            BtnNext = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "buttonNext",
                    BackgroundColor = HotButtonBackgroundColor,
                    Text = GeneralUtils.GetResourceByName("pos_button_label_next_pages_toolbar"),
                    Font = BtnNavigatorFont,
                    FontColor = Color.White,
                    Icon = BtnNextIcon,
                    IconSize = BtnNavigatorIconSize,
                    ButtonSize = BtnNavigatorSize
                });
        }

        private void AddEventHandlers()
        {
            BtnPrevious.Clicked += BtnPrevious_Clicked;
            BtnNext.Clicked += BtnNext_Clicked;
        }

        private void BtnNext_Clicked(object sender, EventArgs e)
        {
            MovePage(true);
        }

        private void BtnPrevious_Clicked(object sender, EventArgs e)
        {
            MovePage(false);
        }

        private void UpdateUI()
        {

            if (CurrentPageIndex == 0)
            {
                BtnPrevious.Sensitive = false;
                BtnNext.Sensitive = true;
            }
            else if (CurrentPageIndex == Tabs.Count - 1)
            {
                BtnPrevious.Sensitive = true;
                BtnNext.Sensitive = ((CurrentPageIndex + 1 <= Tabs.Count - 1) && (Tabs[CurrentPageIndex + 1].Enabled));
            }
            else
            {
                BtnPrevious.Sensitive = true;
                BtnNext.Sensitive = true;
            }

            PageChanged?.Invoke(this, EventArgs.Empty);
        }

        public void MovePage(bool next)
        {
            this.Remove(ActivePage);
            ActivePage.BtnNavigator.Sensitive = false;

            CurrentPageIndex = next ? CurrentPageIndex + 1 : CurrentPageIndex - 1;

            ActivePage = Tabs[CurrentPageIndex];
            this.PackStart(ActivePage);
            ActivePage.BtnNavigator.Sensitive = true;
            ActivePage.BtnNavigator.ModifyBg(StateType.Normal, HotButtonBackgroundColor.ToGdkColor());

            if (ActivePage.Visible == false)
            {
                ActivePage.ShowAll();
            }

            UpdateUI();
        }
    }
}
