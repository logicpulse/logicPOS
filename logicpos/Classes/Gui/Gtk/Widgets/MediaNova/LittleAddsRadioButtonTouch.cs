using Gtk;
using logicpos.datalayer.App;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class LittleAddsRadioButtonTouch : VBox
    {
        //Log4Net
        protected static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<RadioButton> RadioButtonList { get; set; }

        public string Value { get; set; }

        public LittleAddsRadioButtonTouch(List<LittleAddsWidget> pLittleAddsList)
            : this(pLittleAddsList, "") { }

        public LittleAddsRadioButtonTouch(List<LittleAddsWidget> pLittleAddsList, string pInitialValue)
        {
            //Init FontDescription
            Pango.FontDescription fontDescriptionEntry = Pango.FontDescription.FromString(DataLayerFramework.Settings["fontEntryBoxValue"]);

            //Parameters
            RadioButton groupButton = null;

            //Init WidgetList
            RadioButtonList = new List<RadioButton>();

            //Start Processing List
            if (pLittleAddsList.Count > 0)
            {
                //Always Assign First item has initial Value
                if (pLittleAddsList.Count > 0) Value = pLittleAddsList[0].Value;

                for (int i = 0; i < pLittleAddsList.Count; i++)
                {
                    //Store reference to ButtonGroup
                    if (RadioButtonList.Count <= 0)
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
                    RadioButtonList.Add(pLittleAddsList[i].RadioButton);

                    //Initial Value/Active Radio
                    if (pLittleAddsList[i].Value == pInitialValue)
                    {
                        Value = pLittleAddsList[i].Value;
                        RadioButtonList[i].Active = true;
                    }

                    //Change Font
                    pLittleAddsList[i].RadioButton.Child.ModifyFont(fontDescriptionEntry);
                    //Pack
                    EventBox eventBox = new EventBox() { BorderWidth = 2 };
                    eventBox.Add(pLittleAddsList[i]);
                    PackStart(eventBox);
                    //Event
                    RadioButtonList[i].Clicked += radiobutton_Clicked;
                };
            };
        }

        private void radiobutton_Clicked(object sender, System.EventArgs e)
        {
            RadioButton radiobutton = (RadioButton)sender;
            Value = radiobutton.Label;
            _logger.Debug(string.Format("_value: [{0}]", Value));
        }
    }
}
