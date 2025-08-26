using LogicPOS.Api.Entities;
using LogicPOS.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.Api.Features.POS.WorkSessions.Movements.GetDayReportData
{
    public class DayReportData
    {
        public WorkSessionPeriod Day { get; set; }
        public List<Movement> Movements { get; set; }
        public decimal OpeningCashTotal { get; set; }
        public decimal EndOfDayCashTotal { get; set; }
        public decimal CashDrawerIn { get; set; }
        public decimal CashDrawerOut { get; set; }

        public List<(string Method, decimal Quantity , decimal Total)> GetTotalPerPaymentMethod()
        {
            return Movements.Where(m => m.PaymentMethod != null)
                .GroupBy(m => m.PaymentMethod)
                .Select(g => new ValueTuple<string,decimal, decimal>(g.Key, g.Sum(x => x.ArticleQuantity),g.Sum(x => x.MovementAmount))).ToList();
        }

        public List<(string Family, decimal Quantity, decimal Total)> GetTotalPerFamily()
        {
            return Movements.Where(m => m.ArticleFamily != null)
                .GroupBy(m => m.ArticleFamily)
                .Select(g => new ValueTuple<string, decimal,decimal>(g.Key, g.Sum(x => x.ArticleQuantity), g.Sum(x => x.MovementAmount))).ToList();
        }

        public List<(string Subfamily, decimal Quantity, decimal Total)> GetTotalPerSubfamily()
        {
            return Movements.Where(m => m.ArticleSubfamily != null)
                .GroupBy(m => m.ArticleSubfamily)
                .Select(g => new ValueTuple<string, decimal, decimal>(g.Key, g.Sum(x => x.ArticleQuantity), g.Sum(x => x.MovementAmount))).ToList();
        }


        public List<(string Tax, decimal Quantity, decimal Total)> GetTotalPerTax()
        {
            return Movements.Where(m => m.Tax != null)
                .GroupBy(m => m.Tax)
                .Select(g => new ValueTuple<string, decimal, decimal>(g.Key, g.Sum(x => x.ArticleQuantity), g.Sum(x => x.MovementAmount))).ToList();
        }

        public List<(string Article, decimal Quantity, decimal Total)> GetTotalPerArticle()
        {
            return Movements.Where(m => m.Article != null)
                .GroupBy(m => m.Article)
                .Select(g => new ValueTuple<string, decimal, decimal>(g.Key, g.Sum(x => x.ArticleQuantity), g.Sum(x => x.MovementAmount))).ToList();
        }

        public List<(string User, decimal Quantity, decimal Total)> GetTotalPerUser()
        {
            return Movements.Where(m => m.User != null)
                .GroupBy(m => m.User)
                .Select(g => new ValueTuple<string, decimal, decimal>(g.Key, g.Sum(x => x.ArticleQuantity), g.Sum(x => x.MovementAmount))).ToList();
        }


        public List<(string DocumentType, decimal Quantity, decimal Total)> GetTotalPerDocumentType()
        {
            return Movements.Where(m => m.DocumentType != null)
                .GroupBy(m => m.DocumentType)
                .Select(g => new ValueTuple<string, decimal, decimal>(g.Key, g.Sum(x => x.ArticleQuantity), g.Sum(x => x.MovementAmount))).ToList();
        }

        public List<(string Hour, decimal Quantity, decimal Total)> GetTotalPerHour()
        {
            return Movements.Where(m => m.DocumentType != null)
                .GroupBy(m => m.Date.Hour.ToString())
                .Select(g => new ValueTuple<string, decimal, decimal>(g.Key, g.Sum(x => x.ArticleQuantity), g.Sum(x => x.MovementAmount))).ToList();
        }

        public decimal DocumentsTotal => Movements.Where(m => m.Type == WorkSessionMovementType.Document).Sum(m => m.MovementAmount);

    }

    public class Movement
    {
        public DateTime Date { get; set; }
        public decimal MovementAmount { get; set; }
        public WorkSessionMovementType Type { get; set; }
        public string DocumentType { get; set; }
        public string PaymentMethod { get; set; }
        public string Article { get; set; }
        public decimal ArticleQuantity { get; set; }
        public string User { get; set; }
        public string ArticleFamily { get; set; }
        public string ArticleSubfamily { get; set; }
        public string Tax { get; set; }
    }


}
