using Gtk;
using logicpos.financial;
using System;
using System.Data;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    class GenericCRUDWidgetDataTable : GenericCRUDWidget<DataRow>
    {
        public GenericCRUDWidgetDataTable(Widget pWidget, DataRow pDataSourceRow, string pFieldName, string pValidationRule = "", bool pRequired = false)
            : base(pWidget, pDataSourceRow, pFieldName, pValidationRule, pRequired)
        {
            InitObject(pWidget, null, pDataSourceRow, pFieldName, pValidationRule, pRequired);
        }

        public GenericCRUDWidgetDataTable(Widget pWidget, Label pLabel, DataRow pDataSourceRow, string pFieldName, string pValidationRule = "", bool pRequired = false)
            : base(pWidget, pLabel, pDataSourceRow, pFieldName, pValidationRule, pRequired)
        {
            InitObject(pWidget, pLabel, pDataSourceRow, pFieldName, pValidationRule, pRequired);
        }

        public override object GetMemberValue()
        {
            return DataSourceRow[_fieldName];
        }

        public override void SetMemberValue(object pValue)
        {
            DataSourceRow[_fieldName] = pValue;
        }

        public override void SetMembersProperties(DataRow pDataSourceRow)
        {
            _fieldType = pDataSourceRow[_fieldName].GetType();
        }
    }
}
