namespace ISIP224_ZORIN.Models;

public class Weapon : Item
{
    private const int MinBonus = 1;
    private const int MaxBonus = 20;

    public Weapon(string name, int attackBonus)
        : base(name)
    {
        if (attackBonus < MinBonus || attackBonus > MaxBonus)
            throw new ArgumentOutOfRangeException(nameof(attackBonus), attackBonus,
                $"Бонус атаки должен быть от {MinBonus} до {MaxBonus}.");

        AttackBonus = attackBonus;
    }

    public int AttackBonus { get; }

    public override string Stats => $"атака +{AttackBonus}";
}
