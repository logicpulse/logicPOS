using System;
using System.Data;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    internal class GenericCRUDWidgetListDataTable : GenericCRUDWidgetList<DataRow>
    {
        public override bool Save()
        {
            bool result = false;

            try
            {
                //_logger.Debug("GenericCRUDWidgetListDataTable: Save()");
                //foreach (GenericCRUDWidget<DataRow> item in this)
                //{
                //  _logger.Debug(string.Format("Field: [{0} | {1} | {2}] == [{3}]", item.FieldName, item.FieldType, item.Label.Text, item.GetMemberValue()));
                //}
                result = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }
    }
}
