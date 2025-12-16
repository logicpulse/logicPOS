using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.InputFields
{
    public class EntityComboBox<TEntity> : IValidatableField where TEntity : ApiEntity, IWithDesignation
    {
        public Label Label { get; }
        public VBox Component { get; private set; }
        public ComboBox ComboBox { get; private set; } = new ComboBox { HeightRequest = 23 };

        public bool IsRequired { get; set; }

        private ListStore _listStore = new ListStore(typeof(string), typeof(TEntity));
        public IEnumerable<TEntity> Entities { get; set; }
        public TEntity SelectedEntity { get; set; }
        public string FieldName => Label.Text;

        public EntityComboBox(string labelText,
                              IEnumerable<TEntity> entities,
                              TEntity currentEntity = null,
                              bool isRequired = false)
        {
            IsRequired = isRequired;
            Label = CreateLabel(labelText);
            Entities = entities;
            SelectedEntity = currentEntity;

            InitializeComboBox();

            Component = CreateComponent();

            UpdateValidationColors();
        }

        private Label CreateLabel(string text)
        {
            var label = new Label(text);
            label.SetAlignment(0, 0);
            return label;
        }

        private void InitializeComboBox()
        {
            ComboBox = CreateComboBox();
            ReLoad();
        }

        public void ReLoad()
        {
            _listStore.Clear();
            AddDefaultItem();
            AddEntitiesToModel();
        }

        private ComboBox CreateComboBox()
        {
            var combobox = new ComboBox { HeightRequest = 23 };
            CellRendererText cellRenderer = new CellRendererText();
            combobox.PackStart(cellRenderer, true);
            combobox.AddAttribute(cellRenderer, "text", 0);
            combobox.Changed += OnChanged;
            return combobox;
        }

        private void OnChanged(object sender, EventArgs e)
        {
            TreeIter iter;

            if (ComboBox.GetActiveIter(out iter))
            {
                SelectedEntity = (TEntity)ComboBox.Model.GetValue(iter, 1);
            };

            UpdateValidationColors();
        }

        private void AddEntitiesToModel()
        {
            if (Entities == null)
            {
                return;
            }

            TreeIter currentEntity = TreeIter.Zero;

            foreach (var entity in Entities)
            {
                if (entity == null)
                {
                    continue;
                }

                var iterator = _listStore.AppendValues(entity.Designation, entity);

                if (SelectedEntity != null && entity.Id == SelectedEntity.Id)
                {
                    currentEntity = iterator;
                }
            }

            ComboBox.Model = _listStore;

            if (SelectedEntity != null)
            {
                ComboBox.SetActiveIter(currentEntity);
            }
        }

        private void AddDefaultItem()
        {
            var iterator = _listStore.AppendValues(GeneralUtils.GetResourceByName("widget_combobox_undefined"), null);
            ComboBox.SetActiveIter(iterator);

            if (SelectedEntity == null)
            {
                ComboBox.Active = 0;
            }
        }

        private VBox CreateComponent()
        {
            var box = new VBox(false, 2);
            box.PackStart(Label, false, false, 0);
            box.PackStart(ComboBox, false, false, 0);
            return box;
        }

        public bool IsValid()
        {
            return IsRequired ? SelectedEntity != null : true;
        }

        public void UpdateValidationColors()
        {
            ValidationColors.Default.UpdateComponentFontColor(Label, IsValid());
            ValidationColors.Default.UpdateComponent(ComboBox, IsValid());
        }
    }
}
