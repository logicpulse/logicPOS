using System.Globalization;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Services.DatabaseBackup;

/// <summary>
/// LogicPOS 1.4 automatic backup preferences: a backup is due when the time of day is inside the range
/// and the last backup is at least one interval old.
/// </summary>
public sealed record AutomaticBackupSettings(TimeSpan Interval, TimeSpan RangeStart, TimeSpan RangeEnd)
{
    private const string EnabledToken = "DATABASE_BACKUP_AUTOMATIC_ENABLED";
    private const string IntervalToken = "DATABASE_BACKUP_TIMESPAN";
    private const string RangeStartToken = "DATABASE_BACKUP_TIME_SPAN_RANGE_START";
    private const string RangeEndToken = "DATABASE_BACKUP_TIME_SPAN_RANGE_END";
    private static readonly string[] Tokens = [EnabledToken, IntervalToken, RangeStartToken, RangeEndToken];

    public bool IsDue(DateTime now, DateTime lastBackup) =>
        now.TimeOfDay > RangeStart && now.TimeOfDay < RangeEnd && now - lastBackup >= Interval;

    /// <returns>null when automatic backups are disabled or the preferences are invalid.</returns>
    public static AutomaticBackupSettings? Load(LogicPOSDbContext database, ILogger logger)
    {
        var values = database.PreferenceParameters.AsNoTracking()
            .Where(parameter => Tokens.Contains(parameter.Token))
            .Select(parameter => new { parameter.Token, parameter.Value })
            .AsEnumerable()
            .GroupBy(parameter => parameter.Token)
            .ToDictionary(group => group.Key, group => group.First().Value);

        if (bool.TryParse(values.GetValueOrDefault(EnabledToken), out var enabled) == false || enabled == false)
        {
            return null;
        }

        if (TryParseTime(values, IntervalToken, out var interval) &&
            TryParseTime(values, RangeStartToken, out var rangeStart) &&
            TryParseTime(values, RangeEndToken, out var rangeEnd))
        {
            return new AutomaticBackupSettings(interval, rangeStart, rangeEnd);
        }

        logger.LogWarning("Automatic backup is enabled but its preferences are invalid: {Preferences}", values);
        return null;
    }

    private static bool TryParseTime(Dictionary<string, string?> values, string token, out TimeSpan value) =>
        TimeSpan.TryParse(values.GetValueOrDefault(token), CultureInfo.InvariantCulture, out value);
}
