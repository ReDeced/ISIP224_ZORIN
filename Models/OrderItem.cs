namespace ISIP224_ZORIN.Models;

// Таблица OrderItem: позиция заказа. UnitPrice — цена товара на момент
// покупки: это исторический факт, который не должен меняться вместе
// с текущей ценой в Product.
public class OrderItem
{
    public OrderItem(int id, int orderId, int productId, int quantity, decimal unitPrice,
        string productTitle = "")
    {
        Id = id;
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        ProductTitle = productTitle;
    }

    public int Id { get; }

    // Внешний ключ на Order.Id.
    public int OrderId { get; }

    // Внешний ключ на Product.Id.
    public int ProductId { get; }

    public int Quantity { get; }

    public decimal UnitPrice { get; }

    // Заполняется из Product при чтении, для вывода на консоль.
    public string ProductTitle { get; }

    public decimal Total => UnitPrice * Quantity;
}
