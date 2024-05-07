using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.App;
using logicpos.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    /// <summary>
    /// Class for Pages Pad Component
    /// </summary>
    internal class PagePad : VBox, IEnumerable
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Private Fields
        private HBox _navigator;
        private readonly Color _colorPagePadHotButtonBackground = LogicPOS.Settings.GeneralSettings.Settings["colorPagePadHotButtonBackground"].StringToColor();

        public TouchButtonIconWithText ButtonPrev { get; private set; }
        public TouchButtonIconWithText ButtonNext { get; private set; }

        internal List<PagePadPage> Pages { get; set; }
        public int CurrentPageIndex { get; set; } = 0;

        internal PagePadPage ActivePage { get; set; }

        //Custom Events
        public event EventHandler PageChanged;

        //Paremeterless Constructor
        public PagePad() { }
        //Constructor
        public PagePad(List<PagePadPage> pPages)
        {
            Init(pPages);
        }

        public void Init(List<PagePadPage> pPages)
        {
            string fontPagePadNavigatorButton = LogicPOS.Settings.GeneralSettings.Settings["fontPagePadNavigatorButton"];
            Size sizePagesPadNavigatorButton = logicpos.Utils.StringToSize(LogicPOS.Settings.GeneralSettings.Settings["sizePagesPadNavigatorButton"]);
            Size sizePagesPadNavigatorButtonIcon = logicpos.Utils.StringToSize(LogicPOS.Settings.GeneralSettings.Settings["sizePagesPadNavigatorButtonIcon"]);
            string iconPrev = string.Format("{0}{1}", DataLayerFramework.Path["images"], @"Icons/icon_pos_pagepad_prev.png");
            string iconNext = string.Format("{0}{1}", DataLayerFramework.Path["images"], @"Icons/icon_pos_pagepad_next.png");

            //Parameters
            Pages = pPages;

            HBox navigatorButtons = new HBox(true, 0);
            ButtonPrev = new TouchButtonIconWithText("buttonPrev", _colorPagePadHotButtonBackground, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "pos_button_label_prev_pages_toolbar"), fontPagePadNavigatorButton, Color.White, iconPrev, sizePagesPadNavigatorButtonIcon, sizePagesPadNavigatorButton.Width, sizePagesPadNavigatorButton.Height) { Sensitive = false };
            ButtonNext = new TouchButtonIconWithText("buttonNext", _colorPagePadHotButtonBackground, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "pos_button_label_next_pages_toolbar"), fontPagePadNavigatorButton, Color.White, iconNext, sizePagesPadNavigatorButtonIcon, sizePagesPadNavigatorButton.Width, sizePagesPadNavigatorButton.Height);

            //Events
            ButtonPrev.Clicked += buttonPrev_Clicked;
            ButtonNext.Clicked += buttonNext_Clicked;

            //Render/Pack Navigator Buttons
            int i = 0;
            foreach (PagePadPage page in Pages)
            {
                i++;
                page.NavigatorButton = new TouchButtonIconWithText(page.PageName, Color.Transparent, page.PageName, fontPagePadNavigatorButton, Color.White, page.PageIcon, sizePagesPadNavigatorButtonIcon, 0, sizePagesPadNavigatorButton.Height);
                // Start Active Pad Button
                page.NavigatorButton.Sensitive = (i == 1);
                // Change color of current Button
                if ((i == 1)) page.NavigatorButton.ModifyBg(
                    StateType.Normal,
                    _colorPagePadHotButtonBackground.ToGdkColor());
                // Pack
                navigatorButtons.PackStart(page.NavigatorButton, true, true, 2);
            }

            //Final Navigator
            _navigator = new HBox(false, 0);
            _navigator.PackStart(ButtonPrev, false, false, 2);
            _navigator.PackStart(navigatorButtons, true, true, 2);
            _navigator.PackStart(ButtonNext, false, false, 2);
            //Pack Page Navigator
            PackStart(_navigator, false, false, 2);

            //Add ActivePage
            ActivePage = pPages[0];
            PackStart(ActivePage);
        }

        //Implement IEnumerable Interface
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Pages.GetEnumerator();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Override Methods

        public bool Validate()
        {
            return true;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        private void buttonPrev_Clicked(object sender, EventArgs e)
        {
            MovePage(false);
        }

        private void buttonNext_Clicked(object sender, EventArgs e)
        {
            if (ActivePage.Validated) MovePage(true);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        private void UpdateUI()
        {
            try
            {
                //_logger.Debug(string.Format("_pageCurrent: [{0}/{1}]", _currentPageIndex + 1, _pages.Count));

                if (CurrentPageIndex == 0)
                {
                    ButtonPrev.Sensitive = false;
                    ButtonNext.Sensitive = true;
                }
                else if (CurrentPageIndex == Pages.Count - 1)
                {
                    ButtonPrev.Sensitive = true;
                    //Enable if not in last page  && if next Page is Enabled
                    ButtonNext.Sensitive = ((CurrentPageIndex + 1 <= Pages.Count - 1) && (Pages[CurrentPageIndex + 1].Enabled));
                }
                else
                {
                    ButtonPrev.Sensitive = true;
                    ButtonNext.Sensitive = true;
                }

                //Call Event Here
                OnPageChanged();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Shared Method to Prev/Next Page
        /// </summary>
        /// <param name="Next"></param>
        private void MovePage(bool pNext)
        {
            try
            {
                this.Remove(ActivePage);
                ActivePage.NavigatorButton.Sensitive = false;

                if (pNext) { CurrentPageIndex++; } else { CurrentPageIndex--; };
                ActivePage = Pages[CurrentPageIndex];
                this.PackStart(ActivePage);
                ActivePage.NavigatorButton.Sensitive = true;
                // Change color of current Button
                ActivePage.NavigatorButton.ModifyBg(StateType.Normal, _colorPagePadHotButtonBackground.ToGdkColor());
                //The Trick to Show when Hidden, ex Not Packed in Dialog Expose, this way we need to ShowAll here
                if (ActivePage.Visible == false) ActivePage.ShowAll();
                UpdateUI();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Custom Events

        private void OnPageChanged()
        {
            PageChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
