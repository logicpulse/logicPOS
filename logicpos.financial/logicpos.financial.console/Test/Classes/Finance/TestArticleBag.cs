using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.console.App;
using logicpos.datalayer.Enums;
using logicpos.shared.Classes.Finance;
using System;
using System.Collections.Generic;

namespace logicpos.financial.console.Test.Classes
{
    public class TestArticleBag
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ArticleBag GetArticleBag(bool pForceErrors)
        {
            ArticleBag articleBag = new ArticleBag(10);
            Guid xpoOidConfigurationPlaceDefault = new Guid(GlobalFramework.Settings["xpoOidConfigurationPlaceDefault"]);
            Guid xpoOidConfigurationPlaceTableDefaultOpenTable = new Guid(GlobalFramework.Settings["xpoOidConfigurationPlaceTableDefaultOpenTable"]);
            ArticleBagKey articleBagKey;
            ArticleBagProperties articleBagProps;

            Dictionary<Guid, decimal> mockArticles = new Dictionary<Guid, decimal>();
            //P:Products
            mockArticles.Add(new Guid("133cc225-517d-4c24-88b0-cd7c08cf5727"), 2.0m);
            mockArticles.Add(new Guid("4c47be72-6174-4e63-a077-f3cdb6a15e97"), 3.0m);
            mockArticles.Add(new Guid("0f32da9c-e533-489d-8a46-d6da79fd63a0"), 3.0m);
            mockArticles.Add(new Guid("6b547918-769e-4f5b-bcd6-01af54846f73"), 4.0m);
            mockArticles.Add(new Guid("42cd7f86-97b2-44f4-b098-3c9f0ae9f4b5"), 5.0m);
            mockArticles.Add(new Guid("55892c3f-de10-4076-afde-619c54100c9b"), 6.0m);
            mockArticles.Add(new Guid("fc109711-edb0-41dc-87b6-0acb77abd341"), 7.0m);
            mockArticles.Add(new Guid("bf99351b-1556-43c4-a85c-90082fb02d05"), 8.0m);
            mockArticles.Add(new Guid("11062ec9-fed0-43eb-a23e-c6f7ed83ff72"), 9.0m);
            mockArticles.Add(new Guid("32deb30d-ffa2-45e4-bca6-03569b9e8b08"), 2.0m);
            mockArticles.Add(new Guid("78638720-e728-4e96-8643-6d6267ff817b"), 2.0m);
            mockArticles.Add(new Guid("42c327e2-4aad-41ea-b5b6-e2198c337f1c"), 3.0m);
            mockArticles.Add(new Guid("0d30bf31-ecc4-452e-9b43-ee9d5c1d7fb6"), 4.0m);
            mockArticles.Add(new Guid("7b45a01d-50ee-42d3-a4af-0dcde9397e93"), 5.0m);
            mockArticles.Add(new Guid("630ff869-e433-46bb-a53b-563c43535424"), 6.0m);
            mockArticles.Add(new Guid("f71b3648-bb41-4952-ac75-ee93ccf0ec66"), 7.0m);
            mockArticles.Add(new Guid("87ff6f3a-c858-4829-bbcb-c6ea395129da"), 8.0m);
            mockArticles.Add(new Guid("72e8bde8-d03b-4637-90f1-fcb265658af0"), 9.0m);
            //S:Services
            mockArticles.Add(new Guid("5a852060-43b9-4e71-a230-b733bb150427"), 3.0m);
            mockArticles.Add(new Guid("072db1bf-6182-43de-8065-d4bbd8c9f8c2"), 4.0m);

            foreach (var item in mockArticles)
            {
                fin_article article = (fin_article)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(fin_article), item.Key);

                articleBagKey = new ArticleBagKey(
                  article.Oid,
                  article.Designation,
                  article.Price1,
                  article.Discount,
                  article.VatOnTable.Value
                );
                articleBagProps = new ArticleBagProperties(
                  xpoOidConfigurationPlaceDefault,
                  xpoOidConfigurationPlaceTableDefaultOpenTable,
                  PriceType.Price1,
                  article.Code,
                  item.Value,
                  article.UnitMeasure.Acronym
                );

                if (! pForceErrors)
                {
                    //Detect and Add TaxExceptionReason if Miss to Prevent Errors
                    if (articleBagKey.Vat == 0.0m && articleBagKey.VatExemptionReasonOid == Guid.Empty)
                    {
                        articleBagKey.VatExemptionReasonOid = SettingsApp.XpoOidConfigurationVatExemptionReasonM99;
                    }

                    //Add Price to Services, else we have Error with Price 0
                    if (article.Class.Acronym == "S")
                    {
                        articleBagKey.Price = 10.28m;
                    }
                }

                //Send to Bag
                articleBag.Add(articleBagKey, articleBagProps);
            }

            if (pForceErrors)
            {
                //Add Error Article after Loop
                articleBagKey = new ArticleBagKey(
                    Guid.Empty, //Oid
                    "§",        //Designation
                    -1,         //Price
                    101,        //Discount
                    0           //VatOnTable
                );
                articleBagProps = new ArticleBagProperties(
                    xpoOidConfigurationPlaceDefault,
                    xpoOidConfigurationPlaceTableDefaultOpenTable,
                    PriceType.Price1,
                    string.Empty,   //Code
                    -1,             //Quantity
                    "§"             //UnitMeasure.Acronym
                );
                //Add Error Article
                articleBag.Add(articleBagKey, articleBagProps);
                //Assign Error after add To ArticleBag
                articleBagKey.Vat = -1;
            }

            return articleBag;
        }

        public static void ShowArticleBag()
        {
            Guid articleClassProducts = new Guid("6924945d-f99e-476b-9c4d-78fb9e2b30a3");
            Guid articleClassServices = new Guid("7622e5d2-2d52-4be9-bb8b-e5efae5ec791");

            ArticleBag articleBag = TestArticleBag.GetArticleBag(false);

            foreach (var item in articleBag)
            {
                //_log.Debug(string.Format("{0} x {1}", item.Key.Designation, item.Value.Quantity));
                Console.WriteLine(string.Format("{0} x {1}", item.Key.Designation, item.Value.Quantity));
            }

            //Test GetClassTotals
            Dictionary<string,decimal> classTotals = articleBag.GetClassTotals();

            //Show Result
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("TotalFinal P: [{0}]", classTotals["P"]);
            Console.WriteLine("TotalFinal S: [{0}]", classTotals["S"]);
            Console.WriteLine("ArticleBag TotalFinal: [{0}], TotalPAndS: [{1}]", articleBag.TotalFinal, classTotals["P"] + classTotals["S"]);
        }
    }
}
