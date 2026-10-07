using System.Globalization;
using System.Reflection;
using System.Text;
using LogicPOS.Core.Authentication;
using LogicPOS.Core.Fiscal;
using LogicPOS.Core.FrontOffice;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.Common;
using LogicPOS.Domain.Services;
using LogicPOS.Domain.ValueObjects;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.BackOffice;

public sealed class BackOfficeListingService : IBackOfficeListingService
{
    private const int RowLimit = 500;

    private static readonly string[] Hidden =
    [
        "Id", "CreatedAt", "CreatedBy", "CreatedWhere", "UpdatedAt", "UpdatedBy", "UpdatedWhere",
        "DeletedAt", "IsDeleted", "Hash", "Hash4Code", "HashControl", "Password", "AccessPin",
        "AccessPassword", "ExternalDocument", "QrCode", "ButtonImage", "BaseConsumption", "BaseOffers",
        "PVPOffers", "Remarks"
    ];

    private static readonly Dictionary<string, string> Headers = new(StringComparer.Ordinal)
    {
        ["Code"] = "Código",
        ["Designation"] = "Designação",
        ["Name"] = "Nome",
        ["Notes"] = "Notas",
        ["Order"] = "Ordem",
        ["Value"] = "Valor",
        ["FiscalNumber"] = "NIF",
        ["Email"] = "Email",
        ["Phone"] = "Telefone",
        ["MobilePhone"] = "Telemóvel",
        ["Barcode"] = "Código de barras",
        ["Quantity"] = "Quantidade",
        ["Token"] = "Parâmetro",
        ["Acronym"] = "Sigla",
        ["Year"] = "Ano",
        ["Status"] = "Estado",
        ["Amount"] = "Valor",
        ["IsRead"] = "Lida",
        ["Message"] = "Mensagem",
        ["Description"] = "Descrição",
        ["Date"] = "Data",
        ["NextNumber"] = "Próximo número",
        ["HardwareId"] = "Hardware",
        ["IsDefault"] = "Predefinido",
        ["Favorite"] = "Favorito",
        ["Discount"] = "Desconto",
        ["Price1"] = "Preço 1",
        ["Price2"] = "Preço 2",
        ["Price3"] = "Preço 3",
        ["Price4"] = "Preço 4",
        ["Price5"] = "Preço 5",
        ["Address"] = "Morada",
        ["City"] = "Cidade",
        ["ZipCode"] = "Código postal",
        ["Locality"] = "Localidade",
        ["RefNo"] = "Número",
        ["Number"] = "Número",
        ["Type"] = "Tipo",
        ["TotalFinal"] = "Total",
        ["Paid"] = "Pago",
        ["Customer"] = "Cliente",
        ["Documents"] = "Documentos",
        ["Debt"] = "Em dívida",
        ["File"] = "Ficheiro",
        ["Size"] = "Tamanho",
        ["Path"] = "Caminho",
        ["Login"] = "Login",
        ["Pin"] = "PIN",
        ["Month"] = "Mês",
        ["Day"] = "Dia",
        ["Fixed"] = "Fixo",
        ["ResourceString"] = "Descrição",
        ["StartDate"] = "Início",
        ["EndDate"] = "Fim",
        ["TaxCode"] = "Código imposto",
        ["TaxType"] = "Tipo imposto",
        ["CountryRegion"] = "Região",
        ["ReasonCode"] = "Motivo",
        ["Supplier"] = "Fornecedor",
        ["CardNumber"] = "Cartão",
        ["WebSite"] = "Website"
    };

    private readonly IServiceScopeFactory _scopes;
    private readonly IPasswordHasher _passwords;
    private readonly PageSpec[] _pages;

    public BackOfficeListingService(IServiceScopeFactory scopes, IPasswordHasher passwords)
    {
        _scopes = scopes;
        _passwords = passwords;
        _pages = BuildPages();
    }

    public IReadOnlyList<string> PageTitles() => _pages.Select(page => page.Title).ToList();

    public string ResolveTitle(string title) => title switch
    {
        "Documentos Fiscais" => "Documentos",
        "Configuração" => "Parâmetros de Sistema",
        "Rel. de Fatur." => "Relatórios",
        "Vendas por dia" => "Relatórios",
        "Aber. de ano fiscal" => "Abertura de ano fiscal",
        "Ext. C.Cliente" => "Conta.Corr.",
        "Stocks" => "Gestão de Stocks",
        "Outras Tabelas" => "País",
        "Novo Documento" => "Novo Doc.",
        _ => title
    };

    public bool HasPage(string title) => Find(ResolveTitle(title)) is not null;

    public async Task<ListingSnapshot> QueryAsync(string title, string? search = null, string? mode = null, CancellationToken cancellationToken = default, Guid? customerId = null)
    {
        title = ResolveTitle(title);
        var page = Find(title) ?? throw new InvalidOperationException($"Página desconhecida: {title}");
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var snapshot = page.Kind switch
        {
            PageKind.Reports => await QueryReportsAsync(database, mode, cancellationToken),
            PageKind.CurrentAccount => await QueryCurrentAccountAsync(database, cancellationToken),
            PageKind.ReceiptEmission => await QueryOpenDocumentsAsync(database, cancellationToken),
            PageKind.Files => QueryFiles(page),
            PageKind.Notice => NoticePage(page),
            _ => await QueryEntitiesAsync(database, page, cancellationToken)
        };

        var rows = snapshot.Rows;
        if (customerId is Guid selectedCustomer && selectedCustomer != Guid.Empty && title == "Recibos")
        {
            var receiptIds = await database.Payments.AsNoTracking()
                .Join(
                    database.Documents.AsNoTracking(),
                    payment => payment.DocumentId,
                    document => document.Id,
                    (payment, document) => new { payment.ReceiptId, document.CustomerId, document.IsDeleted })
                .Where(item => item.IsDeleted == false && item.CustomerId == selectedCustomer)
                .Select(item => item.ReceiptId)
                .Distinct()
                .ToListAsync(cancellationToken);
            var allowed = receiptIds.ToHashSet();
            rows = rows.Where(row => allowed.Contains(row.Id)).ToList();
        }

        if (string.IsNullOrWhiteSpace(search) == false)
        {
            rows = rows.Where(row => row.Values.Values.Any(value =>
                value.Contains(search, StringComparison.CurrentCultureIgnoreCase))).ToList();
        }

        return new ListingSnapshot
        {
            Title = title,
            CanCreate = page.CanCreate,
            CanEdit = page.CanEdit,
            CanDelete = page.CanDelete,
            Columns = snapshot.Columns,
            Rows = rows,
            Actions = page.Actions,
            Modes = page.Modes,
            Notice = snapshot.Notice
        };
    }

