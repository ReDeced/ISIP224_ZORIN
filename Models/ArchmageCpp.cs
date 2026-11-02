namespace ISIP224_ZORIN.Models;

// Архимаг C++ — босс расы магов: здоровье ×1.8, атака ×1.6, защита ×1.1,
// шанс заморозки +10% к обычному магу.
public class ArchmageCpp : Mage
{
    public ArchmageCpp()
        : base("Архимаг C++ (босс-маг)", 1.8, 1.6, 1.1)
    {
    }

    public override double FreezeChance => BaseFreezeChance + 0.10;
}
