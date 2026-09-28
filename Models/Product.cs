using System.Globalization;
using MyConsoleApp.Enums;

namespace MyConsoleApp.Models;

public class Product
{
    public int Code { get; }

    public string Name { get; }

    public decimal Price { get; }

    public int Quantity { get; private set; }

    public Category Category { get; }

    public bool InStock => Quantity > 0;

    public decimal TotalValue => Price * Quantity;

    public Product(int code, string name, decimal price, int quantity, Category category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название товара не может быть пустым.", nameof(name));

        if (price <= 0m)
            throw new ArgumentOutOfRangeException(nameof(price), price, "Цена товара должна быть больше нуля.");

        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), quantity, "Количество товара не может быть отрицательным.");

        Code = code;
        Name = name.Trim();
        Price = price;
        Quantity = quantity;
        Category = category;
    }

    public void AddQuantity(int amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Количество поставки должно быть больше нуля.");

        Quantity += amount;
    }

    public bool TrySell(int amount)
    {
        if (amount <= 0 || amount > Quantity)
            return false;

        Quantity -= amount;
        return true;
    }

    public void ReturnToStock(int amount)
    {
        if (amount <= 0)
            return;

        Quantity += amount;
    }

    public override string ToString()
        => $"Код {Code} | {Name} | {Price.ToString("0.00", CultureInfo.InvariantCulture)} руб. | {Quantity} шт. | {Category.GetTitle()} | {(InStock ? "есть" : "нет")}";

    public void PrintInfo()
    {
        string price = Price.ToString("0.00", CultureInfo.InvariantCulture);
        string total = TotalValue.ToString("0.00", CultureInfo.InvariantCulture);

        Console.WriteLine("  +----------------------------------------------------+");
        WriteRow("Код товара", Code.ToString());
        WriteRow("Название", Name);
        WriteRow("Цена", $"{price} руб.");
        WriteRow("Количество", $"{Quantity} шт.");
        WriteRow("Остался на складе", InStock ? "да" : "нет");
        WriteRow("Категория", Category.GetTitle());
        WriteRow("Стоимость на складе", $"{total} руб.");
        Console.WriteLine("  +----------------------------------------------------+");
    }

    private static void WriteRow(string label, string value)
        => Console.WriteLine($"  | {label,-22}| {value,-28}|");
}
