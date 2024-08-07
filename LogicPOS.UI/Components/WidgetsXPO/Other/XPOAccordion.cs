﻿using DevExpress.Xpo.DB;
using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Globalization.Resources.Localization;
using System.Collections.Generic;
using LogicPOS.Globalization;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Data.XPO;

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
            SQLSelectResultData xPSelectDataParent = XPOUtility.GetSelectedDataFromQuery(sqlTableParent);
            SQLSelectResultData xPSelectDataChild;
            Dictionary<string, AccordionNode> _accordionChilds = new Dictionary<string, AccordionNode>();

            //Start Render Accordion Parent Nodes
            foreach (SelectStatementResultRow parentRow in xPSelectDataParent.DataRows)
            {
                //Initialize Vars
                string parentId = parentRow.Values[xPSelectDataParent.GetFieldIndexFromName("id")].ToString();
                string parentLabel = parentRow.Values[xPSelectDataParent.GetFieldIndexFromName("label")].ToString();
                if (parentRow.Values[xPSelectDataParent.GetFieldIndexFromName("resource")] != null)
                {
                    string parentResource = parentRow.Values[xPSelectDataParent.GetFieldIndexFromName("resource")].ToString();
                    //Bypass default db label with Resources Localization Label
                    if (Resx
                        .ResourceManager.GetString(parentResource) != null)
                        parentLabel = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, parentResource);
                }

                //Get Child Data
                xPSelectDataChild = XPOUtility.GetSelectedDataFromQuery(string.Format(sqlTableChild, parentId));
                //Init Childs        
                _accordionChilds = new Dictionary<string, AccordionNode>();

                //Start Render Accordion Parent Childs
                foreach (SelectStatementResultRow childRow in xPSelectDataChild.DataRows)
                {
                    string childId = childRow.Values[xPSelectDataChild.GetFieldIndexFromName("id")].ToString();
                    string childLabel = childRow.Values[xPSelectDataChild.GetFieldIndexFromName("label")].ToString();
                    if (childRow.Values[xPSelectDataChild.GetFieldIndexFromName("resource")] != null)
                    {
                        string childResource = childRow.Values[xPSelectDataChild.GetFieldIndexFromName("resource")].ToString();
                        //Bypass default db label with Resources Localization Label
                        if (CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, childResource) != null)
                            childLabel = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, childResource);
                    }
                    _accordionChilds.Add(string.Format("childId_{0}", childId), new AccordionNode(childLabel) { Content = new Button(childLabel) });
                }
                _accordionDefinition.Add(string.Format("parentId_{0}", parentId), new AccordionNode(parentLabel) { Childs = _accordionChilds });
            }
        }
    }
}