    public async Task<IReadOnlyList<ListingField>> LoadFieldsAsync(string title, Guid? id, CancellationToken cancellationToken = default)
    {
        title = ResolveTitle(title);
        var page = Find(title) ?? throw new InvalidOperationException($"Página desconhecida: {title}");
        if (page.EntityType is null)
        {
            return [];
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        object? entity = null;
        if (id is Guid existing)
        {
            entity = await FindEntityAsync(database, page.EntityType, existing, cancellationToken);
        }

        entity ??= Activator.CreateInstance(page.EntityType)!;
        var fields = Describe(page, entity, id is null);
        if (entity is Article article && id is Guid articleId && articleId != Guid.Empty)
        {
            await AddArticleExtrasAsync(database, fields, article, articleId, cancellationToken);
        }

        return fields;
    }

    public async Task<IReadOnlyList<LookupOption>> LookupAsync(string typeName, CancellationToken cancellationToken = default)
    {
        var type = ResolveEntityType(typeName);
        if (type is null)
        {
            return [];
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var rows = await MaterializeAsync(database, type, cancellationToken);
        var articles = type.Name == "Article";
        return rows
            .Cast<object>()
            .Where(row => ReadBool(row, "IsDeleted") == false)
            .Select(row => articles
                ? LookupOption.FromArticle(ReadGuid(row, "Id"), ReadString(row, "Code"), ReadString(row, "Designation"), ReadString(row, "Barcode"))
                : new LookupOption
                {
                    Id = ReadGuid(row, "Id"),
                    Label = DisplayLabel(row),
                    Meta = LookupMeta(row)
                })
            .Where(option => option.Id != Guid.Empty)
            .OrderBy(option => option.Label)
            .ToList();
    }

    private static string? LookupMeta(object item)
    {
        var type = item.GetType();
        var hasPrice = type.GetProperty("HasPrice") ?? type.GetProperty("HavePrice");
        if (hasPrice?.PropertyType == typeof(bool) && hasPrice.GetValue(item) is bool price)
        {
            return price ? "1" : "0";
        }

        var value = type.GetProperty("Value");
        if (value?.PropertyType == typeof(decimal) && value.GetValue(item) is decimal rate)
        {
            return rate.ToString(CultureInfo.InvariantCulture);
        }

        var code = type.GetProperty("Code2")?.GetValue(item)?.ToString();
        return string.IsNullOrWhiteSpace(code) ? null : code;
    }

    public async Task<ListingSaveResult> SaveAsync(string title, Guid? id, IReadOnlyDictionary<string, string> fields, CancellationToken cancellationToken = default)
    {
        title = ResolveTitle(title);
        var page = Find(title);
        if (page?.EntityType is null || page.CanEdit == false && id is not null || page.CanCreate == false && id is null)
        {
            return ListingSaveResult.Fail("Esta listagem não permite gravar.");
        }

        try
        {
            await using var scope = _scopes.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            var creating = id is null || id == Guid.Empty;
            object entity;
            decimal previousQuantity = 0;
            if (creating)
            {
                entity = Activator.CreateInstance(page.EntityType)!;
                page.EntityType.GetProperty("Id")?.SetValue(entity, Guid.NewGuid());
                var now = DateTime.Now;
                page.EntityType.GetProperty("CreatedAt")?.SetValue(entity, now);
                page.EntityType.GetProperty("UpdatedAt")?.SetValue(entity, now);
            }
            else
            {
                entity = await FindEntityAsync(database, page.EntityType, id!.Value, cancellationToken)
                    ?? throw new InvalidOperationException("Registo não encontrado.");
                page.EntityType.GetProperty("UpdatedAt")?.SetValue(entity, DateTime.Now);
                if (entity is WarehouseArticle article)
                {
                    previousQuantity = article.Quantity;
                }
            }

            var error = ApplyFields(page.EntityType, entity, fields);
            if (error is not null)
            {
                return ListingSaveResult.Fail(error);
            }

            PrepareRequired(page.EntityType, entity, database);
            if (entity is User user)
            {
                ApplyPin(user, fields, creating);
            }

            var missing = await FillForeignKeysAsync(database, page.EntityType, entity, cancellationToken);
            if (missing is not null)
            {
                return ListingSaveResult.Fail(missing);
            }

            if (creating)
            {
                database.Add(entity);
            }

            if (entity is WarehouseArticle stock)
            {
                var delta = stock.Quantity - previousQuantity;
                if (delta != 0)
                {
                    database.StockMovements.Add(new StockMovement
                    {
                        ArticleId = stock.ArticleId,
                        WarehouseLocationId = stock.WarehouseLocationId,
                        Quantity = delta,
                        Date = DateTime.Now,
                        DocumentNumber = "STOCK",
                        Notes = delta > 0 ? "Entrada" : "Saída"
                    });
                }
            }

            await database.SaveChangesAsync(cancellationToken);
            if (entity is Article savedArticle)
            {
                await SaveArticleChildrenAsync(database, savedArticle.Id, fields, cancellationToken);
                await database.SaveChangesAsync(cancellationToken);
            }

            return ListingSaveResult.Ok(ReadGuid(entity, "Id"), "Registo gravado.");
        }
        catch (Exception exception)
        {
            return ListingSaveResult.Fail(DescribeException(exception));
        }
    }

    public async Task<ListingSaveResult> DeleteAsync(string title, Guid id, CancellationToken cancellationToken = default)
    {
        title = ResolveTitle(title);
        var page = Find(title);
        if (page?.EntityType is null || page.CanDelete == false)
        {
            return ListingSaveResult.Fail("Esta listagem não permite eliminar.");
        }

        try
        {
            await using var scope = _scopes.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            var entity = await FindEntityAsync(database, page.EntityType, id, cancellationToken);
            if (entity is null)
            {
                return ListingSaveResult.Fail("Registo não encontrado.");
            }

            if (CanRemove(entity) == false)
            {
                return ListingSaveResult.Fail("Este registo está protegido.");
            }

            page.EntityType.GetProperty("IsDeleted")?.SetValue(entity, true);
            page.EntityType.GetProperty("DeletedAt")?.SetValue(entity, DateTime.Now);
            page.EntityType.GetProperty("UpdatedAt")?.SetValue(entity, DateTime.Now);
            await database.SaveChangesAsync(cancellationToken);
            return ListingSaveResult.Ok(id, "Registo eliminado.");
        }
        catch (Exception exception)
        {
            return ListingSaveResult.Fail(DescribeException(exception));
        }
    }

    public async Task<ListingSaveResult> RunActionAsync(string action, Guid? id, string? extra, CancellationToken cancellationToken = default)
    {
        try
        {
            return action switch
            {
                "export-saft" or "saft-period" => await FiscalAsync(module => module.ExportSaftAsync(ParseStart(extra), ParseEnd(extra), cancellationToken), cancellationToken),
                "saft-year" => await ExportSaftRangeAsync(new DateTime(DateTime.Today.Year, 1, 1), new DateTime(DateTime.Today.Year, 12, 31), cancellationToken),
                "saft-month" => await ExportSaftMonthAsync(cancellationToken),
                "register-at" => await FiscalAsync(module => module.RegisterSeriesAsync(id ?? Guid.Empty, cancellationToken), cancellationToken),
                "request-agt" or "request-series" => await FiscalAsync(module => module.RequestSeriesCodeAsync(DateTime.Today.Year, string.IsNullOrWhiteSpace(extra) ? "FT" : extra.Trim(), cancellationToken), cancellationToken),
                "open-day" => await OpenDayAsync(cancellationToken),
                "open-session" => await OpenSessionAsync(cancellationToken),
                "close-session" => await CloseSessionAsync(cancellationToken),
                "close-day" => await CloseDayAsync(cancellationToken),
                "cash-in" => await CashAsync(true, extra, cancellationToken),
                "cash-out" => await CashAsync(false, extra, cancellationToken),
                "cash-total" => await CashTotalAsync(cancellationToken),
                "stock-in" => await AdjustStockAsync(id, extra, 1, cancellationToken),
                "stock-out" => await AdjustStockAsync(id, extra, -1, cancellationToken),
                "emit-receipt" => await EmitReceiptAsync(id, cancellationToken),
                "backup" => await BackupAsync(cancellationToken),
                "restore" => await RestoreAsync(extra, cancellationToken),
                "export-articles" => await ExportArticlesAsync(cancellationToken),
                "export-customers" => await ExportCustomersAsync(cancellationToken),
                "import-articles" => await ImportArticlesAsync(extra, cancellationToken),
                "import-customers" => await ImportCustomersAsync(extra, cancellationToken),
                "undo-receipt" => await UndoReceiptAsync(id ?? Guid.Empty, cancellationToken),
                "view-receipt" => await ViewReceiptAsync(id ?? Guid.Empty, cancellationToken),
                "agt-consult" => ListingSaveResult.Fail("A consulta de documentos AGT online está disponível no modo cloud."),
                _ => ListingSaveResult.Fail("Ação desconhecida.")
            };
        }
        catch (Exception exception)
        {
            return ListingSaveResult.Fail(DescribeException(exception));
        }
    }

    private async Task<ListingSnapshot> QueryEntitiesAsync(LogicPOSDbContext database, PageSpec page, CancellationToken cancellationToken)
    {
        var rows = (await MaterializeAsync(database, page.EntityType!, cancellationToken))
            .Cast<object>()
            .Where(row => ReadBool(row, "IsDeleted") == false)
            .Where(row => MatchesFilter(page, row))
            .OrderByDescending(row => ReadDate(row, "CreatedAt"))
            .Take(RowLimit)
            .ToList();

        var fields = rows.Count == 0
            ? Describe(page, Activator.CreateInstance(page.EntityType!)!, true)
            : Describe(page, rows[0], false);
        var columns = ListingColumns.For(page.Title, fields.Select(field => field.Key));
        var lookups = await LoadLookupsAsync(database, page.EntityType!, columns, cancellationToken);
        return new ListingSnapshot
        {
            Columns = columns,
            Rows = rows.Select(row => ToRow(page, row, columns, lookups)).ToList()
        };
    }

    private async Task<ListingSnapshot> QueryReportsAsync(LogicPOSDbContext database, string? mode, CancellationToken cancellationToken)
    {
        mode = string.IsNullOrWhiteSpace(mode) ? "Vendas por dia" : mode;
        if (mode == "Vendas por artigo")
        {
            var details = await database.DocumentDetails.AsNoTracking()
                .Where(detail => detail.IsDeleted == false)
                .ToListAsync(cancellationToken);
            var rows = details
                .GroupBy(detail => string.IsNullOrWhiteSpace(detail.Designation) ? "(sem designação)" : detail.Designation)
                .Select(group => Row(Guid.NewGuid(), false,
                    ("Designation", group.Key),
                    ("Quantity", group.Sum(item => item.Quantity).ToString("0.##", CultureInfo.CurrentCulture)),
                    ("TotalFinal", group.Sum(item => item.TotalFinal).ToString("0.00", CultureInfo.CurrentCulture))))
                .OrderByDescending(row => ParseDecimal(row["TotalFinal"]))
                .ToList();
            return Grid("Designação", "Designation", rows, ("Quantidade", "Quantity"), ("Total", "TotalFinal"));
        }

        var documents = await database.Documents.AsNoTracking()
            .Where(document => document.IsDeleted == false && document.IsDraft == false)
            .ToListAsync(cancellationToken);
        if (mode == "Vendas por cliente")
        {
            var rows = documents
                .GroupBy(document => string.IsNullOrWhiteSpace(document.Customer.Name) ? "(sem cliente)" : document.Customer.Name)
                .Select(group => Row(Guid.NewGuid(), false,
                    ("Name", group.Key),
                    ("Documents", group.Count().ToString(CultureInfo.CurrentCulture)),
                    ("TotalFinal", group.Sum(item => item.TotalFinal).ToString("0.00", CultureInfo.CurrentCulture))))
                .OrderByDescending(row => ParseDecimal(row["TotalFinal"]))
                .ToList();
            return Grid("Cliente", "Name", rows, ("Documentos", "Documents"), ("Total", "TotalFinal"));
        }

        var byDay = documents
            .GroupBy(document => document.CreatedAt.Date)
            .Select(group => Row(Guid.NewGuid(), false,
                ("Date", group.Key.ToString("yyyy-MM-dd")),
                ("Documents", group.Count().ToString(CultureInfo.CurrentCulture)),
                ("TotalFinal", group.Sum(item => item.TotalFinal).ToString("0.00", CultureInfo.CurrentCulture))))
            .OrderByDescending(row => row["Date"])
            .ToList();
        return Grid("Data", "Date", byDay, ("Documentos", "Documents"), ("Total", "TotalFinal"));
    }

    private async Task<ListingSnapshot> QueryCurrentAccountAsync(LogicPOSDbContext database, CancellationToken cancellationToken)
    {
        var documents = await database.Documents.AsNoTracking()
            .Where(document => document.IsDeleted == false && document.IsDraft == false)
            .ToListAsync(cancellationToken);
        var rows = documents
            .GroupBy(document => document.CustomerId)
            .Select(group =>
            {
                var sample = group.First();
                return Row(sample.CustomerId, false,
                    ("Name", sample.Customer.Name),
                    ("FiscalNumber", sample.Customer.FiscalNumber),
                    ("Documents", group.Count().ToString(CultureInfo.CurrentCulture)),
                    ("TotalFinal", group.Sum(item => item.TotalFinal).ToString("0.00", CultureInfo.CurrentCulture)),
                    ("Debt", group.Where(item => item.Paid == false).Sum(item => item.TotalFinal).ToString("0.00", CultureInfo.CurrentCulture)));
            })
            .OrderBy(row => row["Name"])
            .ToList();
        return Grid("Cliente", "Name", rows,
            ("NIF", "FiscalNumber"),
            ("Documentos", "Documents"),
            ("Total", "TotalFinal"),
            ("Em dívida", "Debt"));
    }

    private async Task<ListingSnapshot> QueryOpenDocumentsAsync(LogicPOSDbContext database, CancellationToken cancellationToken)
    {
        var documents = await database.Documents.AsNoTracking()
            .Where(document => document.IsDeleted == false && document.IsDraft == false && document.Paid == false)
            .OrderByDescending(document => document.CreatedAt)
            .Take(RowLimit)
            .ToListAsync(cancellationToken);
        var rows = documents.Select(document => Row(document.Id, false,
            ("Number", document.Number),
            ("Type", document.Type),
            ("Name", document.Customer.Name),
            ("TotalFinal", document.TotalFinal.ToString("0.00", CultureInfo.CurrentCulture)),
            ("Date", document.CreatedAt.ToString("yyyy-MM-dd HH:mm")))).ToList();
        return Grid("Número", "Number", rows,
            ("Tipo", "Type"),
            ("Cliente", "Name"),
            ("Total", "TotalFinal"),
            ("Data", "Date"));
    }

    private ListingSnapshot QueryFiles(PageSpec page)
    {
        var directory = page.Kind == PageKind.Files ? Folder(page.Filter!) : Folder("backups");
        Directory.CreateDirectory(directory);
        var rows = Directory.GetFiles(directory)
            .Select(path => new FileInfo(path))
            .OrderByDescending(file => file.LastWriteTime)
            .Select(file => Row(Guid.NewGuid(), false,
                ("File", file.Name),
                ("Date", file.LastWriteTime.ToString("yyyy-MM-dd HH:mm")),
                ("Size", file.Length.ToString(CultureInfo.CurrentCulture)),
                ("Path", file.FullName)))
            .ToList();
        return Grid("Ficheiro", "File", rows, ("Data", "Date"), ("Tamanho", "Size"), ("Caminho", "Path"));
    }

    private static ListingSnapshot Grid(string firstHeader, string firstKey, List<ListingRow> rows, params (string Header, string Key)[] rest)
    {
        var columns = new List<ListingColumn> { new() { Header = firstHeader, Key = firstKey } };
        columns.AddRange(rest.Select(item => new ListingColumn { Header = item.Header, Key = item.Key }));
        return new ListingSnapshot { Columns = columns, Rows = rows };
    }

    private static ListingRow Row(Guid id, bool canDelete, params (string Key, string Value)[] cells)
    {
        return new ListingRow
        {
            Id = id,
            CanDelete = canDelete,
            Values = cells.ToDictionary(cell => cell.Key, cell => cell.Value ?? string.Empty)
        };
    }

    private ListingRow ToRow(PageSpec page, object entity, IReadOnlyList<ListingColumn> columns, IReadOnlyDictionary<string, Dictionary<Guid, string>> lookups)
    {
        var values = new Dictionary<string, string>();
        foreach (var column in columns)
        {
            values[column.Key] = ReadCell(entity, column.Key, lookups);
        }

        return new ListingRow
        {
            Id = ReadGuid(entity, "Id"),
            CanDelete = page.CanDelete && CanRemove(entity),
            Values = values
        };
    }

    private List<ListingField> Describe(PageSpec page, object entity, bool creating)
    {
        var fields = new List<ListingField>();
        foreach (var property in page.EntityType!.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.CanRead == false || property.GetMethod is null || property.GetMethod.GetParameters().Length > 0)
            {
                continue;
            }

            if (Hidden.Contains(property.Name) || property.Name is "Button")
            {
                continue;
            }

            if (page.EntityType == typeof(Printer) && property.Name.StartsWith("ThermalMaxCharsPerLine", StringComparison.Ordinal))
            {
                continue;
            }

            var kind = KindOf(property.PropertyType);
            if (kind is null)
            {
                continue;
            }

            string? lookup = null;
            if (kind == "guid")
            {
                var target = LookupType(page.EntityType, property.Name);
                if (target is null)
                {
                    continue;
                }

                lookup = target.Name;
                kind = "lookup";
            }

            var readOnly = page.CanEdit == false || (page.EntityType == typeof(PreferenceParameter) && property.Name is not ("Value" or "Notes"));
            if (creating && property.Name is "Code")
            {
                readOnly = true;
            }

            fields.Add(new ListingField
            {
                Key = property.Name,
                Label = Header(property.Name),
                Kind = property.Name is "Notes" or "Message" or "Value" ? "multiline" : kind,
                Value = ReadRaw(entity, property),
                Lookup = lookup,
                ReadOnly = readOnly,
                Required = kind is not ("lookup") && IsRequired(property) || (lookup is not null && IsRequired(property)),
                Group = FieldGroup(page.EntityType, property.Name)
            });
        }

        if (page.EntityType == typeof(User))
        {
            fields.Add(new ListingField
            {
                Key = "Pin",
                Label = "PIN",
                Kind = "text",
                Value = string.Empty,
                Required = creating,
                Group = "Detalhe"
            });
        }

        if (page.EntityType == typeof(Printer))
        {
            fields.Add(new ListingField
            {
                Key = ThermalPaper.FieldKey,
                Label = ThermalPaper.FieldLabel,
                Kind = "choice",
                Options = ThermalPaper.Labels,
                Value = ThermalPaper.LabelOf((entity as Printer)?.ThermalMaxCharsPerLineNormal),
                ReadOnly = page.CanEdit == false,
                Group = FieldGroup(page.EntityType, ThermalPaper.FieldKey)
            });
        }

        return fields;
    }

    private static string? FieldGroup(Type entityType, string propertyName)
    {
        if (entityType == typeof(User))
        {
            return propertyName switch
            {
                "Name" or "Login" or "ProfileId" or "CommissionGroupId" or "DateOfContract" or "PasswordReset" or "Pin"
                    => "Detalhe",
                "Residence" or "Locality" or "City" or "ZipCode" or "Phone" or "MobilePhone" or "Email"
                    => "Contactos",
                _ => "Outros"
            };
        }

        if (entityType != typeof(Article))
        {
            return null;
        }

        return propertyName switch
        {
            "Ord" or "Order" or "Code" or "CodeDealer" or "Designation" or "FamilyId" or "SubfamilyId"
                or "TypeId" or "IsComposed" or "Favorite" or "UseWeighingBalance" or "Disabled"
                => "Detalhes",
            "Price1" or "Price2" or "Price3" or "Price4" or "Price5" or "PriceWithVat" or "Discount"
                or "PVPVariable" or "ClassId" or "VatOnTableId" or "VatDirectSellingId" or "VatExemptionReasonId"
                => "Detalhes financeiros",
            "UniqueArticles" => "Número de série",
            _ => "Outros detalhes"
        };
    }

    private static List<ListingColumn> PickColumns(IReadOnlyList<ListingField> fields)
    {
        var preferred = new[]
        {
            "Code", "Designation", "Name", "Token", "Acronym", "Number", "RefNo", "FiscalNumber",
            "Barcode", "Value", "Quantity", "Status", "Year", "Email", "Phone"
        };
        var ordered = new List<ListingField>();
        foreach (var name in preferred)
        {
            var field = fields.FirstOrDefault(item => item.Key == name);
            if (field is not null)
            {
                ordered.Add(field);
            }
        }

        foreach (var field in fields)
        {
            if (ordered.Any(item => item.Key == field.Key) == false)
            {
                ordered.Add(field);
            }
        }

        return ordered.Select(field => new ListingColumn
        {
            Key = field.Key,
            Header = field.Label,
            Visible = true
        }).ToList();
    }

    private async Task<Dictionary<string, Dictionary<Guid, string>>> LoadLookupsAsync(LogicPOSDbContext database, Type entityType, IReadOnlyList<ListingColumn> columns, CancellationToken cancellationToken)
    {
        var maps = new Dictionary<string, Dictionary<Guid, string>>();
        foreach (var column in columns.Where(item => item.Key.EndsWith("Id", StringComparison.Ordinal)))
        {
            var type = LookupType(entityType, column.Key);
            if (type is null || maps.ContainsKey(column.Key))
            {
                continue;
            }

            var rows = await MaterializeAsync(database, type, cancellationToken);
            maps[column.Key] = rows.Cast<object>().ToDictionary(ReadId, DisplayLabel);
        }

        return maps;
    }

    private static string ReadCell(object entity, string propertyName, IReadOnlyDictionary<string, Dictionary<Guid, string>> lookups)
    {
        var property = entity.GetType().GetProperty(propertyName);
        if (property is null)
        {
            return string.Empty;
        }

        if (lookups.TryGetValue(propertyName, out var map))
        {
            var id = property.GetValue(entity) as Guid? ?? (property.GetValue(entity) is Guid guid ? guid : Guid.Empty);
            return id != Guid.Empty && map.TryGetValue(id, out var label) ? label : string.Empty;
        }

        return ReadRaw(entity, property);
    }

    private static string ReadRaw(object entity, PropertyInfo property)
    {
        var value = property.GetValue(entity);
        if (value is null)
        {
            return string.Empty;
        }

        if (value is ArticlePrice price)
        {
            return price.Value.ToString("0.##", CultureInfo.CurrentCulture);
        }

        if (value is bool flag)
        {
            return flag ? "Sim" : "Não";
        }

        if (value is DateTime date)
        {
            return date == default ? string.Empty : date.ToString("yyyy-MM-dd HH:mm");
        }

        if (value is decimal number)
        {
            return number.ToString("0.##", CultureInfo.CurrentCulture);
        }

        if (value is Enum)
        {
            return value.ToString() ?? string.Empty;
        }

        return Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty;
    }

    private static string? ApplyFields(Type type, object entity, IReadOnlyDictionary<string, string> fields)
    {
        foreach (var pair in fields)
        {
            if (pair.Key == "Pin")
            {
                continue;
            }

            if (pair.Key == ThermalPaper.FieldKey && entity is Printer printer)
            {
                var columns = ThermalPaper.Columns(pair.Value);
                printer.ThermalMaxCharsPerLineNormal = columns.Normal;
                printer.ThermalMaxCharsPerLineNormalBold = columns.Bold;
                printer.ThermalMaxCharsPerLineSmall = columns.Small;
                continue;
            }

            var property = type.GetProperty(pair.Key);
            if (property is null || property.CanWrite == false)
            {
                continue;
            }

            if (property.PropertyType == typeof(ArticlePrice))
            {
                var price = property.GetValue(entity) as ArticlePrice ?? ArticlePrice.Default();
                if (string.IsNullOrWhiteSpace(pair.Value))
                {
                    price.Value = 0;
                    property.SetValue(entity, price);
                    continue;
                }

                if (decimal.TryParse(pair.Value, NumberStyles.Number, CultureInfo.CurrentCulture, out var amount) == false
                    && decimal.TryParse(pair.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out amount) == false)
                {
                    return $"Valor inválido em {Header(pair.Key)}.";
                }

                price.Value = amount;
                property.SetValue(entity, price);
                continue;
            }

            var converted = ConvertValue(property.PropertyType, pair.Value);
            if (converted.Failed)
            {
                return $"Valor inválido em {Header(pair.Key)}.";
            }

            property.SetValue(entity, converted.Value);
        }

        return null;
    }

    private static void PrepareRequired(Type type, object entity, LogicPOSDbContext database)
    {
        foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.CanWrite == false)
            {
                continue;
            }

            if (property.PropertyType == typeof(Button) && property.GetValue(entity) is null)
            {
                property.SetValue(entity, new Button());
            }

            if (property.PropertyType == typeof(ArticlePrice) && property.GetValue(entity) is null)
            {
                property.SetValue(entity, ArticlePrice.Default());
            }

            if (property.PropertyType == typeof(string) && property.Name != "Code" && IsRequired(property) && string.IsNullOrWhiteSpace(property.GetValue(entity) as string))
            {
                property.SetValue(entity, DefaultString(type, property.Name, entity));
            }
        }

