using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Finance.Agt.RequestSeries;
using LogicPOS.Api.Features.Finance.Documents.Series.CreateAgtSeries;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.DocumentSeries;
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
            var result = DocumentSeriesService.CreateAgtSeries(command);

            if (result)
            {
                CustomAlerts.Information(this)
                    .WithTitle("Solicitar Série")
                    .WithMessage($"Série solicitada com sucesso.")
                    .ShowAlert();
            }

            return result;
        }

        private CreateAgtSeriesCommand CreateAddCommand() => new CreateAgtSeriesCommand
        {
            Designation = $"{_comboDocumentTypes.SelectedEntity?.Designation} {_comboDocumentTypes.SelectedEntity?.Acronym} {_comboFiscalYears.SelectedEntity?.Acronym}",
            FiscalYearId = _comboFiscalYears.SelectedEntity.Id,
            EstablishmentNumber = _txtEstablishmentNumber.Text,
            SeriesContingencyIndicator = _checkContingencyIndicator.Active ? "C" : "N",
            DocumentTypeId = _comboDocumentTypes.SelectedEntity.Id
        };

        protected override void ShowEntityData() { }

        protected override bool UpdateEntity() => false;

    }
}
