namespace LogicPOS.App.Views;

/// <summary>
/// Pixel layout of the retail POS window. Adapts to common landscape resolutions,
/// unusual aspect ratios, and portrait (height &gt; width) by rearranging the ticket column.
/// </summary>
public sealed class PosLayout
{
    public const int Margin = 10;
    public const int StatusBarHeight = 40;
    public const int TicketColumns = 4;
    public const int TicketRows = 4;

    private PosLayout(int screenWidth, int screenHeight)
    {
        ScreenWidth = Math.Max(320, screenWidth);
        ScreenHeight = Math.Max(320, screenHeight);
        IsPortrait = ScreenHeight > ScreenWidth;
        var bucket = BucketFor(ScreenWidth, ScreenHeight);
        TicketFontSize = bucket.TicketFont;
        ToolbarIconSize = bucket.Icon;
        TicketIconSize = bucket.Icon;
        TicketButtonWidth = bucket.ToolbarWidth;
        TicketButtonHeight = bucket.ToolbarHeight;
        ToolbarButtonWidth = bucket.ToolbarWidth;
        ToolbarButtonHeight = bucket.ToolbarHeight;

        if (IsPortrait)
        {
            BuildPortrait(bucket);
        }
        else
        {
            BuildLandscape(bucket);
        }
    }

    public int ScreenWidth { get; }
    public int ScreenHeight { get; }
    public bool IsPortrait { get; }
    public int ButtonWidth { get; private set; }
    public int ButtonHeight { get; private set; }
    public int FamilyRows { get; private set; }
    public int SubfamilyColumns { get; private set; }
    public int ArticleColumns { get; private set; }
    public int ArticleRows { get; private set; }
    public int TicketFontSize { get; }
    public int ToolbarIconSize { get; }
    public int TicketIconSize { get; }
    public int TicketButtonWidth { get; }
    public int TicketButtonHeight { get; }
    public int ToolbarButtonWidth { get; }
    public int ToolbarButtonHeight { get; }
    public int LogoX { get; private set; }
    public int LogoY { get; private set; }
    public int LogoWidth { get; private set; }
    public int LogoHeight { get; private set; }
    public int StatusBar1X { get; private set; }
    public int StatusBar1Y { get; private set; }
    public int StatusBar1Width { get; private set; }
    public int StatusBar1Height { get; private set; }
    public int StatusBar2X { get; private set; }
    public int StatusBar2Y { get; private set; }
    public int StatusBar2Width { get; private set; }
    public int StatusBar2Height { get; private set; }
    public int FavoritesX { get; private set; }
    public int FavoritesY { get; private set; }
    public int FamilyX { get; private set; }
    public int FamilyY { get; private set; }
    public int SubfamilyX { get; private set; }
    public int SubfamilyY { get; private set; }
    public int ArticleX { get; private set; }
    public int ArticleY { get; private set; }
    public int FamilyPreviousX { get; private set; }
    public int FamilyPreviousY { get; private set; }
    public int FamilyNextX { get; private set; }
    public int FamilyNextY { get; private set; }
    public int ScrollWidth { get; private set; }
    public int SubfamilyPreviousX { get; private set; }
    public int SubfamilyPreviousY { get; private set; }
    public int SubfamilyNextX { get; private set; }
    public int SubfamilyNextY { get; private set; }
    public int ArticlePreviousX { get; private set; }
    public int ArticlePreviousY { get; private set; }
    public int ArticleNextX { get; private set; }
    public int ArticleNextY { get; private set; }
    public int ToolbarX { get; private set; }
    public int ToolbarY { get; private set; }
    public int ToolbarWidth { get; private set; }
    public int ToolbarHeight { get; private set; }
    public int TicketListX { get; private set; }
    public int TicketListY { get; private set; }
    public int TicketListWidth { get; private set; }
    public int TicketListHeight { get; private set; }
    public int DesignationColumnWidth { get; private set; }
    public int PriceColumnWidth { get; private set; }
    public int QuantityColumnWidth { get; private set; }
    public int TotalColumnWidth { get; private set; }
    public int TicketPadX { get; private set; }
    public int TicketPadY { get; private set; }
    public int TicketPadWidth { get; private set; }
    public int TicketPadHeight { get; private set; }

