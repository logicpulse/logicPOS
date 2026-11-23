using System;
using System.Collections.Generic;

namespace LogicPOS.Persistence.DataFixes;

/// <summary>
/// Conditional SQL for FixSeedTexts2035R3 (SQLite / MySQL / SQL Server / PostgreSQL).
/// Aligns the SDRVDEP subfamily and reviewer designation tweaks after FixSeedTexts2035 / R2.
/// UPDATEs match seed Id (or the SubfamilyId of SDRVDEP) and the exact previous value (binary).
/// </summary>
public static class FixSeedTexts2035R3Statements
{
    public const string OldSdrSubfamilyDesignation = "Depósito Volta";
    public const string NewSdrSubfamilyDesignation = "Depósito";
    public const string SdrArticleCode = "SDRVDEP";
    public const string SdrSubfamilyCode = "SDRV";

    public static readonly (Guid Id, string Old, string New)[] ArticleDesignations =
    [
        (Guid.Parse("b90be65a-983e-4e1b-a46d-cdb3716c3c47"), "Tarte de Amêndoa", "Tarte de amêndoa"),
        (Guid.Parse("af9e705e-a572-460c-bcb6-60b25b33f0b8"), "Bacalhau à Casa", "Bacalhau à casa"),
        (Guid.Parse("c5a0a6f7-d506-4312-a1b4-16999ae72915"), "Bacalhau à Lagareiro", "Bacalhau à lagareiro"),
        (Guid.Parse("f3958333-43f3-4e94-916a-7a8db56cd35c"), "Polvo à Lagareiro", "Polvo à lagareiro"),
        (Guid.Parse("89d868ec-eb47-43c3-92f8-8be9a1366189"), "Terras do Demo Bruto", "Terras do Demo bruto"),
    ];

    public static IEnumerable<string> ForSqlite() => Build(SqlDialect.Sqlite);

    public static IEnumerable<string> ForMySql() => Build(SqlDialect.MySql);

    public static IEnumerable<string> ForSqlServer() => Build(SqlDialect.SqlServer);

    public static IEnumerable<string> ForPostgreSql() => Build(SqlDialect.PostgreSql);

    private enum SqlDialect
    {
        Sqlite,
        MySql,
        SqlServer,
        PostgreSql
    }

    private static IEnumerable<string> Build(SqlDialect dialect)
    {
        yield return UpdateSdrSubfamilyDesignation(dialect);
        foreach (var (id, oldValue, newValue) in ArticleDesignations)
        {
            yield return UpdateDesignationById(dialect, "Articles", id, oldValue, newValue);
        }
    }

    /// <summary>
    /// Updates the ArticleSubfamilies row for SDRVDEP: match by that row's Id
    /// (via Articles.SubfamilyId) or by Code SDRV, and by the exact old designation.
    /// </summary>
    private static string UpdateSdrSubfamilyDesignation(SqlDialect dialect)
    {
        var subfamilies = Quote(dialect, "ArticleSubfamilies");
        var articles = Quote(dialect, "Articles");
        var designation = Quote(dialect, "Designation");
        var idCol = Quote(dialect, "Id");
        var codeCol = Quote(dialect, "Code");
        var subfamilyIdCol = Quote(dialect, "SubfamilyId");
        var oldLiteral = BinaryStringLiteral(dialect, OldSdrSubfamilyDesignation);
        var newLiteral = Literal(dialect, NewSdrSubfamilyDesignation);
        var articleCodeLiteral = BinaryStringLiteral(dialect, SdrArticleCode);
        var subfamilyCodeLiteral = BinaryStringLiteral(dialect, SdrSubfamilyCode);

        return $"UPDATE {subfamilies} SET {designation} = {newLiteral} "
            + $"WHERE {designation} = {oldLiteral} "
            + $"AND ("
            + $"{codeCol} = {subfamilyCodeLiteral} "
            + $"OR {idCol} IN (SELECT a.{subfamilyIdCol} FROM {articles} a WHERE a.{codeCol} = {articleCodeLiteral})"
            + $");";
    }

    private static string UpdateDesignationById(
        SqlDialect dialect, string table, Guid id, string oldValue, string newValue)
    {
        var quotedTable = Quote(dialect, table);
        var designation = Quote(dialect, "Designation");
        var idColumn = Quote(dialect, "Id");
        var idLiteral = GuidLiteral(dialect, id);
        var oldLiteral = BinaryStringLiteral(dialect, oldValue);
        var newLiteral = Literal(dialect, newValue);
        return $"UPDATE {quotedTable} SET {designation} = {newLiteral} WHERE {idColumn} = {idLiteral} AND {designation} = {oldLiteral};";
    }

    private static string GuidLiteral(SqlDialect dialect, Guid id) =>
        dialect switch
        {
            SqlDialect.Sqlite => $"'{id.ToString().ToUpperInvariant()}'",
            SqlDialect.PostgreSql => $"'{id}'::uuid",
            SqlDialect.SqlServer => $"'{id}'",
            _ => $"'{id}'"
        };

    private static string BinaryStringLiteral(SqlDialect dialect, string value)
    {
        var literal = Literal(dialect, value);
        return dialect switch
        {
            SqlDialect.MySql => $"{literal} COLLATE utf8mb4_bin",
            SqlDialect.SqlServer => $"{literal} COLLATE Latin1_General_100_BIN2",
            _ => literal
        };
    }

    private static string Quote(SqlDialect dialect, string identifier) =>
        dialect == SqlDialect.PostgreSql ? $"\"{identifier}\"" : identifier;

    private static string Literal(SqlDialect dialect, string value)
    {
        var escaped = value.Replace("'", "''");
        return dialect == SqlDialect.SqlServer ? $"N'{escaped}'" : $"'{escaped}'";
    }
}
