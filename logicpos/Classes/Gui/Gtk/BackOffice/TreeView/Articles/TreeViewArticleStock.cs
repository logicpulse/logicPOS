using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Formatters;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using logicpos.Classes.Enums.GenericTreeView;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    class TreeViewArticleStock : GenericTreeViewXPO
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewArticleStock() { }

        public TreeViewArticleStock(Window pSourceWindow)
            : this(pSourceWindow, null, null, null) { }

        //XpoMode
        public TreeViewArticleStock(Window pSourceWindow, XPGuidObject pDefaultValue, CriteriaOperator pXpoCriteria, Type pDialogType, GenericTreeViewMode pGenericTreeViewMode = GenericTreeViewMode.Default, GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode = GenericTreeViewNavigatorMode.Default)
        {
            //Init Vars
            Type xpoGuidObjectType = typeof(FIN_ArticleStock);
            //Override Default Value with Parameter Default Value, this way we can have diferent Default Values for GenericTreeView
            FIN_ArticleStock defaultValue = (pDefaultValue != null) ? pDefaultValue as FIN_ArticleStock : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : null;

            //Configure columnProperties
            List<GenericTreeViewColumnProperty> columnProperties = new List<GenericTreeViewColumnProperty>();
            columnProperties.Add(new GenericTreeViewColumnProperty("Quantity") { Title = Resx.global_stock_movement, MinWidth = 100, FormatProvider = new FormatterStockMovement(), });
            columnProperties.Add(new GenericTreeViewColumnProperty("Date") { Title = Resx.global_date, MinWidth = 100, FormatProvider = new FormatterDate(), });
            columnProperties.Add(new GenericTreeViewColumnProperty("Customer") { Title = Resx.global_entity, ChildName = "Name", MinWidth = 125 });
            columnProperties.Add(new GenericTreeViewColumnProperty("DocumentNumber") { Title = Resx.global_document_number, MinWidth = 125 });
            columnProperties.Add(new GenericTreeViewColumnProperty("Article") { Title = Resx.global_article, ChildName = "Designation", MinWidth = 125 });
            columnProperties.Add(new GenericTreeViewColumnProperty("Quantity")
            {
                Title = Resx.global_quantity,
                MinWidth = 100,
                Alignment = 1.0F,
                FormatProvider = new FormatterDecimal(),
                CellRenderer = new CellRendererText()
                {
                    Alignment = Pango.Alignment.Right,
                    FontDesc = new Pango.FontDescription() { Size = 50 },
                    Xalign = 1.0F
                }
            });
            //columnProperties.Add(new GenericTreeViewColumnProperty("Disabled") { Title = Resx.global_record_disabled });

            //Configure Criteria/XPCollection/Model
            //CriteriaOperator.Parse("Code >= 100 and Code <= 9999");
            CriteriaOperator criteria = pXpoCriteria;
            SortProperty[] sortProperty = new SortProperty[2];
            sortProperty[0] = new SortProperty("CreatedAt", SortingDirection.Ascending);
            sortProperty[1] = new SortProperty("Ord", SortingDirection.Ascending);
            XPCollection xpoCollection = new XPCollection(GlobalFramework.SessionXpo, xpoGuidObjectType, criteria, sortProperty);

            //Call Base Initializer
            base.InitObject(
              pSourceWindow,                  //Pass parameter 
              defaultValue,                   //Pass parameter
              pGenericTreeViewMode,           //Pass parameter
              pGenericTreeViewNavigatorMode,  //Pass parameter
              columnProperties,               //Created Here
              xpoCollection,                  //Created Here
              typeDialogClass                 //Created Here
            );
        }
    }
}
