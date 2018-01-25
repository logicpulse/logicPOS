using DevExpress.Xpo.DB;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.App;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace logicpos.financial.library.Classes.Reports.BOs
{
    //Required to Implemented a IEnumerator Interface to Use in FastReport DataSources
    public class FRBOGenericCollection<T> : IEnumerable<T>
        //Generic Types Constrained to FRBOBaseObject BaseClass or FRBOBaseObject SubClass Objects (New)
      where T : FRBOBaseObject, new()
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool _debug = false;

        //Used to Store if Object Has Attributes Defined or Not
        bool _objectHaveAttributes;

        private List<T> _list = new List<T>();
        public List<T> List
        {
            get { return _list; }
            set { _list = value; }
        }

        //Constructors
        public FRBOGenericCollection() : this("", "", 0) { }
        public FRBOGenericCollection(string pFilter) : this(pFilter, "", 0) { }
        public FRBOGenericCollection(int pLimit) : this("", "", pLimit) { }
        public FRBOGenericCollection(string pFilter, int pLimit) : this(pFilter, "", pLimit) { }
        public FRBOGenericCollection(string pFilter, string pOrder) : this(pFilter, pOrder, 0) { }
        public FRBOGenericCollection(string pFilter, string pOrder, int pLimit)
        {
            //Assign Attributes Defined
            _objectHaveAttributes = ((typeof(T).GetCustomAttribute(typeof(FRBOAttribute)) as FRBOAttribute) != null) ? true : false;

            //Attributes: Sql
            string sqlQuery = (_objectHaveAttributes && (typeof(T).GetCustomAttribute(typeof(FRBOAttribute)) as FRBOAttribute).Sql != null)
                ? (typeof(T).GetCustomAttribute(typeof(FRBOAttribute)) as FRBOAttribute).Sql
                //If not SqlQuery Defined from Attributes, Create it From Reflection and FRBO Object Attributes
                : GenQueryFromFRBOObject(pFilter, pOrder);

            //Atributes: Limit
            int sqlLimit = 0;
            //Get Order From Parameters
            if (pLimit > 0)
            {
                sqlLimit = pLimit;
            }
            //Limit: Get Order From LimitAttribute else Bypass it With 0
            else if (_objectHaveAttributes && (typeof(T).GetCustomAttribute(typeof(FRBOAttribute)) as FRBOAttribute).Limit > 0)
            {
                sqlLimit = (typeof(T).GetCustomAttribute(typeof(FRBOAttribute)) as FRBOAttribute).Limit;
            }

            //Start Process Collection

            //Temporary GenericTypeObject to Add to Collection
            T genericTypeObject;
            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(sqlQuery);
            PropertyInfo propertyInfo;
            //Fields Props
            string fieldName = String.Empty;
            string fieldType = String.Empty;
            string fieldTypeDB = String.Empty;
            System.Object fieldValue;
            int fieldIndex;

            int i = 0;
            foreach (SelectStatementResultRow rowData in xPSelectData.Data)
            {
                i++;
                //If sqlLimit Defined and is Greater Break Loop
                if (sqlLimit > 0 && i > sqlLimit) break;

                //Create a new Fresh T Object to Insert into Collection
                //Cannot create an instance of the variable type 'T' because it does not have the new() constraint
                //Require Constrain new()
                genericTypeObject = new T();
                foreach (SelectStatementResultRow rowMeta in xPSelectData.Meta)
                {
                    fieldName = rowMeta.Values[0].ToString();
                    fieldTypeDB = rowMeta.Values[1].ToString(); ;
                    fieldType = rowMeta.Values[2].ToString(); ;
                    fieldIndex = xPSelectData.GetFieldIndex(fieldName);

                    //Convert DB Types, Else working on Diferent DBs occur Conversion Exceptions
                    switch (fieldType)
                    {
                        case "Int64":
                            fieldValue = Convert.ToInt32(rowData.Values[fieldIndex]);
                            break;
                        case "UInt64":
                            fieldValue = Convert.ToBoolean(rowData.Values[fieldIndex]);
                            break;
                        case "Boolean":
                            fieldValue = Convert.ToBoolean(rowData.Values[fieldIndex]);
                            break;
                        case "Guid":
                            fieldValue = Convert.ToString(rowData.Values[fieldIndex]);
                            break;
                        case "Decimal"://Added to fix MsSql Errors
                        case "Double":
                            fieldValue = Convert.ToDecimal(rowData.Values[fieldIndex]);
                            break;
                        default:
                            fieldValue = rowData.Values[fieldIndex];
                            break;
                    }

                    //If Property Exist in genericTypeObject Assign it from Data
                    propertyInfo = typeof(T).GetProperty(fieldName);

                    //Fix for MSSqlServer that detects UInt32 has Decimal, this way we convert it into UInt32 before above SetValue
                    if (propertyInfo.PropertyType == typeof(UInt32))
                    {
                        fieldValue = Convert.ToUInt32(rowData.Values[fieldIndex]);
                    }

                    try
                    {
                        if (propertyInfo != null) propertyInfo.SetValue(genericTypeObject, fieldValue);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(string.Format("fieldName: [{0}], fieldType: [{1}], fieldTypeDB: [{2}], fieldValue: [{3}]", fieldName, fieldType, fieldTypeDB, fieldValue));
                        _log.Error(ex.Message, ex);
                    }
                }
                //Add genericTypeObject to Collection :)
                Add(genericTypeObject);
            }
        }

        public void Add(T pObject)
        {
            _list.Add(pObject);
        }

        public T Get(int pIndex)
        {
            return _list[pIndex];
        }

        //Foreach Support
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        /// <summary>
        /// Generate Query Fields from FRBOObject Property Fields, used in GenQueryFromFRBOObject
        /// </summary>
        /// <returns>Fields used in Query</returns>
        public string GenQueryFieldsFromFRBOObject()
        {
            string resultFields;
            List<string> fieldList;

            //Do not use Declared Fields Flag else we skip inheited properties from Base Type (ex Oid,Disabled) //BindingFlags.DeclaredOnly
            PropertyInfo[] propertyInfos = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);//BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly
            string attributeField = null;
            bool attributeHide = false;
            string mappedFieldName = String.Empty;

            if (_debug) _log.Debug(string.Format("Class Name: {0}", typeof(T)));
            fieldList = new List<string>();
            foreach (PropertyInfo pInfo in propertyInfos)
            {
                //If not a generic Type ex a List, add it to FieldList
                if (!pInfo.PropertyType.IsGenericType /*&& pInfo.PropertyType.GetGenericTypeDefinition() != typeof(List<>)*/)
                {
                    //Attributes Working Block
                    //Always Reset attributeField and attributeHide
                    attributeField = null;
                    attributeHide = false;
                    //Assign Object Attributes "Field" and "Hide" into attributeField and attributeHide objects
                    var attributes = pInfo.GetCustomAttributes(false);
                    foreach (var attribute in attributes)
                    {
                        //Check if is Working on a FRBO Attribute  
                        if (attribute.GetType() == typeof(FRBOAttribute))
                        {
                            attributeField = ((attribute as FRBOAttribute).Field != null) ? Convert.ToString((attribute as FRBOAttribute).Field) : null;
                            attributeHide = ((attribute as FRBOAttribute).Hide == true) ? true : false;
                        }
                    }
                    if (_debug) _log.Debug(string.Format("Name: [{0}], PropertyType[{1}], attributeField[{2}], attributeHide[{3}]", pInfo.Name, pInfo.PropertyType, attributeField, attributeHide));

                    //If not a Hidden Property, Add it to filedList
                    if (attributeHide == false)
                    {
                        //Get Mapped FieldName (Field Attribute AS Property Name) ex Select "fmOid AS Oid" (QueryField AS FRBOPropertyField)
                        //or Not Mapped Name (Object Property Name) ex "Oid"
                        mappedFieldName = (attributeField != null) ? string.Format("{0} AS {1}", attributeField, pInfo.Name) : pInfo.Name;
                        //Add final FieldName from Attributes or Object Property
                        fieldList.Add(mappedFieldName);
                    }
                    else
                    {
                        //_log.Debug(string.Format("pInfo.Name[{0}].Hide =  [{1}] = ", pInfo.Name, attributeHide));
                    }
                }
            }

            //Finally Convert Generated fieldList into Comma Delimited, Ready to Query Database
            resultFields = string.Join(",", fieldList.ToArray());
            if (_debug) _log.Debug(string.Format("fields: [{0}]", resultFields));

            return resultFields;
        }

        /// <summary>
        /// Generate Query From FRBOObject Property Fields and Attributes
        /// </summary>
        /// <param name="Filter"></param>
        /// <param name="Order"></param>
        /// <returns>Database Query</returns>
        public string GenQueryFromFRBOObject(string pFilter = "", string pOrder = "")
        {
            //SqlEntity: Get Entity Name from FRBO EntityAttribute or From ClassName Without FRBO
            string sqlEntity = (_objectHaveAttributes && (typeof(T).GetCustomAttribute(typeof(FRBOAttribute)) as FRBOAttribute).Entity != null)
              ? (typeof(T).GetCustomAttribute(typeof(FRBOAttribute)) as FRBOAttribute).Entity
              : typeof(T).Name.ToString().Replace("FRBO", String.Empty);
            //Fields: Get Fields from FRBO FieldsAttribute or Generate it With GenQueryFieldsFromFRBOObject
            string sqlFields = (_objectHaveAttributes && (typeof(T).GetCustomAttribute(typeof(FRBOAttribute)) as FRBOAttribute).Fields != null)
              ? (typeof(T).GetCustomAttribute(typeof(FRBOAttribute)) as FRBOAttribute).Fields
              : GenQueryFieldsFromFRBOObject();

            //Filter
            string sqlFilter = String.Empty;
            //Get Filter From Parameters
            if (pFilter != String.Empty)
            {
                sqlFilter = string.Format(" WHERE ({0})", pFilter);
            }
            //Filter: Get Filter From FilterAttribute else Bypass it With Empty String
            else if (_objectHaveAttributes && (typeof(T).GetCustomAttribute(typeof(FRBOAttribute)) as FRBOAttribute).Filter != null)
            {
                sqlFilter = string.Format(" WHERE ({0})", (typeof(T).GetCustomAttribute(typeof(FRBOAttribute)) as FRBOAttribute).Filter);
            }

            //Order
            string sqlOrder = String.Empty;
            //Get Order From Parameters
            if (pOrder != String.Empty)
            {
                sqlOrder = string.Format(" ORDER BY {0}", pOrder);
            }
            //Order: Get Order From OrderAttribute else Bypass it With Empty String
            else if (_objectHaveAttributes && (typeof(T).GetCustomAttribute(typeof(FRBOAttribute)) as FRBOAttribute).Order != null)
            {
                sqlOrder = string.Format(" ORDER BY {0}", (typeof(T).GetCustomAttribute(typeof(FRBOAttribute)) as FRBOAttribute).Order);
            }

            //Finally Generate SqlQuery        
            string sqlQuery = string.Format("SELECT {0} FROM {1}{2}{3};", sqlFields, sqlEntity, sqlFilter, sqlOrder);
            if (_debug) _log.Debug(string.Format("sqlQuery: [{0}]", sqlQuery));

            return sqlQuery;
        }
    }
}