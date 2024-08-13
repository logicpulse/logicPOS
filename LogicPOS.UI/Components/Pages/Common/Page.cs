using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Api.Errors;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.GridViews;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public abstract class Page : Box
    {
        protected readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<ISender>();
        public Window PageParentWindow { get; }
        public TreeView GridView { get; set; }
        public object SelectedEntity { get; set; }
        public bool CanViewEntity { get; set; }
        public bool CanUpdateEntity { get; set; }
        public bool CanDeleteEntity { get; set; }
        public GridViewSettings GridViewSettings { get; } = GridViewSettings.Default;
        internal PageNavigator Navigator { get; }

        public Page(Window parent)
        {
            PageParentWindow = parent;
            Navigator = new PageNavigator(this);
        }
       
        protected void ShowApiErrorAlert()
        {
            SimpleAlerts.Error()
                .WithParent(PageParentWindow)
                .WithTitle("API")
                .WithMessage(ApiErrors.CommunicationError.Description)
                .Show();
        }

        internal abstract void Refresh();
        internal abstract void ViewEntity();
        internal abstract void UpdateEntity();
        internal abstract void DeleteEntity();
        internal abstract void InsertEntity();

    }
}