    public static PosLayout For(int screenWidth, int screenHeight) => new(screenWidth, screenHeight);

    private void BuildLandscape((int BaseWidth, int BaseHeight, int ToolbarWidth, int ToolbarHeight, int Icon, int TicketFont) bucket)
    {
        const int ticketGap = 16;
        const int arrowGap = 12;
        const int arrowRowGap = 8;
        var ticketColumn = TicketButtonWidth * TicketColumns;
        var columnRight = ScreenWidth - ticketColumn - (Margin * 2);
        var menuRight = columnRight - ticketGap;
        var usefulWidth = Math.Max(bucket.BaseWidth, menuRight - Margin);
        var usefulHeight = Math.Max(
            bucket.BaseHeight,
            ScreenHeight - (StatusBarHeight * 2) - Margin - StatusBarHeight - (ToolbarButtonHeight + (Margin * 2)));
        var guessedColumns = Math.Max(1, usefulWidth / bucket.BaseWidth);
        var guessedRows = Math.Max(1, usefulHeight / bucket.BaseHeight);
        ButtonWidth = bucket.BaseWidth + ((usefulWidth - (bucket.BaseWidth * guessedColumns)) / guessedColumns);
        ButtonHeight = bucket.BaseHeight + ((usefulHeight - (bucket.BaseHeight * guessedRows)) / guessedRows);
        var gridBottom = (StatusBarHeight * 2) + Margin + (ButtonHeight * guessedRows);
        var toolbarTop = ScreenHeight - ToolbarButtonHeight - (Margin * 2);
        var overflow = gridBottom + arrowRowGap + StatusBarHeight - toolbarTop;
        if (overflow > 0 && guessedRows > 0)
        {
            ButtonHeight -= (overflow + guessedRows - 1) / guessedRows;
            gridBottom = (StatusBarHeight * 2) + Margin + (ButtonHeight * guessedRows);
        }

        FamilyRows = Math.Max(1, guessedRows - 1);
        SubfamilyColumns = Math.Max(1, guessedColumns - 1);
        ArticleColumns = SubfamilyColumns;
        ArticleRows = FamilyRows;

        var logoSpan = ticketColumn + (Margin * 2);
        LogoWidth = (int)Math.Round(logoSpan * 0.78);
        LogoHeight = (int)Math.Round(LogoWidth * (350.0 / 1024.0));
        LogoX = columnRight + ((logoSpan - LogoWidth) / 2);
        LogoY = 12;

        StatusBar1X = Margin;
        StatusBar1Y = 8;
        StatusBar1Width = menuRight - StatusBar1X;
        StatusBar1Height = StatusBarHeight;

        StatusBar2X = ButtonWidth + Margin;
        StatusBar2Y = StatusBarHeight + Margin;
        StatusBar2Width = Math.Max(0, guessedColumns - 2) * ButtonWidth;
        StatusBar2Height = StatusBarHeight;

        FavoritesX = Margin;
        FavoritesY = (StatusBarHeight * 2) + Margin;
        FamilyX = Margin;
        FamilyY = (StatusBarHeight * 2) + ButtonHeight + Margin;
        SubfamilyX = Margin + ButtonWidth;
        SubfamilyY = FavoritesY;
        ArticleX = SubfamilyX;
        ArticleY = FamilyY;

        FamilyPreviousX = Margin;
        FamilyPreviousY = StatusBarHeight + Margin;
        FamilyNextX = Margin;
        FamilyNextY = gridBottom + arrowRowGap;
        var arrowColumnX = Margin + (ButtonWidth * (guessedColumns - 1));
        var arrowSpan = Math.Max(ButtonWidth - (arrowGap * 2), 40);
        ScrollWidth = Math.Max(20, arrowSpan / 2);
        SubfamilyPreviousX = arrowColumnX + arrowGap;
        SubfamilyPreviousY = FamilyPreviousY;
        SubfamilyNextX = SubfamilyPreviousX + ScrollWidth;
        SubfamilyNextY = FamilyPreviousY;
        ArticlePreviousX = SubfamilyPreviousX;
        ArticlePreviousY = FamilyNextY;
        ArticleNextX = SubfamilyNextX;
        ArticleNextY = FamilyNextY;

        ToolbarX = 0;
        ToolbarY = ScreenHeight - ToolbarButtonHeight - (Margin * 2);
        ToolbarWidth = ScreenWidth - ticketColumn - (Margin * 2);
        ToolbarHeight = ToolbarButtonHeight + (Margin * 2);

        TicketListX = columnRight;
        TicketListY = LogoY + LogoHeight + Margin;
        TicketListWidth = ticketColumn + (Margin * 2);
        TicketListHeight = ScreenHeight - ((TicketButtonHeight * TicketRows) + Margin) - TicketListY;
        AssignTicketColumns(ticketColumn);

        TicketPadX = columnRight;
        TicketPadY = ScreenHeight - (TicketButtonHeight * TicketRows) - Margin;
        TicketPadWidth = (TicketButtonWidth * TicketColumns) + (Margin * 2);
        TicketPadHeight = (TicketButtonHeight * TicketRows) + Margin;
    }

