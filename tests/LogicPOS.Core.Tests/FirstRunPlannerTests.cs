using LogicPOS.Core;

namespace LogicPOS.Core.Tests;

public sealed class FirstRunPlannerTests
{
    [Fact]
    public void Community_stays_local_with_no_download_and_no_browser()
    {
        var plan = FirstRunPlanner.Plan(ProductEdition.Community, ProductHosting.Local);

        Assert.Equal(ProductEdition.Community, plan.Edition);
        Assert.Equal(ProductHosting.Local, plan.Hosting);
        Assert.False(plan.DownloadApi);
        Assert.False(plan.DownloadWebapp);
        Assert.True(plan.AvaloniaStaysLocal);
        Assert.False(plan.OpenBrowser);
        Assert.False(plan.CallLicenceServer);
        Assert.Empty(plan.DownloadUrls);
    }

    [Fact]
    public void Upgrade_without_edition_is_not_inferred_as_business_or_cloud()
    {
        var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder().Build();
        var inferred = ProductEditionSettings.InferLegacy(configuration, Path.GetTempPath());

        Assert.Equal(ProductEdition.Community, inferred.Edition);
        Assert.Equal(ProductHosting.Local, inferred.Hosting);
        Assert.False(ProductEditionSettings.HasEdition(configuration));
    }

    [Fact]
    public void Business_server_without_licence_does_not_download()
    {
        var plan = FirstRunPlanner.Plan(
            ProductEdition.Business,
            ProductHosting.LocalServer,
            licenceValid: false);

        Assert.False(plan.DownloadApi);
        Assert.False(plan.DownloadWebapp);
        Assert.True(plan.AvaloniaStaysLocal);
        Assert.False(plan.OpenBrowser);
        Assert.True(plan.CallLicenceServer);
        Assert.Empty(plan.DownloadUrls);
    }

    [Fact]
    public void Business_server_with_licence_downloads_api_and_webapp_and_opens_browser()
    {
        var plan = FirstRunPlanner.Plan(
            ProductEdition.Business,
            ProductHosting.LocalServer,
            licenceValid: true);

        Assert.True(plan.DownloadApi);
        Assert.True(plan.DownloadWebapp);
        Assert.True(plan.AvaloniaStaysLocal);
        Assert.True(plan.OpenBrowser);
        Assert.Equal(
            [FirstRunPlanner.ApiZipUrl, FirstRunPlanner.WebappZipUrl],
            plan.DownloadUrls);
    }

    [Fact]
    public void Modo_api_downloads_nothing()
    {
        var plan = FirstRunPlanner.Plan(
            ProductEdition.Business,
            ProductHosting.RemoteApi,
            licenceValid: true);

        Assert.False(plan.DownloadApi);
        Assert.False(plan.DownloadWebapp);
        Assert.False(plan.AvaloniaStaysLocal);
        Assert.False(plan.OpenBrowser);
        Assert.Empty(plan.DownloadUrls);
    }

    [Fact]
    public void Cloud_avalonia_only_downloads_nothing()
    {
        var plan = FirstRunPlanner.Plan(
            ProductEdition.Cloud,
            ProductHosting.Cloud,
            CloudClientSurface.Avalonia,
            clientIdAccepted: true);

        Assert.False(plan.DownloadApi);
        Assert.False(plan.DownloadWebapp);
        Assert.False(plan.OpenBrowser);
        Assert.Empty(plan.DownloadUrls);
    }

    [Fact]
    public void Cloud_with_webapp_downloads_only_webapp()
    {
        var plan = FirstRunPlanner.Plan(
            ProductEdition.Cloud,
            ProductHosting.Cloud,
            CloudClientSurface.Both,
            clientIdAccepted: true);

        Assert.False(plan.DownloadApi);
        Assert.True(plan.DownloadWebapp);
        Assert.True(plan.OpenBrowser);
        Assert.Equal([FirstRunPlanner.WebappZipUrl], plan.DownloadUrls);
    }
}
