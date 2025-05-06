using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Families.GetAllArticleFamilies;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Menus
{
    public class ArticleFamiliesMenu : Menu<ArticleFamily>
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();

        public ArticleFamiliesMenu(CustomButton btnPrevious,
                                   CustomButton btnNext,
                                   Window sourceWindow) : base(rows: 6,
                                                               columns: 1,
                                                               new Size(176, 120),
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
                return ArticleImageRepository.GetImage(family.Id) ?? ArticleImageRepository.AddBase64Image(family.Id, family.Button.Image, family.Button.ImageExtension);
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
