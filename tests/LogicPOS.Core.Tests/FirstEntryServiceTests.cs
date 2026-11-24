using System.Text.Json.Nodes;
using LogicPOS.Application.Features.System;
using LogicPOS.Core.Authentication;
using LogicPOS.Core.BackOffice;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests;

[Collection(AppCompositionCollection.Name)]
public sealed class FirstEntryServiceTests
{
    private const string PortugueseFiscalNumber = "999999990";

    [Fact]
    public async Task Public_edition_with_nothing_prepared_is_pending_with_empty_choices()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-pending-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var service = AppComposition.Services!.GetRequiredService<IFirstEntryService>();

            var pending = await service.GetPendingAsync();

            Assert.True(pending.IsPending);
            Assert.Null(pending.OperationMode);
            Assert.False(pending.DemoAccepted);
            Assert.Null(pending.CountryId);
            Assert.Null(pending.CurrencyId);
            Assert.True(string.IsNullOrEmpty(pending.FiscalNumber));
            Assert.Contains("restaurant", pending.Modes);
            Assert.Contains("default", pending.Modes);
            Assert.Contains(pending.Countries, item => item.Code == "PT");
            Assert.Contains(pending.Currencies, item => item.Code == "EUR");
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Currency_is_suggested_by_country()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-currency-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var service = AppComposition.Services!.GetRequiredService<IFirstEntryService>();
            var pending = await service.GetPendingAsync();
            var portugal = pending.Countries.Single(item => item.Code == "PT");
            var euro = pending.Currencies.Single(item => item.Code == "EUR");

