namespace LogicPOS.Core.BackOffice;

/// <summary>A message that may be shown on the BackOffice save dialog. Other exceptions are logged.</summary>
public class BusinessException : Exception
{
    public BusinessException(string message)
        : base(message)
    {
    }

    public BusinessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
