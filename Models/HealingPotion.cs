namespace ISIP224_ZORIN.Models;

// Лечебное зелье: при получении мгновенно полностью лечит игрока.
public class HealingPotion : Item
{
    public HealingPotion(string name = "Лечебное зелье")
        : base(name)
    {
    }

    public override string Stats => $"полное лечение ({Player.MaxHealth} HP)";
}
