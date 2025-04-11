using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.InputFields
{
    public partial class ArticleField
    {
        public EventBox Component { get; } = new EventBox();
        public IconButton BtnSelect { get; set; }
        public IconButton BtnRemove { get; set; }
        public IconButton BtnAdd { get; set; }
        public Entry TxtDesignation { get; set; } = new Entry() { IsEditable = false };
        public Entry TxtQuantity { get; set; } = new Entry() { WidthRequest = 50 };
        public Entry TxtCode { get; set; } = new Entry() { WidthRequest = 50, IsEditable = false };
        public Label Label { get; set; } = new Label(GeneralUtils.GetResourceByName("global_article"));
        private TextBox TxtPrice { get; set; } = TextBox.Simple("global_price",false,true,RegularExpressions.Money);
        private readonly List<SerialNumberField> _serialNumberFields = new List<SerialNumberField>();
        private VBox _serialNumberFieldsContainer { get; set; } = new VBox(false, 2);
        private WarehouseSelectionField _locationField { get; set; } 

        
    }
}
