using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Documents;
using logicpos.datalayer.Xpo;
using logicpos.Extensions;
using LogicPOS.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;

namespace logicpos.Classes.Gui.Gtk.BackOffice.Dialogs.Configuration
{
    internal class DialogConfigurationWarehouse : BOBaseDialog
    {
        private readonly ICollection<Tuple<fin_warehouselocation, Entry, BOWidgetBox, TouchButtonIcon, TouchButtonIcon, GenericCRUDWidgetXPO, HBox>> _warehouseLocationCollection;
        private fin_warehouse _Warehouse;
        private ScrolledWindow _scrolledWindow;
        private VBox vboxTab2;
        private readonly string iconAddRecord = string.Format("{0}{1}", GeneralSettings.Paths["images"], @"Icons/icon_pos_nav_new.png");
        private readonly string iconClearRecord = string.Format("{0}{1}", GeneralSettings.Paths["images"], @"Icons/Windows/icon_window_delete_record.png");

        public DialogConfigurationWarehouse(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_warehouse"));
            _warehouseLocationCollection = new List<Tuple<fin_warehouselocation, Entry, BOWidgetBox, TouchButtonIcon, TouchButtonIcon, GenericCRUDWidgetXPO, HBox>>();
            
            SetSizeRequest(500, 450);
            InitUI();
            InitNotes();
            ShowAll();
        }

