using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleFamilyModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtButtonName = TextBox.Simple("global_button_name");
        private ImagePicker _imagePicker = new ImagePicker(GeneralUtils.GetResourceByName("global_button_image"));
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        private EntityComboBox<CommissionGroup> _comboCommissionGroups;
        private EntityComboBox<Api.Entities.Printer> _comboPrinters;

    }
}
