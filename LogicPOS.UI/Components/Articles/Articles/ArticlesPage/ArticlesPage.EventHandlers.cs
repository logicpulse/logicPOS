using System;

namespace LogicPOS.UI.Components.Pages
{
    public partial class ArticlesPage
    {
        private void AddEventHandlers()
        {
            Navigator.SearchBox.BtnMore.Clicked += BtnMore_Clicked;
        }

        public void BtnMore_Clicked(object sender, EventArgs e)
        {
            if (CurrentQuery.Page >= Articles.TotalPages)
            {
                return;
            }

            var paginatedResult = ShowMore(CurrentQuery);

            if (paginatedResult == null)
            {
                return;
            }

            Articles = paginatedResult.Value;
            LoadCurrentArticlesStocks();

            AddEntitiesToModel(Articles.Items);
        }
    }
}
