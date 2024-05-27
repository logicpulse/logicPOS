namespace logicpos.datalayer.Enums
{
    public enum SaftDocumentType
    {
        None,
        //4.1: Documentos comerciais a clientes (SalesInvoices);
        SalesInvoices,
        //4.2: Documentos de movimentação de mercadorias (MovementOfGoods); 
        MovementOfGoods,
        //4.3: Documentos de conferência de entrega de mercadorias ou da prestação de serviços (WorkingDocuments).
        WorkingDocuments,
        //4.4: Documentos de recibos emitidos
        Payments
    }
}
