using Gtk;
using LogicPOS.Api.Features.Articles.StockManagement.GetUniqueArticles;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Extensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.InputFields
{
    public class UniqueArticleFieldsContainer
    {
        protected readonly ISender _mediator = DependencyInjection.Mediator;
        private readonly Guid _articleId;
        private List<UniqueArticleField> Fields { get; } = new List<UniqueArticleField>();

        public Widget Component { get; private set; }

        public UniqueArticleFieldsContainer(Guid articleId)
        {
            _articleId = articleId;
            InitializeFields();
            Component = CreateComponent();
        }

        private void InitializeFields()
        {
            var result = _mediator.Send(new GetUniqueArticlesQuery(_articleId)).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return;
            }

            foreach (var uniqueArticle in result.Value)
            {
                var field = new UniqueArticleField(uniqueArticle);
                Fields.Add(field);
            }
        }

        private Widget CreateComponent()
        {
            var vbox = new VBox(false, 5);
            foreach (var field in Fields)
            {
                vbox.PackStart(field.Component, false, false, 0);
            }

            var swindow = new ScrolledWindow();
            swindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            swindow.ModifyBg(StateType.Normal, Color.White.ToGdkColor());
            swindow.ShadowType = ShadowType.None;
            swindow.AddWithViewport(vbox);

            return swindow;
        }
    }
}
