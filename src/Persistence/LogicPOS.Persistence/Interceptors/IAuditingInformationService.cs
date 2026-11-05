namespace LogicPOS.Persistence.Interceptors;

public interface IAuditingInformationService
{
    Guid? GetUserId();

    Guid? GetTerminalId();

    void SetUser(Guid? userId);

    void SetTerminal(Guid? terminalId);
}