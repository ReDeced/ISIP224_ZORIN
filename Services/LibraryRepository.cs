using ISIP224_ZORIN.Enums;
using ISIP224_ZORIN.Models;

namespace ISIP224_ZORIN.Services;

public class LibraryRepository
{
    private readonly List<Book> _books = new();
    private int _nextId = 1;

    public IReadOnlyList<Book> Books => _books;

    public int Count => _books.Count;

    public decimal TotalValue => _books.Sum(book => book.Price);

    public Book Add(string title, string author, Genre genre, int year, decimal price)
    {
        Book book = new(_nextId, title, author, genre, year, price);
        _nextId++;

        _books.Add(book);
        return book;
    }

    public Book? Remove(int id)
    {
        Book? book = FindById(id);
        if (book is not null)
            _books.Remove(book);

        return book;
    }

    public Book? FindById(int id)
        => _books.FirstOrDefault(book => book.Id == id);

    public List<Book> FindByTitle(string title)
        => _books
            .Where(book => book.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
            .OrderBy(book => book.Title)
            .ToList();

    public List<Book> FindByAuthor(string author)
        => _books
            .Where(book => book.Author.Contains(author, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(book => book.Year)
            .ToList();

    public List<Book> FindByGenre(Genre genre)
        => _books
            .Where(book => book.Genre == genre)
            .OrderByDescending(book => book.Price)
            .ToList();

    public List<Book> SortByTitle()
        => _books.OrderBy(book => book.Title, StringComparer.OrdinalIgnoreCase).ToList();

    public List<Book> SortByYear()
        => _books.OrderByDescending(book => book.Year).ThenBy(book => book.Title, StringComparer.OrdinalIgnoreCase).ToList();

    public Book? GetMostExpensive()
        => _books.MaxBy(book => book.Price);

    public Book? GetCheapest()
        => _books.MinBy(book => book.Price);

    public IEnumerable<IGrouping<string, Book>> GroupByAuthor()
        => _books.GroupBy(book => book.Author).OrderBy(group => group.Key);

    public IEnumerable<IGrouping<Genre, Book>> GroupByGenre()
        => _books.GroupBy(book => book.Genre).OrderByDescending(group => group.Count());

    public void FillWithTestData()
    {
        Add("Мастер и Маргарита", "Булгаков М.А.", Genre.Роман, 1966, 450.00m);
        Add("Собачье сердце", "Булгаков М.А.", Genre.Сатира, 1925, 380.00m);
        Add("Дюна", "Герберт Ф.", Genre.Фэнтези, 1965, 890.00m);
        Add("Собака Баскервилей", "Дойл А.К.", Genre.Детектив, 1902, 410.00m);
        Add("Основы алгоритмов", "Кормен Т.", Genre.Учебная, 2013, 1250.00m);
    }
}