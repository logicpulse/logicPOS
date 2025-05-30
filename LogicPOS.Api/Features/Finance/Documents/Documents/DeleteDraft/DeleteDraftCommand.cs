using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Documents.DeleteDraft
{
    public class DeleteDraftCommand : DeleteCommand
    {
        public DeleteDraftCommand(Guid id) : base(id)
        {
        }
    }
}
