namespace LogicPOS.Core.FrontOffice;

public sealed class PosTicketLine
{
    public PosTicketLine(PosArticle article)
    {
        ArticleId = article.Id;
        Designation = article.Designation;
        PriceIncludesVat = article.PriceIncludesVat;
        VatPercentage = article.VatPercentage;
        QuantityStep = article.QuantityStep;
        Discount = article.Discount;
        VatRateId = article.VatRateId;
        Quantity = article.QuantityStep;
        NetUnitPrice = ToNetUnitPrice(article.CatalogPrice, article.VatPercentage, article.PriceIncludesVat);
    }

    public Guid ArticleId { get; }

    public string Designation { get; }

    public bool PriceIncludesVat { get; }

    public decimal VatPercentage { get; }

    public decimal QuantityStep { get; }

    public decimal Discount { get; }

    public Guid VatRateId { get; }

    public decimal Quantity { get; set; }

    public decimal NetUnitPrice { get; set; }

    public decimal DisplayUnitPrice => PriceIncludesVat
        ? NetUnitPrice * (1m + VatPercentage / 100m)
        : NetUnitPrice;

    public decimal TotalNet => Quantity * NetUnitPrice * (1m - Discount / 100m);

    public decimal Total => TotalNet * (1m + VatPercentage / 100m);

    public void SetDisplayPrice(decimal price)
    {
        NetUnitPrice = ToNetUnitPrice(price, VatPercentage, PriceIncludesVat);
    }

    public PosTicketLine WithQuantity(decimal quantity)
    {
        var copy = (PosTicketLine)MemberwiseClone();
        copy.Quantity = quantity;
        return copy;
    }

    public static decimal ToNetUnitPrice(decimal price, decimal vatPercentage, bool priceIncludesVat)
    {
        if (priceIncludesVat == false || vatPercentage <= 0)
        {
            return price;
        }

        return price / (1m + vatPercentage / 100m);
    }
}

public sealed class PosTicket
{
    private readonly List<PosTicketLine> _lines = new();

    public IReadOnlyList<PosTicketLine> Lines => _lines;

    public int SelectedIndex { get; private set; } = -1;

    public PosTicketLine? Selected => SelectedIndex >= 0 && SelectedIndex < _lines.Count ? _lines[SelectedIndex] : null;

    public decimal Total => _lines.Sum(line => line.Total);

    public bool HasLines => _lines.Count > 0;

    public void Add(PosArticle article, decimal? displayUnitPrice = null)
    {
        var line = new PosTicketLine(article);
        if (displayUnitPrice is decimal price)
        {
            line.SetDisplayPrice(price);
        }

        var existing = _lines.FindIndex(item =>
            item.ArticleId == article.Id && item.NetUnitPrice == line.NetUnitPrice);
        if (existing >= 0)
        {
            _lines[existing].Quantity += article.QuantityStep;
            SelectedIndex = existing;
            return;
        }

        _lines.Add(line);
        SelectedIndex = _lines.Count - 1;
    }

    public void Select(int index)
    {
        if (index < 0 || index >= _lines.Count)
        {
            return;
        }

        SelectedIndex = index;
    }

    public void SelectPrevious()
    {
        if (SelectedIndex > 0)
        {
            SelectedIndex--;
        }
    }

    public void SelectNext()
    {
        if (SelectedIndex >= 0 && SelectedIndex < _lines.Count - 1)
        {
            SelectedIndex++;
        }
    }

    public void IncreaseSelected()
    {
        var line = Selected;
        if (line is null)
        {
            return;
        }

        line.Quantity += line.QuantityStep;
    }

    public void DecreaseSelected()
    {
        var line = Selected;
        if (line is null)
        {
            return;
        }

        line.Quantity -= line.QuantityStep;
        if (line.Quantity <= 0)
        {
            RemoveAt(SelectedIndex);
        }
    }

    public void SetSelectedQuantity(decimal quantity)
    {
        var line = Selected;
        if (line is null || quantity <= 0)
        {
            return;
        }

        line.Quantity = quantity;
    }

    public void SetSelectedDisplayPrice(decimal price)
    {
        Selected?.SetDisplayPrice(price);
    }

    public void Clear()
    {
        _lines.Clear();
        SelectedIndex = -1;
    }

    public void Load(IReadOnlyList<PosTicketLine> lines)
    {
        _lines.Clear();
        _lines.AddRange(lines);
        SelectedIndex = _lines.Count == 0 ? -1 : 0;
    }

    public void RemoveIndexes(IReadOnlyList<int> indexes)
    {
        foreach (var index in indexes.Distinct().OrderByDescending(index => index))
        {
            RemoveAt(index);
        }
    }

    private void RemoveAt(int index)
    {
        if (index < 0 || index >= _lines.Count)
        {
            return;
        }

        _lines.RemoveAt(index);
        if (_lines.Count == 0)
        {
            SelectedIndex = -1;
            return;
        }

        SelectedIndex = Math.Min(index, _lines.Count - 1);
    }
}
