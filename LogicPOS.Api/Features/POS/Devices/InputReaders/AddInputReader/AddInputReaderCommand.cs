using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.InputReaders.AddInputReader
{
    public class AddInputReaderCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public string ReaderSizes { get; set; }
        public string Notes { get; set; }
    }
}
