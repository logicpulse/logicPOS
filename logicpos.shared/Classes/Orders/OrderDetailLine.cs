using logicpos.shared.Classes.Finance;
using Newtonsoft.Json;
using System;

namespace logicpos.shared.Classes.Orders
{
    public class OrderDetailLine
    {
        //Public Properties
        private Guid _articleOid;
        public Guid ArticleOid
        {
            get { return _articleOid; }
            set { _articleOid = value; }
        }

        public string Designation { get; set; }

        public PriceProperties Properties { get; set; }

        [JsonIgnore]
        public object TreeIter { get; set; }

        //public OrderDetailLine(Guid pArticleOid, String pDesignation, decimal pQnt, decimal pPrice, decimal pDiscount, decimal pVat, bool pHasPrice, bool pPriceWithVAT)
        public OrderDetailLine(Guid pArticleOid, string pDesignation, PriceProperties pPriceProperties)
        {
            _articleOid = pArticleOid;
            Designation = pDesignation;
            Properties = pPriceProperties;
        }
    }
}
