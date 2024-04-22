using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.App;
using logicpos.Extensions;
using logicpos.shared.App;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

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
        private readonly Color _colorPagePadHotButtonBackground = DataLayerFramework.Settings["colorPagePadHotButtonBackground"].StringToColor();

        //Public Properties
        private TouchButtonIconWithText _buttonPrev;
        public TouchButtonIconWithText ButtonPrev
        {
            get { return _buttonPrev; }
        }
        private TouchButtonIconWithText _buttonNext;
        public TouchButtonIconWithText ButtonNext
        {
            get { return _buttonNext; }
        }

        private List<PagePadPage> _pages;
        internal List<PagePadPage> Pages
        {
            get { return _pages; }
            set { _pages = value; }
        }
        private int _currentPageIndex = 0;
        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set { _currentPageIndex = value; }
        }

        private PagePadPage _activePage;
        internal PagePadPage ActivePage
        {
            get { return _activePage; }
            set { _activePage = value; }
        }

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
            string fontPagePadNavigatorButton = DataLayerFramework.Settings["fontPagePadNavigatorButton"];
            Size sizePagesPadNavigatorButton = logicpos.Utils.StringToSize(DataLayerFramework.Settings["sizePagesPadNavigatorButton"]);
            Size sizePagesPadNavigatorButtonIcon = logicpos.Utils.StringToSize(DataLayerFramework.Settings["sizePagesPadNavigatorButtonIcon"]);
            string iconPrev = SharedUtils.OSSlash(string.Format("{0}{1}", DataLayerFramework.Path["images"], @"Icons/icon_pos_pagepad_prev.png"));
            string iconNext = SharedUtils.OSSlash(string.Format("{0}{1}", DataLayerFramework.Path["images"], @"Icons/icon_pos_pagepad_next.png"));

            //Parameters
            _pages = pPages;

            HBox navigatorButtons = new HBox(true, 0);
            _buttonPrev = new TouchButtonIconWithText("buttonPrev", _colorPagePadHotButtonBackground, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "pos_button_label_prev_pages_toolbar"), fontPagePadNavigatorButton, Color.White, iconPrev, sizePagesPadNavigatorButtonIcon, sizePagesPadNavigatorButton.Width, sizePagesPadNavigatorButton.Height) { Sensitive = false };
            _buttonNext = new TouchButtonIconWithText("buttonNext", _colorPagePadHotButtonBackground, resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "pos_button_label_next_pages_toolbar"), fontPagePadNavigatorButton, Color.White, iconNext, sizePagesPadNavigatorButtonIcon, sizePagesPadNavigatorButton.Width, sizePagesPadNavigatorButton.Height);

            //Events
            _buttonPrev.Clicked += buttonPrev_Clicked;
            _buttonNext.Clicked += buttonNext_Clicked;

            //Render/Pack Navigator Buttons
            int i = 0;
            foreach (PagePadPage page in _pages)
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
            _navigator.PackStart(_buttonPrev, false, false, 2);
            _navigator.PackStart(navigatorButtons, true, true, 2);
            _navigator.PackStart(_buttonNext, false, false, 2);
            //Pack Page Navigator
            PackStart(_navigator, false, false, 2);

            //Add ActivePage
            _activePage = pPages[0];
            PackStart(_activePage);
        }

        //Implement IEnumerable Interface
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _pages.GetEnumerator();
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
            if (_activePage.Validated) MovePage(true);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        private void UpdateUI()
        {
            try
            {
                //_logger.Debug(string.Format("_pageCurrent: [{0}/{1}]", _currentPageIndex + 1, _pages.Count));

                if (_currentPageIndex == 0)
                {
                    _buttonPrev.Sensitive = false;
                    _buttonNext.Sensitive = true;
                }
                else if (_currentPageIndex == _pages.Count - 1)
                {
                    _buttonPrev.Sensitive = true;
                    //Enable if not in last page  && if next Page is Enabled
                    _buttonNext.Sensitive = ((_currentPageIndex + 1 <= _pages.Count - 1) && (_pages[_currentPageIndex + 1].Enabled));
                }
                else
                {
                    _buttonPrev.Sensitive = true;
                    _buttonNext.Sensitive = true;
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
                this.Remove(_activePage);
                _activePage.NavigatorButton.Sensitive = false;

                if (pNext) { _currentPageIndex++; } else { _currentPageIndex--; };
                _activePage = _pages[_currentPageIndex];
                this.PackStart(_activePage);
                _activePage.NavigatorButton.Sensitive = true;
                // Change color of current Button
                _activePage.NavigatorButton.ModifyBg(StateType.Normal, _colorPagePadHotButtonBackground.ToGdkColor());
                //The Trick to Show when Hidden, ex Not Packed in Dialog Expose, this way we need to ShowAll here
                if (_activePage.Visible == false) _activePage.ShowAll();
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
