using Gtk;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    class ListComboBox : ComboBox
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Private Members
        private ListStore _comboBoxListStore;
        private Dictionary<int, TreeIter> _treeInterDictionary;
        //Public Members
        private CellRendererText _comboBoxCell;
        public CellRendererText ComboBoxCell
        {
            get { return _comboBoxCell; }
            set { _comboBoxCell = value; }
        }
        private string _value;
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public ListComboBox(List<string> pItemList)
            : this(pItemList, "", true, false) { }

        public ListComboBox(List<string> pItemList, string pInitialValue)
            : this(pItemList, pInitialValue, true, false) { }

        public ListComboBox(List<string> pItemList, string pInitialValue, bool pAddUndefinedValue)
            : this(pItemList, pInitialValue, pAddUndefinedValue, false) { }

        public ListComboBox(List<string> pItemList, string pInitialValue, bool pAddUndefinedValue, bool pRequired)
        {
            //Init CellRenderer
            _comboBoxCell = new CellRendererText();
            PackStart(_comboBoxCell, true);
            AddAttribute(_comboBoxCell, "text", 1);
            //Create/Update Model
            CreateModel(pItemList, pInitialValue, pAddUndefinedValue);
            //Events
            Changed += ListComboBox_Changed;
        }

        public void CreateModel(List<string> pItemList, string pInitialValue, bool pAddUndefinedValue)
        {
            //Local Variables
            TreeIter tempItemIter;
            TreeIter currentItemIter = new TreeIter();
            int positionOffset = -1;
            string initialValueDefault;

            //Store TreeIters in Dictionary
            _treeInterDictionary = new Dictionary<int, TreeIter>();

            //Init ListStore Model
            _comboBoxListStore = new ListStore(typeof(int), typeof(string));

            //Aways Default to UNDEFINED Value - even if Collection is Empty, and if Used
            if (pAddUndefinedValue)
            {
                tempItemIter = _comboBoxListStore.AppendValues(0, resources.CustomResources.GetCustomResources("", "widget_combobox_undefined"));
                _treeInterDictionary.Add(0, tempItemIter);
                initialValueDefault = resources.CustomResources.GetCustomResources("", "widget_combobox_undefined");
                positionOffset = 1;
            }
            else
            {
                initialValueDefault = pItemList[0];
                positionOffset = 0;
            }

            //LINQ: Default Selected, Check if Inital Exist else use Undefined
            _value = (pItemList.Exists(element => element == pInitialValue)) ? pInitialValue : initialValueDefault;

            //Assign CurrentIter
            //currentItemIter = tempItemIter;

            //Start Processing List
            if (pItemList.Count > 0)
            {
                //Create Model
                for (int i = 0; i < pItemList.Count; i++)
                {
                    tempItemIter = _comboBoxListStore.AppendValues(i + positionOffset, pItemList[i]);
                    _treeInterDictionary.Add(i + positionOffset, tempItemIter);

                    //Detected Current/Inital Selected Value
                    if (pInitialValue != null && pInitialValue == pItemList[i])
                    {
                        currentItemIter = tempItemIter;
                        _value = pItemList[i];
                    };
                }
            };

            //Always Update Model and ActiveIter
            Model = _comboBoxListStore;
            SetActiveIter(currentItemIter);
        }


        void ListComboBox_Changed(object sender, System.EventArgs e)
        {
            ListComboBox combo = sender as ListComboBox;
            if (sender == null) return;

            TreeIter iter;

            if (combo.GetActiveIter(out iter))
            {
                _value = (string)combo.Model.GetValue(iter, 1);
            };
            //_log.Debug(string.Format("_value: [{0}]", _value));
        }
    }
}
