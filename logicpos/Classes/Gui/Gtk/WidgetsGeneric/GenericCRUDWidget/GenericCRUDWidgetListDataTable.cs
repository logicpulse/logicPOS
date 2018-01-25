using System;
using System.Data;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    class GenericCRUDWidgetListDataTable : GenericCRUDWidgetList<DataRow>
    {
        public override bool Save()
        {
            bool result = false;

            try
            {
                //_log.Debug("GenericCRUDWidgetListDataTable: Save()");
                //foreach (GenericCRUDWidget<DataRow> item in this)
                //{
                //  _log.Debug(string.Format("Field: [{0} | {1} | {2}] == [{3}]", item.FieldName, item.FieldType, item.Label.Text, item.GetMemberValue()));
                //}
                result = true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }
    }
}
