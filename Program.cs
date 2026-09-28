using System.Globalization;
using MyConsoleApp.Services;

namespace MyConsoleApp;

/// <summary>
/// Точка входа. Пока реализовано хранилище товаров: список из пяти тестовых
/// товаров и вывод полной информации о каждом из них.
/// </summary>
internal class Program
{
    static void Main(string[] args)
    {
        ProductRepository repository = new();
        repository.FillWithTestData();

        Console.WriteLine("=== Товары на складе ===");
        foreach (var product in repository.Products)
        {
            product.PrintInfo();
            Console.WriteLine();
        }

        string total = repository.WarehouseValue.ToString("0.00", CultureInfo.InvariantCulture);
        Console.WriteLine($"Всего товаров: {repository.Count}, стоимость склада: {total} руб.");
    }
}
