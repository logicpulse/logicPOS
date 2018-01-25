namespace logicpos.shared.Enums
{
    public enum PersistFinanceDocumentSourceMode
    {
        CurrentOrderMain,         //From OrderMain, use OrderMain
        CustomArticleBag,         //From Finance Document, Like Invoice > Credit Note
        CurrentAcountDocuments    //From Documents, Like Current Account Documents, used to loop ArticleBag and Create Documents
    }
}
