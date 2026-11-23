using System;
using System.Collections.Generic;

namespace LogicPOS.Persistence.DataFixes;

/// <summary>
/// Conditional SQL for FixSeedTexts2035R2 (SQLite / MySQL / SQL Server / PostgreSQL).
/// Text UPDATEs match seed Id/Code and the exact previous value (binary / case-sensitive).
/// Also soft-deletes orphan duplicate Final Consumer rows (non-text cleanup; not reversed on Down).
/// </summary>
public static class FixSeedTexts2035R2Statements
{
    public const string OldSdrArticleDesignation = "Valor de Depósitivo Volta";
    public const string NewSdrArticleDesignation = "Depósito";
    public const string SdrArticleCode = "SDRVDEP";

    public const string OldArticleClassDesignation = "Imposto (excepto IVA e IS) ou Encargo Parafiscal";
    public const string NewArticleClassDesignation = "Imposto (exceto IVA e IS) ou Encargo Parafiscal";
    public static readonly Guid ArticleClassImpostoId = Guid.Parse("82c158f9-9c30-468f-9d15-8bde112998f4");

    public const string OldMailSubject = "Envio de documentos electrónicos";
    public const string NewMailSubject = "Envio de documentos eletrónicos";

    public const string OldMailBody = "Estimado/a cliente, \n\nVimos por este meio enviar em anexo o(s) vosso(s) documento(s) electrónico(s).\n\n${DOCUMENT_LIST}\n\nPara qualquer dúvida ou esclarecimento adicionais por favor não hesite em contactar-nos através do nr ${COMPANY_TELEPHONE}, todos os dias úteis das 09:00 às 18:00, ou via e-mail para ${COMPANY_EMAIL}.\n\nNo caso de liquidação de faturas, agradecemos envio de comprovativo com indicação Nome / Nr Cliente ou Nr Factura(s) após pagamento para o endereço email ${COMPANY_EMAIL}.\n\nCom os nossos melhores cumprimentos, \nDepartamento Facturação / Cobranças \n\n${COMPANY_NAME}, ${COMPANY_BUSINESS_NAME}\n${COMPANY_WEBSITE}\n${COMPANY_EMAIL}\n\nTel. ${COMPANY_TELEPHONE}\n${COMPANY_ADDRESS}\n${COMPANY_POSTALCODE} ${COMPANY_CITY}\n${COMPANY_COUNTRY}";
    public const string NewMailBody = "Estimado/a cliente, \n\nVimos por este meio enviar em anexo o(s) vosso(s) documento(s) eletrónico(s).\n\n${DOCUMENT_LIST}\n\nPara qualquer dúvida ou esclarecimento adicionais por favor não hesite em contactar-nos através do nr ${COMPANY_TELEPHONE}, todos os dias úteis das 09:00 às 18:00, ou via e-mail para ${COMPANY_EMAIL}.\n\nNo caso de liquidação de faturas, agradecemos envio de comprovativo com indicação Nome / Nr Cliente ou N.º Fatura(s) após pagamento para o endereço email ${COMPANY_EMAIL}.\n\nCom os nossos melhores cumprimentos, \nDepartamento Faturação / Cobranças \n\n${COMPANY_NAME}, ${COMPANY_BUSINESS_NAME}\n${COMPANY_WEBSITE}\n${COMPANY_EMAIL}\n\nTel. ${COMPANY_TELEPHONE}\n${COMPANY_ADDRESS}\n${COMPANY_POSTALCODE} ${COMPANY_CITY}\n${COMPANY_COUNTRY}";

    public static readonly Guid FinalConsumerId = Guid.Parse("0cf40622-578b-417d-b50f-e945fefb5d68");

    public const string FinalConsumerName = "Consumidor Final";

    public static readonly Guid PaymentConditionLpdId = Guid.Parse("c07a5c41-37b2-4bf3-9469-5faf1799a89f");
    public const string OldPaymentConditionLpdDesignation = "Letra p/Desconto";
    public const string NewPaymentConditionLpdDesignation = "Letra para desconto";

