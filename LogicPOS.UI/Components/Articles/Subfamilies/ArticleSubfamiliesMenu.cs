using Gtk;
using logicpos.Classes.Logic.Others;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Subfamilies.GetAllArticleSubfamilies;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles;
using MediatR;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Menus
{
    public class ArticleSubfamiliesMenu : Menu<ArticleSubfamily>
    {
        private readonly ISender _mediator = DependencyInjection.Mediator;

        private ArticleFamiliesMenu FamiliesMenu { get; }

        public ArticleSubfamiliesMenu(ArticleFamiliesMenu familiesMenu,
                                      CustomButton btnPrevious,
                                      CustomButton btnNext,
                                      Window sourceWindow,
                                      Size buttonsSize,
                                      TableConfig tableConfig) : base(tableConfig.Rows,
                                                                  tableConfig.Columns,
                                                                  buttonsSize,
                                                                  "buttonSubFamilyId",
                                                                  btnPrevious,
                                                                  btnNext,
                                                                  sourceWindow)
        {
            FamiliesMenu = familiesMenu;
            AddEventHandlers();
            LoadEntities();
            ListEntities(Entities);
        }

        private void AddEventHandlers()
        {
            FamiliesMenu.OnEntitySelected += FamiliesMenu_FamilySelected;
        }

        private void FamiliesMenu_FamilySelected(ArticleFamily family)
        {
            Refresh();
        }

        protected override string GetButtonLabel(ArticleSubfamily subfamily)
        {
            return subfamily.Button.Label ?? subfamily.Designation;
        }

        protected override string GetButtonImage(ArticleSubfamily subfamily)
        {
            if (string.IsNullOrEmpty(subfamily.Button.ImageExtension) == false)
            {
                return ButtonImageCache.GetImagePath(subfamily.Id, subfamily.Button.ImageExtension) ?? ButtonImageCache.AddBase64Image(subfamily.Id, subfamily.Button.Image, subfamily.Button.ImageExtension);
            }

            return null;
        }

        protected override void LoadEntities()
        {
            Entities.Clear();
            var subfamilies = _mediator.Send(new GetAllArticleSubfamiliesQuery()).Result;

            if (subfamilies.IsError != false)
            {
                return;
            }

            Entities.AddRange(subfamilies.Value);
        }

        protected override IEnumerable<ArticleSubfamily> FilterEntities(IEnumerable<ArticleSubfamily> entities)
        {
            return entities.Where(s => s.FamilyId == FamiliesMenu.SelectedEntity.Id);
        }
    }
}
