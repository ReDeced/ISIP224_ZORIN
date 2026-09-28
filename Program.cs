using MyConsoleApp.Enums;
using MyConsoleApp.Models;
using MyConsoleApp.Services;
using MyConsoleApp.Utils;

namespace MyConsoleApp;

internal class Program
{
    private static readonly ProductRepository Repository = new();
    private static readonly SalesService Sales = new(Repository);

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
            Console.WriteLine("2. Удалить товар");
            Console.WriteLine("3. Заказать поставку товара");
            Console.WriteLine("4. Продать товар");
            Console.WriteLine("5. Поиск товаров");
            Console.WriteLine("6. Показать все товары");
            Console.WriteLine("7. История продаж");
            Console.WriteLine("8. Отменить последнюю продажу");
            Console.WriteLine("0. Выход");

            string choice = Input.ReadText("Выберите команду: ");

            switch (choice)
            {
                case "1":
                    AddProduct();
                    break;

                case "2":
                    RemoveProduct();
                    break;

                case "3":
                    OrderSupply();
                    break;

                case "4":
                    SellProduct();
                    break;

                case "5":
                    SearchProducts();
                    break;

                case "6":
                    PrintProductTable(Repository.Products);
                    break;

                case "7":
                    ShowSalesHistory();
                    break;

                case "8":
                    CancelLastSale();
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
        Category category = Input.ReadCategory();

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

    private static void RemoveProduct()
    {
        Console.WriteLine("\n--- УДАЛЕНИЕ ТОВАРА ---");

        int code = Input.ReadCode();
        Product? product = Repository.FindByCode(code);

        if (product is null)
        {
            Input.ShowError($"Товар с кодом {code} не найден.");
            return;
        }

        product.PrintInfo();
        Console.WriteLine();

        string answer = Input.ReadText("  Удалить этот товар? (д/н): ");
        if (!answer.StartsWith("д", StringComparison.OrdinalIgnoreCase)
            && !answer.StartsWith("y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("  Удаление отменено.");
            return;
        }

        Repository.Remove(code);
        Console.WriteLine($"\n  Товар с кодом {code} удалён.");
        Console.WriteLine($"  Осталось товаров: {Repository.Count}.");
    }

    private static void OrderSupply()
    {
        Console.WriteLine("\n--- ЗАКАЗ ПОСТАВКИ ТОВАРА ---");

        int code = Input.ReadCode();
        Product? product = Repository.FindByCode(code);

        if (product is null)
        {
            Input.ShowError($"Товар с кодом {code} не найден. Сначала добавьте его командой 1.");
            return;
        }

        Console.WriteLine($"\n  Текущий остаток: {product.Name} — {product.Quantity} шт.");

        int amount = Input.ReadPositiveInt("  Сколько штук поставить на склад: ");

        try
        {
            product.AddQuantity(amount);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            Input.ShowError($"Поставка не выполнена: {exception.Message}");
            return;
        }

        Console.WriteLine($"\n  Поставка выполнена. Товара на складе: {product.Quantity} шт.");
        Console.WriteLine($"  Стоимость склада: {Input.FormatNumber(Repository.WarehouseValue)} руб.");
    }

    private static void SellProduct()
    {
        Console.WriteLine("\n--- ПРОДАЖА ТОВАРА ---");

        int code = Input.ReadCode();
        Product? product = Repository.FindByCode(code);

        if (product is null)
        {
            Input.ShowError($"Товар с кодом {code} не найден.");
            return;
        }

        Console.WriteLine();
        product.PrintInfo();
        Console.WriteLine();

        if (!product.InStock)
        {
            Input.ShowError($"Товара «{product.Name}» нет на складе. Сначала закажите поставку (команда 3).");
            return;
        }

        int amount = Input.ReadPositiveInt($"  Сколько штук продать (доступно {product.Quantity}): ");
        SellStatus status = Sales.Sell(code, amount, out Sale? sale, out string message);

        if (status != SellStatus.Ok || sale is null)
        {
            Input.ShowError($"Продажа не оформлена. {message}");
            return;
        }

        Console.WriteLine();
        sale.PrintInfo();
        Console.WriteLine($"\n  {message}");
    }

    private static void ShowSalesHistory()
    {
        Console.WriteLine("\n--- ИСТОРИЯ ПРОДАЖ ---");

        List<Sale> sales = Sales.GetHistory();

        if (sales.Count == 0)
        {
            Console.WriteLine("  Продаж пока не было.");
            return;
        }

        for (int i = sales.Count - 1; i >= 0; i--)
        {
            Sale sale = sales[i];
            string total = Input.FormatNumber(sale.Total);
            string price = Input.FormatNumber(sale.Price);

            Console.WriteLine($"  Продажа №{sale.Number,-4} {sale.Date:dd.MM.yyyy HH:mm} | код {sale.ProductCode,-4} " +
                              $"| {sale.ProductName,-24} | {sale.Quantity} шт. по {price,9} руб. | {total,10} руб.");
        }

        Console.WriteLine($"\n  Всего продаж: {sales.Count}, выручка: {Input.FormatNumber(Sales.GetTotalRevenue())} руб.");
    }

    private static void CancelLastSale()
    {
        Console.WriteLine("\n--- ОТМЕНА ПОСЛЕДНЕЙ ПРОДАЖИ ---");

        Sale? lastSale = Sales.GetLastSale();
        if (lastSale is null)
        {
            Input.ShowError("История продаж пуста, отменять нечего.");
            return;
        }

        Console.WriteLine();
        lastSale.PrintInfo();
        Console.WriteLine();

        string answer = Input.ReadText("  Отменить эту продажу? (д/н): ");
        if (!answer.StartsWith("д", StringComparison.OrdinalIgnoreCase)
            && !answer.StartsWith("y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("  Отмена отменена, продажа остаётся в истории.");
            return;
        }

        CancelStatus status = Sales.CancelLastSale(out Sale? sale, out string message);

        if (status == CancelStatus.Ok)
            Console.WriteLine($"\n  {message}");
        else
            Input.ShowError(message);

        if (status == CancelStatus.ProductNotFound && sale is not null)
            Console.WriteLine($"  Продажа №{sale.Number} убрана из истории.");
    }

    private static void SearchProducts()
    {
        Console.WriteLine("\n--- ПОИСК ТОВАРОВ ---");
        Console.WriteLine("1. По коду");
        Console.WriteLine("2. По названию");
        Console.WriteLine("3. По категории");

        string choice = Input.ReadText("Выберите способ поиска: ");

        switch (choice)
        {
            case "1":
                SearchByCode();
                break;

            case "2":
                SearchByName();
                break;

            case "3":
                SearchByCategory();
                break;

            default:
                Input.ShowError("Такой способа поиска нет.");
                break;
        }
    }

    private static void SearchByCode()
    {
        int code = Input.ReadCode("  Искомый код: ");
        Product? product = Repository.FindByCode(code);

        if (product is null)
        {
            Input.ShowError($"Товар с кодом {code} не найден.");
            return;
        }

        Console.WriteLine("\n  Найден 1 товар:");
        product.PrintInfo();
    }

    private static void SearchByName()
    {
        string name = Input.ReadText("  Название товара (часть названия): ");
        List<Product> products = Repository.SearchByName(name);
        PrintSearchResult(products);
    }

    private static void SearchByCategory()
    {
        Category category = Input.ReadCategory();
        List<Product> products = Repository.GetByCategory(category);
        PrintSearchResult(products);
    }

    private static void PrintSearchResult(List<Product> products)
    {
        if (products.Count == 0)
        {
            Console.WriteLine("\n  Ничего не найдено.");
            return;
        }

        Console.WriteLine($"\n  Найдено товаров: {products.Count}");
        foreach (Product product in products)
        {
            product.PrintInfo();
            Console.WriteLine();
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
