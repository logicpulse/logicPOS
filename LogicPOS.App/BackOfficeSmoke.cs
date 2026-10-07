using System.Globalization;
using System.Text;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App;

internal static class BackOfficeSmoke
{
    public static async Task<string> RunAsync()
    {
        var services = AppComposition.Services;
        if (services is null)
        {
            return "FAIL startup: " + (AppComposition.StartupError ?? "sem serviços");
        }

        var listing = services.GetRequiredService<IBackOfficeListingService>();
        var log = new StringBuilder();
        var stamp = DateTime.Now.ToString("HHmmss");
        var failures = 0;

        foreach (var title in listing.PageTitles())
        {
            try
            {
                var snapshot = await listing.QueryAsync(title);
                log.AppendLine($"OK  {title}: {snapshot.Rows.Count} linhas");
                if (snapshot.CanCreate == false)
                {
                    if (title.StartsWith("Parâmetros", StringComparison.Ordinal) && snapshot.Rows.Count > 0)
                    {
                        failures += await RoundTripParameterAsync(listing, log, title, snapshot.Rows[0].Id) ? 0 : 1;
                    }

                    continue;
                }

                if (await RoundTripAsync(listing, log, title, stamp) == false)
                {
                    failures++;
                }
            }
            catch (Exception exception)
            {
                failures++;
                log.AppendLine($"FAIL {title}: {exception.Message}");
            }
        }

        failures += await RunSpecialsAsync(listing, log, stamp) ? 0 : 1;
        log.Insert(0, failures == 0 ? "SMOKE OK" + Environment.NewLine : $"SMOKE FAIL {failures}" + Environment.NewLine);
        return log.ToString();
    }

    private static async Task<bool> RoundTripAsync(IBackOfficeListingService listing, StringBuilder log, string title, string stamp)
    {
        var fields = await listing.LoadFieldsAsync(title, null);
        var values = fields.ToDictionary(field => field.Key, field => field.Value);
        var label = $"SMOKE {title} {stamp}";
        Set(values, "Designation", label);
        Set(values, "Name", label);
        Set(values, "Message", label);
        Set(values, "Pin", "1234");
        Set(values, "FiscalNumber", "999999990");
        if (values.ContainsKey("Year"))
        {
            values["Year"] = (4000 + Random.Shared.Next(1000, 9000)).ToString(CultureInfo.InvariantCulture);
        }

        var created = await listing.SaveAsync(title, null, values);
        if (created.Succeeded == false)
        {
            log.AppendLine($"FAIL {title} criar: {created.Error}");
            return false;
        }

        Set(values, "Designation", label + " edit");
        Set(values, "Name", label + " edit");
        Set(values, "Message", label + " edit");
        var updated = await listing.SaveAsync(title, created.Id, values);
        if (updated.Succeeded == false)
        {
            log.AppendLine($"FAIL {title} alterar: {updated.Error}");
            await listing.DeleteAsync(title, created.Id);
            return false;
        }

        var deleted = await listing.DeleteAsync(title, created.Id);
        if (deleted.Succeeded == false)
        {
            log.AppendLine($"FAIL {title} eliminar: {deleted.Error}");
            return false;
        }

        var after = await listing.QueryAsync(title, label);
        if (after.Rows.Any(row => row.Id == created.Id))
        {
            log.AppendLine($"FAIL {title} ainda visível depois de eliminar");
            return false;
        }

        log.AppendLine($"OK  {title}: criar, alterar, eliminar");
        return true;
    }

    private static async Task<bool> RoundTripParameterAsync(IBackOfficeListingService listing, StringBuilder log, string title, Guid id)
    {
        var fields = await listing.LoadFieldsAsync(title, id);
        var values = fields.ToDictionary(field => field.Key, field => field.Value);
        var original = values.TryGetValue("Value", out var current) ? current : string.Empty;
        values["Value"] = original + "|smoke";
        var saved = await listing.SaveAsync(title, id, values);
        values["Value"] = original;
        var restored = await listing.SaveAsync(title, id, values);
        if (saved.Succeeded == false || restored.Succeeded == false)
        {
            log.AppendLine($"FAIL {title} alterar: {saved.Error ?? restored.Error}");
            return false;
        }

        log.AppendLine($"OK  {title}: alterar e repor");
        return true;
    }