    private void BuildPortrait((int BaseWidth, int BaseHeight, int ToolbarWidth, int ToolbarHeight, int Icon, int TicketFont) bucket)
    {
        const int arrowGap = 8;
        const int arrowRowGap = 6;
        var ticketPadHeight = (TicketButtonHeight * TicketRows) + Margin;
        var ticketListHeight = Math.Max(120, ScreenHeight / 4);
        var toolbarHeight = ToolbarButtonHeight + (Margin * 2);
        var bottomStack = ticketPadHeight + ticketListHeight + toolbarHeight;
        var menuBottom = ScreenHeight - bottomStack;
        LogoWidth = Math.Min(ScreenWidth - (Margin * 2), (int)Math.Round(ScreenWidth * 0.55));
        LogoHeight = (int)Math.Round(LogoWidth * (350.0 / 1024.0));
        LogoX = (ScreenWidth - LogoWidth) / 2;
        LogoY = 8;

        StatusBar1X = Margin;
        StatusBar1Y = LogoY + LogoHeight + 4;
        StatusBar1Width = ScreenWidth - (Margin * 2);
        StatusBar1Height = StatusBarHeight;

        StatusBar2Y = StatusBar1Y + StatusBarHeight + 4;
        StatusBar2Height = StatusBarHeight;

        var menuTop = StatusBar2Y + StatusBarHeight + Margin;
        var usefulWidth = Math.Max(bucket.BaseWidth, ScreenWidth - (Margin * 2));
        var usefulHeight = Math.Max(bucket.BaseHeight, menuBottom - menuTop - StatusBarHeight);
        var guessedColumns = Math.Max(1, usefulWidth / bucket.BaseWidth);
        var guessedRows = Math.Max(1, usefulHeight / bucket.BaseHeight);
        ButtonWidth = bucket.BaseWidth + ((usefulWidth - (bucket.BaseWidth * guessedColumns)) / guessedColumns);
        ButtonHeight = bucket.BaseHeight + ((usefulHeight - (bucket.BaseHeight * guessedRows)) / guessedRows);
        var gridBottom = menuTop + (ButtonHeight * guessedRows);
        var overflow = gridBottom + arrowRowGap + StatusBarHeight - menuBottom;
        if (overflow > 0 && guessedRows > 0)
        {
            ButtonHeight -= (overflow + guessedRows - 1) / guessedRows;
            gridBottom = menuTop + (ButtonHeight * guessedRows);
        }

        StatusBar2X = ButtonWidth + Margin;
        StatusBar2Width = Math.Max(0, guessedColumns - 2) * ButtonWidth;

        FamilyRows = Math.Max(1, guessedRows - 1);
        SubfamilyColumns = Math.Max(1, guessedColumns - 1);
        ArticleColumns = SubfamilyColumns;
        ArticleRows = FamilyRows;

        FavoritesX = Margin;
        FavoritesY = menuTop;
        FamilyX = Margin;
        FamilyY = menuTop + ButtonHeight;
        SubfamilyX = Margin + ButtonWidth;
        SubfamilyY = FavoritesY;
        ArticleX = SubfamilyX;
        ArticleY = FamilyY;

        FamilyPreviousX = Margin;
        FamilyPreviousY = StatusBar2Y;
        FamilyNextX = Margin;
        FamilyNextY = gridBottom + arrowRowGap;
        var arrowColumnX = Margin + (ButtonWidth * (guessedColumns - 1));
        var arrowSpan = Math.Max(ButtonWidth - (arrowGap * 2), 36);
        ScrollWidth = Math.Max(18, arrowSpan / 2);
        SubfamilyPreviousX = arrowColumnX + arrowGap;
        SubfamilyPreviousY = FamilyPreviousY;
        SubfamilyNextX = SubfamilyPreviousX + ScrollWidth;
        SubfamilyNextY = FamilyPreviousY;
        ArticlePreviousX = SubfamilyPreviousX;
        ArticlePreviousY = FamilyNextY;
        ArticleNextX = SubfamilyNextX;
        ArticleNextY = FamilyNextY;

        ToolbarX = 0;
        ToolbarY = ScreenHeight - ticketPadHeight - ticketListHeight - toolbarHeight;
        ToolbarWidth = ScreenWidth;
        ToolbarHeight = toolbarHeight;

        TicketListX = 0;
        TicketListY = ToolbarY + ToolbarHeight;
        TicketListWidth = ScreenWidth;
        TicketListHeight = ticketListHeight;
        AssignTicketColumns(ScreenWidth - (Margin * 2));

        TicketPadX = 0;
        TicketPadY = ScreenHeight - ticketPadHeight;
        TicketPadWidth = ScreenWidth;
        TicketPadHeight = ticketPadHeight;
    }

