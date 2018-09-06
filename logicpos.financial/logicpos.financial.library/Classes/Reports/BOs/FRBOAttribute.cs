using System;

namespace logicpos.financial.library.Classes.Reports.BOs
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

        /// <summary>
        /// Use in Class
        /// Manually Define SQL, if Used Ignore Above Options Entity, Fields, Filter, Order, Limit
        /// </summary>
        private string _sql;
        public string Sql
        {
            get { return _sql; }
            set { _sql = value; }
        }

        /// <summary>
        /// Use in Class
        /// Map Database Entity Object. ex TableName, View Name, To Bypass default Entity name from [FRBO]EntityName Objects
        /// ex [FRBO(Entity="view_documentfinance")]
        /// </summary>
        private string _entity;
        public string Entity
        {
            get { return _entity; }
            set { _entity = value; }
        }

        /// <summary>
        /// Use in Class
        /// Define Select Fields
        /// ex [FRBO(Fields="Ord, Code, Oid...")]
        /// ex [FRBO(Fields="fmOrd AS Ord, fmCode AS Code, fmOid AS Oid...")]
        /// </summary>
        private string _fields;
        public string Fields
        {
            get { return _fields; }
            set { _fields = value; }
        }

        /// <summary>
        /// Use in Class
        /// Define Where
        /// ex [FRBO(Filter="Code=10, ...")]
        /// </summary>
        private string _filter;
        public string Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        /// <summary>
        /// Use in Class
        /// Define Group By
        /// ex [FRBO(Group="Date, Type, ...")]
        /// </summary>
        private string _group;
        public string Group
        {
            get { return _group; }
            set { _group = value; }
        }

        /// <summary>
        /// Use in Class
        /// Define Order By
        /// ex [FRBO(Order="Date, Type, ...")]
        /// </summary>
        private string _order;
        public string Order
        {
            get { return _order; }
            set { _order = value; }
        }

        /// <summary>
        /// Use in Class
        /// Define a Limit, Usefull when work with Views, To Limit to 1 etc
        /// </summary>
        private int _limit;
        public int Limit
        {
            get { return _limit; }
            set { _limit = value; }
        }

        /// <summary>
        /// Use in Properties
        /// Map to Database Field, ex Map fmDocumentNumber field of View to DocumentNumber Object Field
        /// We can Combine Fields with This
        /// ex [FRBO(Field="CONCAT(FirstName + "" + LastName)")]
        /// ex [FRBO(Field="(fmTotalFinal - fmTotalGross)")]
        /// </summary>
        private string _field;
        public string Field
        {
            get { return _field; }
            set { _field = value; }
        }

        /// <summary>
        /// Use in Properties
        /// Exclude this Field from Query, If True this object is Exclude in GenQueryFieldsFromFRBOObject Method
        /// </summary>
        private bool _hide;
        public bool Hide
        {
            get { return _hide; }
            set { _hide = value; }
        }
    }
}
