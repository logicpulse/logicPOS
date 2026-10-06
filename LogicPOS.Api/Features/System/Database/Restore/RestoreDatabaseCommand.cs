using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Database
{
    public class RestoreDatabaseCommand : IRequest<ErrorOr<Success>>
    {
        /// <summary>
        /// Null restores the latest backup.
        /// </summary>
        public Guid? BackupId { get; set; }
    }
}
