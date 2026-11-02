namespace ISIP224_ZORIN.Models;

// Гоблин: наносит критический урон с фиксированным шансом.
// Конструктор принимает множители — через него создаются боссы этой расы.
public class Goblin : Enemy
{
    public const int BaseHealth = 30;
    public const int BaseAttackPower = 8;
    public const int BaseDefense = 2;
    public const double BaseCritChance = 0.15;

    public Goblin(string name = "Гоблин",
        double healthMultiplier = 1.0,
        double attackMultiplier = 1.0,
        double defenseMultiplier = 1.0)
        : base(name,
            Scale(BaseHealth, healthMultiplier),
            Scale(BaseAttackPower, attackMultiplier),
            Scale(BaseDefense, defenseMultiplier))
    {
    }

    public override double CritChance => BaseCritChance;

    protected static int Scale(double value, double multiplier)
        => (int)Math.Round(value * multiplier);
}
