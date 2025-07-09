using Gtk;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;


namespace LogicPOS.UI.Components.Pages
{
	public partial class ArticlesPage
	{
		protected override void AddColumns()
		{
			GridView.AppendColumn(Columns.CreateCodeColumn(0));
			GridView.AppendColumn(Columns.CreateDesignationColumn(1));
			GridView.AppendColumn(CreatesTotalStockColumn());
			GridView.AppendColumn(CreateCompositeColumn());
			GridView.AppendColumn(CreateFamilyColumn());
			GridView.AppendColumn(CreateSubfamilyColumn());
			GridView.AppendColumn(CreateTypeColumn());
			GridView.AppendColumn(Columns.CreateUpdatedAtColumn(7));
		}

		private TreeViewColumn CreateCompositeColumn()
		{
			void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
			{
				var article = (ArticleViewModel)model.GetValue(iter, 0);
				(cell as CellRendererText).Text = article.IsComposed ? GeneralUtils.GetResourceByName("global_treeview_true") : GeneralUtils.GetResourceByName("global_treeview_false");
			}

			var title = GeneralUtils.GetResourceByName("global_composite_article");
			return Columns.CreateColumn(title, 3, RenderMonth);
		}

		private TreeViewColumn CreateFamilyColumn()
		{
			void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
			{
				var article = (ArticleViewModel)model.GetValue(iter, 0);
				(cell as CellRendererText).Text = article.Family;
			}

			var title = GeneralUtils.GetResourceByName("global_article_family");
			return Columns.CreateColumn(title, 4, RenderMonth);
		}

		private TreeViewColumn CreateSubfamilyColumn()
		{
			void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
			{
				var article = (ArticleViewModel)model.GetValue(iter, 0);
				(cell as CellRendererText).Text = article.Subfamily;
			}

			var title = GeneralUtils.GetResourceByName("global_article_subfamily");
			return Columns.CreateColumn(title, 5, RenderMonth);
		}

		private TreeViewColumn CreatesTotalStockColumn()
        {
            void RenderTotalStock(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
				var articleId = ((ArticleViewModel)model.GetValue(iter, 0)).Id;
				(cell as CellRendererText).Text = (ArticleTotalStockService.GetArticleTotalStock(articleId)).ToString("0.#####");
			}

			var title = GeneralUtils.GetResourceByName("global_total_stock");
			return Columns.CreateColumn(title, 2, RenderTotalStock);
		}

		private TreeViewColumn CreateTypeColumn()
		{
			void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
			{
				var article = (ArticleViewModel)model.GetValue(iter, 0);
				(cell as CellRendererText).Text = article.Type;
			}

			var title = GeneralUtils.GetResourceByName("global_article_type");
			return Columns.CreateColumn(title, 6, RenderMonth);
		}

		protected override void InitializeSort()
		{
			GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

			AddCodeSorting(0);
			AddDesignationSorting(1);
			AddTotalStockSorting();
			AddCompositeSorting();
			AddFamilySorting();
			AddSubfamilySorting();
			AddTypeSorting();
			AddUpdatedAtSorting(7);
		}

		private void AddTotalStockSorting()
		{
			GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
			{
				var leftArticle = (ArticleViewModel)model.GetValue(left, 0);
				var rightArticle = (ArticleViewModel)model.GetValue(right, 0);

				if (leftArticle == null || rightArticle == null)
				{
					return 0;
				}

				return 0;
			});
		}

		private void AddCompositeSorting()
		{
			GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
			{
				var leftArticle = (ArticleViewModel)model.GetValue(left, 0);
				var rightArticle = (ArticleViewModel)model.GetValue(right, 0);

				if (leftArticle == null || rightArticle == null)
				{
					return 0;
				}

				return leftArticle.IsComposed.CompareTo(rightArticle.IsComposed);
			});
		}

		private void AddFamilySorting()
		{
			GridViewSettings.Sort.SetSortFunc(4, (model, left, right) =>
			{
				var leftArticle = (ArticleViewModel)model.GetValue(left, 0);
				var rightArticle = (ArticleViewModel)model.GetValue(right, 0);

				if (leftArticle == null || rightArticle == null)
				{
					return 0;
				}

				return leftArticle.Family.CompareTo(rightArticle.Family);
			});
		}

		private void AddSubfamilySorting()
		{
			GridViewSettings.Sort.SetSortFunc(5, (model, left, right) =>
			{
				var leftArticle = (ArticleViewModel)model.GetValue(left, 0);
				var rightArticle = (ArticleViewModel)model.GetValue(right, 0);

				if (leftArticle == null || rightArticle == null)
				{
					return 0;
				}

				return leftArticle.Subfamily.CompareTo(rightArticle.Subfamily);
			});
		}

		private void AddTypeSorting()
		{
			GridViewSettings.Sort.SetSortFunc(6, (model, left, right) =>
			{
				var leftArticle = (ArticleViewModel)model.GetValue(left, 0);
				var rightArticle = (ArticleViewModel)model.GetValue(right, 0);

				if (leftArticle == null || rightArticle == null)
				{
					return 0;
				}

				return leftArticle.Type.CompareTo(rightArticle.Type);
			});
		}
	}
}
