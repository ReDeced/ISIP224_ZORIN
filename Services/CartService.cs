using Microsoft.Data.Sqlite;
using ISIP224_ZORIN.Models;

namespace ISIP224_ZORIN.Services;

// Корзина пользователя: строки таблицы CartItem со ссылками на User и Product.
public class CartService
{
    private readonly MarketplaceDatabase database;

    public CartService(MarketplaceDatabase database)
    {
        this.database = database;
    }

    public List<CartItem> GetItems(User user)
    {
        return database.Query(
            """
            SELECT CartItem.Id, CartItem.UserId, CartItem.ProductId, CartItem.Quantity,
                   Product.Title AS ProductTitle, Product.Price AS UnitPrice
            FROM CartItem
            JOIN Product ON Product.Id = CartItem.ProductId
            WHERE CartItem.UserId = $userId
            ORDER BY CartItem.Id;
            """,
            MapCartItem,
            MarketplaceDatabase.P("$userId", user.Id));
    }

    // Добавляет товар в корзину, суммируя количество с уже лежащим
    // и не позволяя превысить остаток на складе.
    public CartItem AddItem(User user, Product product, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), quantity, "Количество должно быть больше нуля.");

        CartItem? existing = FindByProduct(user, product.Id);
        int alreadyInCart = existing?.Quantity ?? 0;
        int newQuantity = alreadyInCart + quantity;

        if (newQuantity > product.Stock)
            throw new InvalidOperationException(
                $"Нельзя добавить {quantity} шт.: товара «{product.Title}» на складе {product.Stock} шт., в корзине уже {alreadyInCart} шт.");

        if (existing is null)
        {
            long id = database.Insert(
                "INSERT INTO CartItem (UserId, ProductId, Quantity) VALUES ($userId, $productId, $quantity);",
                MarketplaceDatabase.P("$userId", user.Id),
                MarketplaceDatabase.P("$productId", product.Id),
                MarketplaceDatabase.P("$quantity", quantity));

            return new CartItem((int)id, user.Id, product.Id, quantity, product.Title, product.Price);
        }

        database.Execute(
            "UPDATE CartItem SET Quantity = $quantity WHERE Id = $id;",
            MarketplaceDatabase.P("$quantity", newQuantity),
            MarketplaceDatabase.P("$id", existing.Id));

        return new CartItem(existing.Id, user.Id, product.Id, newQuantity, product.Title, product.Price);
    }

    // Убирает позицию из корзины конкретного пользователя.
    public bool RemoveItem(User user, int cartItemId)
        => database.Execute(
            "DELETE FROM CartItem WHERE Id = $id AND UserId = $userId;",
            MarketplaceDatabase.P("$id", cartItemId),
            MarketplaceDatabase.P("$userId", user.Id)) > 0;

    // Очищает корзину, возвращает число удалённых позиций.
    public int Clear(User user)
        => database.Execute(
            "DELETE FROM CartItem WHERE UserId = $userId;",
            MarketplaceDatabase.P("$userId", user.Id));

    public decimal GetTotal(IReadOnlyList<CartItem> items)
        => items.Sum(item => item.Total);

    private CartItem? FindByProduct(User user, int productId)
    {
        return database
            .Query(
                """
                SELECT CartItem.Id, CartItem.UserId, CartItem.ProductId, CartItem.Quantity,
                       Product.Title AS ProductTitle, Product.Price AS UnitPrice
                FROM CartItem
                JOIN Product ON Product.Id = CartItem.ProductId
                WHERE CartItem.UserId = $userId AND CartItem.ProductId = $productId;
                """,
                MapCartItem,
                MarketplaceDatabase.P("$userId", user.Id),
                MarketplaceDatabase.P("$productId", productId))
            .FirstOrDefault();
    }

    private static CartItem MapCartItem(SqliteDataReader reader)
        => new(
            reader.GetInt32(reader.GetOrdinal("Id")),
            reader.GetInt32(reader.GetOrdinal("UserId")),
            reader.GetInt32(reader.GetOrdinal("ProductId")),
            reader.GetInt32(reader.GetOrdinal("Quantity")),
            reader.GetString(reader.GetOrdinal("ProductTitle")),
            Convert.ToDecimal(reader.GetDouble(reader.GetOrdinal("UnitPrice"))));
}
