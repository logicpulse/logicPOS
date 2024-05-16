using Gtk;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class ListRadioButton : VBox
    {
        //Log4Net
        protected static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<RadioButton> RadioButtonList { get; set; }

        public string Value { get; set; }

        public ListRadioButton(List<string> pItemList)
            : this(pItemList, "") { }

        public ListRadioButton(List<string> pItemList, string pInitialValue)
        {
            //Init WidgetList
            RadioButtonList = new List<RadioButton>();

            RadioButton radiobutton;
            RadioButton groupButton = null;

            //Start Processing List
            if (pItemList.Count > 0)
            {
                //Always Assign First item has initial Value
                if (pItemList.Count > 0) Value = pItemList[0];

                for (int i = 0; i < pItemList.Count; i++)
                {
                    //Store reference to ButtonGroup
                    if (RadioButtonList.Count <= 0)
                    {
                        radiobutton = new RadioButton(null, pItemList[i]);
                        groupButton = radiobutton;
                    }
                    //Or Use Reference
                    else
                    {
                        radiobutton = new RadioButton(groupButton, pItemList[i]);
                    }

                    RadioButtonList.Add(radiobutton);

                    //Initial Value/Active Radio
                    if (pItemList[i] == pInitialValue)
                    {
                        Value = pItemList[i];
                        RadioButtonList[i].Active = true;
                    }

                    //Event
                    RadioButtonList[i].Clicked += radiobutton_Clicked;
                    //Pack
                    PackStart(radiobutton);
                };
            };
        }

        private void radiobutton_Clicked(object sender, System.EventArgs e)
        {
            RadioButton radiobutton = (RadioButton)sender;
            Value = radiobutton.Label;
        }
    }
}
