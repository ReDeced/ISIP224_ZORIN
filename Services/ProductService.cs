using Microsoft.Data.Sqlite;
using ISIP224_ZORIN.Models;

namespace ISIP224_ZORIN.Services;

// Каталог товаров. Смотреть ассортимент можно и без входа в аккаунт.
public class ProductService
{
    private readonly MarketplaceDatabase database;

    public ProductService(MarketplaceDatabase database)
    {
        this.database = database;
    }

    // Все товары вместе с названием категории (JOIN вместо дублирования
    // названия в таблице Product).
    public List<(Product Product, string CategoryName)> GetAllProducts()
    {
        return database.Query(
            """
            SELECT Product.Id, Product.CategoryId, Product.Title, Product.Price,
                   Product.Stock, Product.Description, Category.Name AS CategoryName
            FROM Product
            JOIN Category ON Category.Id = Product.CategoryId
            ORDER BY Product.Title;
            """,
            reader => (MapProduct(reader), reader.GetString(reader.GetOrdinal("CategoryName"))));
    }

    public Product? FindById(int id)
    {
        return database
            .Query(
                "SELECT Id, CategoryId, Title, Price, Stock, Description FROM Product WHERE Id = $id;",
                MapProduct,
                MarketplaceDatabase.P("$id", id))
            .FirstOrDefault();
    }

    public static Product MapProduct(SqliteDataReader reader)
        => new(
            reader.GetInt32(reader.GetOrdinal("Id")),
            reader.GetInt32(reader.GetOrdinal("CategoryId")),
            reader.GetString(reader.GetOrdinal("Title")),
            Convert.ToDecimal(reader.GetDouble(reader.GetOrdinal("Price"))),
            reader.GetInt32(reader.GetOrdinal("Stock")),
            reader.GetString(reader.GetOrdinal("Description")));
}
