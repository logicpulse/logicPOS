namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Default filter ranges: the whole current month for reports, the whole current year for lists that open on the year.
/// </summary>
public static class DefaultPeriods
{
    public static (DateTime Start, DateTime End) Month(DateTime day)
    {
        var start = new DateTime(day.Year, day.Month, 1);
        var last = DateTime.DaysInMonth(day.Year, day.Month);
        return (start, new DateTime(day.Year, day.Month, last, 23, 59, 59));
    }

    public static (DateTime Start, DateTime End) Year(DateTime day)
        => (new DateTime(day.Year, 1, 1), new DateTime(day.Year, 12, 31, 23, 59, 59));
}
