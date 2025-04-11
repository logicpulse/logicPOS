using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleClassModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtAcronym = TextBox.Simple("global_acronym", true, true, "^.{1}$");
        private CheckButton _checkWorkInStock = new CheckButton(GeneralUtils.GetResourceByName("global_work_in_stock"));
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));

    }
}
