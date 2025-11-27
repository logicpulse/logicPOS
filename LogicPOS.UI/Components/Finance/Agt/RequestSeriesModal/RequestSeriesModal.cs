using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Finance.Agt.RequestSeries;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Modals;

namespace LogicPOS.UI.Components.Finance.Agt.RequestSeriesModal
{
    public partial class RequestSeriesModal : EntityEditionModal<ApiEntity>
    {
        public RequestSeriesModal() : base(EntityEditionModalMode.Insert, null)
        {
        }

        protected override bool AddEntity()
        {
            var confirm = CustomAlerts.Question(this)
                .WithTitle("Solicitar Série?")
                .WithMessage($"Tem a certeza que pretende solicitar uma série para o tipo de documento {_comboDocumentTypes.SelectedEntity.Designation}? " +
                $"\nEsta solicitação será enviada à AGT e poderá ser irreversível.")
                .ShowAlert();
            
            if (confirm != Gtk.ResponseType.Yes)
            {
                return false;
            }

            var command = CreateAddCommand();
            var series = AgtService.RequestSeries(command);

            if (series != null)
            {
                CustomAlerts.Information(this)
                    .WithTitle("Solicitar Série")
                    .WithMessage($"Série solicitada com sucesso." +
                    $"\nCódigo da Série: {series.Value.Code}" +
                    $"\nQuantity de Documentos: {series.Value.Quantity}")
                    .ShowAlert();
            }

            return series != null;
        }

        private RequestSeriesCommand CreateAddCommand() => new RequestSeriesCommand
        {
            Year = int.Parse(_txtYear.Text),
            EstablishmentNumber = _txtEstablishmentNumber.Text,
            SeriesContingencyIndicator = _checkContingencyIndicator.Active ? "C" : "N",
            DocumentType = _comboDocumentTypes.SelectedEntity.Acronym
        };

        protected override void ShowEntityData() { }

        protected override bool UpdateEntity() => false;

    }
}
