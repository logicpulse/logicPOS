using Gtk;
using LogicPOS.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PermissionsPage
    {
        protected override void Design()
        {
            VBox verticalBox = new VBox(false, 1);

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.ShadowType = ShadowType.EtchedIn;
            scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            scrolledWindow.Add(GridView);

            HBox horizontalBox = new HBox(false, 1);
            horizontalBox.PackStart(scrolledWindow);

            verticalBox.PackStart(horizontalBox, true, true, 0);
            verticalBox.PackStart(Navigator, false, false, 0);

            PackStart(verticalBox);

            ScrolledWindow scrolledWindowPermissionItem = new ScrolledWindow() { WidthRequest = 500 };
            scrolledWindowPermissionItem.Add(_gridPermissionItems);

            horizontalBox.Add(scrolledWindowPermissionItem);
        }
    }
}
