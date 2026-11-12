using LogicPOS.Core.Authentication;
using LogicPOS.Core.BackOffice;
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
