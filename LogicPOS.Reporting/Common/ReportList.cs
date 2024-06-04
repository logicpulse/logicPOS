using DevExpress.Xpo.DB;
using LogicPOS.Data.XPO;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace LogicPOS.Reporting.Common
{
    public class ReportList<T> : IEnumerable<T>
        //Generic Types Constrained to FRBOBaseObject BaseClass or FRBOBaseObject SubClass Objects (New)
      where T : ReportBase, new()
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly bool _debug = false;

        //Used to Store if Object Has Attributes Defined or Not
        private readonly bool _objectHaveAttributes;

        public List<T> List { get; set; } = new List<T>();

        //Constructors
        public ReportList() : this("", "", "", "", 0, "") { }
        public ReportList(string pFilter) : this(pFilter, "", "", "", 0, "") { }
        public ReportList(string pFilter, int pLimit, string pQuery) : this(pFilter, "", "", "", pLimit, pQuery) { }
        public ReportList(string pFilter, int pLimit) : this(pFilter, "", "", "", pLimit, "") { }
        public ReportList(string pFilter, string pOrder) : this(pFilter, "", pOrder, "", 0, "") { }
        public ReportList(string pFilter, string pGroup, string pOrder) : this(pFilter, pGroup, pOrder, "", 0, "") { }
        public ReportList(string pFilter, string pGroup, string pOrder, string pFields) : this(pFilter, pGroup, pOrder, pFields, 0, "") { }
        public ReportList(string pFilter, string pGroup, string pOrder, string pFields, int pLimit, string pQuery)
        {
            //Assign Attributes Defined
            _objectHaveAttributes = typeof(T).GetCustomAttribute(typeof(ReportAttribute)) as ReportAttribute != null;

            string sqlQuery = pQuery;

            //Attributes: Sql
            if (string.IsNullOrEmpty(sqlQuery))
            {
                sqlQuery = _objectHaveAttributes && (typeof(T).GetCustomAttribute(typeof(ReportAttribute)) as ReportAttribute).Sql != null
                    ? (typeof(T).GetCustomAttribute(typeof(ReportAttribute)) as ReportAttribute).Sql
                    //If not SqlQuery Defined from Attributes, Create it From Reflection and FRBO Object Attributes
                    : GenQueryFromFRBOObject(pFilter, pGroup, pOrder, pFields);
            }

            //Atributes: Limit
            int sqlLimit = 0;
            //Get Order From Parameters
            if (pLimit > 0)
            {
                sqlLimit = pLimit;
            }
            //Limit: Get Order From LimitAttribute else Bypass it With 0
            else if (_objectHaveAttributes && (typeof(T).GetCustomAttribute(typeof(ReportAttribute)) as ReportAttribute).Limit > 0)
            {
                sqlLimit = (typeof(T).GetCustomAttribute(typeof(ReportAttribute)) as ReportAttribute).Limit;
            }

            try
            {
                //Start Process Collection

                //Temporary GenericTypeObject to Add to Collection
                T genericTypeObject;
                SQLSelectResultData xPSelectData;

                xPSelectData = XPOUtility.GetSelectedDataFromQuery(sqlQuery);

                PropertyInfo propertyInfo;
                //Fields Props
                string fieldName = string.Empty;
                string fieldType = string.Empty;
                string fieldTypeDB = string.Empty;
                object fieldValue;
                int fieldIndex;

                int i = 0;
                foreach (SelectStatementResultRow rowData in xPSelectData.DataRows)
                {
                    i++;
                    //If sqlLimit Defined and is Greater Break Loop
                    if (sqlLimit > 0 && i > sqlLimit) break;

                    //Create a new Fresh T Object to Insert into Collection
                    //Cannot create an instance of the variable type 'T' because it does not have the new() constraint
                    //Require Constrain new()
                    genericTypeObject = new T();
                    foreach (SelectStatementResultRow rowMeta in xPSelectData.MetaDataRows)
                    {
                        fieldName = rowMeta.Values[0].ToString();
                        fieldTypeDB = rowMeta.Values[1].ToString(); ;
                        fieldType = rowMeta.Values[2].ToString(); ;
                        fieldIndex = xPSelectData.GetFieldIndexFromName(fieldName);

                        //Convert DB Types, Else working on Diferent DBs occur Conversion Exceptions
                        switch (fieldType)
                        {
                            case "Int64":
                            case "UInt32":
                                //case "UInt64":
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

                        try
                        {
                            //If Property Exist in genericTypeObject Assign it from Data
                            propertyInfo = typeof(T).GetProperty(fieldName);

                            //if (fieldName.Equals("SourceOrderMain") && string.IsNullOrEmpty(fieldValue.ToString()))
                            //{
                            //    _logger.Debug(String.Format("fieldName: [{0}], fieldValue: [{1}]", fieldName, fieldValue));
                            //}

                            //Fix for MSSqlServer that detects UInt32 has Decimal, this way we convert it into UInt32 before above SetValue
                            if (propertyInfo.PropertyType == typeof(uint))
                            {
                                fieldValue = Convert.ToUInt32(rowData.Values[fieldIndex]);
                            }
                            //Fix for SQLite that detects UInt64 has Decimal, this way we convert it into Decimal before above SetValue
                            else if (propertyInfo.PropertyType == typeof(decimal))
                            {
                                fieldValue = Convert.ToDecimal(rowData.Values[fieldIndex]);
                            }
                            // Fix FRBO Object Field od type Guid
                            else if (propertyInfo.PropertyType == typeof(Guid))
                            {
                                fieldValue = new Guid(rowData.Values[fieldIndex].ToString());
                            }
                            // Check id is a Subclass of XPGuidObject and Get its value
                            else if (propertyInfo.PropertyType.IsSubclassOf(typeof(Entity)))
                            {
                                // Protection to prevent assign string value to Guid/unique identifier
                                if (fieldValue != null && string.IsNullOrEmpty(fieldValue.ToString()))
                                {
                                    fieldValue = null;
                                }
                                else
                                {
                                    fieldValue = XPOUtility.GetXPGuidObjectFromCriteria(propertyInfo.PropertyType, string.Format("Oid = '{0}'", fieldValue));
                                }
                                // Debug purpose helper
                                //if(propertyInfo.PropertyType == typeof(sys_userdetail) || propertyInfo.PropertyType == typeof(pos_configurationplaceterminal))
                                //{
                                //    _logger.Debug(String.Format("fieldName: [{0}], fieldValue: [{1}]", fieldName, fieldValue));
                                //}
                            }

                            // Try to Setvalue    
                            if (propertyInfo != null) propertyInfo.SetValue(genericTypeObject, fieldValue);
                        }
                        catch (Exception)
                        {
                            // Intentionnaly Commented ex
                            // Prevent Showing Conversion Error, Only Occur in Sales Per Day(Detailled/Group) Report, Minor problem, it Show Good Values
                            //_logger.Error(string.Format("fieldName: [{0}], fieldType: [{1}], fieldTypeDB: [{2}], fieldValue: [{3}]", fieldName, fieldType, fieldTypeDB, fieldValue));
                            //_logger.Error(ex.Message, ex);
                        }
                    }
                    //Add genericTypeObject to Collection :)
                    Add(genericTypeObject);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                _logger.Error(string.Format("Error sqlQuery: [{0}]", sqlQuery));
            }
        }

        public void Add(T pObject)
        {
            List.Add(pObject);
        }

        public T Get(int pIndex)
        {
            return List[pIndex];
        }

        //Foreach Support
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return List.GetEnumerator();
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

            if (_debug) _logger.Debug(string.Format("Class Name: {0}", typeof(T)));
            fieldList = new List<string>();
            foreach (PropertyInfo pInfo in propertyInfos)
            {
                //If not a generic Type ex a List, add it to FieldList
                if (!pInfo.PropertyType.IsGenericType /*&& pInfo.PropertyType.GetGenericTypeDefinition() != typeof(List<>)*/)
                {
                    //Attributes Working Block
                    //Always Reset attributeField and attributeHide
                    string attributeField = null;
                    bool attributeHide = false;
                    //Assign Object Attributes "Field" and "Hide" into attributeField and attributeHide objects
                    var attributes = pInfo.GetCustomAttributes(false);
                    foreach (var attribute in attributes)
                    {
                        //Check if is Working on a FRBO Attribute  
                        if (attribute.GetType() == typeof(ReportAttribute))
                        {
                            attributeField = (attribute as ReportAttribute).Field != null ? Convert.ToString((attribute as ReportAttribute).Field) : null;
                            attributeHide = (attribute as ReportAttribute).Hide == true;
                        }
                    }
                    if (_debug) _logger.Debug(string.Format("Name: [{0}], PropertyType[{1}], attributeField[{2}], attributeHide[{3}]", pInfo.Name, pInfo.PropertyType, attributeField, attributeHide));

                    //If not a Hidden Property, Add it to filedList
                    if (attributeHide == false)
                    {
                        //Get Mapped FieldName (Field Attribute AS Property Name) ex Select "fmOid AS Oid" (QueryField AS FRBOPropertyField)
                        //or Not Mapped Name (Object Property Name) ex "Oid"
                        string mappedFieldName = attributeField != null ? string.Format("{0} AS {1}", attributeField, pInfo.Name) : pInfo.Name;
                        //Add final FieldName from Attributes or Object Property
                        fieldList.Add(mappedFieldName);
                    }
                    else
                    {
                        //_logger.Debug(string.Format("pInfo.Name[{0}].Hide =  [{1}] = ", pInfo.Name, attributeHide));
                    }
                }
            }

            //Finally Convert Generated fieldList into Comma Delimited, Ready to Query Database
            resultFields = string.Join(",", fieldList.ToArray());
            if (_debug) _logger.Debug(string.Format("fields: [{0}]", resultFields));

            return resultFields;
        }

        /// <summary>
        /// Generate Query From FRBOObject Property Fields and Attributes
        /// </summary>
        /// <param name="Filter"></param>
        /// <param name="Order"></param>
        /// <returns>Database Query</returns>
        public string GenQueryFromFRBOObject(string pFilter = "", string pGroup = "", string pOrder = "", string pQueryFields = "")
        {
            //SqlEntity: Get Entity Name from FRBO EntityAttribute or From ClassName Without FRBO
            string sqlEntity = _objectHaveAttributes && (typeof(T).GetCustomAttribute(typeof(ReportAttribute)) as ReportAttribute).Entity != null
              ? (typeof(T).GetCustomAttribute(typeof(ReportAttribute)) as ReportAttribute).Entity
              : typeof(T).Name.ToString().Replace("FRBO", string.Empty);

            // Used to SQLServer Groups to pass Fields, with this we ignore generated reflectes GenQueryFieldsFromFRBOObject 
            string queryFields = string.IsNullOrEmpty(pQueryFields)
                ? GenQueryFieldsFromFRBOObject()
                : pQueryFields;

            //Fields: Get Fields from FRBO FieldsAttribute or Generate it With GenQueryFieldsFromFRBOObject
            string sqlFields = _objectHaveAttributes && (typeof(T).GetCustomAttribute(typeof(ReportAttribute)) as ReportAttribute).Fields != null
              ? (typeof(T).GetCustomAttribute(typeof(ReportAttribute)) as ReportAttribute).Fields
              : queryFields;

            //Filter
            string sqlFilter = string.Empty;

            //Get Filter From Parameters
            if (pFilter != null && pFilter != string.Empty)
            {
                sqlFilter = string.Format(" WHERE ({0})", pFilter);
            }
            //Filter: Get Filter From FilterAttribute else Bypass it With Empty String
            else if (_objectHaveAttributes && (typeof(T).GetCustomAttribute(typeof(ReportAttribute)) as ReportAttribute).Filter != null)
            {
                sqlFilter = string.Format(" WHERE ({0})", (typeof(T).GetCustomAttribute(typeof(ReportAttribute)) as ReportAttribute).Filter);
            }

            //Group
            string sqlGroup = string.Empty;
            //Get Group From Parameters
            if (pGroup != string.Empty)
            {
                sqlGroup = string.Format(" GROUP BY {0}", pGroup);
            }
            //Group: Get Group From GroupAttribute else Bypass it With Empty String
            else if (_objectHaveAttributes && (typeof(T).GetCustomAttribute(typeof(ReportAttribute)) as ReportAttribute).Group != null)
            {
                sqlGroup = string.Format(" GROUP BY {0}", (typeof(T).GetCustomAttribute(typeof(ReportAttribute)) as ReportAttribute).Group);
            }

            //Order
            string sqlOrder = string.Empty;
            //Get Order From Parameters
            if (pOrder != string.Empty)
            {
                sqlOrder = string.Format(" ORDER BY {0}", pOrder);
            }
            //Order: Get Order From OrderAttribute else Bypass it With Empty String
            else if (_objectHaveAttributes && (typeof(T).GetCustomAttribute(typeof(ReportAttribute)) as ReportAttribute).Order != null)
            {
                sqlOrder = string.Format(" ORDER BY {0}", (typeof(T).GetCustomAttribute(typeof(ReportAttribute)) as ReportAttribute).Order);
            }

            //Finally Generate SqlQuery        
            string sqlQuery = string.Format("SELECT {0} FROM {1}{2}{3}{4};", sqlFields, sqlEntity, sqlFilter, sqlGroup, sqlOrder);
            if (_debug) _logger.Debug(string.Format("sqlQuery: [{0}]", sqlQuery));

            return sqlQuery;
        }
    }
}