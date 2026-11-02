namespace ISIP224_ZORIN.Models;

public class Armor : Item
{
    private const int MinBonus = 1;
    private const int MaxBonus = 20;

    public Armor(string name, int defenseBonus)
        : base(name)
    {
        if (defenseBonus < MinBonus || defenseBonus > MaxBonus)
            throw new ArgumentOutOfRangeException(nameof(defenseBonus), defenseBonus,
                $"Бонус защиты должен быть от {MinBonus} до {MaxBonus}.");

        DefenseBonus = defenseBonus;
    }

    public int DefenseBonus { get; }

    public override string Stats => $"защита +{DefenseBonus}";
}
