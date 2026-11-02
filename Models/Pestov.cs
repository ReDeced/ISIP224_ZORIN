namespace ISIP224_ZORIN.Models;

// Пестов С-- — босс расы скелетов: здоровье ×1.3, атака ×1.8, защита ×0.6.
// Игнорирует защиту игрока как скелет и вдобавок замораживает
// с шансом обычного мага +15%.
public class Pestov : Skeleton
{
    public Pestov()
        : base("Пестов С-- (босс-скелет)", 1.3, 1.8, 0.6)
    {
    }

    public override double FreezeChance => Mage.BaseFreezeChance + 0.15;
}
