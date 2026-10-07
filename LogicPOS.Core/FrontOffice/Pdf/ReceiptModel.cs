using LogicPOS.Application.Features.Finance.Documents.PdfGeneration;
using QrCodes;
using QrCodes.Renderers;
using QrCodes.Renderers.Abstractions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace LogicPOS.Core.FrontOffice.Pdf;

public class ReceiptModel : IDocument
{
    public ReceiptModel(DocumentPdfData data)
    {
        Data = data;
    }

    public uint CopyNumber { get; set; } = 1;
    public bool IsSecondCopy;
    private decimal WithHoldingTaxAmount { get; set; }
    private DocumentPdfData Data { get; init; }
    private string DocumentFormattedDate => Data.Receipt!.CreatedAt.ToShortDateString().Replace("/", "-");

    private string GetDocumentCopyName()
    {
        if (IsSecondCopy)
        {
            return CopyNumber switch
            {
                1 => "Original",
                2 => "Duplicado",
                3 => "Triplicado",
                4 => "Quadruplicado",
                _ => "Original"
            };
        }
        return CopyNumber switch
        {
            1 => "Original",
            2 => "Duplicado",
            3 => "Triplicado",
            4 => "Quadruplicado",
            _ => "Cópia"
        };
    }

    private void DesignBody(PageDescriptor page)
    {
        page.Content().AlignCenter().Column(xcolumn =>
        {
            DocumentCancelVerify(xcolumn, Data.Receipt!.Status);
            xcolumn.Item().Table(itemsTable =>
            {
                itemsTable.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(3, Unit.Centimetre);
                    columns.ConstantColumn(5, Unit.Centimetre);
                    columns.ConstantColumn(4, Unit.Centimetre);
                    columns.ConstantColumn(4, Unit.Centimetre);
                    columns.ConstantColumn(2, Unit.Centimetre);
                });

                itemsTable.Header(header =>
                {
                    header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                        .Text("Data").Bold().FontSize(8);

                    header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                        .Text("Número do Doc.").Bold().FontSize(8);

                    header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                        .Text("Total do Documento").Bold().FontSize(8);

                    header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                        .Text("Total Líquido.").Bold().FontSize(8);

                    header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                        .Text("Pago").Bold().FontSize(8);
                });

                foreach (var item in Data.Receipt.Payments.OrderByDescending(d=>d.Document!.CreatedAt)!)
                {
                    var currentItemIndex = Data.Receipt.Payments.IndexOf(item);

                    var backgroundColor = (currentItemIndex) % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;
                    itemsTable.Cell().Background(backgroundColor).Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .AlignCenter().Padding(1).Text(item.Document!.CreatedAt.ToShortDateString().Replace("/", "-"))
                        .Bold();
                    itemsTable.Cell().Background(backgroundColor).Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .AlignCenter().Padding(1).Text(item.Document!.Number);
                    itemsTable.Cell().Background(backgroundColor).Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .AlignCenter().Padding(1).Text(item.Document!.TotalFinal.ToString("N2"));
                    itemsTable.Cell().Background(backgroundColor).Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                        .AlignCenter().Padding(1).Text(item.AllocatedAmount.ToString("N2"));
                    if (item.Document!.Paid)
                    {
                        itemsTable.Cell().Background(backgroundColor).Border(0.5f)
                            .BorderColor(Color.FromRGB(192, 194, 196)).AlignCenter().Padding(1).Text("✓").Black();
                    }
                    else
                    {
                        itemsTable.Cell().Background(backgroundColor).Border(0.5f)
                            .BorderColor(Color.FromRGB(192, 194, 196)).AlignCenter().Padding(1).Text(" ").Black();
                    }

                    WithHoldingTaxAmount += item.Document.WithholdingTaxAmount;
                }
            });

            DesignFinalTable(xcolumn);
        });
    }

    private void DocumentCancelVerify(ColumnDescriptor col, string documentStatus)
    {
        if (documentStatus == "A")
        {
            col.Item().Unconstrained().TranslateY(9.5f, Unit.Centimetre).TranslateX(1.5f, Unit.Centimetre).Padding(50)
                .AlignCenter().AlignMiddle().Rotate(-45).Text(txt =>
                {
                    txt.Span("Anulado").FontSize(70).FontColor(Color.FromRGB(192, 194, 196));
                });
        }
    }

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

                    if (Data.Company.LogoPng != null && IsBase64String(Data.Company.LogoPng))
                    {
                        var bytes = Convert.FromBase64String(Data.Company.LogoPng);
                        grid.Cell().Image(bytes);
                    }
                    else
                    {
                        grid.Cell();
                    }

                    grid.Cell().Text("");
                    var documentName = Data.Receipt!.RefNo;
                    grid.Cell().AlignRight().Text(txt =>
                    {
                        txt.Span("Recibo Nº " + documentName + "\n" + DocumentFormattedDate + $"\n{GetDocumentCopyName()}\n").Bold().DirectionFromRightToLeft();
                        if (IsSecondCopy)
                        {
                            txt.Span("2ª Via");
                        }

                    });
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
                        header.Cell().PaddingLeft(16).Text($"{Data.Receipt!.Customer.Name}").FontSize(8.5f)
                            .Bold();
                    });

                    infoTable.Cell().PaddingRight(0).Text(txt =>
                    {
                        txt.Span($"{Data.Company.Address}\n").FontSize(8.5f);
                        txt.Span($"{Data.Company.PostalCode} {Data.Company.City}\n").FontSize(8.5f);
                        txt.Span($"Telefone: {Data.Company.Phone}\n").FontSize(8.5f);
                        txt.Span($"Telemóvel: {Data.Company.MobilePhone}\n").FontSize(8.5f);
                        txt.Span($"Email: {Data.Company.Email}\n").FontSize(8.5f);
                        txt.Span($"Website: {Data.Company.Website}\n").FontSize(8.5f);
                    });

                    infoTable.Cell().PaddingLeft(16).Text(txt =>
                    {
                        txt.Span($"{Data.Receipt!.Customer.Address}\n");
                        txt.Span($"{Data.Receipt.Customer.ZipCode} {Data.Receipt.Customer.City}\n");
                        txt.Span($"{Data.Receipt.Customer.Country}\n");
                    });
                });

                headerTable.Cell().Height(10);


                headerTable.Cell().Table(customerTable =>
                {
                    customerTable.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(3, Unit.Centimetre);
                        columns.ConstantColumn(3, Unit.Centimetre);
                        columns.ConstantColumn(8, Unit.Centimetre);
                        columns.ConstantColumn(4, Unit.Centimetre);
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
                            .Text("Método de Pagamento").Bold().FontSize(8);

                        header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                            .Background((Color.FromRGB(225, 232, 242))).AlignCenter().Padding(3)
                            .Text("Moeda").Bold().FontSize(8);
                    });


                    customerTable.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196)).AlignCenter().Padding(1)
                        .Text($"{Data.Document!.CustomerOrder}");
                    customerTable.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196)).AlignCenter().Padding(1)
                        .Text(Data.Receipt!.Customer.FiscalNumber);
                    customerTable.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196)).AlignCenter().Padding(1)
                        .Text(Data.Receipt.PaymentMethod);
                    customerTable.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196)).AlignCenter().Padding(1)
                        .Text(Data.Receipt.CurrencyDesignation + "/" +
                              Data.Receipt.CurrencyAcronym);
                });
                headerTable.Cell().Height(15);
            });
    }

    private void DesignFinalTable(ColumnDescriptor xcolumn)
    {
        xcolumn.Item().ExtendVertical().AlignBottom().PaddingTop(3).ShowEntire().Table(finalTable =>
        {
            finalTable.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(10, Unit.Centimetre);
                columns.ConstantColumn(5f, Unit.Centimetre);
                columns.ConstantColumn(3f, Unit.Centimetre);
            });

            finalTable.Cell().PaddingRight(6).PaddingTop(-4).Border(0.5f)
                .BorderColor(Color.FromRGB(192, 194, 196)).Padding(5).Table(paymentTable =>
                {
                    paymentTable.ColumnsDefinition(column => { column.RelativeColumn(); });
                    paymentTable.Cell().Text($"Recebemos a quantia de:\n{Data.Receipt!.AmountInWords}\n");
                    paymentTable.Cell().Text($"Notas: {Data.Receipt.Notes}\nDocumentos Associados: {string.Join(',',Data.Receipt.RelatedDocs)}").ParagraphSpacing(5)
                        .AlignLeft();
                });


            finalTable.Cell().PaddingTop(-4).Border(0.5f)
                .BorderColor(Color.FromRGB(192, 194, 196)).Padding(5).Table(totalTable =>
                {
                    totalTable.ColumnsDefinition(column => { column.RelativeColumn(); });
                    totalTable.Cell().Text($"Total Líquido:\n {Data.Receipt!.Amount:N2} {Data.Receipt.CurrencyCode}\n")
                        .AlignRight().Bold();

                    if (Data.Country.IsAngola)
                    {
                        totalTable.Cell()
                            .Text($"Retenção na Fonte:\n {WithHoldingTaxAmount:N2} {Data.Receipt.CurrencyCode}")
                            .AlignRight().Bold();
                    }
                });

            finalTable.Cell().PaddingLeft(6).Table(tb =>
            {
                tb.ColumnsDefinition(columns => { columns.RelativeColumn(); });
                if (string.IsNullOrWhiteSpace(Data.Receipt!.QrCode) == false)
                {
                    tb.Cell().PaddingLeft(7).PaddingVertical(3).AlignRight().Image(GetQrCode(Data.Receipt.QrCode)).FitArea();
                }
            });
            if (!Data.Country.IsAngola)
            {
                finalTable.Cell().ColumnSpan(3).ExtendHorizontal().PaddingTop(3).Text("Este documento não serve de fatura").AlignCenter().FontSize(7);
            }
            else
            {
                finalTable.Cell().ExtendHorizontal().PaddingTop(3).Text("").AlignCenter().FontSize(7);
            }
        });
    }

    private byte[] GetQrCode(string text)
    {
        var qrCode = QrCodeGenerator.Generate(plainText: text,
            eccLevel: ErrorCorrectionLevel.Medium,
            forceUtf8: true,
            utf8Bom: true,
            eciMode: ExtendedChannelInterpolationMode.Utf8);

        var renderer = new SkiaSharpRenderer();

        var settings = new RendererSettings
        {
            PixelsPerModule = 100,
            DrawQuietZones = false,
            IconBorderWidth = 100,
            IconSizePercent = 25,
            QuietZoneStyle = QuietZoneStyle.Dotted,
            FileFormat = FileFormat.Png
        };

        if (!string.IsNullOrEmpty(Data.Company.AgtLogo) && IsBase64String(Data.Company.AgtLogo) &&
            Data.Country.IsAngola)
        {
            settings.IconBytes = Convert.FromBase64String(Data.Company.AgtLogo);
        }

        var qrBytes = renderer.RenderToBytes(qrCode, settings);
        var bitmap = SKBitmap.Decode(qrBytes);
        if (bitmap == null)
        {
            return null!;
        }

        var resized = bitmap.Resize(new SKImageInfo(350, 350), SKSamplingOptions.Default);
        if (resized == null)
        {
            return null!;
        }

        var img = SKImage.FromBitmap(resized);
        var png = img.Encode(SKEncodedImageFormat.Png, 100);
        return png.ToArray();
    }

    private void DesignFooter(PageDescriptor page)
    {
        if (Data.Country.IsPortugal)
        {
            DesignPortugalFooter(page);
        }
        else if (Data.Country.IsAngola)
        {
            DesignAngolaFooter(page);
        }
    }

    private void DesignAngolaFooter(PageDescriptor page)
    {
        page.Footer().AlignCenter().Table(footerTable =>
        {
            footerTable.ColumnsDefinition(col => { col.ConstantColumn(18, Unit.Centimetre); });
            footerTable.Cell().LineHorizontal(0.5f).LineColor(Color.FromRGB(192, 194, 196));
            footerTable.Cell().PaddingTop(0.1f, Unit.Centimetre).AlignCenter().Text(txt =>
            {
                txt.Span(
                    $"NIF: {Data.Company.FiscalNumber} || Capital Social: {Data.Company.StockCapital}\n");
                txt.Span($"A data de emissão: {DocumentFormattedDate} || Página:");
                txt.CurrentPageNumber();
                txt.Span(" /");
                txt.TotalPages();
                txt.Span($" || LogicPulse: LogicPOS v{Data.SoftwareVersion}\n").Bold();
                txt.Span($"{Data.Company.DocumentFinalLine1}\n");
                txt.Span($"{Data.Company.DocumentFinalLine2}\n");
            });
        });
    }

    private void DesignPortugalFooter(PageDescriptor page)
    {
        page.Footer().AlignCenter().Table(footerTable =>
        {
            footerTable.ColumnsDefinition(col => { col.ConstantColumn(18, Unit.Centimetre); });
            footerTable.Cell().LineHorizontal(0.5f).LineColor(Color.FromRGB(192, 194, 196));
            footerTable.Cell().PaddingTop(0.1f, Unit.Centimetre).AlignCenter().Text(txt =>
            {
                txt.Span(
                    $"NIF: {Data.Company.FiscalNumber} || Capital Social: {Data.Company.StockCapital}\n");
                txt.Span($"Impresso em: {DocumentFormattedDate} || Página:");
                txt.CurrentPageNumber();
                txt.Span(" /");
                txt.TotalPages();
                txt.Span($" || LogicPulse: LogicPOS v{Data.SoftwareVersion}\n").Bold();
                txt.Span($"{Data.Company.DocumentFinalLine1}\n");
                txt.Span($"{Data.Company.DocumentFinalLine2}\n");
            });
        });
    }

    public void Compose(IDocumentContainer container)
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

    private static bool IsBase64String(string base64)
    {
        if (string.IsNullOrEmpty(base64))
        {
            return false;
        }
        base64 = base64.Trim();
        return (base64.Length % 4 == 0) &&
               !base64.Contains(" ") &&
               (base64.All(c => char.IsLetterOrDigit(c) ||
                                c == '+' ||
                                c == '/' ||
                                c == '='));
    }
}