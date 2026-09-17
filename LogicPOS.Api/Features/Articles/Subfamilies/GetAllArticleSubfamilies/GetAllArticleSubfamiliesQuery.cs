using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Articles.Subfamilies.GetAllArticleSubfamilies
{
    public class GetAllArticleSubfamiliesQuery : IRequest<ErrorOr<IEnumerable<ArticleSubfamily>>>
    {
        public Guid? FamilyId { get; set; }

        public string GetUrlQuery()
        {
            if (FamilyId == null) { return string.Empty; }

            return $"?FamilyId={FamilyId}";
        }
    }
}