        var code = type.GetProperty("Code");
        if (code is not null && string.IsNullOrWhiteSpace(code.GetValue(entity) as string))
        {
            code.SetValue(entity, NextCode(database, type));
        }
    }

    private static string DefaultString(Type type, string property, object entity)
    {
        if (property is "Code" or "Acronym" or "HardwareId" or "Login" or "Token" or "RefNo")
        {
            return "S" + DateTime.Now.ToString("HHmmssfff");
        }

        if (type == typeof(VatRate))
        {
            return property switch
            {
                "TaxType" => "IVA",
                "TaxCode" => "NOR",
                "CountryRegion" => "PT",
                "Description" => ReadString(entity, "Designation"),
                _ => "SMOKE"
            };
        }

        if (property == "FiscalNumber")
        {
            return "999999990";
        }

        var designation = ReadString(entity, "Designation");
        if (string.IsNullOrWhiteSpace(designation) == false && property is "Description" or "Message")
        {
            return designation;
        }

        return string.IsNullOrWhiteSpace(designation) ? "SMOKE" : designation;
    }

    private static string NextCode(LogicPOSDbContext database, Type type)
    {
        var rows = ((System.Collections.IEnumerable)QueryableSet(database, type)).Cast<object>().ToList();
        var max = 0;
        foreach (var row in rows)
        {
            var code = ReadString(row, "Code");
            if (int.TryParse(code, out var number) && number > max)
            {
                max = number;
            }
        }

        return (max + 1).ToString(CultureInfo.InvariantCulture);
    }

    private async Task<string?> FillForeignKeysAsync(LogicPOSDbContext database, Type type, object entity, CancellationToken cancellationToken)
    {
        foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.PropertyType != typeof(Guid) || property.Name is "Id" or "CreatedBy" or "UpdatedBy" or "CreatedWhere" or "UpdatedWhere")
            {
                continue;
            }

            if (property.GetValue(entity) is Guid current && current != Guid.Empty)
            {
                continue;
            }

            var target = LookupType(type, property.Name);
            if (target is null)
            {
                continue;
            }

            var rows = await MaterializeAsync(database, target, cancellationToken);
            var first = rows.Cast<object>().FirstOrDefault(row => ReadBool(row, "IsDeleted") == false);
            if (first is null)
            {
                return $"Crie primeiro um registo de {Header(target.Name)}.";
            }

            property.SetValue(entity, ReadGuid(first, "Id"));
        }

        return null;
    }

    private void ApplyPin(User user, IReadOnlyDictionary<string, string> fields, bool creating)
    {
        fields.TryGetValue("Pin", out var pin);
        if (string.IsNullOrWhiteSpace(pin))
        {
            if (creating)
            {
                user.AccessPin = _passwords.HashPassword("1234");
                user.PasswordReset = true;
            }

            return;
        }

        user.AccessPin = _passwords.HashPassword(pin);
        user.PasswordReset = false;
    }

    private async Task<ListingSaveResult> FiscalAsync(Func<IFiscalModule, Task<string?>> call, CancellationToken cancellationToken)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var module = scope.ServiceProvider.GetService<IFiscalModule>() ?? new NullFiscalModule();
        var message = await call(module);
        if (string.IsNullOrWhiteSpace(message))
        {
            return ListingSaveResult.Ok(Guid.Empty, "Operação fiscal concluída.");
        }

        return File.Exists(message)
            ? ListingSaveResult.Ok(Guid.Empty, message)
            : ListingSaveResult.Fail(message);
    }

    private Task<ListingSaveResult> ExportSaftRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken)
        => FiscalAsync(module => module.ExportSaftAsync(start, end, cancellationToken), cancellationToken);

    private Task<ListingSaveResult> ExportSaftMonthAsync(CancellationToken cancellationToken)
    {
        var first = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
        return ExportSaftRangeAsync(first, first.AddMonths(1).AddDays(-1), cancellationToken);
    }

    private static ListingSnapshot NoticePage(PageSpec page)
    {
        var label = page.Actions.FirstOrDefault()?.Label ?? page.Title;
        return new ListingSnapshot
        {
            Title = page.Title,
            Columns =
            [
                new ListingColumn { Key = "Operação", Header = "Operação" },
                new ListingColumn { Key = "Estado", Header = "Estado" }
            ],
            Rows =
            [
                new ListingRow
                {
                    Id = Guid.Empty,
                    Values = new Dictionary<string, string>
                    {
                        ["Operação"] = label,
                        ["Estado"] = "Pronta"
                    }
                }
            ],
            Notice = page.Filter
        };
    }

    private static DateTime ParseStart(string? extra)
    {
        var part = (extra ?? string.Empty).Split('|').FirstOrDefault();
        return DateTime.TryParse(part, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date.Date
            : new DateTime(DateTime.Today.Year, 1, 1);
    }

    private static DateTime ParseEnd(string? extra)
    {
        var parts = (extra ?? string.Empty).Split('|');
        var part = parts.Length > 1 ? parts[1] : parts[0];
        return DateTime.TryParse(part, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date.Date
            : DateTime.Today;
    }

    private static async Task AddArticleExtrasAsync(LogicPOSDbContext database, List<ListingField> fields, Article article, Guid articleId, CancellationToken cancellationToken)
    {
        var child = await database.ArticleCompositions.AsNoTracking()
            .Where(item => item.ParentId == articleId && item.IsDeleted == false)
            .Select(item => new { item.ChildId, item.Quantity })
            .FirstOrDefaultAsync(cancellationToken);
        fields.Add(new ListingField
        {
            Key = "ChildArticleId",
            Label = "Artigo associado",
            Kind = "lookup",
            Lookup = "Article",
            Group = "Artigos associados",
            ShowWhen = "IsComposed",
            Value = child is null ? string.Empty : child.ChildId.ToString()
        });
        fields.Add(new ListingField
        {
            Key = "ChildQuantity",
            Label = "Quantidade",
            Kind = "text",
            Group = "Artigos associados",
            ShowWhen = "IsComposed",
            Value = child is null ? "1" : child.Quantity.ToString(CultureInfo.CurrentCulture)
        });
        var serials = await database.WarehouseArticles.AsNoTracking()
            .Where(item => item.ArticleId == articleId && item.IsDeleted == false && item.SerialNumber != null && item.SerialNumber != "")
            .Select(item => item.SerialNumber!)
            .Take(20)
            .ToListAsync(cancellationToken);
        fields.Add(new ListingField
        {
            Key = "SerialNotice",
            Label = "Números de série",
            Kind = "multiline",
            ReadOnly = true,
            Group = "Número de série",
            ShowWhen = "UniqueArticles",
            Value = serials.Count == 0 ? "Ainda não há números de série neste artigo." : string.Join(", ", serials)
        });
        _ = article;
    }

    private static async Task SaveArticleChildrenAsync(LogicPOSDbContext database, Guid articleId, IReadOnlyDictionary<string, string> fields, CancellationToken cancellationToken)
    {
        if (fields.TryGetValue("IsComposed", out var composed) == false || composed is not ("true" or "True" or "1"))
        {
            return;
        }

        if (fields.TryGetValue("ChildArticleId", out var childText) == false || Guid.TryParse(childText, out var childId) == false || childId == Guid.Empty)
        {
            return;
        }

        var quantity = 1m;
        if (fields.TryGetValue("ChildQuantity", out var quantityText))
        {
            if (decimal.TryParse(quantityText, NumberStyles.Number, CultureInfo.CurrentCulture, out var parsed) == false)
            {
                decimal.TryParse(quantityText, NumberStyles.Number, CultureInfo.InvariantCulture, out parsed);
            }

            if (parsed > 0)
            {
                quantity = parsed;
            }
        }

        var existing = await database.ArticleCompositions
            .FirstOrDefaultAsync(item => item.ParentId == articleId && item.IsDeleted == false, cancellationToken);
        if (existing is null)
        {
            database.ArticleCompositions.Add(new ArticleComposition
            {
                ParentId = articleId,
                ChildId = childId,
                Quantity = quantity
            });
            return;
        }

        existing.ChildId = childId;
        existing.Quantity = quantity;
    }

    private static bool CanRemove(object entity)
    {
        if (entity is User user && user.Name is "Proprietário" or "Utilizador")
        {
            return false;
        }

        if (entity is Terminal terminal && terminal.IsDefault)
        {
            return false;
        }

        if (entity is PreferenceParameter)
        {
            return false;
        }

        return true;
    }

    private static bool MatchesFilter(PageSpec page, object row)
    {
        if (page.EntityType != typeof(PreferenceParameter) || row is not PreferenceParameter parameter)
        {
            return true;
        }

        var company = parameter.Token.StartsWith("COMPANY", StringComparison.OrdinalIgnoreCase)
            || parameter.Token.StartsWith("REPORT", StringComparison.OrdinalIgnoreCase);
        return page.Filter == "company" ? company : company == false;
    }

    private async Task<ListingSaveResult> OpenDayAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var dayOpen = await database.WorkSessionPeriods.AnyAsync(
            p => p.IsDeleted == false
                && p.Type == Domain.Enums.WorkSessionPeriodType.Day
                && p.Status == Domain.Enums.WorkSessionPeriodStatus.Open,
            cancellationToken);
        if (dayOpen)
        {
            return ListingSaveResult.Fail("O dia já está aberto.");
        }

        var day = new WorkSessionPeriod
        {
            Type = Domain.Enums.WorkSessionPeriodType.Day,
            Status = Domain.Enums.WorkSessionPeriodStatus.Open,
            Designation = "Dia " + DateTime.Now.ToString("yyyy-MM-dd"),
            StartDate = DateTime.Now
        };
        database.WorkSessionPeriods.Add(day);
        await database.SaveChangesAsync(cancellationToken);
        return ListingSaveResult.Ok(day.Id, "Dia aberto.");
    }

    private async Task<ListingSaveResult> OpenSessionAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var open = await database.WorkSessionPeriods.AnyAsync(period => period.IsDeleted == false && period.Status == Domain.Enums.WorkSessionPeriodStatus.Open, cancellationToken);
        if (open)
        {
            return ListingSaveResult.Fail("Já existe uma sessão aberta.");
        }

        var period = new WorkSessionPeriod
        {
            Type = Domain.Enums.WorkSessionPeriodType.Terminal,
            Status = Domain.Enums.WorkSessionPeriodStatus.Open,
            Designation = "Sessão " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            StartDate = DateTime.Now
        };
        database.WorkSessionPeriods.Add(period);
        await database.SaveChangesAsync(cancellationToken);
        return ListingSaveResult.Ok(period.Id, "Sessão aberta.");
    }

    private async Task<ListingSaveResult> CloseSessionAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var period = await database.WorkSessionPeriods
            .Where(item => item.IsDeleted == false && item.Status == Domain.Enums.WorkSessionPeriodStatus.Open)
            .OrderByDescending(item => item.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
        if (period is null)
        {
            return ListingSaveResult.Fail("Não existe uma sessão aberta.");
        }

        period.Status = Domain.Enums.WorkSessionPeriodStatus.Closed;
        period.EndDate = DateTime.Now;
        period.UpdatedAt = DateTime.Now;
        await database.SaveChangesAsync(cancellationToken);
        return ListingSaveResult.Ok(period.Id, "Sessão fechada.");
    }

    private async Task<ListingSaveResult> CloseDayAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var terminalOpen = await database.WorkSessionPeriods.AnyAsync(
            item => item.IsDeleted == false
                && item.Type == Domain.Enums.WorkSessionPeriodType.Terminal
                && item.Status == Domain.Enums.WorkSessionPeriodStatus.Open,
            cancellationToken);
        if (terminalOpen)
        {
            return ListingSaveResult.Fail("Feche a sessão do terminal antes de fechar o dia.");
        }

        var day = await database.WorkSessionPeriods.FirstOrDefaultAsync(
            item => item.IsDeleted == false
                && item.Type == Domain.Enums.WorkSessionPeriodType.Day
                && item.Status == Domain.Enums.WorkSessionPeriodStatus.Open,
            cancellationToken);
        if (day is null)
        {
            return ListingSaveResult.Fail("Não existe um dia aberto.");
        }

        day.Status = Domain.Enums.WorkSessionPeriodStatus.Closed;
        day.EndDate = DateTime.Now;
        day.UpdatedAt = DateTime.Now;
        await database.SaveChangesAsync(cancellationToken);
        return ListingSaveResult.Ok(day.Id, "Dia fechado.");
    }

    private async Task<ListingSaveResult> CashTotalAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var period = await database.WorkSessionPeriods
            .Where(item => item.IsDeleted == false
                && item.Type == Domain.Enums.WorkSessionPeriodType.Terminal
                && item.Status == Domain.Enums.WorkSessionPeriodStatus.Open)
            .OrderByDescending(item => item.StartDate)
            .FirstOrDefaultAsync(cancellationToken);

        decimal total = 0;
        if (period is not null)
        {
            var movements = await database.WorkSessionMovements
                .Where(item => item.PeriodId == period.Id)
                .ToListAsync(cancellationToken);
            foreach (var movement in movements)
            {
                total += movement.Type switch
                {
                    Domain.Entities.POS.WorkSessions.Movements.Common.WorkSessionMovementType.CashDrawerOut => -Math.Abs(movement.Amount),
                    Domain.Entities.POS.WorkSessions.Movements.Common.WorkSessionMovementType.CashDrawerMoneyOut => -Math.Abs(movement.Amount),
                    _ => movement.Amount
                };
            }
        }

        return ListingSaveResult.Ok(Guid.Empty, total.ToString("0.00", CultureInfo.InvariantCulture));
    }

    private async Task<ListingSaveResult> CashAsync(bool incoming, string? extra, CancellationToken cancellationToken)
    {
        var amountText = extra;
        string? description = null;
        var split = extra?.IndexOf('|') ?? -1;
        if (split >= 0 && extra is not null)
        {
            amountText = extra[..split];
            description = extra[(split + 1)..].Trim();
        }

        if (decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.CurrentCulture, out var amount) == false
            || amount <= 0)
        {
            return ListingSaveResult.Fail("Indique um valor maior que zero.");
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var period = await database.WorkSessionPeriods
            .Where(item => item.IsDeleted == false && item.Status == Domain.Enums.WorkSessionPeriodStatus.Open)
            .OrderByDescending(item => item.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
        if (period is null)
        {
            return ListingSaveResult.Fail("Abra uma sessão antes de movimentar a caixa.");
        }

        var movement = new WorkSessionMovement
        {
            PeriodId = period.Id,
            Amount = amount,
            Type = incoming
                ? Domain.Entities.POS.WorkSessions.Movements.Common.WorkSessionMovementType.CashDrawerIn
                : Domain.Entities.POS.WorkSessions.Movements.Common.WorkSessionMovementType.CashDrawerOut,
            Notes = string.IsNullOrWhiteSpace(description)
                ? incoming ? "Entrada de caixa" : "Saída de caixa"
                : description
        };
        database.WorkSessionMovements.Add(movement);
        await database.SaveChangesAsync(cancellationToken);
        return ListingSaveResult.Ok(movement.Id, incoming ? "Entrada registada." : "Saída registada.");
    }

    private async Task<ListingSaveResult> AdjustStockAsync(Guid? id, string? extra, int sign, CancellationToken cancellationToken)
    {
        if (id is null || id == Guid.Empty)
        {
            return ListingSaveResult.Fail("Selecione uma linha de stock.");
        }

        if (decimal.TryParse(extra, NumberStyles.Number, CultureInfo.CurrentCulture, out var amount) == false || amount <= 0)
        {
            amount = 1;
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var stock = await database.WarehouseArticles.FirstOrDefaultAsync(item => item.Id == id && item.IsDeleted == false, cancellationToken);
        if (stock is null)
        {
            return ListingSaveResult.Fail("Linha de stock não encontrada.");
        }

        var delta = sign * amount;
        stock.Quantity += delta;
        stock.UpdatedAt = DateTime.Now;
        database.StockMovements.Add(new StockMovement
        {
            ArticleId = stock.ArticleId,
            WarehouseLocationId = stock.WarehouseLocationId,
            Quantity = delta,
            Date = DateTime.Now,
            DocumentNumber = "STOCK",
            Notes = delta > 0 ? "Entrada" : "Saída"
        });
        await database.SaveChangesAsync(cancellationToken);
        return ListingSaveResult.Ok(stock.Id, "Stock atualizado.");
    }

    private async Task<ListingSaveResult> EmitReceiptAsync(Guid? documentId, CancellationToken cancellationToken)
    {
        if (documentId is null || documentId == Guid.Empty)
        {
            return ListingSaveResult.Fail("Selecione um documento por liquidar.");
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var document = await database.Documents.FirstOrDefaultAsync(item => item.Id == documentId && item.IsDeleted == false, cancellationToken);
        if (document is null)
        {
            return ListingSaveResult.Fail("Documento não encontrado.");
        }

        if (document.Paid)
        {
            return ListingSaveResult.Fail("O documento já está pago.");
        }

        var method = await database.PaymentMethods.FirstOrDefaultAsync(item => item.IsDeleted == false, cancellationToken);
        if (method is null)
        {
            return ListingSaveResult.Fail("Crie primeiro um método de pagamento.");
        }

        document.Paid = true;
        document.PaymentDate = DateTime.Now;
        document.UpdatedAt = DateTime.Now;
        var receipt = new Receipt
        {
            PaymentMethodId = method.Id,
            DocumentType = "RG",
            RefNo = "RG " + DateTime.Now.ToString("yyyyMMddHHmmss"),
            Status = "N",
            SourcePayment = "P",
            Mechanism = "NU",
            Amount = document.TotalFinal,
            AmountInWords = document.TotalFinal.ToString("0.00", CultureInfo.CurrentCulture),
            SourceId = document.Number,
            CurrencyCode = "EUR",
            CurrencyAmount = document.TotalFinal,
            ExchangeRate = 1,
            Notes = document.Id.ToString()
        };
        database.Receipts.Add(receipt);
        await database.SaveChangesAsync(cancellationToken);
        return ListingSaveResult.Ok(receipt.Id, "Recibo emitido.");
    }

    public Task<IReadOnlyCollection<string>?> AllowedMenuTitlesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyCollection<string>?>(null);

    public async Task<ListingSaveResult> UndoReceiptAsync(Guid receiptId, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var receipt = await database.Receipts.FirstOrDefaultAsync(item => item.Id == receiptId, cancellationToken);
        if (receipt is null)
        {
            return ListingSaveResult.Fail("Recibo não encontrado.");
        }

        if (Guid.TryParse(receipt.Notes, out var documentId))
        {
            var document = await database.Documents.FirstOrDefaultAsync(item => item.Id == documentId, cancellationToken);
            if (document is not null)
            {
                document.Paid = false;
                document.PaymentDate = null;
            }
        }

        receipt.IsDeleted = true;
        receipt.DeletedAt = DateTime.Now;
        await database.SaveChangesAsync(cancellationToken);
        return ListingSaveResult.Ok(receiptId, "Recibo anulado.");
    }

    private async Task<ListingSaveResult> ViewReceiptAsync(Guid receiptId, CancellationToken cancellationToken)
    {
        if (receiptId == Guid.Empty)
        {
            return ListingSaveResult.Fail("Selecione o recibo.");
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var documents = scope.ServiceProvider.GetRequiredService<IPosDocumentService>();
        var documentId = receiptId;
        var receipt = await database.Receipts.AsNoTracking().FirstOrDefaultAsync(item => item.Id == receiptId, cancellationToken);
        if (receipt is not null)
        {
            var match = await database.Documents.AsNoTracking()
                .FirstOrDefaultAsync(item => item.IsDeleted == false && item.Number == receipt.RefNo, cancellationToken);
            if (match is null)
            {
                return ListingSaveResult.Fail("Não foi possível abrir o recibo.");
            }

            documentId = match.Id;
        }

        var path = await documents.CreateA4FileAsync(documentId, cancellationToken);
        return string.IsNullOrWhiteSpace(path)
            ? ListingSaveResult.Fail("Não foi possível abrir o recibo.")
            : ListingSaveResult.Ok(receiptId, path);
    }

    private async Task<ListingSaveResult> BackupAsync(CancellationToken cancellationToken)
    {
        var sourcePath = await DatabasePathAsync(cancellationToken);
        var folder = Folder("backups");
        Directory.CreateDirectory(folder);
        var target = Path.Combine(folder, "logicpos-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".db");
        await using var source = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = sourcePath, Mode = SqliteOpenMode.ReadOnly }.ToString());
        await using var destination = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = target }.ToString());
        await source.OpenAsync(cancellationToken);
        await destination.OpenAsync(cancellationToken);
        source.BackupDatabase(destination);
        return ListingSaveResult.Ok(Guid.Empty, "Backup criado em " + target);
    }

    private async Task<ListingSaveResult> RestoreAsync(string? path, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(path) || File.Exists(path) == false)
        {
            return ListingSaveResult.Fail("Selecione um ficheiro de backup.");
        }

        if (string.Equals(Path.GetFileName(path), "RESTAURAR", StringComparison.Ordinal) == false && path.EndsWith(".db", StringComparison.OrdinalIgnoreCase) == false)
        {
            return ListingSaveResult.Fail("O ficheiro de backup não é válido.");
        }

        var livePath = await DatabasePathAsync(cancellationToken);
        await using var source = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = path, Mode = SqliteOpenMode.ReadOnly }.ToString());
        await using var destination = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = livePath }.ToString());
        await source.OpenAsync(cancellationToken);
        await destination.OpenAsync(cancellationToken);
        source.BackupDatabase(destination);
        return ListingSaveResult.Ok(Guid.Empty, "Base restaurada. Reinicie a aplicação.");
    }

    private async Task<ListingSaveResult> ExportArticlesAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var articles = await database.Articles.AsNoTracking().Where(item => item.IsDeleted == false).OrderBy(item => item.Code).ToListAsync(cancellationToken);
        var lines = new List<string> { "Code;Designation;Barcode;Price" };
        lines.AddRange(articles.Select(item => string.Join(';', Csv(item.Code), Csv(item.Designation), Csv(item.Barcode), item.Price1.Value.ToString(CultureInfo.InvariantCulture))));
        return WriteExport("artigos.csv", lines);
    }

    private async Task<ListingSaveResult> ExportCustomersAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var customers = await database.Customers.AsNoTracking().Where(item => item.IsDeleted == false).OrderBy(item => item.Code).ToListAsync(cancellationToken);
        var lines = new List<string> { "Code;Name;FiscalNumber;Email" };
        lines.AddRange(customers.Select(item => string.Join(';', Csv(item.Code), Csv(item.Name), Csv(item.FiscalNumber), Csv(item.Email))));
        return WriteExport("clientes.csv", lines);
    }

    private async Task<ListingSaveResult> ImportArticlesAsync(string? csv, CancellationToken cancellationToken)
    {
        var table = ParseCsv(csv);
        if (table.Count == 0)
        {
            return ListingSaveResult.Fail("O ficheiro não tem linhas.");
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var ids = new List<Guid>();
        foreach (var row in table)
        {
            var code = Cell(row, "Code");
            var designation = Cell(row, "Designation");
            if (string.IsNullOrWhiteSpace(designation))
            {
                continue;
            }

            var article = await database.Articles.FirstOrDefaultAsync(item => item.IsDeleted == false && item.Code == code, cancellationToken);
            var creating = article is null;
            if (creating)
            {
                var fields = new Dictionary<string, string>
                {
                    ["Designation"] = designation,
                    ["Barcode"] = Cell(row, "Barcode"),
                    ["Price1"] = Cell(row, "Price")
                };
                if (string.IsNullOrWhiteSpace(code) == false)
                {
                    fields["Code"] = code;
                }

                var saved = await SaveAsync("Artigos", null, fields, cancellationToken);
                if (saved.Succeeded == false)
                {
                    return saved;
                }

                ids.Add(saved.Id);
                continue;
            }

            article!.Designation = designation;
            if (decimal.TryParse(Cell(row, "Price"), NumberStyles.Number, CultureInfo.InvariantCulture, out var price))
            {
                article.Price1 ??= ArticlePrice.Default();
                article.Price1.Value = price;
            }

            article.Barcode = Cell(row, "Barcode");
            article.UpdatedAt = DateTime.Now;
            ids.Add(article.Id);
        }

        await database.SaveChangesAsync(cancellationToken);
        return ListingSaveResult.OkMany(ids, "Importação de artigos concluída.");
    }

    private async Task<ListingSaveResult> ImportCustomersAsync(string? csv, CancellationToken cancellationToken)
    {
        var table = ParseCsv(csv);
        if (table.Count == 0)
        {
            return ListingSaveResult.Fail("O ficheiro não tem linhas.");
        }

        var ids = new List<Guid>();
        foreach (var row in table)
        {
            var name = Cell(row, "Name");
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            var fields = new Dictionary<string, string>
            {
                ["Name"] = name,
                ["FiscalNumber"] = string.IsNullOrWhiteSpace(Cell(row, "FiscalNumber")) ? "999999990" : Cell(row, "FiscalNumber"),
                ["Email"] = Cell(row, "Email")
            };
            var code = Cell(row, "Code");
            if (string.IsNullOrWhiteSpace(code) == false)
            {
                fields["Code"] = code;
            }

            var saved = await SaveAsync("Clientes", null, fields, cancellationToken);
            if (saved.Succeeded == false)
            {
                return saved;
            }

            ids.Add(saved.Id);
        }

        return ListingSaveResult.OkMany(ids, "Importação de clientes concluída.");
    }

    private static ListingSaveResult WriteExport(string name, IEnumerable<string> lines)
    {
        var folder = Folder("exports");
        Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + name);
        File.WriteAllLines(path, lines, new UTF8Encoding(true));
        return ListingSaveResult.Ok(Guid.Empty, "Exportado para " + path);
    }

    private static List<Dictionary<string, string>> ParseCsv(string? csv)
    {
        if (string.IsNullOrWhiteSpace(csv))
        {
            return [];
        }

        var lines = csv.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
        {
            return [];
        }

        var headers = lines[0].Split(';');
        var rows = new List<Dictionary<string, string>>();
        for (var index = 1; index < lines.Length; index++)
        {
            var cells = lines[index].Split(';');
            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (var column = 0; column < headers.Length && column < cells.Length; column++)
            {
                row[headers[column].Trim()] = cells[column].Trim();
            }

            rows.Add(row);
        }

        return rows;
    }

    private static string Cell(Dictionary<string, string> row, string key) => row.TryGetValue(key, out var value) ? value : string.Empty;

    private static string Csv(string? value) => (value ?? string.Empty).Replace(';', ',');

    private async Task<string> DatabasePathAsync(CancellationToken cancellationToken)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var builder = new SqliteConnectionStringBuilder(database.Database.GetConnectionString());
        var path = builder.DataSource;
        if (Path.IsPathRooted(path) == false)
        {
            path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));
        }

        cancellationToken.ThrowIfCancellationRequested();
        return path;
    }

    private static string Folder(string name) => Path.Combine(AppContext.BaseDirectory, name);

    private static IQueryable QueryableSet(DbContext database, Type type)
    {
        var method = typeof(DbContext).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(item => item.Name == nameof(DbContext.Set) && item.IsGenericMethodDefinition && item.GetParameters().Length == 0);
        return (IQueryable)method.MakeGenericMethod(type).Invoke(database, null)!;
    }

    private static async Task<object?> FindEntityAsync(DbContext database, Type type, Guid id, CancellationToken cancellationToken)
    {
        IQueryable query = QueryableSet(database, type);
        var parameter = System.Linq.Expressions.Expression.Parameter(type, "row");
        var body = System.Linq.Expressions.Expression.Equal(
            System.Linq.Expressions.Expression.Property(parameter, "Id"),
            System.Linq.Expressions.Expression.Constant(id));
        var lambda = System.Linq.Expressions.Expression.Lambda(body, parameter);
        var where = typeof(Queryable).GetMethods().First(method => method.Name == "Where" && method.GetParameters().Length == 2);
        query = (IQueryable)where.MakeGenericMethod(type).Invoke(null, [query, lambda])!;
        var first = typeof(EntityFrameworkQueryableExtensions).GetMethods().First(method =>
            method.Name == "FirstOrDefaultAsync"
            && method.GetParameters().Length == 2
            && method.GetParameters()[1].ParameterType == typeof(CancellationToken));
        var task = (Task)first.MakeGenericMethod(type).Invoke(null, [query, cancellationToken])!;
        await task;
        return task.GetType().GetProperty("Result")!.GetValue(task);
    }

    private static async Task<System.Collections.IList> MaterializeAsync(LogicPOSDbContext database, Type type, CancellationToken cancellationToken)
    {
        IQueryable query = QueryableSet(database, type);
        var parameter = System.Linq.Expressions.Expression.Parameter(type, "row");
        var deleted = System.Linq.Expressions.Expression.Property(parameter, "IsDeleted");
        var body = System.Linq.Expressions.Expression.Equal(deleted, System.Linq.Expressions.Expression.Constant(false));
        var lambda = System.Linq.Expressions.Expression.Lambda(body, parameter);
        var where = typeof(Queryable).GetMethods().First(method => method.Name == "Where" && method.GetParameters().Length == 2);
        query = (IQueryable)where.MakeGenericMethod(type).Invoke(null, [query, lambda])!;
        var toList = typeof(EntityFrameworkQueryableExtensions).GetMethods()
            .First(method => method.Name == "ToListAsync"
                && method.GetParameters().Length == 2
                && method.GetParameters()[1].ParameterType == typeof(CancellationToken));
        var task = (Task)toList.MakeGenericMethod(type).Invoke(null, [query, cancellationToken])!;
        await task;
        return (System.Collections.IList)task.GetType().GetProperty("Result")!.GetValue(task)!;
    }

    private static Type? LookupType(Type entity, string propertyName)
    {
        if (propertyName.EndsWith("Id", StringComparison.Ordinal) == false)
        {
            return null;
        }

        var navigationName = propertyName[..^2];
        if (navigationName is "Created" or "Updated" or "Parent" or "TargetUser" or "TargetTerminal" or "LastReadUser" or "LastReadTerminal" or "Document" or "Detail" or "Payment")
        {
            return null;
        }

        var navigation = entity.GetProperty(navigationName);
        if (navigation is not null)
        {
            var navigationType = Nullable.GetUnderlyingType(navigation.PropertyType) ?? navigation.PropertyType;
            if (navigationType.IsClass && navigationType != typeof(string) && ResolveEntityType(navigationType.Name) == navigationType)
            {
                return navigationType;
            }
        }

        return ResolveEntityType(navigationName);
    }

    private static Type? ResolveEntityType(string name)
    {
        return typeof(Article).Assembly.GetTypes().FirstOrDefault(type => type.Name == name && type.IsClass && type.Namespace == "LogicPOS.Domain.Entities");
    }

    private static string? KindOf(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;
        if (underlying == typeof(ArticlePrice))
        {
            return "number";
        }

        if (underlying == typeof(string))
        {
            return "text";
        }

        if (underlying == typeof(bool))
        {
            return "bool";
        }

        if (underlying == typeof(DateTime))
        {
            return "date";
        }

        if (underlying == typeof(Guid))
        {
            return "guid";
        }

        if (underlying.IsEnum)
        {
            return "enum";
        }

        if (underlying == typeof(decimal) || underlying == typeof(double) || underlying == typeof(float)
            || underlying == typeof(int) || underlying == typeof(long) || underlying == typeof(short)
            || underlying == typeof(uint) || underlying == typeof(byte))
        {
            return "number";
        }

        return null!;
    }

    private static (bool Failed, object? Value) ConvertValue(Type type, string raw)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;
        var nullable = Nullable.GetUnderlyingType(type) is not null || type == typeof(string);
        if (string.IsNullOrWhiteSpace(raw))
        {
            if (underlying == typeof(string))
            {
                return (false, string.Empty);
            }

            if (nullable || underlying.IsClass)
            {
                return (false, null);
            }

            if (underlying == typeof(bool))
            {
                return (false, false);
            }

            if (underlying == typeof(Guid))
            {
                return (false, Guid.Empty);
            }

            if (underlying == typeof(DateTime))
            {
                return (false, DateTime.Now);
            }

            if (underlying.IsEnum)
            {
                return (false, Enum.ToObject(underlying, 0));
            }

            return (false, Convert.ChangeType(0, underlying, CultureInfo.CurrentCulture));
        }

        if (underlying == typeof(string))
        {
            return (false, raw);
        }

        if (underlying == typeof(bool))
        {
            return (false, raw is "true" or "True" or "1");
        }

        if (underlying == typeof(Guid))
        {
            return Guid.TryParse(raw, out var id) ? (false, id) : (true, null);
        }

        if (underlying == typeof(DateTime))
        {
            if (DateTime.TryParse(raw, CultureInfo.CurrentCulture, DateTimeStyles.None, out var date)
                || DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return (false, date);
            }

            return (true, null);
        }

        if (underlying.IsEnum)
        {
            return Enum.TryParse(underlying, raw, true, out var parsed) ? (false, parsed) : (true, null);
        }

        try
        {
            if (decimal.TryParse(raw, NumberStyles.Number, CultureInfo.CurrentCulture, out var number) == false
                && decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out number) == false)
            {
                return (true, null);
            }

            return (false, Convert.ChangeType(number, underlying, CultureInfo.CurrentCulture));
        }
        catch
        {
            return (true, null);
        }
    }

    private static bool IsRequired(PropertyInfo property)
    {
        if (Nullable.GetUnderlyingType(property.PropertyType) is not null)
        {
            return false;
        }

        if (property.PropertyType.IsValueType == false && property.PropertyType != typeof(string))
        {
            return false;
        }

        var context = new NullabilityInfoContext();
        return context.Create(property).WriteState != NullabilityState.Nullable;
    }

    private static string Header(string name) => Headers.TryGetValue(name, out var header) ? header : name;

    private static string DisplayLabel(object row)
    {
        foreach (var name in new[] { "Designation", "Name", "Token", "Acronym", "Code", "Message" })
        {
            var value = ReadString(row, name);
            if (string.IsNullOrWhiteSpace(value) == false)
            {
                return value;
            }
        }

        return ReadGuid(row, "Id").ToString();
    }

    private static Guid ReadId(object row) => ReadGuid(row, "Id");

    private static Guid ReadGuid(object row, string name)
    {
        var value = row.GetType().GetProperty(name)?.GetValue(row);
        return value is Guid id ? id : Guid.Empty;
    }

    private static bool ReadBool(object row, string name)
    {
        var value = row.GetType().GetProperty(name)?.GetValue(row);
        return value is bool flag && flag;
    }

    private static DateTime ReadDate(object row, string name)
    {
        var value = row.GetType().GetProperty(name)?.GetValue(row);
        return value is DateTime date ? date : DateTime.MinValue;
    }

    private static string ReadString(object row, string name) => row.GetType().GetProperty(name)?.GetValue(row) as string ?? string.Empty;

    private static decimal ParseDecimal(string value)
        => decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out var number) ? number : 0;

    private static string DescribeException(Exception exception)
    {
        var current = exception;
        while (current.InnerException is not null)
        {
            current = current.InnerException;
        }

        return current.Message;
    }

    private PageSpec? Find(string title) => _pages.FirstOrDefault(page => page.Title == title);

    private static PageSpec[] BuildPages()
    {
        PageSpec Entity(string title, Type type, bool create = true, bool edit = true, bool delete = true, string? filter = null, ListingAction[]? actions = null)
            => new(title, PageKind.Entity, type, create, edit, delete, filter, actions ?? [], []);

        return
        [
            Entity("Famílias", typeof(ArticleFamily)),
            Entity("Subfamílias", typeof(ArticleSubfamily)),
            Entity("Artigos", typeof(Article)),
            Entity("Tipo de artigos", typeof(ArticleType)),
            Entity("Classe do artigo", typeof(ArticleClass)),
            Entity("Tipo de Preço", typeof(PriceType)),
            Entity("Gestão de Stocks", typeof(WarehouseArticle), actions:
            [
                new ListingAction { Key = "stock-in", Label = "Entrada" },
                new ListingAction { Key = "stock-out", Label = "Saída" }
            ]),
            Entity("Abertura de ano fiscal", typeof(FiscalYear)),
            Entity("Séries", typeof(DocumentSeries), actions:
            [
                new ListingAction { Key = "export-saft", Label = "SAFT" },
                new ListingAction { Key = "register-at", Label = "Registar série" },
                new ListingAction { Key = "request-agt", Label = "Série AGT" }
            ]),
            Entity("Tipo de documento", typeof(DocumentType)),
            Entity("Taxas de imposto", typeof(VatRate)),
            Entity("Motivo de isenção de IVA", typeof(VatExemptionReason)),
            Entity("Cond. de Pagamento", typeof(PaymentCondition)),
            Entity("Métodos de pagamento", typeof(PaymentMethod)),
            Entity("Clientes", typeof(Customer)),
            Entity("Tipo de clientes", typeof(CustomerType)),
            Entity("Grupo de descontos", typeof(DiscountGroup)),
            Entity("Utilizadores", typeof(User)),
            Entity("Permissões", typeof(UserProfile)),
            Entity("Grupo de comissões", typeof(CommissionGroup)),
            Entity("Tipos de impressora", typeof(PrinterType)),
            Entity("Impressoras", typeof(Printer)),
            Entity("Dispositivos de Entrada", typeof(InputReader)),
            Entity("Display de Cliente", typeof(PoleDisplay)),
            Entity("Balanças", typeof(WeighingMachine)),
            Entity("País", typeof(Country)),
            Entity("Moeda", typeof(Currency)),
            Entity("Locais", typeof(Place)),
            Entity("Mesas", typeof(Table)),
            Entity("Tipo de Movimento", typeof(MovementType)),
            Entity("Unidades de medida", typeof(MeasurementUnit)),
            Entity("Unidades de tamanho", typeof(SizeUnit)),
            Entity("Feriados", typeof(Holiday)),
            Entity("Armazém", typeof(Warehouse)),
            Entity("Parâmetros da Empresa", typeof(PreferenceParameter), create: false, delete: false, filter: "company"),
            Entity("Parâmetros de Sistema", typeof(PreferenceParameter), create: false, delete: false, filter: "system"),
            Entity("Terminais", typeof(Terminal)),
            Entity("Recibos", typeof(Receipt), create: false, edit: false, actions:
            [
                new ListingAction { Key = "undo-receipt", Label = "Anular" }
            ]),
            new("Emissão Recibos", PageKind.ReceiptEmission, null, false, false, false, null, [new ListingAction { Key = "emit-receipt", Label = "Emitir" }], []),
            new("Conta.Corr.", PageKind.CurrentAccount, null, false, false, false, null, [], []),
            new("Relatórios", PageKind.Reports, null, false, false, false, null, [], ["Vendas por dia", "Vendas por cliente", "Vendas por artigo"]),
            Entity("Notificações", typeof(SystemNotification)),
            Entity("Registro de alterações (Changelog)", typeof(SystemAudit), create: false, edit: false, delete: false),
            new("Backup DB", PageKind.Files, null, false, false, false, "backups", [new ListingAction { Key = "backup", Label = "Criar backup" }], []),
            new("Restaurar DB", PageKind.Files, null, false, false, false, "backups", [new ListingAction { Key = "restore", Label = "Restaurar" }], []),
            new("Importar Artigos", PageKind.Files, null, false, false, false, "exports", [new ListingAction { Key = "import-articles", Label = "Importar" }], []),
            new("Importar Clientes", PageKind.Files, null, false, false, false, "exports", [new ListingAction { Key = "import-customers", Label = "Importar" }], []),
            new("Exportar Artigos", PageKind.Files, null, false, false, false, "exports", [new ListingAction { Key = "export-articles", Label = "Exportar" }], []),
            new("Exportar Clientes", PageKind.Files, null, false, false, false, "exports", [new ListingAction { Key = "export-customers", Label = "Exportar" }], []),
            new("SAF-T ano", PageKind.Notice, null, false, false, false, "Exporta o SAF-T do ano corrente.", [new ListingAction { Key = "saft-year", Label = "Exportar ano" }], []),
            new("SAF-T último mês", PageKind.Notice, null, false, false, false, "Exporta o SAF-T do mês anterior.", [new ListingAction { Key = "saft-month", Label = "Exportar mês" }], []),
            new("SAF-T período", PageKind.Notice, null, false, false, false, "Use as datas e exporte o SAF-T desse intervalo.", [new ListingAction { Key = "saft-period", Label = "Exportar período" }], []),
            new("Documentos AGT", PageKind.Notice, null, false, false, false, "A consulta online de documentos AGT está disponível no modo cloud.", [new ListingAction { Key = "agt-consult", Label = "Consultar" }], []),
            new("Séries AGT", PageKind.Notice, null, false, false, false, "A consulta online de séries AGT está disponível no modo cloud.", [], []),
            Entity("Sessões de Trab.", typeof(WorkSessionPeriod), actions:
            [
                new ListingAction { Key = "open-session", Label = "Abrir sessão" },
                new ListingAction { Key = "close-session", Label = "Fechar sessão" },
                new ListingAction { Key = "cash-in", Label = "Entrada caixa" },
                new ListingAction { Key = "cash-out", Label = "Saída caixa" }
            ])
        ];
    }

    private sealed record PageSpec(
        string Title,
        PageKind Kind,
        Type? EntityType,
        bool CanCreate,
        bool CanEdit,
        bool CanDelete,
        string? Filter,
        ListingAction[] Actions,
        string[] Modes);

    private enum PageKind
    {
        Entity,
        Reports,
        CurrentAccount,
        ReceiptEmission,
        Files,
        Notice
    }
}
