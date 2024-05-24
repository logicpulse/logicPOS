using System;

namespace LogicPOS.Reporting.BOs
{
    //Declare FRBOAttribute Class
    //a custom attribute FRBOAttribute to be assigned to a class and its members
    [AttributeUsage(AttributeTargets.Class |
        //AttributeTargets.Constructor |
        //AttributeTargets.Field |
        //AttributeTargets.Method |
        AttributeTargets.Property,
        AllowMultiple = true,
        // Prevent Subclasses Inheritance else
        // [ERROR] Foram encontrados vários atributos personalizados do mesmo tipo.
        Inherited = false
        )]
    public class FRBOAttribute : Attribute
    {
        public string Sql { get; set; }

        public string Entity { get; set; }

        public string Fields { get; set; }

        public string Filter { get; set; }

        public string Group { get; set; }

        public string Order { get; set; }

        public int Limit { get; set; }

        public string Field { get; set; }

        public bool Hide { get; set; }
    }
}
