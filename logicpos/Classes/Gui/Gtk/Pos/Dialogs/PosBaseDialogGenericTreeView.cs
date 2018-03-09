using Gtk;
using logicpos.financial;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using System;
using System.Collections.Generic;
using System.Data;
using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    //T DataSourceRow (XPGuidObject|DataRow)
    abstract class PosBaseDialogGenericTreeView<T> : PosBaseDialog
    {
        //Protected Members
        protected DialogMode _dialogMode;
        //Validation Rules
        protected String _regexAlfa = SettingsApp.RegexAlfa;
        protected String _regexAlfaNumeric = SettingsApp.RegexAlfaNumeric;
        protected String _regexAlfaNumericExtended = SettingsApp.RegexAlfaNumericExtended;
        protected String _regexDecimal = SettingsApp.RegexDecimal;
        protected String _regexDecimalGreaterThanZero = SettingsApp.RegexDecimalGreaterThanZero;
        protected String _regexDecimalGreaterEqualThanZero = SettingsApp.RegexDecimalGreaterEqualThanZero;
        protected String _regexGuid = SettingsApp.RegexGuid;
        protected String _regexPercentage = SettingsApp.RegexPercentage;

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
