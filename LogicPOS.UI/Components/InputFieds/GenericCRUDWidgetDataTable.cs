using Gtk;
using LogicPOS.UI.Components.InputFieds;
using System.Data;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    internal class GenericCRUDWidgetDataTable : InputField<DataRow>
    {
        public GenericCRUDWidgetDataTable(Widget widget,
                                          DataRow entity,
                                          string fieldName,
                                          string validationRule = "",
                                          bool required = false)
            : base(widget,
                   entity,
                   fieldName,
                   validationRule: validationRule,
                   required: required)
        {
            Build(widget,
                       null,
                       entity,
                       fieldName,
                       validationRule,
                       required);
        }

        public GenericCRUDWidgetDataTable(Widget widget,
                                          Label label,
                                          DataRow entity,
                                          string fieldName,
                                          string ValidationRule = "",
                                          bool required = false)
            : base(widget,
                   entity,
                   fieldName,
                   label,
                   ValidationRule,
                   required)
        {
            Build(widget,
                       label,
                       entity,
                       fieldName,
                       ValidationRule,
                       required);
        }

        public override object GetFieldValueFromEntity()
        {
            return Entity[FieldName];
        }

        public override void SetMemberValue(object value)
        {
            Entity[FieldName] = value;
        }

        public override void SetMembersProperties(DataRow entity)
        {
            FieldType = entity[FieldName].GetType();
        }
    }
}
