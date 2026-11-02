using ISIP224_ZORIN.Models;

namespace ISIP224_ZORIN.Services;

// Генерирует случайный предмет для сундука: зелье, оружие или доспех.
// Все значения определяются генератором случайных чисел.
public class ItemFactory
{
    private static readonly string[] WeaponNames =
    [
        "Ржавый меч", "Короткий меч", "Боевой топор", "Клинок охотника", "Молот грома"
    ];

    private static readonly string[] ArmorNames =
    [
        "Кожаный доспех", "Кольчуга", "Пластинчатый доспех", "Щит стража", "Доспех драконителя"
    ];

    private readonly Random random;

    public ItemFactory(Random random)
    {
        this.random = random;
    }

    public Item CreateRandom()
    {
        return random.Next(3) switch
        {
            0 => new HealingPotion(),
            1 => new Weapon(Pick(WeaponNames), random.Next(2, 9)),
            _ => new Armor(Pick(ArmorNames), random.Next(1, 7)),
        };
    }

    private string Pick(string[] names)
        => names[random.Next(names.Length)];
}
