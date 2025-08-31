using Gtk;
using logicpos.Classes.Logic.Others;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Articles.GetArticles;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Common.Buttons;
using LogicPOS.UI.Components.Common.Menus;
using System;
using System.ComponentModel;
using System.Drawing;

namespace LogicPOS.UI.Components.Menus
{
    public partial class ArticlesMenu : Menu<ArticleViewModel>
    {
        private readonly ButtonImageProcessor _imageProcessor = new ButtonImageProcessor();
        public ArticleSubfamiliesMenu MenuSubfamilies { get; }
        private PaginatedResult<ArticleViewModel> Articles { get; set; }
        private GetArticlesQuery CurrentQuery { get; set; } = GetFavoriteQuery();
        private string ButtonName => "buttonArticleId";
        private Size ButtonSize { get; }

        private static int DefaultPageSize = 42;
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
            DefaultPageSize = (Int32)(tableConfig.Columns * tableConfig.Rows);
            AddEventHandlers();
        }

        protected override CustomButton CreateButtonForEntity(ArticleViewModel entity)
        {
            string label = string.IsNullOrWhiteSpace(entity.Button?.Label) ? entity.Designation : entity.Button.Label;
 
            if (string.IsNullOrWhiteSpace(entity.Button?.ImageExtension))
            {
                return MenuButton<ArticleViewModel>.CreateButton(ButtonName, label, null, ButtonSize);
            }

            var imagePath = ButtonImageCache.GetImagePath(entity.Id, entity.Button.ImageExtension);
            if (imagePath != null)
            {
                return MenuButton<ArticleViewModel>.CreateButton(ButtonName, label, imagePath, ButtonSize);
            }

            var button = MenuButton<ArticleViewModel>.CreateButton(ButtonName, label, null, ButtonSize);
            _imageProcessor.ProcessButtonImage(button as ImageButton, () => GetButtonImageFromApi(entity));

            return button; 
        }

        private static GetArticlesQuery GetFavoriteQuery()
        {
            return new GetArticlesQuery
            {
                PageSize = DefaultPageSize,
                Favorite = true,
                Image = false
            };
        }

    
        private string GetButtonImageFromApi(ArticleViewModel article)
        {
            string base64Image = ArticlesService.GetArticleImage(article.Id);
            return ButtonImageCache.AddBase64Image(article.Id, base64Image, article.Button.ImageExtension);
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
