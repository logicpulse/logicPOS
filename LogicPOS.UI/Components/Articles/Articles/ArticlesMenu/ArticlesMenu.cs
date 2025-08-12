using Gtk;
using logicpos.Classes.Logic.Others;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.GetArticles;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Common.Menus;
using System.Drawing;

namespace LogicPOS.UI.Components.Menus
{
    public partial class ArticlesMenu : Menu<ArticleViewModel>
    {
        public ArticleSubfamiliesMenu MenuSubfamilies { get; }
        private PaginatedResult<ArticleViewModel> Articles { get; set; }
        private GetArticlesQuery CurrentQuery { get; set; } = GetFavoriteQuery();
        private string ButtonName => "buttonArticleId";
        private Size ButtonSize { get; }

        private const int DefaultPageSize = 42;

        public bool PresentFavorites { get; set; }

        public ArticlesMenu(CustomButton btnPrevious,
                            CustomButton btnNext,
                            Window sourceWindow,
                            ArticleSubfamiliesMenu subfamiliesMenu,
                            Size menuButtonSize,
                            TableConfig tableConfig) : base(tableConfig.Rows,
                                                            tableConfig.Columns,
                                                            btnPrevious,
                                                            btnNext,
                                                            sourceWindow,
                                                            toggleMode: false)
        {
            ButtonSize = menuButtonSize;
            MenuSubfamilies = subfamiliesMenu;
            AddEventHandlers();
        }

        protected override CustomButton CreateButtonForEntity(ArticleViewModel entity)
        {
            string label = string.IsNullOrWhiteSpace(entity.Button?.Label) ? entity.Designation : entity.Button.Label;
            string image = GetButtonImage(entity);

            return MenuButton<ArticleViewModel>.CreateButton(ButtonName, label, image, ButtonSize);
        }

        private static GetArticlesQuery GetFavoriteQuery()
        {
            return new GetArticlesQuery
            {
                PageSize = DefaultPageSize,
                Favorite = true
            };
        }

        private string GetButtonImage(ArticleViewModel article)
        {
            if (string.IsNullOrWhiteSpace(article.Button?.ImageExtension) == false)
            {
                string imagePath = ButtonImageCache.GetImagePath(article.Id, article.Button.ImageExtension);

                if (imagePath != null)
                {
                    return imagePath;
                }

                string base64Image = ArticlesService.GetArticleImage(article.Id);

                imagePath = ButtonImageCache.AddBase64Image(article.Id, base64Image, article.Button.ImageExtension);

                return imagePath;
            }

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
            if (MenuSubfamilies.SelectedEntity == null)
            {
                Entities.Clear();
                return;
            }
            CurrentQuery.SubFamilyId = MenuSubfamilies.SelectedEntity.Id;

            if (PresentFavorites)
            {
                CurrentQuery.SubFamilyId = null;
                PresentFavorites = false;
                return;
            }

            CurrentQuery.SubFamilyId = MenuSubfamilies.SelectedEntity.Id;
        }

        protected override void UpdateNavigationButtons()
        {
            BtnPrevious.Sensitive = Articles.Page > 1;
            BtnNext.Sensitive = Articles.Page < Articles.TotalPages;
        }
    }
}
