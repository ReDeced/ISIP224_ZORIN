using MyConsoleApp.Models;

namespace MyConsoleApp.Services;

public class SalesHistory
{
    private readonly Stack<Sale> _sales = new();
    private int _nextNumber = 1;

    public int Count => _sales.Count;

    public Sale Add(Product product, int quantity)
    {
        Sale sale = new(_nextNumber, product, quantity);
        _sales.Push(sale);
        _nextNumber++;
        return sale;
    }

    public Sale? PeekLast() => _sales.Count > 0 ? _sales.Peek() : null;

    public bool TryCancelLast(out Sale? sale)
    {
        sale = _sales.Count > 0 ? _sales.Pop() : null;
        return sale is not null;
    }

    public List<Sale> GetAll()
    {
        List<Sale> sales = new();

        foreach (Sale sale in _sales)
            sales.Add(sale);

        sales.Reverse();
        return sales;
    }

    public decimal GetTotalRevenue() => _sales.Sum(sale => sale.Total);
}
