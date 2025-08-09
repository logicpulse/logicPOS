using Gtk;
using logicpos.Classes.Logic.Others;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Families.GetAllArticleFamilies;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Common.Menus;
using MediatR;
using System.Drawing;

namespace LogicPOS.UI.Components.Menus
{
    public class ArticleFamiliesMenu : Menu<ArticleFamily>
    {
        private readonly ISender _mediator = DependencyInjection.Mediator;
        private string ButtonName => "buttonFamilyId";
        private Size ButtonSize { get; }

        public ArticleFamiliesMenu(CustomButton btnPrevious,
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
            Refresh();
        }

        protected override CustomButton CreateButtonForEntity(ArticleFamily entity)
        {
            string label = entity.Button.Label ?? entity.Designation;
            string image = GetButtonImage(entity);

            return MenuButton<ArticleFamily>.CreateButton(ButtonName, label, image, ButtonSize);
        }

        private string GetButtonImage(ArticleFamily family)
        {
            if (string.IsNullOrEmpty(family.Button.ImageExtension) == false)
            {
                return ButtonImageCache.GetImagePath(family.Id, family.Button.ImageExtension) ??
                    ButtonImageCache.AddBase64Image(family.Id, family.Button.Image, family.Button.ImageExtension);
            }

            return null;
        }

        protected override void LoadEntities()
        {
            Entities.Clear();

            var families = _mediator.Send(new GetAllArticleFamiliesQuery()).Result;

            if (families.IsError != false)
            {
                return;
            }

            Entities.AddRange(families.Value);
        }

    }
}
