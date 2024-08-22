using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.BackOffice.Dialogs.Configuration
{
    internal class DialogConfigurationWarehouse : EditDialog
    {
        private readonly ICollection<Tuple<fin_warehouselocation, Entry, BOWidgetBox, IconButton, IconButton, GenericCRUDWidgetXPO, HBox>> _warehouseLocationCollection;
        private fin_warehouse _Warehouse;
        private ScrolledWindow _scrolledWindow;
        private VBox vboxTab2;
        private readonly string iconAddRecord = string.Format("{0}{1}", PathsSettings.ImagesFolderLocation, @"Icons/icon_pos_nav_new.png");
        private readonly string iconClearRecord = string.Format("{0}{1}", PathsSettings.ImagesFolderLocation, @"Icons/Windows/icon_window_delete_record.png");

        public DialogConfigurationWarehouse(Window parentWindow, XpoGridView pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(parentWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(GeneralUtils.GetResourceByName("global_warehouse"));
            _warehouseLocationCollection = new List<Tuple<fin_warehouselocation, Entry, BOWidgetBox, IconButton, IconButton, GenericCRUDWidgetXPO, HBox>>();

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
                if (Entity == null) _Warehouse = new fin_warehouse();
                else _Warehouse = (Entity as fin_warehouse);
                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };
                //Ord
                Entry entryOrd = new Entry();
                BOWidgetBox boxOrd = new BOWidgetBox(GeneralUtils.GetResourceByName("global_record_order"), entryOrd);
                vboxTab1.PackStart(boxOrd, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxOrd, Entity, "Ord", RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(GeneralUtils.GetResourceByName("global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxCode, Entity, "Code", RegexUtils.RegexIntegerGreaterThanZero, true));

                //Designation
                Entry entryDesignation = new Entry();
                BOWidgetBox boxDesignation = new BOWidgetBox(GeneralUtils.GetResourceByName("global_designation"), entryDesignation);
                vboxTab1.PackStart(boxDesignation, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxDesignation, Entity, "Designation", RegexUtils.RegexAlfaNumericExtended, true));

                //Default
                CheckButton checkButtonDefault = new CheckButton(GeneralUtils.GetResourceByName("global_default_warehouse"));
                vboxTab1.PackStart(checkButtonDefault, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(checkButtonDefault, Entity, "IsDefault"));

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, Entity, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(GeneralUtils.GetResourceByName("global_record_main_detail")));

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
                        Entity getLocationFromWarehouse = XPOUtility.GetEntityById<fin_warehouselocation>(location.Oid);
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
                _notebook.AppendPage(_scrolledWindow, new Label(GeneralUtils.GetResourceByName("global_locations")));
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void PopulateWarehouseLocationEntrys(Entity pDataSourceRow)
        {
            try
            {
                //Dynamic SerialNumber
                if (pDataSourceRow == null)
                {
                    pDataSourceRow = new fin_warehouselocation(Entity.Session);
                }
                if ((pDataSourceRow as fin_warehouselocation).Warehouse == null) (pDataSourceRow as fin_warehouselocation).Warehouse = _Warehouse;
                HBox hboxLocation = new HBox(false, _boxSpacing);

                //Localização
                Entry entryLocation = new Entry();
                BOWidgetBox boxLocation = new BOWidgetBox(GeneralUtils.GetResourceByName("global_ConfigurationDevice_PlaceTerminal"), entryLocation);
                GenericCRUDWidgetXPO genericCRUDWidgetXPO = new GenericCRUDWidgetXPO(boxLocation, pDataSourceRow, "Designation", RegexUtils.RegexAlfaNumeric, true);
                InputFields.Add(genericCRUDWidgetXPO);
                hboxLocation.PackStart(boxLocation);

                //Apagar
                IconButton buttonClearLocation = new IconButton(
                    new ButtonSettings
                    {
                        Name = "touchButtonIcon",
                        Icon = iconClearRecord,
                        IconSize = new Size(15, 15),
                        ButtonSize = new Size(20, 15)
                    });

                hboxLocation.PackEnd(buttonClearLocation, false, false, 1);

                //Adicionar
                IconButton buttonAddLocation = new IconButton(
                    new ButtonSettings
                    {
                        Name = "touchButtonIcon",
                        Icon = iconAddRecord,
                        IconSize = new Size(15, 15),
                        ButtonSize = new Size(20, 15)
                    });

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
                _warehouseLocationCollection.Add(new Tuple<fin_warehouselocation, Entry, BOWidgetBox, IconButton, IconButton, GenericCRUDWidgetXPO, HBox>(pDataSourceRow as fin_warehouselocation, entryLocation, boxLocation, buttonClearLocation, buttonAddLocation, genericCRUDWidgetXPO, hboxLocation));

            }
            catch (Exception ex)
            {
                _logger.Error("Error populating Locations Entrys : " + ex.Message);
            }
        }

        private void ButtonClearLocation_Clicked(object sender, System.EventArgs e)
        {
            ResponseType responseType = SimpleAlerts.Question()
                                              .WithParent(this)
                                              .WithTitleResource("global_warning")
                                              .WithMessageResource("dialog_message_delete_record")
                                              .Show();

            if (responseType == ResponseType.Yes)
            {
                foreach (var location in _warehouseLocationCollection)
                {
                    if (_warehouseLocationCollection.Count == 1)
                    {
                        location.Item2.Text = "";
                        return;
                    }
                    else if (location.Item4.Equals(sender as IconButton))
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
                        InputFields.Remove(location.Item6);
                        _warehouseLocationCollection.Remove(location);
                        break;
                    }
                }
            }

        }
    }
}
