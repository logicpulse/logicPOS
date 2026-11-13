using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Services.DemoData;

/// <summary>
/// Ensures «Usar dados demo» preferences exist (hidden last-run marker + gated checkbox).
/// </summary>
public static class DemoDataPreferenceSeeder
{
    private static readonly Guid EnabledId = new("c3e8a1f2-7b4d-4e9a-9c2f-1a5d8e0b3f47");
    private static readonly Guid LastGeneratedId = new("d4f9b203-8c5e-4f0b-a13e-2b6e9f1c4a58");

    public static async Task EnsureAsync(LogicPOSDbContext database, CancellationToken ct)
    {
        var existing = await database.PreferenceParameters
            .Where(item => item.IsDeleted == false)
            .Select(item => item.Token)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        var now = DateTime.Now;
        var added = false;

        if (existing.Contains(DemoDataParameterTokens.Enabled) == false)
        {
            database.PreferenceParameters.Add(new PreferenceParameter
            {
                Id = EnabledId,
                Token = DemoDataParameterTokens.Enabled,
                Value = "False",
                Required = false,
                RegEx = "RegexBoolean",
                ResourceString = "Usar dados demo",
                ValueTip =
                    "Requer conta em modo demo (cloud «Conta em modo demo» / licença local Demo). Quando activo, gera automaticamente actividade de faturação realista para cobrir Hoje, 7 dias, 30 dias, este mês e este ano, com base nos artigos e terminais existentes.",
                FormType = 2,
                FormPageNo = 1,
                InputType = PreferenceParameterInputType.CheckButton,
                Order = 320,
                Code = "320",
                CreatedAt = now,
                UpdatedAt = now
            });
            added = true;
        }

        if (existing.Contains(DemoDataParameterTokens.LastGeneratedDate) == false)
        {
            database.PreferenceParameters.Add(new PreferenceParameter
            {
                Id = LastGeneratedId,
                Token = DemoDataParameterTokens.LastGeneratedDate,
                Value = string.Empty,
                Required = false,
                RegEx = string.Empty,
                ResourceString = "Demo data last generated",
                FormType = 0,
                FormPageNo = 0,
                InputType = PreferenceParameterInputType.Text,
                Order = 321,
                Code = "321",
                CreatedAt = now,
                UpdatedAt = now
            });
            added = true;
        }

        if (added)
            await database.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