    public static readonly Guid DocumentTypeTrvId = Guid.Parse("020f94b0-8006-4465-aad6-aa478df20f42");
    public const string OldDocumentTypeTrvDesignation = "Talão Reembolso Volta";
    public const string NewDocumentTypeTrvDesignation = "Talão de reembolso de depósito";

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
        yield return UpdateArticleDesignationByCode(
            dialect, SdrArticleCode, OldSdrArticleDesignation, NewSdrArticleDesignation);
        yield return UpdateDesignationById(
            dialect, "ArticleClasses", ArticleClassImpostoId,
            OldArticleClassDesignation, NewArticleClassDesignation);
        yield return UpdateTokenValue(dialect, "SEND_MAIL_FINANCE_DOCUMENTS_SUBJECT", OldMailSubject, NewMailSubject);
        yield return UpdateTokenValue(dialect, "SEND_MAIL_FINANCE_DOCUMENTS_BODY", OldMailBody, NewMailBody);
        yield return SoftDeleteDuplicateFinalConsumersWithoutDocuments(dialect);
        yield return UpdateDesignationById(
            dialect, "PaymentConditions", PaymentConditionLpdId,
            OldPaymentConditionLpdDesignation, NewPaymentConditionLpdDesignation);
        yield return UpdateDesignationById(
            dialect, "DocumentTypes", DocumentTypeTrvId,
            OldDocumentTypeTrvDesignation, NewDocumentTypeTrvDesignation);
    }

    /// <summary>
    /// Soft-delete extra «Consumidor Final» rows that have no documents and no stock movements.
    /// Keeps the well-known seed Id; Name match uses the keep-row ciphertext with binary comparison.
    /// Derived-table form avoids MySQL error 1093 (same-table subquery in UPDATE).
    /// </summary>
    private static string SoftDeleteDuplicateFinalConsumersWithoutDocuments(SqlDialect dialect)
    {
        var customers = Quote(dialect, "Customers");
        var documents = Quote(dialect, "Documents");
        var stockMovements = Quote(dialect, "StockMovements");
        var idCol = Quote(dialect, "Id");
        var nameCol = Quote(dialect, "Name");
        var deletedCol = Quote(dialect, "IsDeleted");
        var deletedAtCol = Quote(dialect, "DeletedAt");
        var customerIdCol = Quote(dialect, "CustomerId");
        var keepId = GuidLiteral(dialect, FinalConsumerId);
        var (falseLiteral, trueLiteral) = BoolLiterals(dialect);
        var deletedAt = dialect switch
        {
            SqlDialect.SqlServer => "SYSUTCDATETIME()",
            SqlDialect.PostgreSql => "NOW() AT TIME ZONE 'UTC'",
            _ => "CURRENT_TIMESTAMP"
        };

        // PK lookup — no LIMIT (invalid on SQL Server; unnecessary for a single Id).
        var keepNameSubquery = $"(SELECT k.{nameCol} FROM {customers} k WHERE k.{idCol} = {keepId})";
        var nameMatch = dialect switch
        {
            SqlDialect.MySql =>
                $"c.{nameCol} = {keepNameSubquery} COLLATE utf8mb4_bin",
            SqlDialect.SqlServer =>
                $"c.{nameCol} = {keepNameSubquery} COLLATE Latin1_General_100_BIN2",
            _ =>
                $"c.{nameCol} = {keepNameSubquery}"
        };

        var innerSelect =
            $"SELECT c.{idCol} FROM {customers} c "
            + $"WHERE c.{deletedCol} = {falseLiteral} "
            + $"AND c.{idCol} <> {keepId} "
            + $"AND {nameMatch} "
            + $"AND NOT EXISTS (SELECT 1 FROM {documents} d WHERE d.{customerIdCol} = c.{idCol}) "
            + $"AND NOT EXISTS (SELECT 1 FROM {stockMovements} s WHERE s.{customerIdCol} = c.{idCol})";

        // Derived table required for MySQL 8 (error 1093); harmless on the other providers.
        return $"UPDATE {customers} SET {deletedCol} = {trueLiteral}, {deletedAtCol} = {deletedAt} "
            + $"WHERE {idCol} IN (SELECT {idCol} FROM ({innerSelect}) AS DuplicateFinalConsumers);";
    }

    private static (string FalseLiteral, string TrueLiteral) BoolLiterals(SqlDialect dialect) =>
        dialect == SqlDialect.PostgreSql ? ("FALSE", "TRUE") : ("0", "1");

    private static string GuidLiteral(SqlDialect dialect, Guid id) =>
        dialect switch
        {
            // EF Core SQLite stores GUIDs as uppercase TEXT (8-4-4-4-12).
            SqlDialect.Sqlite => $"'{id.ToString().ToUpperInvariant()}'",
            SqlDialect.PostgreSql => $"'{id}'::uuid",
            SqlDialect.SqlServer => $"'{id}'",
            _ => $"'{id}'"
        };

    private static string UpdateTokenValue(SqlDialect dialect, string token, string oldValue, string newValue)
    {
        var table = Quote(dialect, "PreferenceParameters");
        var valueColumn = Quote(dialect, "Value");
        var tokenColumn = Quote(dialect, "Token");
        return $"UPDATE {table} SET {valueColumn} = {Literal(dialect, newValue)} WHERE {tokenColumn} = {Literal(dialect, token)} AND {valueColumn} = {Literal(dialect, oldValue)};";
    }

    private static string UpdateArticleDesignationByCode(
        SqlDialect dialect, string code, string oldValue, string newValue)
    {
        var quotedTable = Quote(dialect, "Articles");
        var designation = Quote(dialect, "Designation");
        var codeColumn = Quote(dialect, "Code");
        var oldLiteral = BinaryStringLiteral(dialect, oldValue);
        var newLiteral = Literal(dialect, newValue);
        var codeLiteral = BinaryStringLiteral(dialect, code);
        return $"UPDATE {quotedTable} SET {designation} = {newLiteral} WHERE {codeColumn} = {codeLiteral} AND {designation} = {oldLiteral};";
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

    private static string BinaryStringLiteral(SqlDialect dialect, string value)
    {
        var literal = Literal(dialect, value);
        return dialect switch
        {
            SqlDialect.MySql => $"{literal} COLLATE utf8mb4_bin",
            SqlDialect.SqlServer => $"{literal} COLLATE Latin1_General_100_BIN2",
            // SQLite / PostgreSQL string equality is binary by default for these columns.
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
