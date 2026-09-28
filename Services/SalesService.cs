using MyConsoleApp.Models;

namespace MyConsoleApp.Services;

public enum SellStatus
{
    Ok,
    ProductNotFound,
    InvalidQuantity,
    NotEnoughStock
}

public enum CancelStatus
{
    Ok,
    NothingToCancel,
    ProductNotFound
}

public class SalesService
{
    private readonly ProductRepository _repository;
    private readonly SalesHistory _history = new();

    public SalesService(ProductRepository repository)
    {
        _repository = repository;
    }

    public int SalesCount => _history.Count;

    public List<Sale> GetHistory() => _history.GetAll();

    public decimal GetTotalRevenue() => _history.GetTotalRevenue();

    public Sale? GetLastSale() => _history.PeekLast();

    public SellStatus Sell(int code, int quantity, out Sale? sale, out string message)
    {
        sale = null;

        Product? product = _repository.FindByCode(code);
        if (product is null)
        {
            message = $"Товар с кодом {code} не найден.";
            return SellStatus.ProductNotFound;
        }

        if (quantity <= 0)
        {
            message = "Количество должно быть больше нуля.";
            return SellStatus.InvalidQuantity;
        }

        if (quantity > product.Quantity)
        {
            message = product.Quantity == 0
                ? $"Товара «{product.Name}» нет на складе."
                : $"На складе только {product.Quantity} шт. Товара «{product.Name}».";
            return SellStatus.NotEnoughStock;
        }

        if (!product.TrySell(quantity))
        {
            message = $"Не удалось продать товар «{product.Name}».";
            return SellStatus.NotEnoughStock;
        }

        sale = _history.Add(product, quantity);
        message = $"Продажа оформлена. Товара на складе осталось: {product.Quantity} шт.";
        return SellStatus.Ok;
    }

    public CancelStatus CancelLastSale(out Sale? sale, out string message)
    {
        sale = null;

        if (!_history.TryCancelLast(out sale) || sale is null)
        {
            message = "История продаж пуста, отменять нечего.";
            return CancelStatus.NothingToCancel;
        }

        Product? product = _repository.FindByCode(sale.ProductCode);
        if (product is null)
        {
            message = $"Продажа №{sale.Number} отменена, но товар «{sale.ProductName}» был удалён из списка — остатки не восстановлены.";
            return CancelStatus.ProductNotFound;
        }

        product.ReturnToStock(sale.Quantity);
        message = $"Продажа №{sale.Number} отменена. На склад возвращено {sale.Quantity} шт. Товара «{sale.ProductName}» теперь: {product.Quantity} шт.";
        return CancelStatus.Ok;
    }
}
