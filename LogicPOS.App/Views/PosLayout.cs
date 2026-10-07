namespace LogicPOS.App.Views;

/// <summary>
/// Pixel layout of the retail POS window, using the same formulas as the GTK theme evaluator.
/// </summary>
public sealed class PosLayout
{
    public const int Margin = 10;
    public const int StatusBarHeight = 40;
    public const int TicketColumns = 4;
    public const int TicketRows = 4;

    private PosLayout(int screenWidth, int screenHeight)
    {
        ScreenWidth = screenWidth;
        ScreenHeight = screenHeight;
        var bucket = BucketFor(screenWidth, screenHeight);
        TicketFontSize = bucket.TicketFont;
        ToolbarIconSize = bucket.Icon;
        TicketIconSize = bucket.Icon;
        TicketButtonWidth = bucket.ToolbarWidth;
        TicketButtonHeight = bucket.ToolbarHeight;
        ToolbarButtonWidth = bucket.ToolbarWidth;
        ToolbarButtonHeight = bucket.ToolbarHeight;

        const int ticketGap = 16;
        const int arrowGap = 12;
        const int arrowRowGap = 8;
        var ticketColumn = TicketButtonWidth * TicketColumns;
        var columnRight = screenWidth - ticketColumn - (Margin * 2);
        var menuRight = columnRight - ticketGap;
        var usefulWidth = menuRight - Margin;
        var usefulHeight = screenHeight - (StatusBarHeight * 2) - Margin - StatusBarHeight - (ToolbarButtonHeight + (Margin * 2));
        var guessedColumns = Math.Max(1, usefulWidth / bucket.BaseWidth);
        var guessedRows = Math.Max(1, usefulHeight / bucket.BaseHeight);
        ButtonWidth = bucket.BaseWidth + ((usefulWidth - (bucket.BaseWidth * guessedColumns)) / guessedColumns);
        ButtonHeight = bucket.BaseHeight + ((usefulHeight - (bucket.BaseHeight * guessedRows)) / guessedRows);
        var gridBottom = (StatusBarHeight * 2) + Margin + (ButtonHeight * guessedRows);
        var toolbarTop = screenHeight - ToolbarButtonHeight - (Margin * 2);
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
        ScrollWidth = arrowSpan / 2;
        SubfamilyPreviousX = arrowColumnX + arrowGap;
        SubfamilyPreviousY = FamilyPreviousY;
        SubfamilyNextX = SubfamilyPreviousX + ScrollWidth;
        SubfamilyNextY = FamilyPreviousY;
        ArticlePreviousX = SubfamilyPreviousX;
        ArticlePreviousY = FamilyNextY;
        ArticleNextX = SubfamilyNextX;
        ArticleNextY = FamilyNextY;

        ToolbarX = 0;
        ToolbarY = screenHeight - ToolbarButtonHeight - (Margin * 2);
        ToolbarWidth = screenWidth - ticketColumn - (Margin * 2);
        ToolbarHeight = ToolbarButtonHeight + (Margin * 2);

        TicketListX = columnRight;
        TicketListY = LogoY + LogoHeight + Margin;
        TicketListWidth = ticketColumn + (Margin * 2);
        TicketListHeight = screenHeight - ((TicketButtonHeight * TicketRows) + Margin) - TicketListY;
        DesignationColumnWidth = ticketColumn - 10 - 65 - 55 - 75;
        PriceColumnWidth = 65;
        QuantityColumnWidth = 55;
        TotalColumnWidth = 75;

        TicketPadX = columnRight;
        TicketPadY = screenHeight - (TicketButtonHeight * TicketRows) - Margin;
        TicketPadWidth = (TicketButtonWidth * TicketColumns) + (Margin * 2);
        TicketPadHeight = (TicketButtonHeight * TicketRows) + Margin;
    }

    public int ScreenWidth { get; }
    public int ScreenHeight { get; }
    public int ButtonWidth { get; }
    public int ButtonHeight { get; }
    public int FamilyRows { get; }
    public int SubfamilyColumns { get; }
    public int ArticleColumns { get; }
    public int ArticleRows { get; }
    public int TicketFontSize { get; }
    public int ToolbarIconSize { get; }
    public int TicketIconSize { get; }
    public int TicketButtonWidth { get; }
    public int TicketButtonHeight { get; }
    public int ToolbarButtonWidth { get; }
    public int ToolbarButtonHeight { get; }
    public int LogoX { get; }
    public int LogoY { get; }
    public int LogoWidth { get; }
    public int LogoHeight { get; }
    public int StatusBar1X { get; }
    public int StatusBar1Y { get; }
    public int StatusBar1Width { get; }
    public int StatusBar1Height { get; }
    public int StatusBar2X { get; }
    public int StatusBar2Y { get; }
    public int StatusBar2Width { get; }
    public int StatusBar2Height { get; }
    public int FavoritesX { get; }
    public int FavoritesY { get; }
    public int FamilyX { get; }
    public int FamilyY { get; }
    public int SubfamilyX { get; }
    public int SubfamilyY { get; }
    public int ArticleX { get; }
    public int ArticleY { get; }
    public int FamilyPreviousX { get; }
    public int FamilyPreviousY { get; }
    public int FamilyNextX { get; }
    public int FamilyNextY { get; }
    public int ScrollWidth { get; }
    public int SubfamilyPreviousX { get; }
    public int SubfamilyPreviousY { get; }
    public int SubfamilyNextX { get; }
    public int SubfamilyNextY { get; }
    public int ArticlePreviousX { get; }
    public int ArticlePreviousY { get; }
    public int ArticleNextX { get; }
    public int ArticleNextY { get; }
    public int ToolbarX { get; }
    public int ToolbarY { get; }
    public int ToolbarWidth { get; }
    public int ToolbarHeight { get; }
    public int TicketListX { get; }
    public int TicketListY { get; }
    public int TicketListWidth { get; }
    public int TicketListHeight { get; }
    public int DesignationColumnWidth { get; }
    public int PriceColumnWidth { get; }
    public int QuantityColumnWidth { get; }
    public int TotalColumnWidth { get; }
    public int TicketPadX { get; }
    public int TicketPadY { get; }
    public int TicketPadWidth { get; }
    public int TicketPadHeight { get; }

    public static PosLayout For(int screenWidth, int screenHeight) => new(screenWidth, screenHeight);

    private static (int BaseWidth, int BaseHeight, int ToolbarWidth, int ToolbarHeight, int Icon, int TicketFont) BucketFor(int width, int height)
    {
        if (width == 800 && height == 600)
        {
            return (100, 75, 54, 38, 22, 8);
        }

        if ((width == 1024 && height == 600) || (width == 1024 && height == 768))
        {
            return (120, 90, 80, 60, 34, 9);
        }

        if (width >= 1680)
        {
            return (160, 120, 120, 90, 50, 10);
        }

        if (width >= 1152)
        {
            return (140, 105, 100, 75, 42, 10);
        }

        return (120, 90, 80, 60, 34, 9);
    }
}
