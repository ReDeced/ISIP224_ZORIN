using Microsoft.Data.Sqlite;
using ISIP224_ZORIN.Models;

namespace ISIP224_ZORIN.Services;

// Оформление заказов. Списание товаров со склада, запись заказа и его
// позиций выполняются в одной транзакции: либо всё, либо ничего.
public class OrderService
{
    private readonly MarketplaceDatabase database;

    public OrderService(MarketplaceDatabase database)
    {
        this.database = database;
    }

    // Список ПВЗ, из которого пользователь выбирает, где забрать заказ.
    public List<PickupPoint> GetPickupPoints()
        => database.Query(
            "SELECT Id, Address FROM PickupPoint ORDER BY Id;",
            MapPickupPoint);

    // Покупка набора позиций (товар, количество) с выдачей в выбранном ПВЗ.
    public Order CreateOrder(User user, PickupPoint pickupPoint,
        IReadOnlyList<(Product Product, int Quantity)> lines)
    {
        if (lines.Count == 0)
            throw new InvalidOperationException("Нечего покупать: список позиций пуст.");

        using SqliteTransaction transaction = database.Connection.BeginTransaction();

        try
        {
            // Списываем со склада, проверяя остатки внутри транзакции:
            // данные могли измениться после того, как корзина была собрана.
            foreach ((Product product, int quantity) in lines)
            {
                int stock = GetStock(product.Id, transaction);

                if (stock < quantity)
                    throw new InvalidOperationException(
                        $"Товара «{product.Title}» на складе осталось только {stock} шт.");

                ExecuteInTransaction(transaction,
                    "UPDATE Product SET Stock = Stock - $quantity WHERE Id = $productId;",
                    MarketplaceDatabase.P("$quantity", quantity),
                    MarketplaceDatabase.P("$productId", product.Id));
            }

            DateTime createdAt = DateTime.Now;

            long orderId = InsertInTransaction(transaction,
                """
                INSERT INTO "Order" (UserId, PickupPointId, CreatedAt)
                VALUES ($userId, $pickupPointId, $createdAt);
                """,
                MarketplaceDatabase.P("$userId", user.Id),
                MarketplaceDatabase.P("$pickupPointId", pickupPoint.Id),
                MarketplaceDatabase.P("$createdAt", UserService.FormatDate(createdAt)));

            foreach ((Product product, int quantity) in lines)
            {
                InsertInTransaction(transaction,
                    """
                    INSERT INTO OrderItem (OrderId, ProductId, Quantity, UnitPrice)
                    VALUES ($orderId, $productId, $quantity, $unitPrice);
                    """,
                    MarketplaceDatabase.P("$orderId", orderId),
                    MarketplaceDatabase.P("$productId", product.Id),
                    MarketplaceDatabase.P("$quantity", quantity),
                    MarketplaceDatabase.P("$unitPrice", (double)product.Price));
            }

            transaction.Commit();

            return new Order((int)orderId, user.Id, pickupPoint.Id, createdAt, pickupPoint.Address);
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private int GetStock(int productId, SqliteTransaction transaction)
        => Convert.ToInt32(ScalarInTransaction(transaction,
            "SELECT Stock FROM Product WHERE Id = $productId;",
            MarketplaceDatabase.P("$productId", productId)));

    private SqliteCommand CreateCommand(SqliteTransaction transaction, string sql, params SqliteParameter[] parameters)
    {
        SqliteCommand command = database.Connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        command.Parameters.AddRange(parameters);
        return command;
    }

    private void ExecuteInTransaction(SqliteTransaction transaction, string sql, params SqliteParameter[] parameters)
    {
        using SqliteCommand command = CreateCommand(transaction, sql, parameters);
        command.ExecuteNonQuery();
    }

    private long InsertInTransaction(SqliteTransaction transaction, string sql, params SqliteParameter[] parameters)
    {
        using SqliteCommand command = CreateCommand(transaction, sql + "; SELECT last_insert_rowid();", parameters);
        return (long)command.ExecuteScalar()!;
    }

    private object? ScalarInTransaction(SqliteTransaction transaction, string sql, params SqliteParameter[] parameters)
    {
        using SqliteCommand command = CreateCommand(transaction, sql, parameters);
        return command.ExecuteScalar();
    }

    private static PickupPoint MapPickupPoint(SqliteDataReader reader)
        => new(
            reader.GetInt32(reader.GetOrdinal("Id")),
            reader.GetString(reader.GetOrdinal("Address")));
}
