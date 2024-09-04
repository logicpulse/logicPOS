using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.InputFields
{
    public class EntityComboBox<TEntity> where TEntity : ApiEntity, IWithDesignation
    {
        public ComboBox Component { get; private set; } = new ComboBox { HeightRequest = 23 };
        private ListStore _listStore = new ListStore(typeof(string), typeof(TEntity));
        public IEnumerable<TEntity> Entities { get; private set; }
        public TEntity SelectedEntity { get; private set; }

        public EntityComboBox(IEnumerable<TEntity> entities, Guid? selectedEntityId = null)
        {
            Entities = entities;
            
            if (selectedEntityId.HasValue)
            {
                SelectEntityById(selectedEntityId.Value);
            }

            InitializeComboBox();
        }

        private void InitializeComboBox()
        {
            Component = new ComboBox { HeightRequest = 23 };
            CellRendererText cellRenderer = new CellRendererText();
            Component.PackStart(cellRenderer, true);
            Component.AddAttribute(cellRenderer, "text", 0);

            AddDefaultItem();
            AddEntitiesToModel();

            Component.Changed += OnChanged;
        }

        private void OnChanged(object sender, EventArgs e)
        {
            TreeIter iter;

            if (Component.GetActiveIter(out iter))
            {
                SelectedEntity = (TEntity)Component.Model.GetValue(iter, 1);
            };
        }

        private void SelectEntityById(Guid id)
        {
            SelectedEntity = Entities.FirstOrDefault(e => e.Id == id);
        }

        private void AddEntitiesToModel()
        {
            foreach (var entity in Entities)
            {
                var iterator = _listStore.AppendValues(entity.Designation, entity);

                if (SelectedEntity != null && entity.Id == SelectedEntity.Id)
                {
                    Component.SetActiveIter(iterator);
                }
            }

            Component.Model = _listStore;
        }

        private void AddDefaultItem()
        {
            _listStore.AppendValues(GeneralUtils.GetResourceByName("widget_combobox_undefined"), null);
        }
    }
}
