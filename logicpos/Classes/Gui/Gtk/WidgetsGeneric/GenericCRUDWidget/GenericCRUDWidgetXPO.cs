using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    class GenericCRUDWidgetXPO : GenericCRUDWidget<XPGuidObject>
    {
        private bool _hasXPGuidObjectValue;

        public GenericCRUDWidgetXPO(Widget pWidget, XPGuidObject pDataSourceRow, String pFieldName, String pValidationRule = "", bool pRequired = false)
            : base(pWidget, pDataSourceRow, pFieldName, pValidationRule, pRequired)
        {
            InitObject(pWidget, null, pDataSourceRow, pFieldName, pValidationRule, pRequired);
        }

        public GenericCRUDWidgetXPO(BOWidgetBox pBOWidgetBox, XPGuidObject pDataSourceRow, String pFieldName, String pValidationRule = "", bool pRequired = false)
            : base(pBOWidgetBox.WidgetComponent, pBOWidgetBox.LabelComponent, pDataSourceRow, pFieldName, pValidationRule, pRequired)
        {
            InitObject(pBOWidgetBox.WidgetComponent, pBOWidgetBox.LabelComponent, pDataSourceRow, pFieldName, pValidationRule, pRequired);
        }

        public GenericCRUDWidgetXPO(Widget pWidget, Label pLabel, XPGuidObject pDataSourceRow, String pFieldName, String pValidationRule = "", bool pRequired = false)
            : base(pWidget, pLabel, pDataSourceRow, pFieldName, pValidationRule, pRequired)
        {
            InitObject(pWidget, pLabel, pDataSourceRow, pFieldName, pValidationRule, pRequired);
        }

        public override object GetMemberValue()
        {
            return DataSourceRow.GetMemberValue(_fieldName);
        }

        public override void SetMemberValue(object pValue)
        {
            try
            {
                DataSourceRow.SetMemberValue(_fieldName, pValue);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public override void SetMembersProperties(XPGuidObject pDataSourceRow)
        {
            //Reflection Source
            Type sourceType = _dataSourceRow.GetType();

            IList<PropertyInfo> props = new List<PropertyInfo>(sourceType.GetProperties());
            foreach (PropertyInfo prop in props)
            {
                if (prop.Name == _fieldName)
                {
                    _fieldProperty = prop;

                    //Return GetUnderlyingType when is Nullable ex Int16? | [System.Nullable1[System.Int16]]
                    if (FrameworkUtils.IsNullable(prop.PropertyType))
                    {
                        //Required Nullable.GetUnderlyingType, else Type is item.FieldType:[System.Nullable1[System.Int16]], this returns item.FieldType:[System.Int16]
                        _fieldType = Nullable.GetUnderlyingType(prop.PropertyType);
                    }
                    //Return Default Type when is Not Nullable ex Int16 | [System.Int16]
                    else
                    {
                        _fieldType = prop.PropertyType;
                        //If Not Nullable, for sure it is Required :)
                        //if (FieldType != typeof(System.String)) Required = true;
                    };
                    _fieldValue = prop.GetValue(_dataSourceRow, null);
                    _hasXPGuidObjectValue = (_fieldType.BaseType.Name == "XPGuidObject");
                    //_log.Debug(string.Format("prop.Name:[{0}] FieldType:[{1}] FieldType.BaseType.Name:[{2}] Value:[{3}] _hasXPGuidObjectValue:[{4}]", prop.Name, FieldType, FieldType.BaseType.Name, _fieldValue, _hasXPGuidObjectValue));
                }
            }
        }
    }
}