    private void AssignTicketColumns(int ticketColumn)
    {
        PriceColumnWidth = Math.Max(48, Math.Min(72, ticketColumn / 8));
        QuantityColumnWidth = Math.Max(40, Math.Min(60, ticketColumn / 10));
        TotalColumnWidth = Math.Max(56, Math.Min(80, ticketColumn / 7));
        DesignationColumnWidth = Math.Max(
            40,
            ticketColumn - 10 - PriceColumnWidth - QuantityColumnWidth - TotalColumnWidth);
    }

    /// <summary>
    /// Size buckets by short/long side ranges (covers 4:3, 16:9, 16:10, ultrawide, portrait).
    /// </summary>
    private static (int BaseWidth, int BaseHeight, int ToolbarWidth, int ToolbarHeight, int Icon, int TicketFont) BucketFor(
        int width,
        int height)
    {
        var shortSide = Math.Min(width, height);
        var longSide = Math.Max(width, height);

        // ~800x600 / small tablets
        if (shortSide <= 600)
        {
            return (100, 75, 54, 38, 22, 8);
        }

        // ~1024x600, 1024x768, 1280x720, 1280x800
        if (shortSide <= 800 && longSide <= 1366)
        {
            return (120, 90, 80, 60, 34, 9);
        }

        // ~1366x768, 1440x900, 1600x900
        if (longSide < 1680)
        {
            return (140, 105, 100, 75, 42, 10);
        }

        // 1680+, 1920x1080, 2560+, portrait 1080x1920 (longSide 1920)
        return (160, 120, 120, 90, 50, 10);
    }
}
