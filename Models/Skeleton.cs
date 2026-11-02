namespace ISIP224_ZORIN.Models;

// Скелет: его атака полностью игнорирует защиту игрока.
public class Skeleton : Enemy
{
    public const int BaseHealth = 40;
    public const int BaseAttackPower = 10;
    public const int BaseDefense = 4;

    public Skeleton(string name = "Скелет",
        double healthMultiplier = 1.0,
        double attackMultiplier = 1.0,
        double defenseMultiplier = 1.0)
        : base(name,
            Scale(BaseHealth, healthMultiplier),
            Scale(BaseAttackPower, attackMultiplier),
            Scale(BaseDefense, defenseMultiplier))
    {
    }

    public override bool IgnoresDefense => true;

    protected static int Scale(double value, double multiplier)
        => (int)Math.Round(value * multiplier);
}
