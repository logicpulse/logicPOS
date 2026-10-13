using System.Globalization;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using LogicPOS.Application.Features.System;
using LogicPOS.Core;
using LogicPOS.Core.BackOffice;
using LogicPOS.Core.Fiscal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Views;

/// <summary>
/// Shared SAF-T export UI: run fiscal export, save to PATH_SAFTPT (or SaveFilePicker), format notices.
/// Angola shows AGT wording; actions remain saft-* (API picks AngolaSaftBuilder).
/// </summary>
internal static class SaftExportUi
{
    public static Task<ListingSaveResult> RunExportAsync(
        IBackOfficeListingService service,
        string action,
        string? extra)
        => service.RunActionAsync(action, null, extra);

    /// <summary>
    /// Resolves PATH_SAFTPT / SaveAs and builds the user-facing success or error notice.
    /// </summary>
    public static async Task<string> FormatResultAsync(ListingSaveResult result, TopLevel? top)
    {
        if (result.Succeeded == false)
        {
            if (UseAgtLabels())
            {
                return string.IsNullOrWhiteSpace(result.Error)
                    ? "Erro ao Exportar o Ficheiro de AGT!"
                    : result.Error;
            }

            return PreferenceLabels.Text(
                "dialog_message_saftpt_exported_error",
                result.Error ?? "Erro ao Exportar o Ficheiro de SAF-T!");
        }

        var sourcePath = result.Message;
        if (string.IsNullOrWhiteSpace(sourcePath) || File.Exists(sourcePath) == false)
        {
            var completed = result.Message ?? "Operação fiscal concluída.";
            if (string.IsNullOrWhiteSpace(result.Detail) == false)
            {
                completed = completed + Environment.NewLine + Environment.NewLine + result.Detail;
            }

            return completed;
        }

        var savedPath = sourcePath;
        if (result.NeedsSaveAs)
        {
            var picked = await PickAndSaveAsync(top, sourcePath);
            if (picked.Path is null)
            {
                return picked.Error ?? ExportCancelledText();
            }

            savedPath = picked.Path;
        }

        var notice = string.Format(CultureInfo.CurrentCulture, ExportSuccessTemplate(), savedPath);
        if (string.IsNullOrWhiteSpace(result.Detail) == false)
        {
            notice = notice + Environment.NewLine + Environment.NewLine + result.Detail;
        }

        return notice;
    }

    public static async Task<(string? Path, string? Error)> PickAndSaveAsync(TopLevel? top, string sourcePath)
    {
        if (top is null)
        {
            return (null, ExportCancelledText());
        }

        var suggested = SaftExportPaths.BuildFileName(sourcePath);
        var file = await top.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = FolderPickerTitle(),
            SuggestedFileName = suggested,
            DefaultExtension = "xml",
            FileTypeChoices =
            [
                new FilePickerFileType(XmlFileTypeLabel()) { Patterns = ["*.xml"] }
            ]
        });
        if (file is null)
        {
            return (null, ExportCancelledText());
        }

        try
        {
            var local = file.TryGetLocalPath();
            if (string.IsNullOrWhiteSpace(local) == false)
            {
                var directory = Path.GetDirectoryName(local);
                if (string.IsNullOrWhiteSpace(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }

                File.Copy(sourcePath, local, overwrite: true);
                return (local, null);
            }

            await using var destination = await file.OpenWriteAsync();
            await using var source = File.OpenRead(sourcePath);
            await source.CopyToAsync(destination);
            return (file.Name ?? suggested, null);
        }
        catch (Exception exception)
        {
            return (null, ExportErrorText() + Environment.NewLine + exception.Message);
        }
    }

    public static bool IsMenuTitle(string title)
        => title is "SAF-T ano" or "SAF-T último mês" or "SAF-T período"
            or "AGT ano" or "AGT último mês" or "AGT período";

    public static string ActionForMenu(string title) => title switch
    {
        "SAF-T ano" or "AGT ano" => "saft-year",
        "SAF-T último mês" or "AGT último mês" => "saft-month",
        "SAF-T período" or "AGT período" => "saft-period",
        _ => title
    };

    /// <summary>True when fiscal country is Angola — UI says AGT, actions stay saft-*.</summary>
    public static bool UseAgtLabels()
    {
        var system = AppComposition.Services?.GetService<ISystemInformationService>();
        if (system is not null)
        {
            return system.IsAngola();
        }

        var country = AppComposition.Services?.GetService<IConfiguration>()?["Country"];
        return string.Equals(country, "AO", StringComparison.OrdinalIgnoreCase)
            || string.Equals(country, "Angola", StringComparison.OrdinalIgnoreCase);
    }

    public static string ExportDialogTitle()
        => UseAgtLabels()
            ? "Exportar AGT"
            : PreferenceLabels.Text("global_export_saftpt", "Exportar SAF-T");

    public static string CustomPeriodTitle()
        => UseAgtLabels()
            ? "AGT Personalizado"
            : PreferenceLabels.Text("global_export_saftpt_custom", "SAF-T Personalizado");

    public static string ConfirmYearMessage()
        => UseAgtLabels()
            ? "Exportar o AGT do ano corrente?"
            : "Exportar o SAF-T do ano corrente?";

    public static string ConfirmMonthMessage()
        => UseAgtLabels()
            ? "Exportar o AGT do mês anterior?"
            : "Exportar o SAF-T do mês anterior?";

    public static string ExportErrorText()
        => UseAgtLabels()
            ? "Erro ao Exportar o Ficheiro de AGT!"
            : PreferenceLabels.Text(
                "dialog_message_saftpt_exported_error",
                "Erro ao Exportar o Ficheiro de SAF-T!");

    private static string ExportSuccessTemplate()
        => UseAgtLabels()
            ? "AGT Exportado para" + Environment.NewLine + "{0}"
            : PreferenceLabels.Text(
                "dialog_message_saftpt_exported_successfully",
                "SAF-T Exportado para" + Environment.NewLine + "{0}");

    private static string ExportCancelledText()
        => UseAgtLabels() ? "Exportação AGT cancelada." : "Exportação SAF-T cancelada.";

    private static string FolderPickerTitle()
        => UseAgtLabels()
            ? "Pasta :: Exportação do AGT"
            : PreferenceLabels.Text("prefparam_path_saftpt", "Pasta :: Exportação do Saft");

    private static string XmlFileTypeLabel()
        => UseAgtLabels() ? "AGT XML" : "SAF-T XML";
}
