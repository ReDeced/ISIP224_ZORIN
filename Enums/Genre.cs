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
}