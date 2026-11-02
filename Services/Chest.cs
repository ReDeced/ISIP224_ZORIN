using ISIP224_ZORIN.Models;
using ISIP224_ZORIN.Utils;

namespace ISIP224_ZORIN.Services;

// Сундук: выдаёт случайный предмет. Зелье выпивается сразу,
// оружие и доспех игрок сравнивает с текущей экипировкой и решает,
// забрать предмет или выбросить.
public class Chest
{
    private readonly ItemFactory items;

    public Chest(ItemFactory items)
    {
        this.items = items;
    }

    public void Open(Player player)
    {
        Console.WriteLine("\n  >>> СУНДУК <<<");

        Item item = items.CreateRandom();

        switch (item)
        {
            case HealingPotion potion:
                Console.WriteLine($"  В сундуке: {potion.Name} ({potion.Stats}).");
                player.HealFully();
                Console.WriteLine($"  Зелье выпито: здоровье восстановлено до {Player.MaxHealth} HP.");
                break;

            case Weapon weapon:
                OfferEquip(player, weapon, "оружие", player.Weapon);
                break;

            case Armor armor:
                OfferEquip(player, armor, "доспех", player.Armor);
                break;
        }
    }

    private void OfferEquip(Player player, Item newItem, string kind, Item? currentItem)
    {
        Console.WriteLine($"  В сундуке: {newItem.Name} ({newItem.Stats}).");
        Console.WriteLine(currentItem is null
            ? $"  Сейчас у игрока {kind} не экипировано."
            : $"  Текущее {kind}: {currentItem.Name} ({currentItem.Stats}).");

        if (Input.ReadYesNo($"  Взять новое {kind} вместо текущего? (д/н): "))
        {
            Equip(player, newItem);
            Console.WriteLine($"  Экипировано: {newItem.Name} ({newItem.Stats}).");
        }
        else
        {
            Console.WriteLine($"  {newItem.Name} выброшен(о).");
        }
    }

    private static void Equip(Player player, Item item)
    {
        switch (item)
        {
            case Weapon weapon:
                player.Equip(weapon);
                break;

            case Armor armor:
                player.Equip(armor);
                break;
        }
    }
}
