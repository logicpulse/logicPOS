namespace LogicPOS.Core;

/// <summary>
/// External behaviour of the first-run edition choice: what to download, how the post is hosted,
/// whether the browser opens. The Avalonia window and download installer follow this plan.
/// </summary>
public sealed record FirstRunPlan(
    ProductEdition Edition,
    ProductHosting Hosting,
    bool DownloadApi,
    bool DownloadWebapp,
    bool AvaloniaStaysLocal,
    bool OpenBrowser,
    bool CallLicenceServer,
    IReadOnlyList<string> DownloadUrls);

public static class FirstRunPlanner
{
    public const string ApiZipUrl = "https://box.track.pt/files/latest/logicpos_api.zip";
    public const string WebappZipUrl = "https://box.track.pt/files/latest/logicpos_portal.zip";
    public const string ProductionApiAddress = "https://api.logic-pos.com/";

    public static FirstRunPlan Plan(
        ProductEdition edition,
        ProductHosting hosting,
        CloudClientSurface cloudSurface = CloudClientSurface.Avalonia,
        bool licenceValid = false,
        bool clientIdAccepted = false)
    {
        return edition switch
        {
            ProductEdition.Community => Community(),
            ProductEdition.Business => Business(hosting, licenceValid),
            ProductEdition.Cloud => Cloud(cloudSurface, clientIdAccepted),
            _ => Community(),
        };
    }

    private static FirstRunPlan Community() => new(
        ProductEdition.Community,
        ProductHosting.Local,
        DownloadApi: false,
        DownloadWebapp: false,
        AvaloniaStaysLocal: true,
        OpenBrowser: false,
        CallLicenceServer: false,
        DownloadUrls: []);

    private static FirstRunPlan Business(ProductHosting hosting, bool licenceValid)
    {
        if (hosting == ProductHosting.RemoteApi)
        {
            return new FirstRunPlan(
                ProductEdition.Business,
                ProductHosting.RemoteApi,
                DownloadApi: false,
                DownloadWebapp: false,
                AvaloniaStaysLocal: false,
                OpenBrowser: false,
                CallLicenceServer: true,
                DownloadUrls: []);
        }

        // Local server (this PC). Download only after the ficheiro de licença is valid.
        if (licenceValid == false)
        {
            return new FirstRunPlan(
                ProductEdition.Business,
                ProductHosting.LocalServer,
                DownloadApi: false,
                DownloadWebapp: false,
                AvaloniaStaysLocal: true,
                OpenBrowser: false,
                CallLicenceServer: true,
                DownloadUrls: []);
        }

        return new FirstRunPlan(
            ProductEdition.Business,
            ProductHosting.LocalServer,
            DownloadApi: true,
            DownloadWebapp: true,
            AvaloniaStaysLocal: true,
            OpenBrowser: true,
            CallLicenceServer: true,
            DownloadUrls: [ApiZipUrl, WebappZipUrl]);
    }

    private static FirstRunPlan Cloud(CloudClientSurface surface, bool clientIdAccepted)
    {
        if (clientIdAccepted == false)
        {
            return new FirstRunPlan(
                ProductEdition.Cloud,
                ProductHosting.Cloud,
                DownloadApi: false,
                DownloadWebapp: false,
                AvaloniaStaysLocal: false,
                OpenBrowser: false,
                CallLicenceServer: false,
                DownloadUrls: []);
        }

        var wantsWebapp = surface is CloudClientSurface.Webapp or CloudClientSurface.Both;
        return new FirstRunPlan(
            ProductEdition.Cloud,
            ProductHosting.Cloud,
            DownloadApi: false,
            DownloadWebapp: wantsWebapp,
            AvaloniaStaysLocal: false,
            OpenBrowser: wantsWebapp,
            CallLicenceServer: false,
            DownloadUrls: wantsWebapp ? [WebappZipUrl] : []);
    }
}
