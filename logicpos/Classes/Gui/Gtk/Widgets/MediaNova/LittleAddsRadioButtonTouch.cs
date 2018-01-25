using Gtk;
using logicpos.App;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    class LittleAddsRadioButtonTouch : VBox
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

        public LittleAddsRadioButtonTouch(List<LittleAddsWidget> pLittleAddsList)
            : this(pLittleAddsList, "") { }

        public LittleAddsRadioButtonTouch(List<LittleAddsWidget> pLittleAddsList, string pInitialValue)
        {
            //Init FontDescription
            Pango.FontDescription fontDescriptionEntry = Pango.FontDescription.FromString(GlobalFramework.Settings["fontEntryBoxValue"]);

            //Parameters
            RadioButton groupButton = null;

            //Init WidgetList
            _radioButtonList = new List<RadioButton>();

            //Start Processing List
            if (pLittleAddsList.Count > 0)
            {
                //Always Assign First item has initial Value
                if (pLittleAddsList.Count > 0) _value = pLittleAddsList[0].Value;

                for (int i = 0; i < pLittleAddsList.Count; i++)
                {
                    //Store reference to ButtonGroup
                    if (_radioButtonList.Count <= 0)
                    {
                        pLittleAddsList[i].RadioButton = new RadioButton(null, pLittleAddsList[i].Value);
                        //References
                        groupButton = pLittleAddsList[i].RadioButton;
                    }
                    //Or Use ButtonGroup Reference
                    else
                    {
                        pLittleAddsList[i].RadioButton = new RadioButton(groupButton, pLittleAddsList[i].Value);
                    }

                    //Pack generated RadioButton and Labels inside current LittleAddsWidget VBox
                    pLittleAddsList[i].PackStart(pLittleAddsList[i].RadioButton);
                    pLittleAddsList[i].PackStart(pLittleAddsList[i].Image);
                    pLittleAddsList[i].PackStart(pLittleAddsList[i].LabelInfo);
                    pLittleAddsList[i].PackStart(pLittleAddsList[i].LabelModules);
                    pLittleAddsList[i].PackStart(pLittleAddsList[i].LabelDimensions);

                    //Add to RadioButton List
                    _radioButtonList.Add(pLittleAddsList[i].RadioButton);

                    //Initial Value/Active Radio
                    if (pLittleAddsList[i].Value == pInitialValue)
                    {
                        _value = pLittleAddsList[i].Value;
                        _radioButtonList[i].Active = true;
                    }

                    //Change Font
                    pLittleAddsList[i].RadioButton.Child.ModifyFont(fontDescriptionEntry);
                    //Pack
                    EventBox eventBox = new EventBox() { BorderWidth = 2 };
                    eventBox.Add(pLittleAddsList[i]);
                    PackStart(eventBox);
                    //Event
                    _radioButtonList[i].Clicked += radiobutton_Clicked;
                };
            };
        }

        void radiobutton_Clicked(object sender, System.EventArgs e)
        {
            RadioButton radiobutton = (RadioButton)sender;
            _value = radiobutton.Label;
            _log.Debug(string.Format("_value: [{0}]", _value));
        }
    }
}