    private static async Task<bool> RunSpecialsAsync(IBackOfficeListingService listing, StringBuilder log, string stamp)
    {
        var ok = true;
        ok &= await Step(log, "Relatórios", async () =>
        {
            foreach (var mode in new[] { "Vendas por dia", "Vendas por cliente", "Vendas por artigo" })
            {
                var report = await listing.QueryAsync("Relatórios", mode: mode);
                if (report.Columns.Count == 0)
                {
                    return "sem colunas em " + mode;
                }
            }

            return null;
        });

        ok &= await Step(log, "Conta corrente", async () =>
        {
            await listing.QueryAsync("Conta.Corr.");
            return null;
        });

        ok &= await Step(log, "Sessão", async () =>
        {
            var opened = await listing.RunActionAsync("open-session", null, null);
            if (opened.Succeeded == false)
            {
                return opened.Error;
            }

            var cashIn = await listing.RunActionAsync("cash-in", null, "1,50");
            var cashOut = await listing.RunActionAsync("cash-out", null, "0,50");
            var closed = await listing.RunActionAsync("close-session", null, null);
            await listing.DeleteAsync("Sessões de Trab.", opened.Id);
            return FirstError(cashIn, cashOut, closed);
        });

        ok &= await Step(log, "Stock", async () =>
        {
            var stock = await listing.QueryAsync("Gestão de Stocks");
            var row = stock.Rows.FirstOrDefault();
            Guid? createdId = null;
            if (row is null)
            {
                var fields = await listing.LoadFieldsAsync("Gestão de Stocks", null);
                var values = fields.ToDictionary(field => field.Key, field => field.Value);
                if (values.ContainsKey("Quantity"))
                {
                    values["Quantity"] = "1";
                }

                var created = await listing.SaveAsync("Gestão de Stocks", null, values);
                if (created.Succeeded == false)
                {
                    return created.Error;
                }

                createdId = created.Id;
                row = new ListingRow { Id = created.Id };
            }

            var inbound = await listing.RunActionAsync("stock-in", row.Id, "1");
            var outbound = await listing.RunActionAsync("stock-out", row.Id, "1");
            if (createdId is Guid id)
            {
                await listing.DeleteAsync("Gestão de Stocks", id);
            }

            return FirstError(inbound, outbound);
        });

        ok &= await Step(log, "Recibo", async () =>
        {
            var open = await listing.QueryAsync("Emissão Recibos");
            var document = open.Rows.FirstOrDefault();
            if (document is null)
            {
                return null;
            }

            var emitted = await listing.RunActionAsync("emit-receipt", document.Id, null);
            if (emitted.Succeeded == false)
            {
                return emitted.Error;
            }

            var undone = await listing.UndoReceiptAsync(emitted.Id);
            return undone.Succeeded ? null : undone.Error;
        });

        ok &= await Step(log, "Backup", async () =>
        {
            var backup = await listing.RunActionAsync("backup", null, null);
            return backup.Succeeded ? null : backup.Error;
        });

        ok &= await Step(log, "Exportar", async () =>
        {
            var articles = await listing.RunActionAsync("export-articles", null, null);
            var customers = await listing.RunActionAsync("export-customers", null, null);
            return FirstError(articles, customers);
        });

        ok &= await Step(log, "Importar artigos", async () =>
        {
            var csv = $"Code;Designation;Barcode;Price{Environment.NewLine}SM{stamp};SMOKE artigo {stamp};;1.25";
            var imported = await listing.RunActionAsync("import-articles", null, csv);
            foreach (var id in imported.Ids)
            {
                await listing.DeleteAsync("Artigos", id);
            }

            return imported.Succeeded ? null : imported.Error;
        });

        ok &= await Step(log, "Importar clientes", async () =>
        {
            var csv = $"Code;Name;FiscalNumber;Email{Environment.NewLine}SC{stamp};SMOKE cliente {stamp};999999990;smoke@logicpos.local";
            var imported = await listing.RunActionAsync("import-customers", null, csv);
            foreach (var id in imported.Ids)
            {
                await listing.DeleteAsync("Clientes", id);
            }

            return imported.Succeeded ? null : imported.Error;
        });

        return ok;
    }

    private static async Task<bool> Step(StringBuilder log, string name, Func<Task<string?>> action)
    {
        try
        {
            var error = await action();
            if (error is null)
            {
                log.AppendLine($"OK  {name}");
                return true;
            }

            log.AppendLine($"FAIL {name}: {error}");
            return false;
        }
        catch (Exception exception)
        {
            log.AppendLine($"FAIL {name}: {exception.Message}");
            return false;
        }
    }

    private static string? FirstError(params ListingSaveResult[] results)
        => results.FirstOrDefault(result => result.Succeeded == false)?.Error;

    private static void Set(Dictionary<string, string> values, string key, string value)
    {
        if (values.ContainsKey(key))
        {
            values[key] = value;
        }
    }
}
