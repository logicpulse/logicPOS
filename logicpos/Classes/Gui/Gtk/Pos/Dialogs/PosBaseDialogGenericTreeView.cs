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
    internal abstract class PosBaseDialogGenericTreeView<T> : PosBaseDialog
    {
        //Protected Members
        protected DialogMode _dialogMode;
        //Validation Rules
        protected string _regexAlfa = SettingsApp.RegexAlfa;
        protected string _regexAlfaNumeric = SettingsApp.RegexAlfaNumeric;
        protected string _regexAlfaNumericExtended = SettingsApp.RegexAlfaNumericExtended;
        protected string _regexDecimal = SettingsApp.RegexDecimal;
        protected string _regexDecimalGreaterThanZero = SettingsApp.RegexDecimalGreaterThanZero;
        protected string _regexDecimalGreaterEqualThanZero = SettingsApp.RegexDecimalGreaterEqualThanZero;
        protected string _regexGuid = SettingsApp.RegexGuid;
        protected string _regexPercentage = SettingsApp.RegexPercentage;

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
