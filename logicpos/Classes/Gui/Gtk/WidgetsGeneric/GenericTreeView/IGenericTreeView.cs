using Gtk;
using logicpos.Classes.Gui.Gtk.BackOffice;
using System;
using System.Collections.Generic;

//Dummy interface: Used Only to Constraing Generics, View for ex.: PosSelectRecordDialog<T>, 
//this way we force T to be a valid Generic that Implements IGenericTreeView, ex GenericTreeViewXPO and GenericTreeViewDataTable :)

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    interface IGenericTreeView
    {
    }
}
