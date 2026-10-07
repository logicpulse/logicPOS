namespace LogicPOS.Persistence.Interceptors;

public interface IAuditingInformationService
{
    Guid? GetUserId();
    Guid? GetTerminalId();
}