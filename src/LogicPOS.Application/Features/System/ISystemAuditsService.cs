namespace LogicPOS.Application.Features.System
{
    public interface ISystemAuditsService
    {
        Task UserLogoutAsync(string suffix="");
        Task FinanceDocumentCreatedAsync(string suffix="");
        Task TableUnreservedAsync(string suffix="");
        Task DatabaseBackupAsync(string suffix="");
        Task DatabaseUpdateAsync(string suffix="");
        Task DatabaseCreateAsync(string suffix="");
        Task TableReservedAsync(string suffix="");
        Task AppCloseAsync(string suffix="");
        Task TableCloseAsync(string suffix="");
        Task SessionDayCloseAsync(string suffix="");
        Task StockMovementInAsync(string suffix="", Guid userId = new Guid(), Guid terminalId = new Guid());
        Task StockMovementOutAsync(string suffix="", Guid userId = new Guid(), Guid terminalId = new Guid());
        Task SystemPrintFinanceDocumentAsync(string suffix="");
        Task DatabaseRestoreAsync(string suffix="");
        Task TableOpenAsync(string suffix="");
        Task CashDrawerOpenAsync(string suffix="");
        Task CashDrawerInAsync(decimal value);
        Task UserLoginErrorAsync(string suffix="", Guid userId = new Guid(), Guid terminalId = new Guid());
        Task FinanceDocumentCancelledAsync(string suffix="");
        Task SessionTerminalOpenAsync(string suffix="");
        Task SessionTerminalCloseAsync(string suffix="");
        Task OrderArticleRemovedAsync(string suffix="");
        Task ExportSafTAsync(string suffix="");
        Task FinanceSeriesCreatedAsync(string suffix="");
        Task SessionDayOpenAsync(string suffix="");
        Task CashDrawerOutAsync(decimal value);
        Task UserLoginAsync(string suffix="", Guid userId = new Guid(), Guid terminalId = new Guid());
        Task UserChangeAsync(string suffix="");
        Task UserChangePasswordAsync(string suffix="");
        Task AppStartAsync(string suffix="");
        Task TerminalAddAsync(string suffix="");
    }
}
