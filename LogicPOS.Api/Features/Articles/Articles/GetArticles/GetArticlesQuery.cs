using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Common.Pagination;
using System;
using System.Text;

namespace LogicPOS.Api.Features.Articles.GetArticles
{
    public class GetArticlesQuery : PaginationQuery<ArticleViewModel>
    {
        public Guid? FamilyId { get; set; }
        public Guid? SubFamilyId { get; set; }
        public bool? Favorite { get; set; }

        protected override void BuildQuery(StringBuilder urlQueryBuilder)
        {
            if (FamilyId.HasValue)
            {
                urlQueryBuilder.Append($"&familyId={FamilyId}");
            }

            if (SubFamilyId.HasValue)
            {
                urlQueryBuilder.Append($"&subfamilyId={SubFamilyId}");
            }

            if (Favorite.HasValue)
            {
                urlQueryBuilder.Append($"&favorite={Favorite}");
            }
        }
    }

}
