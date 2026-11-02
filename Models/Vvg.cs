namespace ISIP224_ZORIN.Models;

// ВВГ — босс расы гоблинов: здоровье ×2.0, атака ×1.5, защита ×1.2,
// шанс крита +10% к обычному гоблину.
public class Vvg : Goblin
{
    public Vvg()
        : base("ВВГ (босс-гоблин)", 2.0, 1.5, 1.2)
    {
    }

    public override double CritChance => BaseCritChance + 0.10;
}
