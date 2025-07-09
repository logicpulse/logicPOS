using Gtk;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class CheckButtonBoxGroup : EntryBoxBase
    {
       
         public List<CheckButtonExtended> Buttons { get; } 
        public CheckButtonBoxGroup(string label,
                                   List<CheckButtonExtended> buttons)
            : base(label)
        {
            Buttons = buttons;
            VBox verticalLayout = new VBox() { BorderWidth = 5 };
            EventBox eventBox = new EventBox() { BorderWidth = 2 };

            foreach (var button in buttons)
            {
                button.Child.ModifyFont(_fontDescription);

                verticalLayout.PackStart(button);
            }

            //Put in White EventBox
            eventBox.Add(verticalLayout);

            //Pack in Base VBox
            Vbox.PackStart(eventBox);
        }
    
    }
}
