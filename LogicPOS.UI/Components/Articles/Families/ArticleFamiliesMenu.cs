using Gtk;
using logicpos.Classes.Logic.Others;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Families.GetAllArticleFamilies;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles;
using MediatR;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Menus
{
    public class ArticleFamiliesMenu : Menu<ArticleFamily>
    {
        private readonly ISender _mediator = DependencyInjection.Mediator;

        public ArticleFamiliesMenu(CustomButton btnPrevious,
                                   CustomButton btnNext,
                                   Window sourceWindow, 
                                   Size buttonsSize,
                                   TableConfig tableConfig) : base(tableConfig.Rows,
                                                               tableConfig.Columns,
                                                               buttonsSize,
                                                               buttonName: "buttonFamilyId",
                                                               btnPrevious,
                                                               btnNext,
                                                               sourceWindow)
        {
            LoadEntities();
            ListEntities(Entities);
        }

        protected override string GetButtonLabel(ArticleFamily family)
        {
            return family.Button.Label ?? family.Designation;
        }

        protected override string GetButtonImage(ArticleFamily family)
        {
            if (string.IsNullOrEmpty(family.Button.ImageExtension) == false)
            {
                return ButtonImageRepository.GetImage(family.Id) ?? ButtonImageRepository.AddBase64Image(family.Id, family.Button.Image, family.Button.ImageExtension);
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

        protected override IEnumerable<ArticleFamily> FilterEntities(IEnumerable<ArticleFamily> entities)
        {
            
            return entities;
        }
    }
}
