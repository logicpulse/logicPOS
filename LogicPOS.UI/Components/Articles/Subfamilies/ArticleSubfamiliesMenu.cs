using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Subfamilies.GetAllArticleSubfamilies;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Articles;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Menus
{
    public class ArticleSubfamiliesMenu : Menu<ArticleSubfamily>
    {
        private readonly ISender _mediator = DependencyInjection.Mediator;

        public uint Rows { get; set; } = 1;
        public uint Columns { get; set; } = 7;
        public Size ButtonSize { get; set; } = new Size(176, 120);
        private ArticleFamiliesMenu FamiliesMenu { get; }

        public ArticleSubfamiliesMenu(ArticleFamiliesMenu familiesMenu,
                                      CustomButton btnPrevious,
                                      CustomButton btnNext,
                                      Window sourceWindow) : base(1,
                                                                  7,
                                                                  new Size(176, 120),
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
                return ArticleImageRepository.GetImage(subfamily.Id) ?? ArticleImageRepository.AddBase64Image(subfamily.Id, subfamily.Button.Image, subfamily.Button.ImageExtension);
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
