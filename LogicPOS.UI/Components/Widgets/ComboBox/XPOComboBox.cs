﻿using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using System;
using System.Collections.Generic;
using LogicPOS.Globalization;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;

//TODO : Implement Required Outside CrudWidgetList :  Add Required and Use SettingsApp.RegexGuid to Validate

namespace logicpos.Classes.Gui.Gtk.WidgetsXPO
{
    internal class XPOComboBox : ComboBox
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Private Members to Store Parameters
        private Session _xpoSession;
        private Type _xpoObjectType;
        private string _fieldLabel;

        //Private Members
        private XPCollection _XpCollection;
        private Dictionary<Guid, TreeIter> _treeInterDictionary;
        private ListStore _comboBoxListStore;

        public Entity Value { get; set; }

        [Obsolete]
        public XPOComboBox(Session pXpoSession, Type pXPGuidObjectType, Entity pCurrentValue, string pFieldLabel, CriteriaOperator pCriteria)
        {
            InitComboBox(pXpoSession, pXPGuidObjectType, pCurrentValue, pFieldLabel, pCriteria);
        }

        [Obsolete]
        public XPOComboBox(Session pXpoSession, Type pXPGuidObjectType, Entity pCurrentValue, string pFieldLabel, CriteriaOperator pCriteria, SortProperty[] pSortProperty = null)
        {
            InitComboBox(pXpoSession, pXPGuidObjectType, pCurrentValue, pFieldLabel, pCriteria, pSortProperty);
        }

        //IN:009261 Overload for default value selected
        [Obsolete]
        public XPOComboBox(Session pXpoSession, Type pXPGuidObjectType, Entity pCurrentValue, string pFieldLabel, CriteriaOperator pCriteria, SortProperty[] pSortProperty = null, int active = 0, XPCollection pXPCollection = null)
        {
            InitComboBox(pXpoSession, pXPGuidObjectType, pCurrentValue, pFieldLabel, pCriteria, pSortProperty, active, pXPCollection);
        }

        [Obsolete]
        public void InitComboBox(Session pXpoSession, Type pXPGuidObjectType, Entity pCurrentValue, string pFieldLabel, CriteriaOperator pCriteria, SortProperty[] pSortProperty = null, int active = 0, XPCollection pXPCollection = null)
        {
            //Required to Force Combo to be same Height has Entrys
            HeightRequest = 23;

            //Parameters
            _xpoSession = pXpoSession;
            _xpoObjectType = pXPGuidObjectType;
            _fieldLabel = pFieldLabel;



            // Override Criteria with hide Undefined Records Criteria
            // Tip : Leave == null here and ignore warnings/suggestions, else it wont work as expected
            if (pCriteria == null)
            {
                //Get PriceType Collection : Require Criteria to exclude SettingsApp.XpoOidUndefinedRecord, else we get a Price0 here
                //pCriteria = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1) OR (Oid <> '{0}')", SettingsApp.XpoOidUndefinedRecord));
                pCriteria = CriteriaOperator.Parse(string.Format("(Oid <> '{0}')", XPOSettings.XpoOidUndefinedRecord));
            }

            //Init Collection  based on xpoObjectType
            if (pXPCollection != null) _XpCollection = pXPCollection;
            else _XpCollection = new XPCollection(_xpoSession, _xpoObjectType, pCriteria, pSortProperty);

            //Init CellRenderer
            CellRendererText comboBoxCell = new CellRendererText();
            PackStart(comboBoxCell, true);
            AddAttribute(comboBoxCell, "text", 0);

            //Create/Update Model
            CreateModel(_XpCollection, pCurrentValue);

            //Events
            Changed += XPOComboBox_Changed;

            //IN:009261 Set Default selected item
            if (active != 0) Active = active;
        }

        public void UpdateModel(CriteriaOperator pCriteria, SortProperty pSortProperty = null)
        {
            XPCollection xpCollection = new XPCollection(_xpoSession, _xpoObjectType, pCriteria);
            CreateModel(xpCollection, null, pSortProperty);
        }

        public void UpdateModel(CriteriaOperator pCriteria, Entity pCurrentValue, SortProperty pSortProperty = null)
        {
            XPCollection xpCollection = new XPCollection(_xpoSession, _xpoObjectType, pCriteria);
            CreateModel(xpCollection, pCurrentValue, pSortProperty);
        }

        public void CreateModel(XPCollection pXpCollection, Entity pCurrentValue, SortProperty pSortProperty = null)
        {
            //Local Variables
            TreeIter tempItemIter;
            TreeIter currentItemIter = new TreeIter();

            //Parameters
            _XpCollection = pXpCollection;

            //Add sort property
            if (pSortProperty != null) _XpCollection.Sorting.Add(pSortProperty);

            //Add Default Sorting Order, if Not Assigned by Parameter
            if (_XpCollection.Sorting.Count == 0) _XpCollection.Sorting = XPOUtility.GetXPCollectionDefaultSortingCollection();

            //Store TreeIters in Dictionary
            _treeInterDictionary = new Dictionary<Guid, TreeIter>();

            //Init ListStore Model
            _comboBoxListStore = new ListStore(typeof(string), typeof(Entity));

            //Aways Default to Null Value - Undefined, even if Collection is Empty
            tempItemIter = _comboBoxListStore.AppendValues(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "widget_combobox_undefined"), null);
            _treeInterDictionary.Add(new Guid(), tempItemIter);
            //Default Selected
            currentItemIter = tempItemIter;

            //Start Processing Collection
            if (_XpCollection.Count > 0)
            {
                //Create Model
                foreach (Entity item in _XpCollection)
                {
                    //Console.WriteLine("fieldLabel: {0}, fieldValue: {1} fieldValue.Oid: {2}", _fieldLabel, item.GetMemberValue(_fieldLabel), item.Oid);
                    tempItemIter = _comboBoxListStore.AppendValues(item.GetMemberValue(_fieldLabel), item);
                    _treeInterDictionary.Add(new Guid(Convert.ToString(item.GetMemberValue("Oid"))), tempItemIter);

                    //Detected Current/Selected Value
                    if (pCurrentValue != null && pCurrentValue == item)
                    {
                        currentItemIter = tempItemIter;
                        //Always assign Active Value (XPGuidObject) to Value Property
                        Value = item;
                    };
                };
            };
            //Always Update Model and ActiveIter, even is Collection is Empty, example it always have Null Default Value in Model
            Model = _comboBoxListStore;
            SetActiveIter(currentItemIter);
        }

        private void XPOComboBox_Changed(object sender, EventArgs e)
        {
            XPOComboBox combo = sender as XPOComboBox;
            if (sender == null) return;

            TreeIter iter;

            if (combo.GetActiveIter(out iter))
            {
                Value = (Entity)combo.Model.GetValue(iter, 1);
            };
        }
    }
}
