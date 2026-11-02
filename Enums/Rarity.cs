namespace ISIP224_ZORIN.Enums;

// Редкость предметов: усиливает характеристики и задаёт шанс выпадения.
public enum Rarity
{
    Обычный = 1,
    Редкий = 2,
    Эпический = 3,
    Легендарный = 4
}

public static class RarityExtensions
{
    public static readonly Rarity[] All = (Rarity[])Enum.GetValues(typeof(Rarity));

    public static string GetTitle(this Rarity rarity) => rarity switch
    {
        Rarity.Обычный => "Обычный",
        Rarity.Редкий => "Редкий",
        Rarity.Эпический => "Эпический",
        Rarity.Легендарный => "Легендарный",
        _ => "Неизвестно"
    };

    // Во сколько раз редкость усиливает базовую силу предмета.
    public static double GetPowerMultiplier(this Rarity rarity) => rarity switch
    {
        Rarity.Обычный => 1.0,
        Rarity.Редкий => 1.5,
        Rarity.Эпический => 2.0,
        Rarity.Легендарный => 3.0,
        _ => 1.0
    };

    // Вес при розыгрыше: чем больше вес, тем чаще выпадает редкость.
    public static int GetDropWeight(this Rarity rarity) => rarity switch
    {
        Rarity.Обычный => 50,
        Rarity.Редкий => 30,
        Rarity.Эпический => 15,
        Rarity.Легендарный => 5,
        _ => 0
    };
}
