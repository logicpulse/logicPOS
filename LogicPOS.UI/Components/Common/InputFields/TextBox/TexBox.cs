using Gtk;
using logicpos;
using logicpos.Classes.Enums.Keyboard;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Extensions;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.InputFields
{
    public partial class TextBox : IValidatableField
    {
        public Window SourceWindow { get; private set; }
        public Entry Entry { get; private set; } = new Entry();
        public string Text { get => Entry.Text; set => Entry.Text = value; }
        public Label Label { get; private set; }
        public bool IsRequired { get; set; }
        private bool IsEmpty => string.IsNullOrWhiteSpace(Entry.Text);
        public bool IsValidatable { get; set; }
        public string Regex { get; set; }
        public Widget Component { get; private set; }
        public string FieldName => Label.Text;
        public event EventHandler SelectEntityClicked;
        public object SelectedEntity { get; set; }
        public Predicate<string> IsValidFunction { get; set; }
        private readonly TextBoxStyle _style;

        public TextBox(Window sourceWindow,
                       string labelText,
                       bool isRequired = false,
                       bool isValidatable = false,
                       string regex = null,
                       bool includeSelectButton = true,
                       bool includeKeyBoardButton = false,
                       bool includeClearButton = true,
                       TextBoxStyle style = TextBoxStyle.Bold)
        {
            if (isValidatable && string.IsNullOrEmpty(regex))
            {
                throw new ArgumentException("Regex must be provided when the field is validatable");
            }

            _style = style;
            SourceWindow = sourceWindow;
            Label = CreateLabel(labelText);
            IsRequired = isRequired;
            IsValidatable = isValidatable;
            Regex = regex;
            Component = CreateComponent(includeSelectButton,
                                        includeKeyBoardButton,
                                        includeClearButton);
            AddEventHandlers();
            UpdateValidationColors();
            ApplyStyle();
        }

        private void ApplyStyle()
        {
            switch (_style)
            {
                case TextBoxStyle.Bold:
                    Entry.ModifyFont(Pango.FontDescription.FromString(AppSettings.Instance.fontEntryBoxValue));
                    Label.ModifyFont(Pango.FontDescription.FromString(AppSettings.Instance.fontEntryBoxLabel));
                    break;
                default:
                    break;
            }
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
                    var isValid = System.Text.RegularExpressions.Regex.IsMatch(Entry.Text, Regex);
                    if (!isValid)
                    {
                        return false;
                    }

                    if (IsValidFunction != null)
                    {
                        return IsValidFunction(Entry.Text);
                    }
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

        public void UpdateValidationColors()
        {
            ValidationColors.Default.UpdateComponentFontColor(Label, IsValid());
            ValidationColors.Default.UpdateComponent(Entry, IsValid());
        }

        private Label CreateLabel(string labelText)
        {
            var label = new Label(labelText);
            label.SetAlignment(0.0F, 0.0F);
            return label;
        }

        private Widget CreateComponent(bool includeSelectButton,
                                         bool includeKeyBoardButton,
                                         bool includeClearButton)
        {
            var verticalLayout = new VBox(false, 2);
            verticalLayout.PackStart(Label, false, false, 0);
            verticalLayout.BorderWidth = 2;

            var hbox = new HBox(false, 2);
            hbox.PackStart(Entry, true, true, 0);

            if (includeSelectButton)
            {
                hbox.PackStart(CreateSelecEntityButton(), false, false, 0);
            }

            if (includeKeyBoardButton)
            {
                hbox.PackStart(CreateKeyBoardButton(), false, false, 0);
            }

            if (includeClearButton)
            {
                hbox.PackStart(CreateClearButton(), false, false, 0);
            }

            verticalLayout.PackStart(hbox, false, false, 0);

            if(_style == TextBoxStyle.Simple)
            {
                return verticalLayout;
            }

            var eventBox = new EventBox();
            eventBox.Add(verticalLayout);
            eventBox.BorderWidth = 2;

            if (_style == TextBoxStyle.Bold)
            {
                eventBox.ModifyBg(StateType.Normal, AppSettings.Instance.colorBaseDialogEntryBoxBackground.ToGdkColor());
            } 

            return eventBox;
        }


        private IconButton CreateKeyBoardButton()
        {
            var button = new IconButton(
               new ButtonSettings
               {
                   Name = "buttonUserId",
                   Icon = $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_keyboard.png"}",
                   IconSize = new Size(20, 20),
                   ButtonSize = new Size(30, 30)
               });

            button.Clicked += (sender, args) => CallKeyboard();

            return button;
        }

        private IconButton CreateClearButton()
        {
            var button = new IconButton(
                new ButtonSettings
                {
                    Name = "buttonUserId",
                    Icon = $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_delete_record.png"}",
                    IconSize = new Size(20, 20),
                    ButtonSize = new Size(30, 30)
                });

            button.Clicked += (sender, args) => Clear();

            return button;
        }

        private IconButton CreateSelecEntityButton()
        {
            var button = new IconButton(
                new ButtonSettings
                {
                    Name = "buttonUserId",
                    Icon = $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_select_record.png"}",
                    IconSize = new Size(20, 20),
                    ButtonSize = new Size(30, 30)
                });

            button.Clicked += (sender, args) => SelectEntityClicked?.Invoke(this, EventArgs.Empty);
            return button;
        }

        private void CallKeyboard()
        {
            KeyboardMode mode = KeyboardMode.AlfaNumeric;
            string rule = Regex;

            string input = Utils.GetVirtualKeyBoardInput(this.SourceWindow,
                                                         mode,
                                                         Entry.Text,
                                                         rule);

            if (input == null)
            {
                return;
            }

            Entry.Text = input;
            Entry.GrabFocus();

        }

        public void Clear()
        {
            Entry.Text = string.Empty;
            SelectedEntity = null;
        }

        public void Require(bool require = true, bool sensitive = true)
        {
            Clear();
            IsRequired = require;
            UpdateValidationColors();
            Component.Sensitive = require ? true : sensitive;
        }

        public TextBox WithText(string text)
        {
            Text = text;
            return this;
        }
    }
}
