using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Articles.Classes.AddArticleClass
{
    public class AddArticleClassCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation {  get; set; }
        public string Acronym { get; set; }
        public bool WorkInStock {  get; set; }
        public string Notes { get; set; }
    }
}