        private void InitUI()
        {
            try
            {
                string lastArticleCode = "0";
                try
                {
                    //IN009261 BackOffice - Inserir mais auto-completes nos forms
                    if (DatabaseSettings.DatabaseType.ToString() == "MSSqlServer")
                    {
                        string lastArticleSql = string.Format("SELECT MAX(CAST(Code AS INT))FROM fin_warehouse");
                        lastArticleCode = XPOSettings.Session.ExecuteScalar(lastArticleSql).ToString();
                    }
                    else if (DatabaseSettings.DatabaseType.ToString() == "SQLite")
                    {
                        string lastArticleSql = string.Format("SELECT MAX(CAST(Code AS INT))FROM fin_warehouse");
                        lastArticleCode = XPOSettings.Session.ExecuteScalar(lastArticleSql).ToString();
                    }
                    else if (DatabaseSettings.DatabaseType.ToString() == "MySql")
                    {
                        string lastArticleSql = string.Format("SELECT MAX(CAST(code AS UNSIGNED)) as Cast FROM fin_warehouse;");
                        lastArticleCode = XPOSettings.Session.ExecuteScalar(lastArticleSql).ToString();
                    }

                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
                if (_dataSourceRow == null) _Warehouse = new fin_warehouse();
                else _Warehouse = (_dataSourceRow as fin_warehouse);
                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };
                //Ord
                Entry entryOrd = new Entry();
                BOWidgetBox boxOrd = new BOWidgetBox(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxOrd, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxOrd, _dataSourceRow, "Ord", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDesignation, _dataSourceRow, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //Default
                CheckButton checkButtonDefault = new CheckButton(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_default_warehouse"));
                vboxTab1.PackStart(checkButtonDefault, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDefault, _dataSourceRow, "IsDefault"));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_record_main_detail")));

                //Tab1
                vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                _scrolledWindow = new ScrolledWindow();
                _scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
                _scrolledWindow.ModifyBg(StateType.Normal, Color.White.ToGdkColor());
                _scrolledWindow.ShadowType = ShadowType.None;

                if (_Warehouse != null && _Warehouse.WarehouseLocation.Count > 0)
                {
                    foreach (var location in _Warehouse.WarehouseLocation)
                    {
                        XPGuidObject getLocationFromWarehouse = XPOHelper.GetXPGuidObject(typeof(fin_warehouselocation), location.Oid);
                        PopulateWarehouseLocationEntrys(getLocationFromWarehouse);
                    }
                }
                else
                {
                    PopulateWarehouseLocationEntrys(null);
                }
                int lcode = 0;
                lcode = Convert.ToInt32(lastArticleCode.ToString()) + 10;
                if (lcode != 10 && entryCode.Text == "") { entryOrd.Text = lcode.ToString(); entryCode.Text = lcode.ToString(); }

                //Append Tab
                _notebook.AppendPage(_scrolledWindow, new Label(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_locations")));
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void PopulateWarehouseLocationEntrys(XPGuidObject pDataSourceRow)
        {
            try
            {
                //Dynamic SerialNumber
                if (pDataSourceRow == null)
                {
                    pDataSourceRow = new fin_warehouselocation(_dataSourceRow.Session);
                }
                if ((pDataSourceRow as fin_warehouselocation).Warehouse == null) (pDataSourceRow as fin_warehouselocation).Warehouse = _Warehouse;
                HBox hboxLocation = new HBox(false, _boxSpacing);

                //Localização
                Entry entryLocation = new Entry();
                BOWidgetBox boxLocation = new BOWidgetBox(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_ConfigurationDevice_PlaceTerminal"), entryLocation);
                GenericCRUDWidgetXPO genericCRUDWidgetXPO = new GenericCRUDWidgetXPO(boxLocation, pDataSourceRow, "Designation", LogicPOS.Utility.RegexUtils.RegexAlfaNumeric, true);
                _crudWidgetList.Add(genericCRUDWidgetXPO);
                hboxLocation.PackStart(boxLocation);

                //Apagar
                TouchButtonIcon buttonClearLocation = new TouchButtonIcon("touchButtonIcon", Color.Transparent, iconClearRecord, new Size(15, 15), 20, 15);
                hboxLocation.PackEnd(buttonClearLocation, false, false, 1);

                //Adicionar
                TouchButtonIcon buttonAddLocation = new TouchButtonIcon("touchButtonIcon", Color.Transparent, iconAddRecord, new Size(15, 15), 20, 15);
                hboxLocation.PackEnd(buttonAddLocation, false, false, 1);

                vboxTab2.PackStart(hboxLocation, false, false, 0);
                _scrolledWindow.Add(vboxTab2);

                //Events
                buttonAddLocation.Clicked += delegate
                {
                    PopulateWarehouseLocationEntrys(null);
                };
                buttonClearLocation.Clicked += ButtonClearLocation_Clicked; ;
                vboxTab2.ShowAll();

                //Add to collection
                _warehouseLocationCollection.Add(new Tuple<fin_warehouselocation, Entry, BOWidgetBox, TouchButtonIcon, TouchButtonIcon, GenericCRUDWidgetXPO, HBox>(pDataSourceRow as fin_warehouselocation, entryLocation, boxLocation, buttonClearLocation, buttonAddLocation, genericCRUDWidgetXPO, hboxLocation));

            }
            catch (Exception ex)
            {
                _logger.Error("Error populating Locations Entrys : " + ex.Message);
            }
        }

        private void ButtonClearLocation_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                ResponseType responseType = logicpos.Utils.ShowMessageNonTouch(this, DialogFlags.DestroyWithParent, MessageType.Question, ButtonsType.YesNo, CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "dialog_message_delete_record"), string.Format(CultureResources.GetResourceByLanguage(GeneralSettings.Settings.GetCultureName(), "global_warning"), GeneralSettings.ServerVersion));

                if (responseType == ResponseType.Yes)
                {
                    foreach (var location in _warehouseLocationCollection)
                    {
                        if (_warehouseLocationCollection.Count == 1)
                        {
                            location.Item2.Text = "";
                            return;
                        }
                        else if (location.Item4.Equals(sender as TouchButtonIcon))
                        {
                            var xpObject = location.Item1;
                            xpObject.Delete();
                            var xpEntry = location.Item2;
                            var xpBoxWidget = location.Item3;
                            xpBoxWidget.Hide();
                            var xpButtonClear = location.Item4;
                            xpButtonClear.Hide();
                            var xpButtonAdd = location.Item5;
                            xpButtonAdd.Hide();
                            vboxTab2.Remove(location.Item7);
                            _crudWidgetList.Remove(location.Item6);
                            _warehouseLocationCollection.Remove(location);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error clear warehouse location Entrys : " + ex.Message);
            }
        }
    }
}
