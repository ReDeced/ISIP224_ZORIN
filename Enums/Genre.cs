namespace ISIP224_ZORIN.Enums;

public enum Genre
{
    Роман = 1,
    Фэнтези = 2,
    Детектив = 3,
    Фантастика = 4,
    Сатира = 5,
    Учебная = 6
}

public static class GenreExtensions
{
    public static readonly Genre[] All = (Genre[])Enum.GetValues(typeof(Genre));

    public static string GetTitle(this Genre genre) => genre switch
    {
        Genre.Роман => "Роман",
        Genre.Фэнтези => "Фэнтези",
        Genre.Детектив => "Детектив",
        Genre.Фантастика => "Фантастика",
        Genre.Сатира => "Сатира",
        Genre.Учебная => "Учебная литература",
        _ => "Жанр не определён"
    };

    public static string GetNumberedTitle(this Genre genre)
        => $"{(int)genre}. {genre.GetTitle()}";

    public static Genre FromText(string text)
    {
        foreach (Genre genre in All)
        {
            if (string.Equals(genre.GetTitle(), text, StringComparison.OrdinalIgnoreCase)
                || string.Equals(genre.ToString(), text, StringComparison.OrdinalIgnoreCase))
            {
                return genre;
            }
        }

        if (int.TryParse(text, out int number) && Enum.IsDefined(typeof(Genre), number))
            return (Genre)number;

        throw new FormatException($"Жанр «{text}» не найден.");
    }
}