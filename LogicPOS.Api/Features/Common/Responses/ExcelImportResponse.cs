using System.Collections.Generic;

namespace LogicPOS.Api.Features.Common.Responses
{
    public sealed class ExcelImportResponse
    {
        public int RowsFound { get; set; }
        public int Created { get; set; }
        public int Skipped { get; set; }
        public int Failed { get; set; }
        public List<ExcelImportItem> Items { get; set; } = new List<ExcelImportItem>();
    }

    public sealed class ExcelImportItem
    {
        public int RowNumber { get; set; }
        public string Key { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
    }
}
