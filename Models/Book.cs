using System.Globalization;
using ISIP224_ZORIN.Enums;

namespace ISIP224_ZORIN.Models;

public class Book
{
    public int Id { get; }

    public string Title { get; }

    public string Author { get; }

    public Genre Genre { get; }

    public int Year { get; }

    public decimal Price { get; }

    public const int MinYear = 1450;

    public static int MaxYear => DateTime.Now.Year + 1;

    public Book(int id, string title, string author, Genre genre, int year, decimal price)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Название книги не может быть пустым.", nameof(title));

        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Автор не может быть пустым.", nameof(author));

        if (price <= 0m)
            throw new ArgumentOutOfRangeException(nameof(price), price, "Цена книги должна быть больше нуля.");

        if (year < MinYear || year > MaxYear)
            throw new ArgumentOutOfRangeException(nameof(year), year, $"Год издания должен быть от {MinYear} до {MaxYear}.");

        Id = id;
        Title = title.Trim();
        Author = author.Trim();
        Genre = genre;
        Year = year;
        Price = price;
    }

    public override string ToString()
        => $"№{Id} | {Title} | {Author} | {Genre.GetTitle()} | {Year} г. | {FormatPrice(Price)}";

    public void PrintInfo()
    {
        Console.WriteLine("  +------------------------------------------------------+");
        WriteRow("Идентификатор", Id.ToString());
        WriteRow("Название", Title);
        WriteRow("Автор", Author);
        WriteRow("Жанр", Genre.GetTitle());
        WriteRow("Год издания", Year.ToString());
        WriteRow("Цена", FormatPrice(Price));
        Console.WriteLine("  +------------------------------------------------------+");
    }

    private static void WriteRow(string label, string value)
        => Console.WriteLine($"  | {label,-18}| {value,-52}|");

    public static string FormatPrice(decimal value)
        => value.ToString("0.00", CultureInfo.InvariantCulture) + " руб.";
}