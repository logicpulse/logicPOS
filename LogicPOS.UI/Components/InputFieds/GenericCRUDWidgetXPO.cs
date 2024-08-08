using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using LogicPOS.Domain.Entities;
using LogicPOS.UI.Components.InputFieds;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    internal class GenericCRUDWidgetXPO : InputField<Entity>
    {
        private bool _hasXPGuidObjectValue;

        public GenericCRUDWidgetXPO(Widget widget,
                                    Entity entity,
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

        public GenericCRUDWidgetXPO(BOWidgetBox verticalBox,
                                    Entity entity,
                                    string fieldName,
                                    string validationRule = "",
                                    bool required = false)
            : base(verticalBox.WidgetComponent,
                   entity,
                   fieldName,
                   verticalBox.LabelComponent,
                   validationRule,
                   required)
        {
            Build(verticalBox.WidgetComponent,
                       verticalBox.LabelComponent,
                       entity,
                       fieldName,
                       validationRule,
                       required);
        }

        public GenericCRUDWidgetXPO(Widget widget,
                                    Label label,
                                    Entity entity,
                                    string fieldName,
                                    string validationRule = "",
                                    bool required = false)
            : base(widget,
                   entity,
                   fieldName,
                   label,
                   validationRule,
                   required)
        {
            Build(widget,
                       label,
                       entity,
                       fieldName,
                       validationRule,
                       required);
        }

        public override object GetFieldValueFromEntity()
        {
            return Entity.GetMemberValue(FieldName);
        }

        public override void SetMemberValue(object value)
        {

            Entity.SetMemberValue(FieldName, value);
        }

        public override void SetMembersProperties(Entity entity)
        {
            Type sourceType = Entity.GetType();

            IList<PropertyInfo> props = new List<PropertyInfo>(sourceType.GetProperties());
            foreach (PropertyInfo prop in props)
            {
                if (prop.Name == FieldName)
                {
                    FieldProperty = prop;

                    if (GeneralUtils.IsNullable(prop.PropertyType))
                    {
                        FieldType = Nullable.GetUnderlyingType(prop.PropertyType);
                    }
                    else
                    {
                        FieldType = prop.PropertyType;
                    };
                    FieldValue = prop.GetValue(Entity, null);
                    _hasXPGuidObjectValue = (FieldType.BaseType.Name == "XPGuidObject");
                }
            }
        }
    }
}
