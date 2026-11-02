using ISIP224_ZORIN.Enums;

namespace ISIP224_ZORIN.Models;

// Абстрактный предмет из сундука: название, редкость и характеристики.
public abstract class Item
{
    protected Item(string name, Rarity rarity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название предмета не может быть пустым.", nameof(name));

        Name = name.Trim();
        Rarity = rarity;
    }

    public string Name { get; }

    public Rarity Rarity { get; }

    // Характеристики предмета одной строкой для консольного вывода.
    public abstract string Stats { get; }

    public virtual void PrintInfo()
        => Console.WriteLine($"  {Name} [{Rarity.GetTitle()}] ({Stats})");
}
