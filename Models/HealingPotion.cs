using ISIP224_ZORIN.Enums;

namespace ISIP224_ZORIN.Models;

// Лечебное зелье: при получении мгновенно полностью лечит игрока.
public class HealingPotion : Item
{
    public HealingPotion(Rarity rarity = Rarity.Обычный, string name = "Лечебное зелье")
        : base(name, rarity)
    {
    }

    public override string Stats => $"полное лечение ({Player.MaxHealth} HP)";
}
