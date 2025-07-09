using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Modals.Common
{
    public class ModalTabsNavigator : VBox
    {
        public Color HotButtonBackgroundColor = AppSettings.Instance.ColorPagePadHotButtonBackground;
        public Size BtnNavigatorSize => AppSettings.Instance.SizePagesPadNavigatorButton;
        public Size BtnNavigatorIconSize => AppSettings.Instance.SizePagesPadNavigatorButtonIcon;
        public string BtnPreviousIcon => $"{AppSettings.Paths.Images}{@"Icons/icon_pos_pagepad_prev.png"}";
        public string BtnNextIcon => $"{AppSettings.Paths.Images}{@"Icons/icon_pos_pagepad_next.png"}";
        public string BtnNavigatorFont => AppSettings.Instance.FontPagePadNavigatorButton;

        public HBox Component { get; set; }
        public IconButtonWithText BtnPrevious { get; private set; }
        public IconButtonWithText BtnNext { get; private set; }
        public List<ModalTab> Tabs { get; set; }
        public ModalTab CurrentTab { get; set; }

        public ModalTabsNavigator(params ModalTab[] pages)
        {
            Tabs = new List<ModalTab>(pages);
            Initialize();
        }

        private void Initialize()
        {
            InitializeButtons();
            AddEventHandlers();

            HBox navigatorButtons = new HBox(true, 0);

            for (int i = 0; i < Tabs.Count; i++)
            {
                ModalTab page = Tabs[i];
                page.Index = i;

                page.Button = new IconButtonWithText(
                    new ButtonSettings
                    {
                        Name = "buttonUserId",
                        Text = page.TabName,
                        Font = BtnNavigatorFont,
                        FontColor = Color.White,
                        Icon = page.TabIcon,
                        IconSize = BtnNavigatorIconSize,
                        ButtonSize = BtnNavigatorSize
                    });

                page.Button.Sensitive = (i == 0);

                if (i == 0)
                {
                    page.Button.ModifyBg(StateType.Normal,HotButtonBackgroundColor.ToGdkColor());

                    CurrentTab = page;
                }

                navigatorButtons.PackStart(page.Button, true, true, 2);
            }

            Component = new HBox(false, 0);
            Component.PackStart(BtnPrevious, false, false, 2);
            Component.PackStart(navigatorButtons, true, true, 2);
            Component.PackStart(BtnNext, false, false, 2);
            PackStart(Component, false, false, 2);
            PackStart(CurrentTab);
        }

        public void HideTab(ModalTab tab)
        {
            tab.ShowTab = false;
            tab.Button.Hide();
        }

        public void ShowTab(ModalTab tab)
        {
            tab.ShowTab = true;
            tab.Button.Show();
        }

        public void UpdateUI()
        {
            Tabs.ForEach(t =>
            {
                if(t.ShowTab == false)
                {
                    t.Button.Hide();
                } else
                {
                    t.Button.Show();
                }
            });

            UpdateNavigatorButtons();
        }

        private void InitializeButtons()
        {
            BtnPrevious = new IconButtonWithText(
                new ButtonSettings
                {
                    Name = "buttonUserId",
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
                    Name = "buttonUserId",
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
            MoveNext();
        }

        private void BtnPrevious_Clicked(object sender, EventArgs e)
        {
            MovePrevious();
        }

        private bool CanMoveNext()
        {
            return Tabs.Where(t => t.Index > CurrentTab.Index && t.ShowTab).Any();
        }

        private void MoveNext()
        {
            if(CanMoveNext() == false)
            {
                return;
            }

            this.Remove(CurrentTab);
            CurrentTab.Button.Sensitive = false;

            CurrentTab = Tabs[GetNextTabIndex()];
            this.PackStart(CurrentTab);
            ShowCurrentTab();
        }

        private void MovePrevious()
        {
            if (CanMovePrevious() == false)
            {
                return;
            }

            this.Remove(CurrentTab);
            CurrentTab.Button.Sensitive = false;

            CurrentTab = Tabs[GetPreviousTabIndex()];
            this.PackStart(CurrentTab);
            ShowCurrentTab();
        }

        private void ShowCurrentTab()
        {
            CurrentTab.Button.Sensitive = true;
            CurrentTab.Button.ModifyBg(StateType.Normal, HotButtonBackgroundColor.ToGdkColor());


            if (CurrentTab.Visible == false)
            {
                CurrentTab.ShowAll();
            }

            UpdateUI();
        }

        private int GetNextTabIndex()
        {
            return Tabs.IndexOf(Tabs.First(t => t.Index > CurrentTab.Index && t.ShowTab));
        }

        private bool CanMovePrevious()
        {
            return Tabs.Where(t => t.Index < CurrentTab.Index && t.ShowTab).Any();
        }

        private int GetPreviousTabIndex()
        {
            return Tabs.IndexOf(Tabs.Last(t => t.Index < CurrentTab.Index && t.ShowTab));
        }

        private void UpdateNavigatorButtons()
        {
            if (CurrentTab.Index == 0)
            {
                BtnPrevious.Sensitive = false;
                BtnNext.Sensitive = true;
            }
            else if (CurrentTab.Index == Tabs.Count - 1)
            {
                BtnPrevious.Sensitive = true;
                BtnNext.Sensitive = (CurrentTab.Index + 1 <= Tabs.Count - 1);
            }
            else
            {
                BtnPrevious.Sensitive = true;
                BtnNext.Sensitive = true;
            }
        }

    }
}
