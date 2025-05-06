using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.GetArticles;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Menus
{
    public partial class ArticlesMenu : Menu<ArticleViewModel>
    {
        public ArticleSubfamiliesMenu MenuSubfamilies { get; }
        private PaginatedResult<ArticleViewModel> Articles { get; set; }
        private GetArticlesQuery CurrentQuery { get; set; } = GetFavoriteQuery();

        private const int DefaultPageSize = 42;

        public bool PresentFavorites { get; set; }

        public ArticlesMenu(CustomButton btnPrevious,
                            CustomButton btnNext,
                            Window sourceWindow,
                            ArticleSubfamiliesMenu subfamiliesMenu) : base(rows: 6,
                                                                           columns: 7,
                                                                           buttonSize: new Size(176, 120),
                                                                           buttonName: "buttonArticleId",
                                                                           btnPrevious,
                                                                           btnNext,
                                                                           sourceWindow,
                                                                           toggleMode: false)
        {
            MenuSubfamilies = subfamiliesMenu;
            AddEventHandlers();
        }

        private static GetArticlesQuery GetFavoriteQuery()
        {
            return new GetArticlesQuery
            {
                PageSize = DefaultPageSize,
                Favorite = true
            };
        }


        protected override string GetButtonLabel(ArticleViewModel entity)
        {
            return entity.ButtonLabel ?? entity.Designation;
        }

        protected override string GetButtonImage(ArticleViewModel article)
        {
            //if (string.IsNullOrEmpty(article.Button.ImageExtension) == false)
            //{
            //    return ArticleImageRepository.GetImage(article.Id) ?? ArticleImageRepository.AddBase64Image(article.Id, article.Button.Image, article.Button.ImageExtension);
            //}

            return null;
        }

        protected override void LoadEntities()
        {
            Entities.Clear();
            UpdateCurrentQuery();
            Articles = ArticlesService.GetArticles(CurrentQuery);
            Entities.AddRange(Articles.Items);
        }

        private void UpdateCurrentQuery()
        {
            CurrentQuery.Favorite = PresentFavorites ? true : (bool?)null;
            CurrentQuery.PageSize = DefaultPageSize;
            CurrentQuery.SubFamilyId = MenuSubfamilies.SelectedEntity.Id;

            if (PresentFavorites)
            {
                CurrentQuery.SubFamilyId = null;
                PresentFavorites = false;
                return;
            }

            CurrentQuery.SubFamilyId = MenuSubfamilies.SelectedEntity.Id;
        }

        protected override IEnumerable<ArticleViewModel> FilterEntities(IEnumerable<ArticleViewModel> entities)
        {
            throw new NotImplementedException();
        }

        public override void Refresh()
        {
            Buttons.Clear();
            LoadEntities();
            ListEntities(Entities);
        }

        protected override void UpdateNavigationButtons()
        {
            BtnPrevious.Sensitive = Articles.Page > 1;
            BtnNext.Sensitive = Articles.Page < Articles.TotalPages;

        }
    }
}
