using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.shared.App;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    //T DataSourceRow (XPGuidObject|DataRow)
    internal abstract class PosBaseDialogGenericTreeView<T> : PosBaseDialog
    {
        //Protected Members
        protected DialogMode _dialogMode;
        //Validation Rules
        protected string _regexAlfa = SharedSettings.RegexAlfa;
        protected string _regexAlfaNumeric = SharedSettings.RegexAlfaNumeric;
        protected string _regexAlfaNumericExtended = SharedSettings.RegexAlfaNumericExtended;
        protected string _regexDecimal = SharedSettings.RegexDecimal;
        protected string _regexDecimalGreaterThanZero = SharedSettings.RegexDecimalGreaterThanZero;
        protected string _regexDecimalGreaterEqualThanZero = SharedSettings.RegexDecimalGreaterEqualThanZero;
        protected string _regexGuid = SharedSettings.RegexGuid;
        protected string _regexPercentage = SharedSettings.RegexPercentage;

        //Public Properties, to have access to/from TreeView EventHandlers
        protected T _dataSourceRow;
        public T DataSourceRow
        {
            get { return _dataSourceRow; }
            set { _dataSourceRow = value; }
        }

        public PosBaseDialogGenericTreeView(Window pSourceWindow, DialogFlags pDialogFlags, DialogMode pDialogMode, T pDataSourceRow)
            : base(pSourceWindow, pDialogFlags)
        {
            //Parameters
            _sourceWindow = pSourceWindow;
            _dialogMode = pDialogMode;
            _dataSourceRow = pDataSourceRow;
        }
    }
}
