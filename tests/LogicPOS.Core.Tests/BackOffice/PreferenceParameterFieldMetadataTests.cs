using LogicPOS.Core.BackOffice;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;

namespace LogicPOS.Core.Tests.BackOffice;

public sealed class PreferenceParameterFieldMetadataTests
{
    [Fact]
    public void Apply_UpgradesAtPasswordTokenToTextPassword()
    {
        var row = new PreferenceParameter
        {
            Token = "SERVICE_AT_PRODUCTION_ACCOUNT_PASSWORD",
            InputType = PreferenceParameterInputType.Text,
        };

        PreferenceParameterFieldMetadata.Apply(row);

        Assert.Equal(PreferenceParameterInputType.TextPassword, row.InputType);
    }

    [Theory]
    [InlineData("SERVICE_AT_PRODUCTION_ACCOUNT_PASSWORD", true)]
    [InlineData("SERVICE_AT_PRODUCTION_ACCOUNT_FISCAL_NUMBER", false)]
    public void IsPasswordToken_RecognizesAtCredential(string token, bool expected)
        => Assert.Equal(expected, PreferenceParameterFieldMetadata.IsPasswordToken(token));
}