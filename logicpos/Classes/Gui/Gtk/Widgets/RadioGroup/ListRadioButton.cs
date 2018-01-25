using Gtk;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    class ListRadioButton : VBox
    {
        //Log4Net
        protected static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Public Properties
        List<RadioButton> _radioButtonList;
        public List<RadioButton> RadioButtonList
        {
            get { return _radioButtonList; }
            set { _radioButtonList = value; }
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public ListRadioButton(List<string> pItemList)
            : this(pItemList, "") { }

        public ListRadioButton(List<string> pItemList, string pInitialValue)
        {
            //Init WidgetList
            _radioButtonList = new List<RadioButton>();

            RadioButton radiobutton;
            RadioButton groupButton = null;

            //Start Processing List
            if (pItemList.Count > 0)
            {
                //Always Assign First item has initial Value
                if (pItemList.Count > 0) _value = pItemList[0];

                for (int i = 0; i < pItemList.Count; i++)
                {
                    //Store reference to ButtonGroup
                    if (_radioButtonList.Count <= 0)
                    {
                        radiobutton = new RadioButton(null, pItemList[i]);
                        groupButton = radiobutton;
                    }
                    //Or Use Reference
                    else
                    {
                        radiobutton = new RadioButton(groupButton, pItemList[i]);
                    }

                    _radioButtonList.Add(radiobutton);

                    //Initial Value/Active Radio
                    if (pItemList[i] == pInitialValue)
                    {
                        _value = pItemList[i];
                        _radioButtonList[i].Active = true;
                    }

                    //Event
                    _radioButtonList[i].Clicked += radiobutton_Clicked;
                    //Pack
                    PackStart(radiobutton);
                };
            };
        }

        void radiobutton_Clicked(object sender, System.EventArgs e)
        {
            RadioButton radiobutton = (RadioButton)sender;
            _value = radiobutton.Label;
        }
    }
}
