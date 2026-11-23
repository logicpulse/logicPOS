using LogicPOS.Domain.Enums;

namespace LogicPOS.Core.BackOffice;

/// <summary>Sessões de Trabalho lists open terminal sessions only.</summary>
public static class WorkSessionListing
{
    public static bool Include(WorkSessionPeriodType type, WorkSessionPeriodStatus status)
        => type == WorkSessionPeriodType.Terminal && status == WorkSessionPeriodStatus.Open;
}
