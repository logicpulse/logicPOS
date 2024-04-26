using Gtk;
using System;
using System.Collections.Generic;
using System.Linq;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    internal class CheckButtonBoxGroup : EntryBoxBase
    {
        private readonly bool _debug = false;

        public List<CheckButtonExtended> Buttons { get; set; } = new List<CheckButtonExtended>();

        public Dictionary<int, CheckButtonExtended> Items { get; set; } = new Dictionary<int, CheckButtonExtended>();

        public List<int> ItemsList { get; } = new List<int>();

        //Custom Event Handlers
        public event EventHandler Clicked;

        public CheckButtonBoxGroup(string pLabelText, Dictionary<string, bool> pButtonGroup)
            : base(pLabelText)
        {
            VBox vbox = new VBox() { BorderWidth = 5 };
            EventBox eventBox = new EventBox() { BorderWidth = 2 };

            int key = -1;
            foreach (var item in pButtonGroup)
            {
                key++;

                //CheckButtonExtended
                CheckButtonExtended checkButtonExtended = new CheckButtonExtended(item.Key) { Active = item.Value, Index = key };
                checkButtonExtended.Child.ModifyFont(_fontDescription);
                checkButtonExtended.Clicked += checkButtonExtended_Clicked;
                //Pack in local Vbox
                vbox.PackStart(checkButtonExtended);
                //Add to value List
                Buttons.Add(checkButtonExtended);
                //Add checkButton to items
                if (item.Value) Items.Add(key, checkButtonExtended);
            }

            //Put in White EventBox
            eventBox.Add(vbox);

            //Pack in Base VBox
            Vbox.PackStart(eventBox);
        }

        private void checkButtonExtended_Clicked(object sender, EventArgs e)
        {
            try
            {
                CheckButtonExtended checkButtonExtended = (CheckButtonExtended)sender;

                if (checkButtonExtended.Active)
                {
                    //Add checkButton to items
                    Items.Add(checkButtonExtended.Index, checkButtonExtended);
                }
                else
                {
                    //Remove checkButton to items
                    if (Items.ContainsKey(checkButtonExtended.Index)) Items.Remove(checkButtonExtended.Index);
                }

                //Always Sort Dictionary after change
                SortDictionaryByKey();

                //Debug Items
                if (_debug)
                {
                    _logger.Debug(Environment.NewLine);
                    foreach (var item in Items)
                    {
                        _logger.Debug(string.Format("item[{0}]: [{1}]", item.Key, item.Value.Label));
                    }
                }

                //If Assigned Redirect to Caller
                Clicked?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void SortDictionaryByKey()
        {
            // LINQ : Order by Keys
	        var items = from pair in Items
		        orderby pair.Key ascending
		        select pair;

            // Recreate Dictionary After have items in memory
            Items = new Dictionary<int, CheckButtonExtended>();

	        // Add order Items
	        foreach (KeyValuePair<int, CheckButtonExtended> pair in items)
	        {
                Items.Add(pair.Key, pair.Value);
	        }
        }
    }
}
