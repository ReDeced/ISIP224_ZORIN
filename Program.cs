using ISIP224_ZORIN.Enums;
using ISIP224_ZORIN.Models;
using ISIP224_ZORIN.Services;
using ISIP224_ZORIN.Utils;

namespace ISIP224_ZORIN;

internal class Program
{
    private static readonly LibraryRepository Library = new();
    private static readonly ShoppingCart Cart = new();

    static void Main(string[] args)
    {
        try
        {
            Library.FillWithTestData();

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
        Console.WriteLine("              БИБЛИОТЕКА");
        Console.WriteLine("==========================================================");
        Console.WriteLine($"Книг в фонде: {Library.Count}");
        Console.WriteLine($"Общая стоимость фонда: {Input.FormatMoney(Library.TotalValue)} руб.");
    }

    private static void RunMainMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- ГЛАВНОЕ МЕНЮ ---");
            Console.WriteLine("1. Добавить книгу");
            Console.WriteLine("2. Удалить книгу");
            Console.WriteLine("3. Найти книги");
            Console.WriteLine("4. Отсортировать книги");
            Console.WriteLine("5. Самая дорогая и самая дешёвая книга");
            Console.WriteLine("6. Книги по авторам");
            Console.WriteLine("7. Показать все книги");
            Console.WriteLine("8. Вставить блок книг");
            Console.WriteLine("9. Добавить книгу в корзину");
            Console.WriteLine("10. Показать корзину");
            Console.WriteLine("11. Убрать книгу из корзины");
            Console.WriteLine("12. Очистить корзину");
            Console.WriteLine("0. Выход");

            string choice = Input.ReadText("Выберите команду: ");

            switch (choice)
            {
                case "1":
                    AddBook();
                    break;

                case "2":
                    RemoveBook();
                    break;

                case "3":
                    FindBooks();
                    break;

                case "4":
                    SortBooks();
                    break;

                case "5":
                    ShowPriceExtremes();
                    break;

                case "6":
                    ShowAuthors();
                    break;

                case "7":
                    PrintBookTable(Library.Books);
                    break;

                case "8":
                    ImportBlock();
                    break;

                case "9":
                    AddToCart();
                    break;

                case "10":
                    ShowCart();
                    break;

                case "11":
                    RemoveFromCart();
                    break;

                case "12":
                    ClearCart();
                    break;

                case "0":
                    return;

                default:
                    Input.ShowError("Такой команды нет. Введите номер из меню.");
                    break;
            }
        }
    }

    private static void AddBook()
    {
        Console.WriteLine("\n--- ДОБАВЛЕНИЕ КНИГИ ---");

        string title = Input.ReadText("  Название: ");
        string author = Input.ReadText("  Автор: ");
        Genre genre = ReadGenre();
        int year = Input.ReadYear("  Год издания: ");
        decimal price = Input.ReadPrice("  Цена: ");

        try
        {
            Book book = Library.Add(title, author, genre, year, price);
            Console.WriteLine($"\n  Книга добавлена. Идентификатор: {book.Id}");
            book.PrintInfo();
        }
        catch (ArgumentException exception)
        {
            Input.ShowError($"Книга не добавлена: {exception.Message}");
        }
    }

    private static void ImportBlock()
    {
        Console.WriteLine("\n--- ПАКЕТНЫЙ ИМПОРТ КНИГ ---");

        List<string> lines = Input.ReadLines(
            "  Формат строки: Название;Автор;Жанр;Год;Цена",
            "!done");

        if (lines.Count == 0)
        {
            Input.ShowError("Не введено ни одной строки.");
            return;
        }

        BatchImportResult result = BatchImporter.Import(Library, lines);

        Console.WriteLine($"\n  Добавлено книг: {result.Added}.");

        if (result.Errors.Count > 0)
        {
            Console.WriteLine($"  Ошибок ввода: {result.Errors.Count}.");

            foreach (string error in result.Errors)
                Console.WriteLine($"    {error}");
        }

        Console.WriteLine($"  Всего книг в фонде: {Library.Count}, стоимость: {Input.FormatMoney(Library.TotalValue)} руб.");
    }

    private static void AddToCart()
    {
        Console.WriteLine("\n--- ДОБАВЛЕНИЕ КНИГИ В КОРЗИНУ ---");

        int id = Input.ReadId();
        Book? book = Library.FindById(id);

        if (book is null)
        {
            Input.ShowError($"Книга с идентификатором {id} не найдена.");
            return;
        }

        book.PrintInfo();
        Console.WriteLine();

        int quantity = Input.ReadPositiveInt("  Сколько экземпляров взять: ");

        try
        {
            CartItem item = Cart.Add(book, quantity);
            Console.WriteLine($"\n  В корзине: {item.Book.Title} — {item.Quantity} шт.");
            Console.WriteLine($"  Стоимость корзины: {Input.FormatMoney(Cart.TotalPrice)} руб.");
        }
        catch (ArgumentOutOfRangeException exception)
        {
            Input.ShowError($"Книга не добавлена в корзину: {exception.Message}");
        }
    }

    private static void ShowCart()
    {
        Console.WriteLine("\n--- КОРЗИНА ---");

        List<CartItem> items = Cart.GetItems();

        if (items.Count == 0)
        {
            Console.WriteLine("  Корзина пуста.");
            return;
        }

        Console.WriteLine($"  {"№",-5}{"Название",-30}{"Цена, руб.",-13}{"Кол-во",-9}{"Сумма, руб.",-14}");
        Console.WriteLine($"  {"",-5}{"",-30}{"",-13}{"",-9}{"",-14}");

        int position = 1;

        foreach (CartItem item in items)
        {
            Console.WriteLine($"  {position,-5}{Cut(item.Book.Title, 29),-30}{Input.FormatMoney(item.Book.Price),-13}" +
                              $"{item.Quantity,-9}{Input.FormatMoney(item.Total),-14}");
            position++;
        }

        Console.WriteLine($"  {"",-5}{"",-30}{"",-13}{"",-9}{"",-14}");
        Console.WriteLine($"  {"ИТОГО",-49}{Cart.TotalQuantity,-9}{Input.FormatMoney(Cart.TotalPrice),-14}");

        Console.WriteLine($"\n  Позиций в корзине: {Cart.Count}, всего экземпляров: {Cart.TotalQuantity}.");
        Console.WriteLine($"  Итоговая стоимость корзины: {Input.FormatMoney(Cart.TotalPrice)} руб.");
    }

    private static void RemoveFromCart()
    {
        Console.WriteLine("\n--- УДАЛЕНИЕ КНИГИ ИЗ КОРЗИНЫ ---");

        if (Cart.IsEmpty)
        {
            Input.ShowError("Корзина пуста.");
            return;
        }

        List<CartItem> items = Cart.GetItems();

        Console.WriteLine($"  {"№",-5}{"Название",-32}{"Кол-во",-9}");
        int position = 1;

        foreach (CartItem item in items)
        {
            Console.WriteLine($"  {position,-5}{Cut(item.Book.Title, 31),-32}{item.Quantity,-9}");
            position++;
        }

        int index = Input.ReadInt("Номер позиции в корзине: ", 1, items.Count);
        CartItem chosen = items[index - 1];

        if (!Cart.Decrease(chosen.Book.Id))
            Input.ShowError("Не удалось изменить корзину.");

        Console.WriteLine($"\n  Позиция обновлена. Стоимость корзины: {Input.FormatMoney(Cart.TotalPrice)} руб.");
    }

    private static void ClearCart()
    {
        Console.WriteLine("\n--- ОЧИСТКА КОРЗИНЫ ---");

        if (Cart.IsEmpty)
        {
            Input.ShowError("Корзина уже пуста.");
            return;
        }

        decimal total = Cart.TotalPrice;
        Cart.Clear();

        Console.WriteLine($"  Корзина очищена. Утраченная сумма: {Input.FormatMoney(total)} руб.");
    }

    private static void FindBooks()
    {
        Console.WriteLine("\n--- ПОИСК КНИГ ---");
        Console.WriteLine("1. По названию");
        Console.WriteLine("2. По автору");
        Console.WriteLine("3. По жанру");

        string choice = Input.ReadText("Выберите способ поиска: ");

        switch (choice)
        {
            case "1":
                string title = Input.ReadText("  Название книги (часть названия): ");
                PrintFound(Library.FindByTitle(title), $"по названию «{title}»");
                break;

            case "2":
                string author = Input.ReadText("  Автор (часть имени): ");
                PrintFound(Library.FindByAuthor(author), $"по автору «{author}»");
                break;

            case "3":
                Genre genre = ReadGenre();
                PrintFound(Library.FindByGenre(genre), $"по жанру «{genre.GetTitle()}»");
                break;

            default:
                Input.ShowError("Такой способ поиска не найден. Введите номер из списка.");
                break;
        }
    }

    private static void PrintFound(List<Book> books, string description)
    {
        if (books.Count == 0)
        {
            Console.WriteLine($"\n  Книги {description} не найдены.");
            return;
        }

        Console.WriteLine($"\n  Найдено книг {description}: {books.Count}");

        foreach (Book book in books)
        {
            book.PrintInfo();
            Console.WriteLine();
        }
    }

    private static void SortBooks()
    {
        Console.WriteLine("\n--- СОРТИРОВКА КНИГ ---");
        Console.WriteLine("1. По названию");
        Console.WriteLine("2. По году издания");

        string choice = Input.ReadText("Выберите поле сортировки: ");

        switch (choice)
        {
            case "1":
                PrintBookTable(Library.SortByTitle());
                break;

            case "2":
                PrintBookTable(Library.SortByYear());
                break;

            default:
                Input.ShowError("Такой вариант сортировки не найден. Введите номер из списка.");
                break;
        }
    }

    private static void ShowPriceExtremes()
    {
        Console.WriteLine("\n--- САМАЯ ДОРОГАЯ И САМАЯ ДЁШЕВАЯ КНИГА ---");

        Book? mostExpensive = Library.GetMostExpensive();
        Book? cheapest = Library.GetCheapest();

        if (mostExpensive is null || cheapest is null)
        {
            Console.WriteLine("  В фонде пока нет книг.");
            return;
        }

        Console.WriteLine("\n  Самая дорогая:");
        mostExpensive.PrintInfo();

        Console.WriteLine("\n  Самая дешёвая:");
        cheapest.PrintInfo();

        decimal difference = mostExpensive.Price - cheapest.Price;
        Console.WriteLine($"\n  Разница в цене: {Input.FormatMoney(difference)} руб.");
    }

    private static void ShowAuthors()
    {
        Console.WriteLine("\n--- КНИГИ ПО АВТОРАМ ---");

        List<IGrouping<string, Book>> groups = Library.GroupByAuthor().ToList();

        if (groups.Count == 0)
        {
            Console.WriteLine("  В фонде пока нет книг.");
            return;
        }

        Console.WriteLine($"  Авторов в фонде: {groups.Count}");
        Console.WriteLine($"  {"Автор",-28}{"Книг",-8}{"Сумма, руб.",-14}Названия");

        int totalBooks = 0;

        foreach (IGrouping<string, Book> group in groups)
        {
            List<Book> books = group.ToList();
            totalBooks += books.Count;

            decimal sum = books.Sum(book => book.Price);
            string titles = string.Join(", ", books.Select(book => Cut(book.Title, 18)));

            Console.WriteLine($"  {Cut(group.Key, 27),-28}{books.Count,-8}{Input.FormatMoney(sum),-14}{titles}");
        }

        Console.WriteLine($"\n  Всего авторов: {groups.Count}, книг: {totalBooks}.");
    }

    private static void RemoveBook()
    {
        Console.WriteLine("\n--- УДАЛЕНИЕ КНИГИ ---");

        int id = Input.ReadId();
        Book? book = Library.FindById(id);

        if (book is null)
        {
            Input.ShowError($"Книга с идентификатором {id} не найдена.");
            return;
        }

        book.PrintInfo();
        Console.WriteLine();

        if (!Input.ReadYesNo("  Удалить эту книгу? (д/н): "))
        {
            Console.WriteLine("  Удаление отменено.");
            return;
        }

        Library.Remove(id);
        Console.WriteLine($"\n  Книга с идентификатором {id} удалена.");
        Console.WriteLine($"  Осталось книг: {Library.Count}.");
    }

    private static Genre ReadGenre()
    {
        while (true)
        {
            Console.WriteLine("  Жанры:");
            foreach (Genre genre in GenreExtensions.All)
                Console.WriteLine($"    {genre.GetNumberedTitle()}");

            string text = Input.ReadLine("  Выберите жанр (номер или название): ").Trim();

            foreach (Genre genre in GenreExtensions.All)
            {
                if (string.Equals(genre.GetTitle(), text, StringComparison.OrdinalIgnoreCase))
                    return genre;
            }

            if (int.TryParse(text, out int number) && Enum.IsDefined(typeof(Genre), number))
                return (Genre)number;

            Input.ShowError("Такого жанра нет. Выберите номер или название из списка.");
        }
    }

    private static void PrintBookTable(IReadOnlyList<Book> books)
    {
        Console.WriteLine("\n--- СПИСОК КНИГ ---");

        if (books.Count == 0)
        {
            Console.WriteLine("  Книг в фонде пока нет.");
            return;
        }

        Console.WriteLine($"  {"ID",-5}{"Название",-28}{"Автор",-22}{"Жанр",-20}{"Год",-7}{"Цена, руб.",-11}");

        foreach (Book book in books)
        {
            Console.WriteLine($"  {book.Id,-5}{Cut(book.Title, 27),-28}{Cut(book.Author, 21),-22}{Cut(book.Genre.GetTitle(), 19),-20}" +
                              $"{book.Year,-7}{Input.FormatMoney(book.Price),-11}");
        }

        decimal total = books.Sum(book => book.Price);
        Console.WriteLine($"\n  Всего книг: {books.Count}, стоимость: {Input.FormatMoney(total)} руб.");
    }

    private static string Cut(string text, int maxLength)
        => text.Length <= maxLength ? text : text[..(maxLength - 1)] + "…";
}