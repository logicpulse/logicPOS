﻿using Gtk;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System;
using System.Text.RegularExpressions;

namespace LogicPOS.UI.Components.InputFields
{
    public class TextBox : IValidatableField
    {
        public Entry Entry { get; private set; } = new Entry();
        public string Text { get => Entry.Text; set => Entry.Text = value; }
        public Label Label { get; private set; }
        public bool IsRequired { get; private set; }
        private bool IsEmpty => string.IsNullOrEmpty(Entry.Text);
        public bool IsValidatable { get; private set; }
        private readonly string _regex;
        public VBox Component { get; private set; }
        public string FieldName => Label.Text;

        public TextBox(string labelResourceName,
                       bool isRequired = false,
                       bool isValidatable = false,
                       string regex = null)
        {
            if (isValidatable && string.IsNullOrEmpty(regex))
            {
                throw new ArgumentException("Regex must be provided when the field is validatable");
            }

            Label = CreateLabel(labelResourceName);
            IsRequired = isRequired;
            IsValidatable = isValidatable;
            _regex = regex;
            Component = CreateComponent();
            AddEventHandlers();
            UpdateValidationColors();
        }

        public bool IsValid()
        {
            if (IsEmpty)
            {
                if (IsRequired)
                {
                    return false;
                }
            }
            else
            {
                if (IsValidatable)
                {
                    return Regex.IsMatch(Entry.Text, _regex);
                }
            }

            return true;
        }

        private void AddEventHandlers()
        {
            Entry.Changed += (sender, args) =>
            {
                UpdateValidationColors();
            };
        }

        private void UpdateValidationColors()
        {
            ValidationColors.Default.UpdateComponentFontColor(Label, IsValid());
            ValidationColors.Default.UpdateComponent(Entry, IsValid());
        }

        private Label CreateLabel(string labelResourceName)
        {
            var label = new Label(GeneralUtils.GetResourceByName(labelResourceName));
            label.SetAlignment(0.0F, 0.0F);
            return label;
        }

        private VBox CreateComponent()
        {
            var box = new VBox(false, 2);
            box.PackStart(Label, false, false, 0);
            box.PackStart(Entry, false, false, 0);

            return box;
        }

    }
}
