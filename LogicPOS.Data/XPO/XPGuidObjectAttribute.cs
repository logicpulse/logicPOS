using System;

namespace LogicPOS.Data.XPO
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
        public bool Encrypted { get; set; }
    }
}
