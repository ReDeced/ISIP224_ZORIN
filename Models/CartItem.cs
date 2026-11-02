namespace ISIP224_ZORIN.Models;

// Таблица CartItem: строка корзины пользователя.
public class CartItem
{
    public CartItem(int id, int userId, int productId, int quantity,
        string productTitle = "", decimal unitPrice = 0m)
    {
        Id = id;
        UserId = userId;
        ProductId = productId;
        Quantity = quantity;
        ProductTitle = productTitle;
        UnitPrice = unitPrice;
    }

    public int Id { get; }

    // Внешний ключ на User.Id.
    public int UserId { get; }

    // Внешний ключ на Product.Id.
    public int ProductId { get; }

    public int Quantity { get; }

    // Поля ниже не хранятся в таблице: они заполняются из Product
    // при чтении и нужны только для вывода на консоль.
    public string ProductTitle { get; }

    public decimal UnitPrice { get; }

    public decimal Total => UnitPrice * Quantity;
}
