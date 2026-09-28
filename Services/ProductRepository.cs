using MyConsoleApp.Enums;
using MyConsoleApp.Models;

namespace MyConsoleApp.Services;

public class ProductRepository
{
    private readonly List<Product> _products = new();
    private int _nextCode = 1;

    public IReadOnlyList<Product> Products => _products;

    public int Count => _products.Count;

    public decimal WarehouseValue
        => _products.Sum(product => product.TotalValue);

    public Product Add(string name, decimal price, int quantity, Category category)
    {
        Product product = new(_nextCode, name, price, quantity, category);
        _nextCode++;

        _products.Add(product);
        return product;
    }

    public Product? Remove(int code)
    {
        Product? product = FindByCode(code);
        if (product is not null)
            _products.Remove(product);

        return product;
    }

    public Product? FindByCode(int code)
        => _products.FirstOrDefault(product => product.Code == code);

    public List<Product> SearchByName(string name)
        => _products
            .Where(product => product.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToList();

    public List<Product> GetByCategory(Category category)
        => _products.Where(product => product.Category == category).ToList();

    public List<Product> GetOutOfStock()
        => _products.Where(product => !product.InStock).ToList();

    public List<Product> Search(string query)
    {
        if (int.TryParse(query, out int code))
        {
            Product? product = FindByCode(code);
            return product is null ? new List<Product>() : new List<Product> { product };
        }

        List<Product> result = SearchByName(query);

        foreach (Category category in CategoryExtensions.All)
        {
            if (category.GetTitle().Contains(query, StringComparison.OrdinalIgnoreCase))
                result.AddRange(GetByCategory(category));
        }

        return result.Distinct().ToList();
    }

    public void FillWithTestData()
    {
        Add("Хлеб «Бородинский»", 45.00m, 30, Category.Продукты);
        Add("Молоко 3,2% 0,9 л", 89.90m, 24, Category.Продукты);
        Add("Сок яблочный 1 л", 129.50m, 0, Category.Напитки);
        Add("Чайник электрический 1,7 л", 4500.00m, 3, Category.БытоваяТехника);
        Add("Тетрадь в клетку 96 л", 55.00m, 40, Category.Канцтовары);
    }
}
