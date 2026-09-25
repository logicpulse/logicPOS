using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.UI.Services;
using System;

namespace LogicPOS.UI.Components.Finance
{
    /// <summary>
    /// AGT line rounding (Angola + FE module), same as the API: only for articles priced without VAT.
    /// Line net is truncated to 2 decimals and tax is rounded up to the next cent.
    /// </summary>
    public static class AgtLineRounding
    {
        public static bool Applies(ArticleViewModel article)
            => SystemInformationService.UseAgtFe && article?.PriceWithVat != true;

        public static decimal Truncate2(decimal value) => Math.Truncate(value * 100M) / 100M;

        public static decimal Ceiling2(decimal value) => Math.Ceiling(value * 100M) / 100M;
    }
}
