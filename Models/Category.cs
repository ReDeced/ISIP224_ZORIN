namespace ISIP224_ZORIN.Models;

// Таблица Category: справочник категорий товаров (3НФ — категория
// вынесена из Product в отдельную таблицу).
public class Category
{
    public Category(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; }

    public string Name { get; }
}
