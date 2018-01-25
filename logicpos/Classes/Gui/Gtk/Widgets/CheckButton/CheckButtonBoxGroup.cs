using Gtk;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    class CheckButtonBoxGroup : EntryBoxBase
    {
        private bool _debug = false;

        //Full Buttons List
        private List<CheckButtonExtended> _buttons = new List<CheckButtonExtended>();
        public List<CheckButtonExtended> Buttons
        {
            get { return _buttons; }
            set { _buttons = value; }
        }
        //Partial Check Buttons Dictionary
        private Dictionary<int, CheckButtonExtended> _items = new Dictionary<int, CheckButtonExtended>();
        public Dictionary<int, CheckButtonExtended> Items
        {
            get { return _items; }
            set { _items = value; }
        }
        private List<int> _itemsList = new List<int>();
        public List<int> ItemsList
        {
            get {
                //foreach (var item in _items) { _itemsList.Add(item.Key); }
                return _itemsList; 
            }
        }

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
                _buttons.Add(checkButtonExtended);
                //Add checkButton to items
                if (item.Value) _items.Add(key, checkButtonExtended);
            }

            //Put in White EventBox
            eventBox.Add(vbox);

            //Pack in Base VBox
            Vbox.PackStart(eventBox);
        }

        void checkButtonExtended_Clicked(object sender, EventArgs e)
        {
            try
            {
                CheckButtonExtended checkButtonExtended = (CheckButtonExtended)sender;

                if (checkButtonExtended.Active)
                {
                    //Add checkButton to items
                    _items.Add(checkButtonExtended.Index, checkButtonExtended);
                }
                else
                {
                    //Remove checkButton to items
                    if (_items.ContainsKey(checkButtonExtended.Index)) _items.Remove(checkButtonExtended.Index);
                }

                //Always Sort Dictionary after change
                SortDictionaryByKey();

                //Debug Items
                if (_debug)
                {
                    _log.Debug(Environment.NewLine);
                    foreach (var item in _items)
                    {
                        _log.Debug(string.Format("item[{0}]: [{1}]", item.Key, item.Value.Label));
                    }
                }

                //If Assigned Redirect to Caller
                if (Clicked != null) Clicked(sender, e);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private void SortDictionaryByKey()
        {
            // LINQ : Order by Keys
	        var items = from pair in _items
		        orderby pair.Key ascending
		        select pair;

            // Recreate Dictionary After have items in memory
            _items = new Dictionary<int, CheckButtonExtended>();

	        // Add order Items
	        foreach (KeyValuePair<int, CheckButtonExtended> pair in items)
	        {
                _items.Add(pair.Key, pair.Value);
	        }
        }
    }
}
