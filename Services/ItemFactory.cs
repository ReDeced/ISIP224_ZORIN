using ISIP224_ZORIN.Enums;
using ISIP224_ZORIN.Models;

namespace ISIP224_ZORIN.Services;

// Генерирует случайный предмет для сундука: зелье, оружие или доспех.
// Сначала по весам разыгрывается редкость, затем сила предмета
// умножается на коэффициент редкости. Всё определяется генератором
// случайных чисел.
public class ItemFactory
{
    private const int MinBasePower = 2;
    private const int MaxBasePowerPlusOne = 7;

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
        Rarity rarity = RollRarity();
        int power = RollPower(rarity);

        return random.Next(3) switch
        {
            0 => new HealingPotion(rarity),
            1 => new Weapon(Pick(WeaponNames), rarity, power),
            _ => new Armor(Pick(ArmorNames), rarity, power),
        };
    }

    // Взвешенный розыгрыш: обычные предметы выпадают чаще легендарных.
    private Rarity RollRarity()
    {
        int totalWeight = RarityExtensions.All.Sum(rarity => rarity.GetDropWeight());
        int roll = random.Next(totalWeight);
        int cumulative = 0;

        foreach (Rarity rarity in RarityExtensions.All)
        {
            cumulative += rarity.GetDropWeight();

            if (roll < cumulative)
                return rarity;
        }

        return Rarity.Обычный;
    }

    // Базовая сила умножается на коэффициент редкости.
    private int RollPower(Rarity rarity)
    {
        int basePower = random.Next(MinBasePower, MaxBasePowerPlusOne);
        return Math.Max(1, (int)Math.Round(basePower * rarity.GetPowerMultiplier()));
    }

    private string Pick(string[] names)
        => names[random.Next(names.Length)];
}
