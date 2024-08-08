using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using System;
using System.Collections.Generic;
using LogicPOS.Globalization;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;

namespace LogicPOS.UI.Components
{
    internal class GridViewCountries : XpoGridView
    {
        public GridViewCountries() { }

        public GridViewCountries(Window parentWindow,
                                 Entity entity,
                                 CriteriaOperator criteria,
                                 Type dialogType,
                                 GridViewMode gridViewMode = GridViewMode.Default,
                                 GridViewNavigatorMode navigatorMode = GridViewNavigatorMode.Default)
        {
            Type entityType = typeof(cfg_configurationcountry);
            cfg_configurationcountry defaultValue = entity as cfg_configurationcountry;

            Type dialogClass = typeof(CountryModal);

            //Configure columnProperties
            List<GridViewColumnProperty> columnProperties = new List<GridViewColumnProperty>
            {
                new GridViewColumnProperty("Code") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_code"), MinWidth = 100 },
                new GridViewColumnProperty("Designation") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), Expand = true },
                new GridViewColumnProperty("UpdatedAt") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_date_updated"), MinWidth = 150, MaxWidth = 150 }
            };

            SortProperty[] sortProperty = new SortProperty[1];
            sortProperty[0] = new SortProperty("Code", SortingDirection.Ascending);
            XPCollection xpoCollection = new XPCollection(XPOSettings.Session, entityType, criteria);

            //Call Base Initializer
            base.InitObject(
              parentWindow,
              defaultValue,
              gridViewMode,
              navigatorMode,
              columnProperties,
              xpoCollection,
              dialogClass
            );
        }
    }
}