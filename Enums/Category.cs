namespace MyConsoleApp.Enums;

public enum Category
{
    Продукты = 1,
    Напитки = 2,
    БытоваяТехника = 3,
    Канцтовары = 4
}

public static class CategoryExtensions
{
    public static readonly Category[] All = (Category[])Enum.GetValues(typeof(Category));

    public static string GetTitle(this Category category) => category switch
    {
        Category.Продукты => "Продукты",
        Category.Напитки => "Напитки",
        Category.БытоваяТехника => "Бытовая техника",
        Category.Канцтовары => "Канцтовары",
        _ => "Категория не определена"
    };

    public static string GetNumberedTitle(this Category category)
        => $"{(int)category}. {category.GetTitle()}";
}
