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

        private string _designation;
        public string Designation
        {
            get { return _designation; }
            set { _designation = value; }
        }

        private PriceProperties _properties;
        public PriceProperties Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }

        //this TreeIter as object Type, to Remove Gtk from Framework, need to be cast as (TreeIter as TreeIter) when used
        private object _treeIter;
        [JsonIgnore]
        public object TreeIter
        {
            get { return _treeIter; }
            set { _treeIter = value; }
        }

        //public OrderDetailLine(Guid pArticleOid, String pDesignation, decimal pQnt, decimal pPrice, decimal pDiscount, decimal pVat, bool pHasPrice, bool pPriceWithVAT)
        public OrderDetailLine(Guid pArticleOid, String pDesignation, PriceProperties pPriceProperties)
        {
            _articleOid = pArticleOid;
            _designation = pDesignation;
            _properties = pPriceProperties;
        }
    }
}