            Assert.Equal(euro.Id, await service.SuggestCurrencyIdAsync(portugal.Id));
            Assert.Null(await service.SuggestCurrencyIdAsync(null));
            Assert.Null(await service.SuggestCurrencyIdAsync(Guid.NewGuid()));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Theory]
    [InlineData("missing-mode")]
    [InlineData("unknown-mode")]
    [InlineData("missing-country")]
    [InlineData("missing-currency")]
    [InlineData("missing-nif")]
    [InlineData("blank-nif")]
    public async Task Incomplete_confirmation_writes_nothing_and_stays_pending(string missing)
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-incomplete-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var service = AppComposition.Services!.GetRequiredService<IFirstEntryService>();
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);

            var result = await service.ConfirmAsync(new FirstEntryRequest
            {
                OperationMode = missing switch
                {
                    "missing-mode" => null,
                    "unknown-mode" => "spaceship",
                    _ => "restaurant"
                },
                DemoAccepted = false,
                CountryId = missing == "missing-country" ? null : countryId,
                CurrencyId = missing == "missing-currency" ? null : currencyId,
                FiscalNumber = missing switch
                {
                    "missing-nif" => null,
                    "blank-nif" => "   ",
                    _ => PortugueseFiscalNumber
                }
            });

            Assert.Equal(FirstEntryConfirmationResult.Incomplete, result);
            Assert.True((await service.GetPendingAsync()).IsPending);
            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.Null(await TokenValueAsync(database, "FIRST_ENTRY_CONFIRMED"));
            Assert.Null(await TokenValueAsync(database, "SYSTEM_OPERATION_MODE"));
            Assert.True(string.IsNullOrWhiteSpace(await TokenValueAsync(database, "COMPANY_FISCALNUMBER")));
            Assert.Equal("default", ReadModuleFromAppSettings(root));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Closing_without_confirming_leaves_the_first_entry_pending()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-closed-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var service = AppComposition.Services!.GetRequiredService<IFirstEntryService>();

            Assert.True((await service.GetPendingAsync()).IsPending);
            Assert.True((await service.GetPendingAsync()).IsPending);

            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.Null(await TokenValueAsync(database, "FIRST_ENTRY_CONFIRMED"));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Confirming_without_demo_saves_mode_company_and_country_data_without_sample()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-confirm-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var services = AppComposition.Services!;
            var service = services.GetRequiredService<IFirstEntryService>();
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);

            var result = await service.ConfirmAsync(Request("restaurant", countryId, currencyId));

            Assert.Equal(FirstEntryConfirmationResult.Confirmed, result);
            Assert.False((await service.GetPendingAsync()).IsPending);

            await using var scope = services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.Equal("1", await TokenValueAsync(database, "FIRST_ENTRY_CONFIRMED"));
            Assert.Equal("restaurant", await TokenValueAsync(database, "SYSTEM_OPERATION_MODE"));
            Assert.Equal(countryId.ToString(), await TokenValueAsync(database, "COMPANY_COUNTRY_OID"));
            Assert.Equal(currencyId.ToString(), await TokenValueAsync(database, "SYSTEM_CURRENCY_OID"));
            Assert.Equal(PortugueseFiscalNumber, await TokenValueAsync(database, "COMPANY_FISCALNUMBER"));
            Assert.Equal("restaurant", ReadModuleFromAppSettings(root));

            Assert.True(await database.VatRates.AnyAsync(item => item.IsDeleted == false));
            Assert.True(await database.PaymentMethods.AnyAsync(item => item.IsDeleted == false));
            Assert.True(await database.DocumentTypes.AnyAsync(item => item.IsDeleted == false));

            Assert.False(await database.Articles.AnyAsync(item => item.IsDeleted == false));
            Assert.False(await database.Tables.AnyAsync(item => item.IsDeleted == false));
            Assert.False(await database.Customers.AnyAsync(item => item.IsDeleted == false));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Accepting_the_demo_loads_the_sample_of_the_confirmed_mode_not_the_prepared_one()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-demo-");
        try
        {
            // Prepared mode is restaurant (large sample); the person confirms parking (small sample).
            ConfigureApp(root, module: "restaurant", useSeed: false);
            var services = AppComposition.Services!;
            var service = services.GetRequiredService<IFirstEntryService>();
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);

            var result = await service.ConfirmAsync(Request("parking", countryId, currencyId, demo: true));

            Assert.Equal(FirstEntryConfirmationResult.Confirmed, result);
            Assert.False((await service.GetPendingAsync()).IsPending);
            var seeds = Path.Combine(DatabaseStartup.ResolveSeedPath(root.FullName), "modules", "parking");
            await using var scope = services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.Equal(SeedItemCount(seeds, "articles.json"), await database.Articles.CountAsync());
            Assert.Equal(SeedItemCount(seeds, "tables.json"), await database.Tables.CountAsync());
            Assert.Equal("1", await TokenValueAsync(database, "FIRST_ENTRY_CONFIRMED"));
            Assert.Equal("parking", await TokenValueAsync(database, "SYSTEM_OPERATION_MODE"));
            Assert.Equal(PortugueseFiscalNumber, await TokenValueAsync(database, "COMPANY_FISCALNUMBER"));
            Assert.Equal("parking", ReadModuleFromAppSettings(root));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Onboarding_no_longer_asks_article_or_table_after_the_sample_but_still_asks_fiscal_year()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-demo-onboarding-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var services = AppComposition.Services!;
            var service = services.GetRequiredService<IFirstEntryService>();
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);
            Assert.Equal(
                FirstEntryConfirmationResult.Confirmed,
                await service.ConfirmAsync(Request("parking", countryId, currencyId, demo: true)));

            var status = await services.GetRequiredService<IOnboardingService>().GetStatusAsync();

            foreach (var key in new[] { "article", "table" })
            {
                Assert.Contains(status.Steps, step => step.Key == key && step.IsComplete);
            }

            Assert.Contains(status.Steps, step => step.Key == "fiscal-year" && step.IsComplete == false);
            // The sample customers (db_seeds/<country>/customers.json) already include "Consumidor Final",
            // so the final-consumer step is also done after the sample. Series need the fiscal year.
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Theory]
    [InlineData("throws")]
    [InlineData("returns-false")]
    public async Task Failed_sample_rolls_back_everything_and_keeps_the_first_entry_pending(string failure)
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-demo-failed-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var services = AppComposition.Services!;
            var configuration = services.GetRequiredService<IConfiguration>();
            var failing = new FirstEntryService(
                services.GetRequiredService<IServiceScopeFactory>(),
                services.GetRequiredService<LocalCompanySetupService>(),
                configuration,
                root.FullName,
                (provider, _) =>
                {
                    // The real sample goes in first, so the rollback has something to remove.
                    Assert.True(provider.GetRequiredService<IDatabaseSeeder>().ApplyDemoSample());
                    return failure == "throws"
                        ? throw new InvalidOperationException("sample failed mid-way")
                        : Task.FromResult(false);
                });
            var (countryId, currencyId) = await PortugalAndEuroAsync(failing);

            var result = await failing.ConfirmAsync(Request("parking", countryId, currencyId, demo: true));

            Assert.Equal(FirstEntryConfirmationResult.DemoFailed, result);
            Assert.True((await failing.GetPendingAsync()).IsPending);
            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                Assert.Null(await TokenValueAsync(database, "FIRST_ENTRY_CONFIRMED"));
                Assert.Null(await TokenValueAsync(database, "SYSTEM_OPERATION_MODE"));
                Assert.True(string.IsNullOrWhiteSpace(await TokenValueAsync(database, "COMPANY_FISCALNUMBER")));
                Assert.True(string.IsNullOrWhiteSpace(await TokenValueAsync(database, "COMPANY_COUNTRY_OID")));
                Assert.False(await database.Articles.AnyAsync());
                Assert.False(await database.Tables.AnyAsync());
                Assert.False(await database.Places.AnyAsync());
                Assert.False(await database.Customers.AnyAsync());
            }

            Assert.Equal("default", ReadModuleFromAppSettings(root));
            Assert.Equal("default", configuration["DatabaseSettings:Module"]);

            // The next attempt starts clean and can succeed.
            var retry = await services.GetRequiredService<IFirstEntryService>()
                .ConfirmAsync(Request("parking", countryId, currencyId, demo: true));
            Assert.Equal(FirstEntryConfirmationResult.Confirmed, retry);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Confirming_with_the_sample_does_not_change_the_administrator_pin()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-demo-admin-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var services = AppComposition.Services!;
            var wizard = services.GetRequiredService<IFirstUserSetupWizard>();
            var draft = await wizard.LoadAsync();
            var created = await wizard.CreateAsync(new FirstUserSetupRequest
            {
                ProfileId = draft.PreferredProfileId ?? draft.Profiles[0].Id,
                Name = "Administrador",
                Login = "admin",
                AccessPin = "4321",
                AccessPinConfirm = "4321"
            });
            Assert.True(created.Ok, created.Message);
            var before = await UserPinsAsync();

            var service = services.GetRequiredService<IFirstEntryService>();
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);
            Assert.Equal(
                FirstEntryConfirmationResult.Confirmed,
                await service.ConfirmAsync(Request("parking", countryId, currencyId, demo: true)));

            Assert.Equal(before, await UserPinsAsync());
            var user = Assert.Single(await services.GetRequiredService<ILoginService>().GetUsersAsync());
            var signIn = await services.GetRequiredService<ILoginService>().SignInAsync(user.Id, "4321");
            Assert.True(signIn.Succeeded, signIn.Message);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Second_confirmation_does_not_change_mode_or_company()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-second-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var services = AppComposition.Services!;
            var service = services.GetRequiredService<IFirstEntryService>();
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);
            Assert.Equal(
                FirstEntryConfirmationResult.Confirmed,
                await service.ConfirmAsync(Request("restaurant", countryId, currencyId)));

            var second = await service.ConfirmAsync(Request("cafe", countryId, currencyId, nif: "123456789"));

            Assert.Equal(FirstEntryConfirmationResult.AlreadyConfirmed, second);
            await using var scope = services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.Equal("restaurant", await TokenValueAsync(database, "SYSTEM_OPERATION_MODE"));
            Assert.Equal(PortugueseFiscalNumber, await TokenValueAsync(database, "COMPANY_FISCALNUMBER"));
            Assert.Equal("restaurant", ReadModuleFromAppSettings(root));
            Assert.False(await database.Articles.AnyAsync(item => item.IsDeleted == false));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Onboarding_still_asks_fiscal_year_article_final_consumer_and_table_after_confirming()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-onboarding-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var services = AppComposition.Services!;
            var service = services.GetRequiredService<IFirstEntryService>();
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);
            Assert.Equal(
                FirstEntryConfirmationResult.Confirmed,
                await service.ConfirmAsync(Request("restaurant", countryId, currencyId)));

            var status = await services.GetRequiredService<IOnboardingService>().GetStatusAsync();

            Assert.Contains(status.Steps, step => step.Key == "company" && step.IsComplete);
            foreach (var key in new[] { "fiscal-year", "article", "final-consumer", "table" })
            {
                Assert.Contains(status.Steps, step => step.Key == key && step.IsComplete == false);
            }

            Assert.DoesNotContain(status.Steps, step => step.Key == "payment-method" && step.IsComplete == false);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Database_with_company_already_configured_is_not_pending()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-legacy-company-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var services = AppComposition.Services!;
            var service = services.GetRequiredService<IFirstEntryService>();
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);
            var saved = await services.GetRequiredService<ICompanySetupService>().SaveAsync(new CompanySetupRequest
            {
                CountryId = countryId,
                CurrencyId = currencyId,
                Values = LocalCompanySetupService.DemoCompanyValues
            });
            Assert.True(saved.Succeeded, saved.Error);

            Assert.False((await service.GetPendingAsync()).IsPending);
            Assert.Equal(
                FirstEntryConfirmationResult.AlreadyConfirmed,
                await service.ConfirmAsync(Request("cafe", countryId, currencyId)));
            await using var scope = services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.Null(await TokenValueAsync(database, "FIRST_ENTRY_CONFIRMED"));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Database_that_already_has_articles_is_not_pending()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-legacy-data-");
        try
        {
            ConfigureApp(root, module: "restaurant", useSeed: true);
            var service = AppComposition.Services!.GetRequiredService<IFirstEntryService>();

            await using (var scope = AppComposition.Services!.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                Assert.True(await database.Articles.AnyAsync(item => item.IsDeleted == false));
            }

            Assert.False((await service.GetPendingAsync()).IsPending);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Installer_draft_preselects_the_first_entry_without_confirming_it()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-draft-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var service = AppComposition.Services!.GetRequiredService<IFirstEntryService>();
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);

            Assert.True(await service.TrySaveDraftAsync(new FirstEntryDraft
            {
                OperationMode = "Restaurant",
                DemoAccepted = true,
                CountryId = countryId,
                CurrencyId = currencyId,
                FiscalNumber = PortugueseFiscalNumber
            }));

            var pending = await service.GetPendingAsync();
            Assert.True(pending.IsPending);
            Assert.Equal("restaurant", pending.OperationMode);
            Assert.True(pending.DemoAccepted);
            Assert.Equal(countryId, pending.CountryId);
            Assert.Equal(currencyId, pending.CurrencyId);
            Assert.Equal(PortugueseFiscalNumber, pending.FiscalNumber);
            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.Null(await TokenValueAsync(database, "FIRST_ENTRY_CONFIRMED"));
            Assert.Null(await TokenValueAsync(database, "SYSTEM_OPERATION_MODE"));
            Assert.False(await database.Articles.AnyAsync());
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Draft_with_only_a_country_suggests_its_currency_and_bad_values_are_ignored()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-draft-partial-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var service = AppComposition.Services!.GetRequiredService<IFirstEntryService>();
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);

            FirstEntryDraftStore.Write(root.FullName, new FirstEntryDraft { CountryId = countryId });
            var pending = await service.GetPendingAsync();
            Assert.Equal(countryId, pending.CountryId);
            Assert.Equal(currencyId, pending.CurrencyId);
            Assert.Null(pending.OperationMode);

            FirstEntryDraftStore.Write(root.FullName, new FirstEntryDraft
            {
                OperationMode = "spaceship",
                CountryId = Guid.NewGuid(),
                CurrencyId = Guid.NewGuid()
            });
            pending = await service.GetPendingAsync();
            Assert.Null(pending.OperationMode);
            Assert.Null(pending.CountryId);
            Assert.Null(pending.CurrencyId);

            File.WriteAllText(FirstEntryDraftStore.PathFor(root.FullName), "{ not json");
            Assert.Null((await service.GetPendingAsync()).OperationMode);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Confirming_uses_the_request_not_the_draft_and_closing_leaves_the_draft_unchanged()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-draft-confirm-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var services = AppComposition.Services!;
            var service = services.GetRequiredService<IFirstEntryService>();
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);
            Assert.True(await service.TrySaveDraftAsync(new FirstEntryDraft
            {
                OperationMode = "restaurant",
                DemoAccepted = true,
                CountryId = countryId,
                CurrencyId = currencyId,
                FiscalNumber = "123456789"
            }));
            var draftPath = FirstEntryDraftStore.PathFor(root.FullName);
            var draftBefore = File.ReadAllText(draftPath);

            // Closing without confirming: nothing changes.
            await service.GetPendingAsync();
            Assert.Equal(draftBefore, File.ReadAllText(draftPath));
            Assert.Equal("restaurant", FirstEntryDraftStore.Read(root.FullName)!.OperationMode);

            // The app changes the draft: other mode, demo refused, other NIF.
            var result = await service.ConfirmAsync(Request("cafe", countryId, currencyId));

            Assert.Equal(FirstEntryConfirmationResult.Confirmed, result);
            await using var scope = services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.Equal("cafe", await TokenValueAsync(database, "SYSTEM_OPERATION_MODE"));
            Assert.Equal(PortugueseFiscalNumber, await TokenValueAsync(database, "COMPANY_FISCALNUMBER"));
            Assert.Equal("cafe", ReadModuleFromAppSettings(root));
            Assert.False(await database.Articles.AnyAsync());
            Assert.False(await database.Tables.AnyAsync());
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Writing_the_draft_again_after_confirmation_changes_nothing()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-draft-rerun-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var services = AppComposition.Services!;
            var service = services.GetRequiredService<IFirstEntryService>();
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);
            Assert.True(await service.TrySaveDraftAsync(new FirstEntryDraft { OperationMode = "restaurant" }));
            var draftBefore = File.ReadAllText(FirstEntryDraftStore.PathFor(root.FullName));
            Assert.Equal(
                FirstEntryConfirmationResult.Confirmed,
                await service.ConfirmAsync(Request("restaurant", countryId, currencyId)));

            var rerun = await service.TrySaveDraftAsync(new FirstEntryDraft
            {
                OperationMode = "bakery",
                DemoAccepted = true,
                CountryId = countryId,
                CurrencyId = currencyId,
                FiscalNumber = "123456789"
            });

            Assert.False(rerun);
            Assert.Equal(draftBefore, File.ReadAllText(FirstEntryDraftStore.PathFor(root.FullName)));
            var pending = await service.GetPendingAsync();
            Assert.False(pending.IsPending);
            Assert.Null(pending.OperationMode);
            await using var scope = services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.Equal("restaurant", await TokenValueAsync(database, "SYSTEM_OPERATION_MODE"));
            Assert.Equal("1", await TokenValueAsync(database, "FIRST_ENTRY_CONFIRMED"));
            Assert.Equal("restaurant", ReadModuleFromAppSettings(root));
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Cloud_account_mode_and_country_come_preselected_and_demo_starts_refused()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-cloud-pending-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var account = new FakeAccount { Mode = "restaurant", CountryCode2 = "PT", IsDemo = true };
            var service = CloudService(root, account);
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);
            // An installer draft is a local-edition thing; the cloud account wins.
            FirstEntryDraftStore.Write(root.FullName, new FirstEntryDraft { OperationMode = "cafe", DemoAccepted = true });

            var pending = await service.GetPendingAsync();

            Assert.True(pending.IsPending);
            Assert.Equal("restaurant", pending.OperationMode);
            Assert.False(pending.DemoAccepted);
            Assert.Equal(countryId, pending.CountryId);
            Assert.Equal(currencyId, pending.CurrencyId);
            Assert.True(string.IsNullOrEmpty(pending.FiscalNumber));

            account.Mode = null;
            Assert.Null((await service.GetPendingAsync()).OperationMode);
            account.CountryCode2 = null;
            Assert.Null((await service.GetPendingAsync()).CountryId);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Demo_account_mark_neither_forces_nor_blocks_the_sample(bool demoAccepted)
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-cloud-demo-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var account = new FakeAccount { IsDemo = true, CountryCode2 = "PT" };
            var service = CloudService(root, account);
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);

            var result = await service.ConfirmAsync(Request("parking", countryId, currencyId, demo: demoAccepted));

            Assert.Equal(FirstEntryConfirmationResult.Confirmed, result);
            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.Equal(demoAccepted, await database.Articles.AnyAsync());
            Assert.Equal(demoAccepted, await database.Tables.AnyAsync());
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Cloud_confirmation_stores_the_mode_on_the_account_even_when_it_is_unchanged()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-cloud-store-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var account = new FakeAccount { Mode = "restaurant", CountryCode2 = "PT" };
            var service = CloudService(root, account);
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);

            Assert.Equal(
                FirstEntryConfirmationResult.Confirmed,
                await service.ConfirmAsync(Request("restaurant", countryId, currencyId)));

            Assert.Equal(["restaurant"], account.Stored);
            Assert.Equal("restaurant", account.Mode);
            Assert.Equal("default", ReadModuleFromAppSettings(root));
            Assert.Equal("restaurant", await service.GetResolvedOperationModeAsync());
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Two_overlapping_cloud_confirmations_keep_only_the_first()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-cloud-race-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var account = new FakeAccount { CountryCode2 = "PT" };
            var service = CloudService(root, account);
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);

            var results = await Task.WhenAll(
                service.ConfirmAsync(Request("restaurant", countryId, currencyId, nif: "111111111", demo: true)),
                service.ConfirmAsync(Request("parking", countryId, currencyId, nif: "222222222", demo: true)));

            Assert.Single(results, FirstEntryConfirmationResult.Confirmed);
            Assert.Single(results, FirstEntryConfirmationResult.AlreadyConfirmed);
            var winner = Assert.Single(account.Stored);
            await using var scope = AppComposition.Services!.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
            Assert.Equal(winner, await TokenValueAsync(database, "SYSTEM_OPERATION_MODE"));
            Assert.Equal(
                winner == "restaurant" ? "111111111" : "222222222",
                await TokenValueAsync(database, "COMPANY_FISCALNUMBER"));
            var seeds = Path.Combine(DatabaseStartup.ResolveSeedPath(root.FullName), "modules", winner);
            Assert.Equal(SeedItemCount(seeds, "tables.json"), await database.Tables.CountAsync());
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Changing_the_account_mode_after_confirmation_keeps_the_mark_and_the_sample()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-cloud-later-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var account = new FakeAccount { CountryCode2 = "PT" };
            var service = CloudService(root, account);
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);
            Assert.Equal(
                FirstEntryConfirmationResult.Confirmed,
                await service.ConfirmAsync(Request("parking", countryId, currencyId, demo: true)));
            int articles;
            await using (var scope = AppComposition.Services!.CreateAsyncScope())
            {
                articles = await scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>().Articles.CountAsync();
            }

            account.Mode = "bakery";

            Assert.False((await service.GetPendingAsync()).IsPending);
            Assert.Equal("bakery", await service.GetResolvedOperationModeAsync());
            await using (var scope = AppComposition.Services!.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                Assert.Equal("1", await TokenValueAsync(database, "FIRST_ENTRY_CONFIRMED"));
                Assert.Equal(articles, await database.Articles.CountAsync());
                Assert.True(articles > 0);
            }

            account.Mode = null;
            Assert.Equal("parking", await service.GetResolvedOperationModeAsync());
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Failing_to_store_the_mode_on_the_account_keeps_the_confirmation_and_falls_back_to_it()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-cloud-setfail-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var account = new FakeAccount { CountryCode2 = "PT", FailStore = true };
            var service = CloudService(root, account);
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);

            Assert.Equal(
                FirstEntryConfirmationResult.Confirmed,
                await service.ConfirmAsync(Request("cafe", countryId, currencyId)));

            Assert.False((await service.GetPendingAsync()).IsPending);
            Assert.Null(account.Mode);
            Assert.Equal("cafe", await service.GetResolvedOperationModeAsync());
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Cloud_confirmation_never_changes_the_administrator_pin()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-firstentry-cloud-admin-");
        try
        {
            ConfigureApp(root, module: "default", useSeed: false);
            var services = AppComposition.Services!;
            var wizard = services.GetRequiredService<IFirstUserSetupWizard>();
            var draft = await wizard.LoadAsync();
            var created = await wizard.CreateAsync(new FirstUserSetupRequest
            {
                ProfileId = draft.PreferredProfileId ?? draft.Profiles[0].Id,
                Name = "Administrador",
                Login = "admin",
                AccessPin = "4321",
                AccessPinConfirm = "4321"
            });
            Assert.True(created.Ok, created.Message);
            var before = await UserPinsAsync();
            var service = CloudService(root, new FakeAccount { CountryCode2 = "PT" });
            var (countryId, currencyId) = await PortugalAndEuroAsync(service);

            Assert.Equal(
                FirstEntryConfirmationResult.Confirmed,
                await service.ConfirmAsync(Request("parking", countryId, currencyId, demo: true)));

            Assert.Equal(before, await UserPinsAsync());
        }
        finally
        {
            Cleanup(root);
        }
    }

    private static FirstEntryService CloudService(DirectoryInfo root, IFirstEntryAccountContext account)
    {
        var services = AppComposition.Services!;
        return new FirstEntryService(
            services.GetRequiredService<IServiceScopeFactory>(),
            services.GetRequiredService<LocalCompanySetupService>(),
            services.GetRequiredService<IConfiguration>(),
            root.FullName,
            account: account);
    }

    private sealed class FakeAccount : IFirstEntryAccountContext
    {
        public string? Mode { get; set; }
        public string? CountryCode2 { get; set; }
        public bool IsDemo { get; set; }
        public bool FailStore { get; set; }
        public List<string> Stored { get; } = [];

        public bool IsCloudAccount => true;
        public string? AccountOperationMode => Mode;
        public string? AccountCountryCode2 => CountryCode2;
        public bool IsDemoAccount => IsDemo;

        public Task SetAccountOperationModeAsync(string mode, CancellationToken cancellationToken = default)
        {
            if (FailStore)
            {
                throw new InvalidOperationException("account unavailable");
            }

            Stored.Add(mode);
            Mode = mode;
            return Task.CompletedTask;
        }
    }

    private static async Task<List<(Guid Id, string? Pin)>> UserPinsAsync()
    {
        await using var scope = AppComposition.Services!.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var users = await database.Users.AsNoTracking().OrderBy(item => item.Id).ToListAsync();
        return users.Select(item => (item.Id, (string?)item.AccessPin)).ToList();
    }

    private static int SeedItemCount(string moduleSeedDirectory, string fileName)
        => JsonNode.Parse(File.ReadAllText(Path.Combine(moduleSeedDirectory, fileName)))!.AsArray().Count;

    private static FirstEntryRequest Request(
        string mode,
        Guid countryId,
        Guid currencyId,
        string nif = PortugueseFiscalNumber,
        bool demo = false)
        => new()
        {
            OperationMode = mode,
            DemoAccepted = demo,
            CountryId = countryId,
            CurrencyId = currencyId,
            FiscalNumber = nif
        };

    private static async Task<(Guid CountryId, Guid CurrencyId)> PortugalAndEuroAsync(IFirstEntryService service)
    {
        var pending = await service.GetPendingAsync();
        return (
            pending.Countries.Single(item => item.Code == "PT").Id,
            pending.Currencies.Single(item => item.Code == "EUR").Id);
    }

    private static async Task<string?> TokenValueAsync(LogicPOSDbContext database, string token)
        => await database.PreferenceParameters.AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Token == token)
            .Select(item => item.Value)
            .FirstOrDefaultAsync();

    private static string? ReadModuleFromAppSettings(DirectoryInfo root)
    {
        var json = JsonNode.Parse(File.ReadAllText(Path.Combine(root.FullName, "appsettings.json")));
        return json?["DatabaseSettings"]?["Module"]?.GetValue<string>();
    }

    private static void ConfigureApp(DirectoryInfo root, string module, bool useSeed)
    {
        var databasePath = Path.Combine(root.FullName, "logicpos.db");
        File.WriteAllText(
            Path.Combine(root.FullName, "appsettings.json"),
            $$"""
            {
              "Culture": "pt-PT",
              "DatabaseSettings": {
                "DatabaseType": "Sqlite",
                "ConnectionString": "Data Source={{databasePath.Replace("\\", "\\\\")}}",
                "UseCloud": false,
                "UseSeed": {{(useSeed ? "true" : "false")}},
                "Module": "{{module}}"
              }
            }
            """);

        AppComposition.Configure(root.FullName);
        Assert.Null(AppComposition.StartupError);
        Assert.NotNull(AppComposition.Services);
    }

    private static void Cleanup(DirectoryInfo root)
    {
        if (AppComposition.Services is IDisposable disposable)
        {
            disposable.Dispose();
        }

        SqliteConnection.ClearAllPools();
        AppLogging.CloseAndFlush();

        const int maxRetries = 5;
        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                if (root.Exists)
                {
                    root.Delete(recursive: true);
                }

                return;
            }
            catch (IOException) when (attempt < maxRetries)
            {
                Thread.Sleep(200 * attempt);
            }
            catch (UnauthorizedAccessException) when (attempt < maxRetries)
            {
                Thread.Sleep(200 * attempt);
            }
        }
    }
}
