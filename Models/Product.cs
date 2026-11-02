namespace ISIP224_ZORIN.Models;

// Таблица Product: товар маркетплейса.
public class Product
{
    public Product(int id, int categoryId, string title, decimal price, int stock, string description)
    {
        Id = id;
        CategoryId = categoryId;
        Title = title;
        Price = price;
        Stock = stock;
        Description = description;
    }

    public int Id { get; }

    // Внешний ключ на Category.Id.
    public int CategoryId { get; }

    public string Title { get; }

    public decimal Price { get; }

    public int Stock { get; }

    public string Description { get; }
}
