using System;

namespace logicpos.datalayer.DataLayer
{
    //Declare FRBOAttribute Class
    //a custom attribute FRBOAttribute to be assigned to a class and its members
    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Property,
        AllowMultiple = true,
        // Prevent Subclasses Inheritance else
        // [ERROR] Foram encontrados vários atributos personalizados do mesmo tipo.
        Inherited = false
        )]
    public class XPGuidObjectAttribute : Attribute
    {
        // Valid on String Type Only, we need a String field to Store Encrypted Base64 Strings Like 'ng7cy5csklaIUIanFeOP7Q=='
        private bool _encrypted;
        public bool Encrypted { get => _encrypted; set => _encrypted = value; }
    }
}
