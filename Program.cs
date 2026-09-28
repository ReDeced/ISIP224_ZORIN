using System.Globalization;
using MyConsoleApp.Enums;
using MyConsoleApp.Models;
using MyConsoleApp.Services;
using MyConsoleApp.Utils;

namespace MyConsoleApp;

internal class Program
{
    private static readonly ProductRepository Repository = new();

    static void Main(string[] args)
    {
        try
        {
            Repository.FillWithTestData();

            PrintWelcome();
            RunMainMenu();
        }
        catch (InputClosedException)
        {
            Console.WriteLine("\nВвод закрыт, программа завершает работу.");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"\nНепредвиденная ошибка: {exception.Message}");
        }

        Console.WriteLine("\nДо свидания!");
    }

    private static void PrintWelcome()
    {
        Console.WriteLine("==========================================================");
        Console.WriteLine("            УЧЁТ ТОВАРОВ В МАГАЗИНЕ");
        Console.WriteLine("==========================================================");
        Console.WriteLine($"На складе товаров: {Repository.Count}");
        Console.WriteLine($"Стоимость склада: {Input.FormatNumber(Repository.WarehouseValue)} руб.");
    }

    private static void RunMainMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- ГЛАВНОЕ МЕНЮ ---");
            Console.WriteLine("1. Добавить товар");
            Console.WriteLine("2. Показать все товары");
            Console.WriteLine("0. Выход");

            string choice = Input.ReadText("Выберите команду: ");

            switch (choice)
            {
                case "1":
                    AddProduct();
                    break;

                case "2":
                    PrintProductTable(Repository.Products);
                    break;

                case "0":
                    return;

                default:
                    Input.ShowError("Такой команды нет. Введите номер из меню.");
                    break;
            }
        }
    }

    private static void AddProduct()
    {
        Console.WriteLine("\n--- ДОБАВЛЕНИЕ ТОВАРА ---");

        string name = Input.ReadText("  Название товара: ");
        decimal price = Input.ReadPositiveDecimal("  Цена за штуку: ");
        int quantity = Input.ReadNonNegativeInt("  Количество на складе: ");
        var category = Input.ReadCategory();

        try
        {
            Product product = Repository.Add(name, price, quantity, category);

            Console.WriteLine($"\n  Товар добавлен. Ему присвоен код {product.Code}.");
            product.PrintInfo();
        }
        catch (ArgumentException exception)
        {
            Input.ShowError($"Товар не добавлен: {exception.Message}");
        }
    }

    private static void PrintProductTable(IReadOnlyList<Product> products)
    {
        Console.WriteLine("\n--- СПИСОК ТОВАРОВ ---");

        if (products.Count == 0)
        {
            Console.WriteLine("  Товаров пока нет.");
            return;
        }

        Console.WriteLine($"  {"Код",-5}{"Название",-26}{"Категория",-20}{"Цена, руб.",-12}{"Кол-во",-9}{"Наличие",-10}");

        foreach (Product product in products)
        {
            Console.WriteLine($"  {product.Code,-5}{Cut(product.Name, 25),-26}{Cut(product.Category.GetTitle(), 19),-20}" +
                              $"{Input.FormatNumber(product.Price),-12}{product.Quantity,-9}{(product.InStock ? "есть" : "нет"),-10}");
        }

        Console.WriteLine($"\n  Всего товаров: {products.Count}, стоимость склада: {Input.FormatNumber(Repository.WarehouseValue)} руб.");
    }

    private static string Cut(string text, int maxLength)
        => text.Length <= maxLength ? text : text[..(maxLength - 1)] + "…";
}
