using ErrorOr;
using MediatR;

namespace LogicPOS.Api.Features.Database.Restore
{
    public class RestoreDatabaseCommand : IRequest<ErrorOr<Unit>>
    {
        public string BackupPath { get; set; }
    }
}
