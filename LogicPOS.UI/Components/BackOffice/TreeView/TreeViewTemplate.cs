﻿using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using System;
using System.Collections.Generic;
using LogicPOS.Globalization;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO;
using LogicPOS.Domain.Entities;

//Note
//1) To disable navigator butons ex INS,DEL, use privileges

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewTemplate : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewTemplate() { }

        [Obsolete]
        public TreeViewTemplate(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        [Obsolete]
        public TreeViewTemplate(Window pSourceWindow, Entity pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(TemplateEntity);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            TemplateEntity defaultValue = (pDefaultValue != null) ? pDefaultValue as TemplateEntity : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : null;

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>
            {
                new GenericTreeViewColumnProperty("Code") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_code"), MinWidth = 100 },
                new GenericTreeViewColumnProperty("Designation") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), Expand = true }
            };
            //columnProperties.Add(new GenericTreeViewColumnProperty("Disabled") { Title = CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_disabled });

            //Configure Criteria/XPCollection/Model
            //CriteriaOperator.Parse("Code >= 100 and Code <= 9999");
            CriteriaOperator criteria;
            if (pXpoCriteria != null)
            {
                criteria = CriteriaOperator.Parse($"({pXpoCriteria}) AND (DeletedAt IS NULL)");
            }
            else
            {
                criteria = CriteriaOperator.Parse($"(DeletedAt IS NULL)");
            }
            SortProperty[] sortProperty = new SortProperty[1];
            sortProperty[0] = new SortProperty("Designation", SortingDirection.Ascending);
            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, xpoGuidObjectType, criteria, sortProperty);

            //Call Base Initializer
            base.InitObject(
              pSourceWindow,                  //Pass parameter 
              defaultValue,                   //Pass parameter
              pGenericTreeViewMode,           //Pass parameter
              pGenericTreeViewNavigatorMode,  //Pass parameter
              columnProperties,               //Created Here
              xpoCollection,                  //Created Here
              typeDialogClass                 //Created Here
            );

            //Protected Records
            //ProtectedRecords.Add(GUID);

            /*
            //Events Sample
            this.RecordBeforeDelete += TreeView_RecordBeforeDelete;
            this.RecordBeforeUpdate += TreeView_RecordBeforeUpdate;
            */
        }

        /*
        //Override Automatic Protected Records Events
        void TreeView_RecordBeforeDelete(object sender, EventArgs e)
        {
            //Prevent Delete Protected Records, assigning TreeView Base _skipRecordDelete
            _skipRecordDelete = (_protectedRecords.Count > 0 && _protectedRecords.Contains(_dataSourceRow.Oid));
            //Show Message
            if (_skipRecordDelete)
            {
                Utils.ShowMessageTouchProtectedDeleteRecordMessage(_sourceWindow);
            }
        }
          
        void TreeView_RecordBeforeUpdate(object sender, EventArgs e)
        {
            //Prevent Update Protected Records, assigning TreeView Base _skipRecordUpdate
            _skipRecordUpdate = (_protectedRecords.Count > 0 && _protectedRecords.Contains(_dataSourceRow.Oid));
            //Show Message
            if (_skipRecordUpdate)
            {
                Utils.ShowMessageTouchProtectedUpdateRecordMessage(_sourceWindow);
            }
        }
        */
    }
}
