using Gtk;
using LogicPOS.UI.Components.Enums;
using LogicPOS.UI.Components.InputFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Articles
{
    public partial class ArticleFieldsContainer
    {
        public List<ArticleField> Fields { get; } = new List<ArticleField>();
        public VBox Container { get; } = new VBox(false, 5);
        public ScrolledWindow Component { get; private set; }
        private readonly ArticlesBoxMode _mode;
    }
}
