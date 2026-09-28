using System.Globalization;
using MyConsoleApp.Enums;

namespace MyConsoleApp.Models;

public class Sale
{
    public int Number { get; }

    public int ProductCode { get; }

    public string ProductName { get; }

    public Category Category { get; }

    public decimal Price { get; }

    public int Quantity { get; }

    public DateTime Date { get; }

    public decimal Total => Price * Quantity;

    public Sale(int number, Product product, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), quantity, "Количество продажи должно быть больше нуля.");

        Number = number;
        ProductCode = product.Code;
        ProductName = product.Name;
        Category = product.Category;
        Price = product.Price;
        Quantity = quantity;
        Date = DateTime.Now;
    }

    public override string ToString()
    {
        string price = Price.ToString("0.00", CultureInfo.InvariantCulture);
        string total = Total.ToString("0.00", CultureInfo.InvariantCulture);

        return $"Продажа №{Number} от {Date:dd.MM.yyyy HH:mm} | {ProductName} | {Quantity} шт. по {price} руб. | итого {total} руб.";
    }

    public void PrintInfo()
    {
        string price = Price.ToString("0.00", CultureInfo.InvariantCulture);
        string total = Total.ToString("0.00", CultureInfo.InvariantCulture);

        Console.WriteLine("  +----------------------------------------------------+");
        Console.WriteLine($"  | Номер продажи       | {Number,-28}|");
        Console.WriteLine($"  | Дата и время        | {Date.ToString("dd.MM.yyyy HH:mm:ss"),-28}|");
        Console.WriteLine($"  | Код товара          | {ProductCode,-28}|");
        Console.WriteLine($"  | Название            | {ProductName,-28}|");
        Console.WriteLine($"  | Категория           | {Category.GetTitle(),-28}|");
        Console.WriteLine($"  | Цена за штуку       | {price + " руб.",-28}|");
        Console.WriteLine($"  | Количество          | {Quantity + " шт.",-28}|");
        Console.WriteLine($"  | Сумма продажи       | {total + " руб.",-28}|");
        Console.WriteLine("  +----------------------------------------------------+");
    }
}
