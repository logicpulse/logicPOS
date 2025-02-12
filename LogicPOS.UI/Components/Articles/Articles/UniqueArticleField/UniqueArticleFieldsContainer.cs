using Gtk;
using LogicPOS.UI.Extensions;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.InputFields
{
    public class UniqueArticleFieldsContainer
    {
        private List<UniqueArticleField> Fields { get; } = new List<UniqueArticleField>();

        public Widget Component { get; private set; }

        public UniqueArticleFieldsContainer()
        {
            Fields.Add(new UniqueArticleField());
            Fields.Add(new UniqueArticleField());
            Fields.Add(new UniqueArticleField());
            Fields.Add(new UniqueArticleField());
            Fields.Add(new UniqueArticleField());

            Component = CreateComponent();
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
