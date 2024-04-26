using DevExpress.Xpo.DB;
using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.resources.Resources.Localization;
using logicpos.shared.App;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.WidgetsXPO
{
    internal class XPOAccordion : Accordion
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
                _logger.Debug(string.Format("InitDefinition(): sqlTableParent: [{0}]", sqlTableParent));
                _logger.Debug(string.Format("InitDefinition(): sqlTableChild: [{0}]", sqlTableChild));
            }

            //Get XPSelectData for Parent and Child
            XPSelectData xPSelectDataParent = SharedUtils.GetSelectedDataFromQuery(sqlTableParent);
            XPSelectData xPSelectDataChild;
            Dictionary<string, AccordionNode> _accordionChilds = new Dictionary<string, AccordionNode>();

            //Start Render Accordion Parent Nodes
            foreach (SelectStatementResultRow parentRow in xPSelectDataParent.Data)
            {
                //Initialize Vars
                string parentId = parentRow.Values[xPSelectDataParent.GetFieldIndex("id")].ToString();
                string parentLabel = parentRow.Values[xPSelectDataParent.GetFieldIndex("label")].ToString();
                if (parentRow.Values[xPSelectDataParent.GetFieldIndex("resource")] != null)
                {
                    string parentResource = parentRow.Values[xPSelectDataParent.GetFieldIndex("resource")].ToString();
                    //Bypass default db label with Resources Localization Label
                    if (Resx
                        .ResourceManager.GetString(parentResource) != null)
                        parentLabel = resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], parentResource);
                }

                //Get Child Data
                xPSelectDataChild = SharedUtils.GetSelectedDataFromQuery(string.Format(sqlTableChild, parentId));
                //Init Childs        
                _accordionChilds = new Dictionary<string, AccordionNode>();

                //Start Render Accordion Parent Childs
                foreach (SelectStatementResultRow childRow in xPSelectDataChild.Data)
                {
                    string childId = childRow.Values[xPSelectDataChild.GetFieldIndex("id")].ToString();
                    string childLabel = childRow.Values[xPSelectDataChild.GetFieldIndex("label")].ToString();
                    if (childRow.Values[xPSelectDataChild.GetFieldIndex("resource")] != null)
                    {
                        string childResource = childRow.Values[xPSelectDataChild.GetFieldIndex("resource")].ToString();
                        //Bypass default db label with Resources Localization Label
                        if (resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], childResource) != null)
                            childLabel = resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], childResource);
                    }
                    _accordionChilds.Add(string.Format("childId_{0}", childId), new AccordionNode(childLabel) { Content = new Button(childLabel) });
                }
                _accordionDefinition.Add(string.Format("parentId_{0}", parentId), new AccordionNode(parentLabel) { Childs = _accordionChilds });
            }
        }
    }
}