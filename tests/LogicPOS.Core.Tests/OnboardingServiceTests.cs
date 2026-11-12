using LogicPOS.Core.Authentication;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.FrontOffice;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.Tests;

public sealed class OnboardingServiceTests
{
    [Fact]
    public async Task Fresh_database_marks_company_and_fiscal_year_incomplete()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-onboarding-fresh-");
        try
        {
            ConfigureApp(root, module: "restaurant");
            var services = AppComposition.Services!;
            var onboarding = services.GetRequiredService<IOnboardingService>();

            var status = await onboarding.GetStatusAsync();

            Assert.False(status.IsComplete);
            Assert.Contains(status.Steps, step => step.Key == "first-user" && step.IsComplete == false);
            Assert.Contains(status.Steps, step => step.Key == "company" && step.IsComplete == false);
            Assert.Contains(status.Steps, step => step.Key == "fiscal-year" && step.IsComplete == false);
            Assert.Contains(status.Steps, step => step.Key == "table");
            Assert.Contains(status.Steps, step => step.Key == "article");
            Assert.Contains(status.Steps, step => step.Key == "final-consumer");
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Fresh_database_has_no_users_and_prefers_owner_profile()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-onboarding-nousers-");
        try
        {
            ConfigureApp(root, module: "restaurant");
            var services = AppComposition.Services!;
            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                Assert.Equal(0, await database.Users.CountAsync());
                Assert.True(await database.UserProfiles.AnyAsync());
            }

            var wizard = services.GetRequiredService<IFirstUserSetupWizard>();
            Assert.True(await wizard.NeedsSetupAsync());
            var draft = await wizard.LoadAsync();
            var preferred = draft.Profiles.Single(profile => profile.Id == draft.PreferredProfileId);
            Assert.Equal("Proprietário", preferred.Label);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task First_user_is_created_without_forced_pin_change()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-onboarding-firstuser-");
        try
        {
            ConfigureApp(root, module: "restaurant");
            var services = AppComposition.Services!;
            var wizard = services.GetRequiredService<IFirstUserSetupWizard>();
            var draft = await wizard.LoadAsync();

            var created = await wizard.CreateAsync(new FirstUserSetupRequest
            {
                ProfileId = draft.PreferredProfileId!.Value,
                Name = "Dono",
                Login = "dono",
                Code = "7",
                AccessPin = "4321",
                AccessPinConfirm = "4321"
            });

            Assert.True(created.Ok, created.Message);
            Assert.False(await wizard.NeedsSetupAsync());
            var users = await services.GetRequiredService<ILoginService>().GetUsersAsync();
            var user = Assert.Single(users);
            Assert.Equal("Dono", user.Name);
            Assert.False(user.RequiresPasswordReset);
            var signIn = await services.GetRequiredService<ILoginService>().SignInAsync(user.Id, "4321");
            Assert.True(signIn.Succeeded, signIn.Message);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public void Optional_steps_do_not_count_towards_required_progress()
    {
        var status = new OnboardingStatus(new[]
        {
            new OnboardingStep("first-user", "Primeiro utilizador", "", "Utilizadores", isComplete: true),
            new OnboardingStep("article", "Primeiro artigo", "", "Artigos", isComplete: false),
            new OnboardingStep("more-users", "Adicionar utilizadores", "", "Utilizadores", isComplete: false, isOptional: true)
        });

        Assert.Equal(1, status.CompletedCount);
        Assert.Equal(2, status.TotalCount);
        Assert.Equal(50, status.PercentComplete);
        Assert.Equal("article", status.NextStep?.Key);
        Assert.False(status.IsComplete);

        var requiredDone = new OnboardingStatus(new[]
        {
            new OnboardingStep("first-user", "Primeiro utilizador", "", "Utilizadores", isComplete: true),
            new OnboardingStep("more-users", "Adicionar utilizadores", "", "Utilizadores", isComplete: false, isOptional: true)
        });

        Assert.True(requiredDone.IsComplete);
        Assert.Equal(100, requiredDone.PercentComplete);
        Assert.Null(requiredDone.NextStep);
    }

    [Fact]
    public async Task More_users_step_is_optional_and_completes_with_a_second_user()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-onboarding-moreusers-");
        try
        {
            ConfigureApp(root, module: "restaurant");
            var services = AppComposition.Services!;
            var wizard = services.GetRequiredService<IFirstUserSetupWizard>();
            var draft = await wizard.LoadAsync();
            var created = await wizard.CreateAsync(new FirstUserSetupRequest
            {
                ProfileId = draft.PreferredProfileId!.Value,
                Name = "Dono",
                Login = "dono",
                AccessPin = "4321",
                AccessPinConfirm = "4321"
            });
            Assert.True(created.Ok, created.Message);

            var onboarding = services.GetRequiredService<IOnboardingService>();
            var before = await onboarding.GetStatusAsync();
            var moreUsers = Assert.Single(before.Steps, step => step.Key == "more-users");
            Assert.True(moreUsers.IsOptional);
            Assert.False(moreUsers.IsComplete);
            Assert.Null(moreUsers.StatusText);
            Assert.Equal(before.Steps.Count(step => step.IsOptional == false), before.TotalCount);

            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                database.Users.Add(new User
                {
                    Id = Guid.NewGuid(),
                    Order = 100,
                    Code = "8",
                    Name = "Empregado",
                    Login = "empregado",
                    AccessPin = "x",
                    ProfileId = draft.PreferredProfileId!.Value,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
                await database.SaveChangesAsync();
            }

            var after = await onboarding.GetStatusAsync();
            moreUsers = Assert.Single(after.Steps, step => step.Key == "more-users");
            Assert.True(moreUsers.IsComplete);
            Assert.Equal("2 utilizadores", moreUsers.StatusText);
            Assert.Equal(before.TotalCount, after.TotalCount);
            Assert.Equal(before.CompletedCount, after.CompletedCount);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Printer_step_is_optional_and_completes_when_the_terminal_has_a_printer()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-onboarding-printer-");
        try
        {
            ConfigureApp(root, module: "restaurant");
            var services = AppComposition.Services!;
            var onboarding = services.GetRequiredService<IOnboardingService>();
            var before = Assert.Single((await onboarding.GetStatusAsync()).Steps, step => step.Key == "printer");
            Assert.True(before.IsOptional);
            Assert.False(before.IsComplete);

            var wizard = services.GetRequiredService<IPrinterSetupWizard>();
            var draft = await wizard.LoadAsync();
            Assert.Null(draft.BlockedReason);
            var thermal = Assert.Single(draft.Types, type => type.Thermal);
            Assert.Contains(draft.Types, type => type.Thermal == false);

            var request = new PrinterSetupRequest { TypeId = thermal.Id, PrinterName = "POS-58", PaperLabel = "58 mm" };
            var saved = await wizard.SaveAsync(request);
            Assert.True(saved.Ok, saved.Message);
            var again = await wizard.SaveAsync(request);
            Assert.True(again.Ok, again.Message);

            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                Assert.Equal(1, await database.Printers.CountAsync(item => item.IsDeleted == false && item.Designation == "POS-58"));
            }

            var printer = await services.GetRequiredService<IThermalPrintSource>().GetTerminalPrinterAsync();
            Assert.NotNull(printer);
            Assert.Equal("POS-58", printer.Designation);
            Assert.Equal(58, printer.PaperWidthMm);

            var after = Assert.Single((await onboarding.GetStatusAsync()).Steps, step => step.Key == "printer");
            Assert.True(after.IsComplete);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Backoffice_module_omits_table_step()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-onboarding-bo-");
        try
        {
            ConfigureApp(root, module: "backoffice");
            var onboarding = AppComposition.Services!.GetRequiredService<IOnboardingService>();

            var status = await onboarding.GetStatusAsync();

            Assert.DoesNotContain(status.Steps, step => step.Key == "table");
            Assert.Contains(status.Steps, step => step.Key == "company");
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Missing_articles_mark_article_step_incomplete()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-onboarding-articles-");
        try
        {
            ConfigureApp(root, module: "restaurant");
            var services = AppComposition.Services!;
            await using (var scope = services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
                foreach (var article in database.Articles)
                {
                    article.IsDeleted = true;
                }

                await database.SaveChangesAsync();
            }

            var status = await services.GetRequiredService<IOnboardingService>().GetStatusAsync();

            Assert.Contains(status.Steps, step => step.Key == "article" && step.IsComplete == false);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public async Task Company_and_fiscal_year_complete_mandatory_seeded_steps()
    {
        var root = Directory.CreateTempSubdirectory("logicpos-onboarding-ready-");
        try
        {
            ConfigureApp(root, module: "restaurant");
            var services = AppComposition.Services!;

            var firstUser = services.GetRequiredService<IFirstUserSetupWizard>();
            var firstUserDraft = await firstUser.LoadAsync();
            Assert.Null(firstUserDraft.BlockedReason);
            var firstUserCreated = await firstUser.CreateAsync(new FirstUserSetupRequest
            {
                ProfileId = firstUserDraft.PreferredProfileId ?? firstUserDraft.Profiles[0].Id,
                Name = "Administrador",
                Login = "admin",
                AccessPin = "1234",
                AccessPinConfirm = "1234"
            });
            Assert.True(firstUserCreated.Ok, firstUserCreated.Message);

            var company = services.GetRequiredService<ICompanySetupService>();
            var companyDraft = await company.LoadAsync();
            var companySaved = await company.SaveAsync(new CompanySetupRequest
            {
                CountryId = companyDraft.CountryId ?? Guid.Empty,
                CurrencyId = companyDraft.CurrencyId ?? Guid.Empty,
                Values = LocalCompanySetupService.DemoCompanyValues
            });
            Assert.True(companySaved.Succeeded, companySaved.Error);

            var wizard = services.GetRequiredService<IFiscalYearWizard>();
            var draft = await wizard.LoadAsync();
            var setup = await wizard.CreateAsync(new FiscalYearSetupRequest
            {
                Designation = draft.Designation,
                Year = draft.Year,
                Acronym = draft.Acronym,
                CloseCurrent = false,
                CreateSeries = true,
                SeriesForEachTerminal = false,
                CommunicateWithAt = false,
                TerminalIds = draft.Terminals.Select(item => item.Id).ToList(),
                DocumentTypeIds = draft.DocumentTypes.Select(item => item.Id).ToList()
            });
            Assert.True(setup.Ok, setup.Message);

            var status = await services.GetRequiredService<IOnboardingService>().GetStatusAsync();

            Assert.True(status.IsComplete, string.Join(", ", status.Steps.Where(s => s.IsComplete == false).Select(s => s.Key)));
            Assert.Equal(100, status.PercentComplete);
        }
        finally
        {
            Cleanup(root);
        }
    }

    [Fact]
    public void Unchecked_facts_become_optional_and_do_not_block_setup()
    {
        var status = OnboardingService.Build(new OnboardingFacts
        {
            CompanyComplete = true,
            UserCount = 2,
            HasFiscalYearAndSeries = true,
            HasArticle = null,
            HasFinalConsumer = true,
            HasPlace = true,
            HasTable = true,
            HasPaymentMethod = null,
            HasTerminalPrinter = null
        });

        Assert.True(status.IsComplete);
        var article = status.Steps.Single(step => step.Key == "article");
        Assert.True(article.IsOptional);
        Assert.False(article.IsComplete);
        Assert.Equal("Não foi possível verificar", article.StatusText);
        Assert.True(status.Steps.Single(step => step.Key == "payment-method").IsOptional);
        Assert.Equal("Não foi possível verificar", status.Steps.Single(step => step.Key == "printer").StatusText);
    }

    private static void ConfigureApp(DirectoryInfo root, string module)
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
