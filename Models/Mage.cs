namespace ISIP224_ZORIN.Models;

// Маг: с некоторым шансом накладывает заморозку — игрок пропускает следующий ход.
public class Mage : Enemy
{
    public const int BaseHealth = 35;
    public const int BaseAttackPower = 9;
    public const int BaseDefense = 2;
    public const double BaseFreezeChance = 0.20;

    public Mage(string name = "Маг",
        double healthMultiplier = 1.0,
        double attackMultiplier = 1.0,
        double defenseMultiplier = 1.0)
        : base(name,
            Scale(BaseHealth, healthMultiplier),
            Scale(BaseAttackPower, attackMultiplier),
            Scale(BaseDefense, defenseMultiplier))
    {
    }

    public override double FreezeChance => BaseFreezeChance;

    protected static int Scale(double value, double multiplier)
        => (int)Math.Round(value * multiplier);
}
