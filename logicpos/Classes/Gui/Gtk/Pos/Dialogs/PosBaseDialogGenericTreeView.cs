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
        protected string _regexAlfa = LogicPOS.Utility.RegexUtils.RegexAlfa;
        protected string _regexAlfaNumeric = LogicPOS.Utility.RegexUtils.RegexAlfaNumeric;
        protected string _regexAlfaNumericExtended = LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended;
        protected string _regexDecimal = LogicPOS.Utility.RegexUtils.RegexDecimal;
        protected string _regexDecimalGreaterThanZero = LogicPOS.Utility.RegexUtils.RegexDecimalGreaterThanZero;
        protected string _regexDecimalGreaterEqualThanZero = LogicPOS.Utility.RegexUtils.RegexDecimalGreaterEqualThanZero;
        protected string _regexGuid = LogicPOS.Utility.RegexUtils.RegexGuid;
        protected string _regexPercentage = LogicPOS.Utility.RegexUtils.RegexPercentage;

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
