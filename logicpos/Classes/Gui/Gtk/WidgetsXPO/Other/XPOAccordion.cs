using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.resources.Resources.Localization;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.WidgetsXPO
{
    class XPOAccordion : Accordion
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public XPOAccordion(string pTableParent, string pTableChild, string pTableChildWhereField, string pNodePrivilegesTokenFormat)
        {
            InitDefinition(pTableParent, pTableChild, pTableChildWhereField);
            InitObject(_accordionDefinition, pNodePrivilegesTokenFormat);
        }

        private void InitDefinition(string pTableParent, string pTableChild, string pTableChildWhereField)
        {
            bool debug = false;

            //Init Definition
            _accordionDefinition = new Dictionary<string, AccordionNode>();

            //Generate Base Queries
            string sqlTableParent = string.Format(@"SELECT Oid AS id, Designation AS label, ResourceString AS resource FROM {0} WHERE (Disabled IS NULL or Disabled  <> 1) ORDER BY Ord;", pTableParent);
            string sqlTableChild = string.Format(@"SELECT Oid AS id, Designation AS label, ResourceString AS resource FROM {0} WHERE (Disabled IS NULL or Disabled  <> 1) AND ({1} = '{2}') ORDER BY Ord;", pTableChild, pTableChildWhereField, "{0}");
            if (debug)
            {
                _log.Debug(string.Format("InitDefinition(): sqlTableParent: [{0}]", sqlTableParent));
                _log.Debug(string.Format("InitDefinition(): sqlTableChild: [{0}]", sqlTableChild));
            }

            //Get XPSelectData for Parent and Child
            XPSelectData xPSelectDataParent = FrameworkUtils.GetSelectedDataFromQuery(sqlTableParent);
            XPSelectData xPSelectDataChild;
            //Initialize Vars
            string parentId = string.Empty, parentLabel = string.Empty, parentResource = string.Empty;
            string childId = string.Empty, childLabel = string.Empty, childResource = string.Empty;
            Dictionary<string, AccordionNode> _accordionChilds = new Dictionary<string, AccordionNode>();

            //Start Render Accordion Parent Nodes
            foreach (SelectStatementResultRow parentRow in xPSelectDataParent.Data)
            {
                parentId = parentRow.Values[xPSelectDataParent.GetFieldIndex("id")].ToString();
                parentLabel = parentRow.Values[xPSelectDataParent.GetFieldIndex("label")].ToString();
                if (parentRow.Values[xPSelectDataParent.GetFieldIndex("resource")] != null)
                {
                    parentResource = parentRow.Values[xPSelectDataParent.GetFieldIndex("resource")].ToString();
                    //Bypass default db label with Resources Localization Label
                    if (Resx
                        .ResourceManager.GetString(parentResource) != null)
                        parentLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], parentResource);
                }

                //Get Child Data
                xPSelectDataChild = FrameworkUtils.GetSelectedDataFromQuery(string.Format(sqlTableChild, parentId));
                //Init Childs        
                _accordionChilds = new Dictionary<string, AccordionNode>();

                //Start Render Accordion Parent Childs
                foreach (SelectStatementResultRow childRow in xPSelectDataChild.Data)
                {
                    childId = childRow.Values[xPSelectDataChild.GetFieldIndex("id")].ToString();
                    childLabel = childRow.Values[xPSelectDataChild.GetFieldIndex("label")].ToString();
                    if (childRow.Values[xPSelectDataChild.GetFieldIndex("resource")] != null)
                    {
                        childResource = childRow.Values[xPSelectDataChild.GetFieldIndex("resource")].ToString();
                        //Bypass default db label with Resources Localization Label
                        if (resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], childResource) != null)
                            childLabel = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], childResource);
                    }
                    _accordionChilds.Add(string.Format("childId_{0}", childId), new AccordionNode(childLabel) { Content = new Button(childLabel) });
                }
                _accordionDefinition.Add(string.Format("parentId_{0}", parentId), new AccordionNode(parentLabel) { Childs = _accordionChilds });
            }
        }
    }
}