using LogicPOS.Application.Features.Finance.Documents.PdfGeneration;
using LogicPOS.Domain.Entities.Finance.Calculations;
using LogicPOS.Domain.ValueObjects;
using QrCodes;
using QrCodes.Renderers;
using QrCodes.Renderers.Abstractions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;
using ZXing.SkiaSharp.Rendering;
using Color = QuestPDF.Infrastructure.Color;

namespace LogicPOS.Core.FrontOffice.Pdf;

public abstract class DocumentModel : IDocument
{
    public int Copy { get; set; } = 1;
    public bool IsSecondCopy;
    private int ItemIndex { get; set; }
    protected DocumentPdfData Data { get; init; }
    private int PagesCount { get; set; } = 1;
    private decimal ValueToTransport { get; set; }
    private decimal TransportedValue { get; set; }
    private int PrintedDetailsCount { get; set; }
    private int CurrentPageIndex { get; set; }
    private int DetailsCount { get; }
    private string? References { get; set; }
    private bool IsAngola;
    protected DocumentModel(DocumentPdfData data)
    {
        Data = data;
        DetailsCount = Data.Document!.Details.Count;
        _ = Data.Document.Details.OrderByDescending(x => x.Order);
        IsAngola = Data.Country.IsAngola;
    }

    private bool IsGuide()
    {
        return Data.Document!.Type == "GT" || Data.Document.Type == "GD" || Data.Document.Type == "GR" ||
               Data.Document.Type == "GC" || Data.Document.Type == "GA";
    }

    private string GetDocumentCopyName()
    {
        return Copy switch
        {
            1 => "Original",
            2 => "Duplicado",
            3 => "Triplicado",
            4 => "Quadruplicado",
            _ => "Original"
        };
    }

    private protected string? GetReferences()
    {
        References = Data.Document!.RelatedDocuments != null
            ? string.Join(", ", Data.Document.RelatedDocuments)
            : string.Empty;
        return References;
    }

    private void CalculatePages()
    {
        if (DetailsCount % 10 == 0)
        {
            PagesCount = DetailsCount / 10;
        }
        else
        {
            PagesCount = (DetailsCount / 10) + 1;
        }
    }

