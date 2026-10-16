using ISIP224_ZORIN.Enums;
using ISIP224_ZORIN.Models;
using ISIP224_ZORIN.Services;
using ISIP224_ZORIN.Utils;

namespace ISIP224_ZORIN;

internal class Program
{
    private static readonly LibraryRepository Library = new();

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

                case "0":
                    return;

                default:
                    Input.ShowError("Такой команды нет. Введите номер из меню.");
                    break;
            }
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