using Gtk;
using logicpos.Classes.Logic.Others;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Subfamilies.GetAllArticleSubfamilies;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Common.Menus;
using MediatR;
using System;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Menus
{
    public class ArticleSubfamiliesMenu : Menu<ArticleSubfamily>
    {
        private readonly ISender _mediator = DependencyInjection.Mediator;
        private string ButtonName => "buttonSubFamilyId";
        private Size ButtonSize { get; }

        public ArticleFamiliesMenu FamiliesMenu { get; }

        public ArticleSubfamiliesMenu(ArticleFamiliesMenu familiesMenu,
                                      CustomButton btnPrevious,
                                      CustomButton btnNext,
                                      Window sourceWindow,
                                      Size buttonsSize,
                                      TableConfig tableConfig) : base(tableConfig.Rows,
                                                                  tableConfig.Columns,
                                                                  btnPrevious,
                                                                  btnNext,
                                                                  sourceWindow)
        {
            SelectFirstOnReload = true;
            ButtonSize = buttonsSize;
            FamiliesMenu = familiesMenu;
            AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            FamiliesMenu.OnEntitySelected += FamiliesMenu_FamilySelected;
        }

        private void FamiliesMenu_FamilySelected(ArticleFamily family)
        {
            Refresh();
        }

        protected override CustomButton CreateButtonForEntity(ArticleSubfamily entity)
        {
            string label = string.IsNullOrWhiteSpace(entity.Button.Label) ? entity.Designation : entity.Button.Label;
            string image = GetButtonImage(entity);

            return MenuButton<ArticleSubfamily>.CreateButton(ButtonName, label, image, ButtonSize);
        }

        private string GetButtonImage(ArticleSubfamily subfamily)
        {
            if (string.IsNullOrEmpty(subfamily.Button.ImageExtension) == false)
            {
                return ButtonImageCache.GetImageLocation(subfamily.Id, subfamily.Button.ImageExtension) ?? ButtonImageCache.AddBase64Image(subfamily.Id, subfamily.Button.Image, subfamily.Button.ImageExtension);
            }

            return null;
        }

        protected override void LoadEntities()
        {
            Entities.Clear();

            if (FamiliesMenu.SelectedEntity == null)
            {
                return;
            }

            var subfamilies = _mediator.Send(new GetAllArticleSubfamiliesQuery { FamilyId = FamiliesMenu.SelectedEntity.Id }).Result;

            if (subfamilies.IsError != false)
            {
                return;
            }
            if (subfamilies.Value.Count() == 0)
            {
                SelectedEntity = null;
            }
            Entities.AddRange(subfamilies.Value);
        }

    }
}
