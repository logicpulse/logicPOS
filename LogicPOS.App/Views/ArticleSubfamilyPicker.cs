using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using LogicPOS.Core.BackOffice;
using TextSearch = LogicPOS.Core.TextSearch;

namespace LogicPOS.App.Views;

internal sealed class ArticleSubfamilyPicker : Button
{
    protected override Type StyleKeyOverride => typeof(Button);

    public LookupOption? Selected { get; private set; }

    public ArticleSubfamilyPicker(
        IReadOnlyList<LookupOption> options,
        string value,
        bool readOnly,
        Action<LookupOption?>? changed = null,
        bool showFamilyIcon = false)
    {
        Classes.Add("bo_subfamily_picker");
        if (showFamilyIcon)
        {
            Classes.Add("bo_new_doc_tool");
            HorizontalAlignment = HorizontalAlignment.Left;
            MinWidth = 160;
        }

        IsEnabled = readOnly == false;
        HorizontalContentAlignment = HorizontalAlignment.Stretch;

        var label = new TextBlock
        {
            FontSize = 13,
            TextTrimming = TextTrimming.CharacterEllipsis,
            VerticalAlignment = VerticalAlignment.Center
        };
        var chevron = new PathIcon
        {
            Width = 12,
            Height = 12,
            Margin = new Thickness(8, 0, 0, 0),
            Data = StreamGeometry.Parse("M1939 486L2029 576L1024 1581L19 576L109 486L1024 1401L1939 486Z"),
            Foreground = new SolidColorBrush(Color.Parse("#666666")),
            IsHitTestVisible = false,
            VerticalAlignment = VerticalAlignment.Center
        };
        DockPanel.SetDock(chevron, Dock.Right);
        var content = new DockPanel { Children = { chevron, label } };
        if (showFamilyIcon)
        {
            var icon = new Avalonia.Svg.Skia.Svg(new Uri("avares://LogicPOS.App/"))
            {
                Classes = { "bo_line_icon_image" },
                Path = "avares://LogicPOS.App/Assets/Images/BackOffice/Menu/menu_artigos_familas_b.svg",
                Width = 16,
                Height = 16,
                Margin = new Thickness(0, 0, 6, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            DockPanel.SetDock(icon, Dock.Left);
            content.Children.Insert(0, icon);
        }

        Content = content;

        var search = new TextBox { PlaceholderText = "Procurar família ou subfamília" };
        var results = new StackPanel { Spacing = 2 };
        var scroll = new ScrollViewer
        {
            Content = results,
            MaxHeight = 300,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
        };
        var panel = new StackPanel { Spacing = 6, Children = { search, scroll } };
        var dropdown = new Flyout { Content = panel, Placement = PlacementMode.BottomEdgeAlignedLeft };
        Flyout = dropdown;

        void Select(LookupOption? option)
        {
            Selected = option;
            label.Text = option is null ? "Família / Sub-família"
                : changed is not null && !string.IsNullOrWhiteSpace(option.FamilyLabel)
                    ? $"{option.FamilyLabel} / {option.Label}" : option.Label;
            ToolTip.SetTip(this, label.Text);
        }

        Select(Guid.TryParse(value, out var id) ? options.FirstOrDefault(option => option.Id == id) : null);
        var families = options.GroupBy(option => option.FamilyId)
            .OrderBy(group => FamilyName(group.First()), StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        void Filter()
        {
            var term = search.Text?.Trim() ?? string.Empty;
            results.Children.Clear();
            scroll.Offset = default;
            if (changed is not null && term.Length == 0)
            {
                var all = new Button { Content = "Todas as famílias" };
                all.Click += (_, _) => { Select(null); dropdown.Hide(); changed(null); };
                results.Children.Add(all);
            }
            foreach (var family in families)
            {
                var familyName = FamilyName(family.First());
                var familyMatches = term.Length == 0 || TextSearch.Contains(familyName, term);
                var matches = family.Where(option => familyMatches || TextSearch.Contains(option.Label, term)).ToList();
                if (matches.Count == 0)
                {
                    continue;
                }

                results.Children.Add(new TextBlock
                {
                    Text = familyName,
                    FontWeight = FontWeight.SemiBold,
                    Margin = new Thickness(8, 6, 8, 2),
                    TextWrapping = TextWrapping.Wrap
                });
                var list = new ListBox
                {
                    ItemsSource = matches,
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Padding = new Thickness(0)
                };
                list.SelectionChanged += (_, _) =>
                {
                    if (list.SelectedItem is LookupOption option)
                    {
                        Select(option);
                        changed?.Invoke(option);
                        dropdown.Hide();
                        Focus();
                    }
                };
                results.Children.Add(list);
            }

            if (results.Children.Count == 0)
            {
                results.Children.Add(new TextBlock { Text = "Nenhuma subfamília encontrada" });
            }
        }

        search.TextChanged += (_, _) => Filter();
        dropdown.Opening += (_, _) =>
        {
            panel.Width = Math.Max(220, Bounds.Width);
            search.Clear();
            Filter();
        };
        dropdown.Opened += (_, _) => search.Focus();
    }

    private static string FamilyName(LookupOption option)
        => string.IsNullOrWhiteSpace(option.FamilyLabel) ? "Sem família" : option.FamilyLabel;
}