    public void Compose(IDocumentContainer container)
    {
        CalculatePages();

        for (int i = 0; i < PagesCount; i++)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(TextStyle.Default.FontFamily("Arial").FontSize(8));
                DesignHeader(page);
                DesignBody(page);
                DesignFooter(page);
            });
        }
    }

    private void DesignBody(PageDescriptor page)
    {
        page.Content().AlignCenter().Column(xcolumn =>
        {
            if (Data.Document!.Status == "A")
            {
                ApplyWaterMark(xcolumn, "Anulado");
            }
            else if (Data.Document.IsDraft)
            {
                ApplyWaterMark(xcolumn, "Rascunho");
            }


            xcolumn.Item().Table(markedTable =>
            {
                markedTable.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(2, Unit.Centimetre);
                    columns.ConstantColumn(5, Unit.Centimetre);
                    columns.ConstantColumn(1, Unit.Centimetre);
                    columns.ConstantColumn(1, Unit.Centimetre);
                    columns.ConstantColumn(2, Unit.Centimetre);
                    columns.ConstantColumn(1.5f, Unit.Centimetre);
                    columns.ConstantColumn(1.5f, Unit.Centimetre);
                    columns.ConstantColumn(2, Unit.Centimetre);
                    columns.ConstantColumn(2, Unit.Centimetre);
                });

                markedTable.Header(header =>
                {
                    header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                        .Text("Código").Bold().FontSize(8);

                    header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                        .Text("Designação").Bold().FontSize(8);

                    header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                        .Text("Qnt").Bold().FontSize(8);
                    header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                        .Text("Uni.").Bold().FontSize(8);

                    header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                        .Text("Preço").Bold().FontSize(8);

                    header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                        .Text("Desc (%)").Bold().FontSize(8);
                    header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                        .Text("Taxa (%)").Bold().FontSize(8);

                    header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                        .Text("Total").Bold().FontSize(8);

                    header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                        .Text("Total c/ IVA").Bold().FontSize(8);
                });

                for (int x = 0; x < 10; x++)
                {
                    if (ItemIndex < DetailsCount)
                    {
                        var item = Data.Document.Details[ItemIndex];
                        float borderBottomSize = 0.5f;
                        markedTable.Cell().ColumnSpan(9).MinHeight(1f, Unit.Centimetre).Border(borderBottomSize)
                            .BorderColor(Color.FromRGB(192, 194, 196)).ExtendHorizontal().Table(itemTable =>
                            {
                                itemTable.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(2, Unit.Centimetre);
                                    columns.ConstantColumn(5, Unit.Centimetre);
                                    columns.ConstantColumn(1, Unit.Centimetre);
                                    columns.ConstantColumn(1, Unit.Centimetre);
                                    columns.ConstantColumn(2, Unit.Centimetre);
                                    columns.ConstantColumn(1.5f, Unit.Centimetre);
                                    columns.ConstantColumn(1.5f, Unit.Centimetre);
                                    columns.ConstantColumn(2, Unit.Centimetre);
                                    columns.ConstantColumn(2, Unit.Centimetre);
                                });


                                itemTable.Cell().PaddingVertical(2).AlignCenter().Text(item.Code).Bold();
                                itemTable.Cell().PaddingVertical(2).PaddingHorizontal(2).AlignLeft()
                                    .Text(item.Designation);
                                itemTable.Cell().PaddingVertical(2).AlignCenter().Text(item.Quantity.ToString("F2"));
                                itemTable.Cell().PaddingVertical(2).AlignCenter().Text(item.Unit);
                                itemTable.Cell().Padding(2).AlignRight().Text($"{item.Price:N2}");
                                itemTable.Cell().Padding(2).AlignRight().Text($"{item.Discount:N2}");
                                itemTable.Cell().Padding(2).AlignRight().Text($"{item.Tax.Percentage:N2}");
                                itemTable.Cell().Padding(2).AlignRight().Text(IsAngola?$"{ item.TotalNet.TruncateDecimal():N2}" : $"{item.TotalNet:N2}");
                                itemTable.Cell().Padding(2).AlignRight().Text($"{item.TotalFinal:N2}").Bold();

                                if (!string.IsNullOrEmpty(item.SerialNumber))
                                {
                                    itemTable.Cell().ColumnSpan(9).ExtendHorizontal().MaxWidth(15, Unit.Centimetre)
                                        .TranslateX(2.1f, Unit.Centimetre).AlignTop().Text("SN: "+item.SerialNumber).FontSize(6);
                                }
                                itemTable.Cell().ColumnSpan(9).ExtendHorizontal().MaxWidth(15, Unit.Centimetre)
                                    .TranslateX(2.1f, Unit.Centimetre).AlignTop().Text(item.Notes).FontSize(6);

                                if (item.ArticleClassAcronym == "S" && IsAngola)
                                {
                                    itemTable.Cell().ColumnSpan(9).MaxWidth(15, Unit.Centimetre)
                                        .TranslateX(2.1f, Unit.Centimetre).AlignTop().Text("(Há Retenção na fonte)")
                                        .FontSize(6);
                                }

                                if (item.Tax.Percentage == 0)
                                {
                                    borderBottomSize = 0f;
                                    itemTable.Cell().ColumnSpan(9).ExtendHorizontal().MaxWidth(15, Unit.Centimetre)
                                        .TranslateX(2.5f, Unit.Centimetre).AlignTop()
                                        .Text($"({item.VatExemptionReason})").FontSize(6);
                                }


                                PrintedDetailsCount++;
                                ValueToTransport += item.TotalFinal;


                                if (PrintedDetailsCount == 10 && PagesCount > 1 &&
                                    ((CurrentPageIndex + 1) != PagesCount))
                                {
                                    markedTable.Cell().ColumnSpan(9).ExtendHorizontal().Padding(3).Text(txt =>
                                    {
                                        txt.Span($"Valor a transportar: {ValueToTransport:N2}").Bold()
                                            .FontSize(7);
                                    });
                                }

                                ItemIndex++;
                            });
                    }
                }

                CurrentPageIndex++;
                PrintedDetailsCount = 0;
            });

            if (CurrentPageIndex == PagesCount)
            {
                DesignFinalTable(xcolumn);

                if (ValueToTransport != TransportedValue)
                {
                    TransportedValue = ValueToTransport;
                }
            }
        });
    }

    protected abstract void DesignFinalTable(ColumnDescriptor xcolumn);

    protected bool HasCompleteTransportData() =>
        ShipAddress.HasCompleteTransportData(
            Data.Document!.ShipToAddress,
            Data.Document!.ShipFromAddress,
            Data.Country.IsPortugal);

    protected void DesignTransportShipmentSection(TableDescriptor table)
    {
        table.Cell().Border(0.5f)
            .BorderColor(Color.FromRGB(192, 194, 196)).Column(col =>
            {
                col.Item().DefaultTextStyle(ts => ts.FontSize(6)).Table(shipmentTable =>
                {
                    shipmentTable.ColumnsDefinition(descriptor =>
                    {
                        descriptor.ConstantColumn(18, Unit.Centimetre);
                    });

                    shipmentTable.Cell().PaddingTop(3).PaddingLeft(6).Table(articleCarryTable =>
                    {
                        articleCarryTable.ColumnsDefinition(descriptor =>
                        {
                            descriptor.ConstantColumn(5, Unit.Centimetre);
                            descriptor.ConstantColumn(3, Unit.Centimetre);
                            descriptor.ConstantColumn(3, Unit.Centimetre);
                            descriptor.ConstantColumn(3, Unit.Centimetre);
                            descriptor.ConstantColumn(3, Unit.Centimetre);
                        });

                        articleCarryTable.Header(header =>
                        {
                            header.Cell().Text("Local de Carga").Bold();
                            header.Cell().Text("Data de Expedição").Bold();
                            header.Cell().Text("Matrícula").Bold();
                            header.Cell().Text("Ident. Arm.Partida").Bold();
                            header.Cell().Text("Loc.Bens.Arm.Partida").Bold();
                        });

                        var carryAddress = Data.Document!.ShipFromAddress;

                        articleCarryTable.Cell().PaddingTop(3).Text($"{carryAddress.AddressDetail}\n" +
                                                                    $"{carryAddress.PostalCode} {carryAddress.City} {carryAddress.Country}");
                        articleCarryTable.Cell().PaddingTop(3)
                            .Text($"{carryAddress.DeliveryDate!.Value}");
                        articleCarryTable.Cell().PaddingTop(3).Text($"{carryAddress.DeliveryID}");
                        articleCarryTable.Cell().PaddingTop(3).Text($"{carryAddress.WarehouseID}");
                        articleCarryTable.Cell().PaddingTop(3).Text($"{carryAddress.Region}");
                    });

                    shipmentTable.Cell().PaddingBottom(3).PaddingLeft(6).PaddingTop(3).Table(articleDeliveryTable =>
                    {
                        articleDeliveryTable.ColumnsDefinition(descriptor =>
                        {
                            descriptor.ConstantColumn(5, Unit.Centimetre);
                            descriptor.ConstantColumn(3, Unit.Centimetre);
                            descriptor.ConstantColumn(3, Unit.Centimetre);
                            descriptor.ConstantColumn(3, Unit.Centimetre);
                            descriptor.ConstantColumn(3, Unit.Centimetre);
                        });

                        articleDeliveryTable.Header(header =>
                        {
                            header.Cell().Text("Local de Descarga").Bold();
                            header.Cell().Text("Data da Entrega").Bold();
                            header.Cell().Text("Ident.Entrega").Bold();
                            header.Cell().Text("Ident.Arm.Destino").Bold();
                            header.Cell().Text("Loc.Bens.Arm.Destino").Bold();
                        });

                        var deliveryAddress = Data.Document!.ShipToAddress;

                        articleDeliveryTable.Cell().PaddingTop(3).Text($"{deliveryAddress.AddressDetail}\n" +
                                                                       $"{deliveryAddress.PostalCode} {deliveryAddress.City} {deliveryAddress.Country}");

                        articleDeliveryTable.Cell().PaddingTop(3)
                            .Text($"{deliveryAddress.DeliveryDate!.Value}");
                        articleDeliveryTable.Cell().PaddingTop(3).Text($"{deliveryAddress.DeliveryID}");
                        articleDeliveryTable.Cell().PaddingTop(3).Text($"{deliveryAddress.WarehouseID}");
                        articleDeliveryTable.Cell().PaddingTop(3).Text($"{deliveryAddress.Region}");
                    });
                });
            });
    }

    private static void ApplyWaterMark(ColumnDescriptor col, string mark)
    {
        col.Item().Unconstrained().TranslateY(9f, Unit.Centimetre).TranslateX(1.5f, Unit.Centimetre).Padding(50)
            .AlignCenter().AlignMiddle().Rotate(-45).Text(txt =>
            {
                txt.Span(mark).FontSize(70).FontColor(Color.FromRGB(192, 194, 196));
            });
    }

    private string DocumentFormattedDate => Data.Document!.CreatedAt.ToShortDateString().Replace("/", "-");

    private void DesignHeader(PageDescriptor page)

    {
        page.Header().AlignCenter()
            .Table(headerTable =>
            {
                headerTable.ColumnsDefinition(col => { col.ConstantColumn(18, Unit.Centimetre); });

                headerTable.Cell().Table(grid =>
                {
                    grid.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(5, Unit.Centimetre);
                        columns.ConstantColumn(3, Unit.Centimetre);
                        columns.ConstantColumn(10, Unit.Centimetre);
                    });

                    if (!string.IsNullOrEmpty(Data.Company.LogoPng) &&
                        !string.IsNullOrWhiteSpace(Data.Company.LogoPng) && IsBase64String(Data.Company.LogoPng))
                    {
                        var bytes = Convert.FromBase64String(Data.Company.LogoPng);
                        grid.Cell().Image(bytes);
                    }

                    else
                    {
                        grid.Cell();
                    }

                    grid.Cell().Text("");
                    var documentName = Data.Document!.TypeName + " Nº " + Data.Document.Number;
                    grid.Cell().AlignRight().Text(txt =>
                    {
                        txt.Span(documentName + "\n" + DocumentFormattedDate + $"\n{GetDocumentCopyName()}\n").Bold()
                            .DirectionFromRightToLeft();
                        if (IsSecondCopy)
                        {
                            txt.Span("2ª Via");
                        }
                    });
                    headerTable.Cell().Table(infoTable =>
                    {
                        infoTable.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(9, Unit.Centimetre);
                            columns.ConstantColumn(9, Unit.Centimetre);
                        });


                        infoTable.Header(header =>
                        {
                            header.Cell().PaddingRight(0).Text($"{Data.Company.Name}").FontSize(8.5f).Bold();
                            header.Cell().PaddingLeft(16).Text($"{Data.Document!.Customer.Name}").FontSize(8.5f).Bold();
                        });

                        infoTable.Cell().PaddingRight(0).Text(txt =>
                        {
                            txt.Span($"{Data.Company.Address}\n").FontSize(8.5f);
                            txt.Span(
                                    $"{(Data.Country.IsPortugal ? $"{Data.Company.PostalCode} " : "")}{Data.Company.City}\n")
                                .FontSize(8.5f);
                            txt.Span($"Telefone: {Data.Company.Phone}\n").FontSize(8.5f);
                            txt.Span($"Telemóvel: {Data.Company.MobilePhone}\n").FontSize(8.5f);
                            txt.Span($"Email: {Data.Company.Email}\n").FontSize(8.5f);
                            txt.Span($"Website: {Data.Company.Website}\n").FontSize(8.5f);
                        });

                        infoTable.Cell().PaddingLeft(16).Text(txt =>
                        {
                            txt.Span($"{Data.Document!.Customer.Address}\n");
                            txt.Span(
                                $"{(Data.Country.IsPortugal ? $"{Data.Document.Customer.ZipCode} " : "")}{Data.Document.Customer.City}\n");
                            txt.Span($"{Data.Document.Customer.Country}\n");
                        });
                        if (PagesCount > 0)
                        {
                            if (ValueToTransport != TransportedValue)
                            {
                                TransportedValue = ValueToTransport;

                                infoTable.Cell().ColumnSpan(2).ExtendHorizontal().AlignRight().Text(txt =>
                                {
                                    txt.Span($"Valor transportado: {TransportedValue:N2}").Bold()
                                        .FontSize(7);
                                });
                            }
                        }
                    });

                    headerTable.Cell().Height(10);


                    headerTable.Cell().Table(customerTable =>
                    {
                        customerTable.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(2, Unit.Centimetre);
                            columns.ConstantColumn(3, Unit.Centimetre);
                            columns.ConstantColumn(7, Unit.Centimetre);
                            columns.ConstantColumn(4, Unit.Centimetre);
                            columns.ConstantColumn(2, Unit.Centimetre);
                        });

                        customerTable.Header(header =>
                        {
                            header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                                .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                                .Text("Nº Cliente").Bold().FontSize(8);

                            header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                                .Background(Color.FromRGB(225, 232, 242)).AlignCenter().Padding(3)
                                .Text("NIF Cliente").Bold().FontSize(8);

                            header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                                .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                                .Text("Cond. de Pagamento").Bold().FontSize(8);

                            header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                                .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                                .Text("Método de Pagamento").Bold().FontSize(8);

                            header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                                .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                                .Text("Moeda").Bold().FontSize(8);
                        });


                        customerTable.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196)).AlignCenter()
                            .Padding(1)
                            .Text($"{Data.Document!.CustomerOrder}");
                        customerTable.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196)).AlignCenter()
                            .Padding(1)
                            .Text(Data.Document.Customer.FiscalNumber);
                        customerTable.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196)).AlignCenter()
                            .Padding(1)
                            .Text(Data.Document.PaymentCondition);

                        var paymentMethods = Data.Document.PaymentMethods?.Select(x => x.Acronym);
                        string paymentMethod = "";
                        if (paymentMethods != null)
                        {
                            paymentMethod = string.Join(", ", paymentMethods);
                        }

                        customerTable.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196)).AlignCenter()
                            .Padding(1)
                            .Text(paymentMethod);
                        customerTable.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196)).AlignCenter()
                            .Padding(1)
                            .Text(Data.Document.Currency);
                    });
                    headerTable.Cell().Height(15);
                });
            });
    }

    private void DesignFooter(PageDescriptor page)
    {
        string dateLabel = IsAngola ? "A data de emissão:" : "Impresso em:";

        page.Footer().AlignCenter().Table(footerTable =>
        {
            footerTable.ColumnsDefinition(col => { col.ConstantColumn(18, Unit.Centimetre); });
            footerTable.Cell().LineHorizontal(0.5f).LineColor(Color.FromRGB(192, 194, 196));
            footerTable.Cell().PaddingTop(0.1f, Unit.Centimetre).AlignCenter().Text(txt =>
            {
                txt.DefaultTextStyle(x => x.FontSize(6));
                txt.Span($"NIF: {Data.Company.FiscalNumber} || Capital Social: {Data.Company.StockCapital}\n");
                txt.Span($"{dateLabel} {DocumentFormattedDate} || Página:");
                txt.CurrentPageNumber();
                txt.Span(" /");
                txt.TotalPages();
                txt.Span($" || LogicPulse: LogicPOS v{Data.SoftwareVersion}\n").Bold();
                txt.Span($"{Data.Company.DocumentFinalLine1}\n");
                txt.Span($"{Data.Company.DocumentFinalLine2}\n");
            });
        });
    }

    protected void DesignTaxResumesTable(TableDescriptor table)
    {
        table.ColumnsDefinition(columns =>
        {
            columns.ConstantColumn(2, Unit.Centimetre);
            columns.ConstantColumn(2, Unit.Centimetre);
            columns.ConstantColumn(2.7f, Unit.Centimetre);
            columns.ConstantColumn(2f, Unit.Centimetre);
        });
        string firstHeader = IsAngola ? "Código" : "Designação";
        table.Header(header =>
        {
            header.Cell().PaddingTop(4).PaddingLeft(4).Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                .Background(Color.FromRGB(225, 232, 242)).AlignRight().Padding(3)
                .Text(firstHeader).FontSize(8);
            header.Cell().PaddingTop(4).PaddingLeft(0).Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                .Background(Color.FromRGB(225, 232, 242)).AlignRight().Padding(3)
                .Text("Imposto (%)").FontSize(8);
            header.Cell().PaddingTop(4).PaddingLeft(0).Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                .Background(Color.FromRGB(225, 232, 242)).AlignRight().Padding(3)
                .Text("Base Imp.").FontSize(8);
            header.Cell().PaddingTop(4).PaddingRight(4).Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                .Background(Color.FromRGB(225, 232, 242)).AlignRight().Padding(3)
                .Text("Total Imp.").Bold().FontSize(8);
        });
        var taxResumes = Data.Document!.GetTaxResumes();
        foreach (var resume in taxResumes)
        {
            string firstValue = IsAngola
                ? resume.Code
                : (resume.Designation.Length < 10 ? resume.Designation : resume.Designation.Substring(0, 10) + "...");

            table.Cell().PaddingTop(3).PaddingLeft(4).Text(firstValue).AlignLeft();
            table.Cell().PaddingTop(3).Text($"{resume.Rate:N2}").AlignRight();
            table.Cell().PaddingTop(3).PaddingRight(7).Text(IsAngola? $"{resume.Base.TruncateDecimal():N2}": $"{resume.Base:N2}").AlignRight();
            table.Cell().PaddingTop(3).PaddingRight(8).Text(IsAngola? $"{resume.Total.ApplyAgtRounding():N2}" : $"{resume.Total:N2}").AlignRight().Bold();
        }
    }

    protected virtual void DesignTotalsTable(TableDescriptor table)
    {
        table.ColumnsDefinition(columns =>
        {
            columns.ConstantColumn(9, Unit.Centimetre);
            columns.ConstantColumn(9, Unit.Centimetre);
        });

        table.Cell().PaddingTop(-4).PaddingRight(6).Border(0.5f)
            .BorderColor(Color.FromRGB(192, 194, 196))
            .Table(DesignTaxResumesTable);

        table.Cell().ContentFromLeftToRight().AlignMiddle().PaddingTop(-4).PaddingLeft(0).Border(0.5f)
            .BorderColor(Color.FromRGB(192, 194, 196))
            .Table(discountTable =>
            {
                discountTable.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(6, Unit.Centimetre);
                    columns.ConstantColumn(3, Unit.Centimetre);
                });

                discountTable.Cell().PaddingTop(6).Text("Desconto Cliente (%)").AlignRight();
                discountTable.Cell().PaddingTop(6).PaddingRight(10).Text($"{Data.Document!.Discount:N2}")
                    .AlignRight();

                discountTable.Cell().PaddingTop(6).Text("Total de Descontos").AlignRight();
                discountTable.Cell().PaddingTop(6).PaddingRight(10).Text($"{Data.Document.TotalDiscount:N2}")
                    .AlignRight();

                discountTable.Cell().PaddingTop(6).Text("Total líquido").AlignRight();
                discountTable.Cell().PaddingTop(6).PaddingRight(10).Text(IsAngola ? $"{Data.Document.TotalNet.TruncateDecimal():N2}" : $"{Data.Document.TotalNet:N2}")
                    .AlignRight();

                discountTable.Cell().PaddingTop(6).Text("Total Imposto").AlignRight();
                discountTable.Cell().PaddingTop(6).PaddingRight(10).Text(IsAngola ? $"{Data.Document.TotalTax.ApplyAgtRounding():N2}" : $"{Data.Document.TotalTax:N2}")
                    .AlignRight();

                if (IsAngola)
                {
                    discountTable.Cell().PaddingTop(6).Text("Retenção na Fonte").AlignRight();
                    discountTable.Cell().PaddingTop(6).PaddingRight(10)
                        .Text($"{Data.Document.WithholdingTaxAmount:N2}").AlignRight();
                }

                discountTable.Cell().PaddingTop(6).Text("Total do Documento").AlignRight().Bold();
                discountTable.Cell().PaddingTop(6).PaddingRight(10).Text($"{Data.Document.TotalFinal:N2}")
                    .AlignRight().Bold();

                discountTable.Cell().Height(14);
            });

        table.Cell().Height(10);
    }

    protected virtual void DesignNotesAndQrSection(TableDescriptor table)
    {
        GetReferences();
        AddNotesCell(table);
        AddQrCell(table);
    }

    protected virtual void AddNotesCell(TableDescriptor table)
    {
        var isNc = Data.Document!.Type.Equals("NC", StringComparison.OrdinalIgnoreCase);
        var notesText = BuildNotesText(isNc);

        table.Cell()
            .Border(0.5f)
            .BorderColor(Color.FromRGB(192, 194, 196))
            .Padding(6)
            .Text(txt =>
            {
                txt.Span("Notas\n").Bold().FontSize(7);
                txt.Span(notesText);
            });
    }

    private string? BuildNotesText(bool isCreditNote)
    {
        if (isCreditNote)
        {
            return $"Anulação ou rectificação do documento de origem:{GetReferences()}\n\n" +
                   $"Motivo: {Data.Document!.Notes}";
        }

        if (string.IsNullOrEmpty(References))
            return Data.Document!.Notes;

        var isAssociated = References.Contains("RC") || References.Contains("RG");
        var label = isAssociated ? "Documento(s) Associado(s)" : "Documento(s) de Origem";

        return $"{Data.Document!.Notes}\n{label}: {References}";
    }

    protected void AddQrCell(TableDescriptor table)
    {
        var payload = Data.Document!.AtQRCode;
        var hasCodeLine = string.IsNullOrWhiteSpace(Data.Document.FiscalCodeLine) == false;
        if (string.IsNullOrWhiteSpace(payload) && hasCodeLine == false)
        {
            return;
        }

        table.Cell()
            .Height(hasCodeLine && string.IsNullOrWhiteSpace(payload) ? 1f : 3.5f, Unit.Centimetre)
            .PaddingLeft(0)
            .Table(tb =>
            {
                tb.ColumnsDefinition(c => c.RelativeColumn());

                if (hasCodeLine)
                {
                    tb.Cell()
                        .PaddingLeft(20)
                        .PaddingBottom(3)
                        .AlignRight()
                        .Text(Data.Document.FiscalCodeLine)
                        .FontSize(6);
                }

                if (string.IsNullOrWhiteSpace(payload) == false)
                {
                    tb.Cell().AlignRight()
                        .Padding(1)
                        .Image(GetQrCode(payload))
                        .FitArea();
                }
            });
    }

    protected string GetInvoiceDeclarationLine()
    {
        return Data.Country.IsPortugal ? PortugalInvoiceDeclarationLine : AngolaInvoiceDeclarationLine;
    }

    private string AngolaInvoiceDeclarationLine =>
        $"Os bens/serviços foram colocados à disposição do adquirente nesta data: {Data.Document!.Date}";

    private string PortugalInvoiceDeclarationLine =>
        $"Os Artigos faturados foram colocados à disposição do cliente nesta data - Alínea f) do nº 5 do Art. 36 do CIVA.";

    private byte[] GetQrCode(string text)
    {
        var writer = new ZXing.BarcodeWriter<SKBitmap>
        {
            Format = ZXing.BarcodeFormat.QR_CODE,
            
            Options = new ZXing.QrCode.QrCodeEncodingOptions
            {
                Width = 300,
                Height = 300,
                Margin = 4, // Quiet Zone
                CharacterSet = "UTF-8",
                ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.M

            },
            Renderer = new SKBitmapRenderer
            {
                Background = SKColors.White,
                Foreground = SKColors.Black,
            }
        };


        SKBitmap qrBitmap = writer.Write(text);
        if (IsAngola)
        {
        return GenerateAngolanQrCode(text);

        }

        SKImage image = SKImage.FromBitmap(qrBitmap);
        SKData data = image.Encode(SKEncodedImageFormat.Png, 100);

       return data.ToArray();
    }

    private byte[] GenerateAngolanQrCode(string text)
    {
        var qrCode = QrCodeGenerator.Generate(
            plainText: text,
            eccLevel: ErrorCorrectionLevel.Medium,
            forceUtf8: true,
            utf8Bom: false,
            eciMode: ExtendedChannelInterpolationMode.Utf8);

        var renderer = new SkiaSharpRenderer();

        var settings = new RendererSettings
        {
            PixelsPerModule = 100,
            DrawQuietZones = true,
            QuietZoneStyle = QuietZoneStyle.Flat,
            FileFormat = FileFormat.Png,

        };

        if (!string.IsNullOrEmpty(Data.Company.AgtLogo) &&
            IsBase64String(Data.Company.AgtLogo) &&
            IsAngola)
        {
            settings.IconBytes = Convert.FromBase64String(Data.Company.AgtLogo);
            settings.IconSizePercent = 20;
            settings.IconBorderWidth = 10;
        }

        var qrBytes = renderer.RenderToBytes(qrCode, settings);
        return qrBytes;
    }

    private static bool IsBase64String(string base64)
    {
        base64 = base64.Trim();
        return (base64.Length % 4 == 0) &&
               !base64.Contains(" ") &&
               base64.All(c => char.IsLetterOrDigit(c) ||
                               c == '+' ||
                               c == '/' ||
                               c == '=');
    }
}