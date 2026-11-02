namespace ISIP224_ZORIN.Models;

// Абстрактный предмет из сундука: у каждого есть название и характеристики.
public abstract class Item
{
    protected Item(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название предмета не может быть пустым.", nameof(name));

        Name = name.Trim();
    }

    public string Name { get; }

    // Характеристики предмета одной строкой для консольного вывода.
    public abstract string Stats { get; }

    public virtual void PrintInfo()
        => Console.WriteLine($"  {Name} ({Stats})");
}
