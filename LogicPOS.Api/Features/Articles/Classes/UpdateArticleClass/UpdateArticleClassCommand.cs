using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.Classes.UpdateArticleClass
{
    public class UpdateArticleClassCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; }
        public string NewAcronym {  get; set; }
        public bool WorkInStock {  get; set; }
        public string NewNotes { get; set; }
        public bool IsDeleted { get; set; }
    }
}
