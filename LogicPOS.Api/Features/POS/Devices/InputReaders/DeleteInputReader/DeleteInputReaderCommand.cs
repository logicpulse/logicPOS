using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.InputReaders.DeleteInputReader
{
    public class DeleteInputReaderCommand : DeleteCommand
    {
        public DeleteInputReaderCommand(Guid id) : base(id)
        {
        }
    }
}
