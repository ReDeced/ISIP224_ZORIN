using System.Text.Encodings.Web;
using System.Text.Json;
using ISIP224_ZORIN.Enums;
using ISIP224_ZORIN.Models;

namespace ISIP224_ZORIN.Services;

// Сохранение и загрузка прогресса в JSON-файл рядом с программой.
// Повреждённый или устаревший файл не должен приводить к падению игры.
public static class SaveManager
{
    private const string FileName = "roguelike_save.json";

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private sealed class ItemData
    {
        public string Name { get; set; } = "";

        public Rarity Rarity { get; set; }

        public int Power { get; set; }
    }

    private sealed class SaveData
    {
        public string PlayerName { get; set; } = "";

        public int Health { get; set; }

        public int Turn { get; set; }

        public bool IsFrozen { get; set; }

        public ItemData? Weapon { get; set; }

        public ItemData? Armor { get; set; }
    }

    public static bool HasSave()
        => File.Exists(FileName);

    public static void Save(Player player, int turn)
    {
        SaveData data = new()
        {
            PlayerName = player.Name,
            Health = player.Health,
            Turn = turn,
            IsFrozen = player.IsFrozen,
            Weapon = player.Weapon is null
                ? null
                : new ItemData { Name = player.Weapon.Name, Rarity = player.Weapon.Rarity, Power = player.Weapon.AttackBonus },
            Armor = player.Armor is null
                ? null
                : new ItemData { Name = player.Armor.Name, Rarity = player.Armor.Rarity, Power = player.Armor.DefenseBonus },
        };

        File.WriteAllText(FileName, JsonSerializer.Serialize(data, Options));
        Console.WriteLine($"  Прогресс сохранён в файл {FileName}.");
    }

    // Возвращает героя и номер хода либо null, если сохранения нет или оно испорчено.
    public static (Player Player, int Turn)? Load()
    {
        if (!HasSave())
            return null;

        try
        {
            SaveData? data = JsonSerializer.Deserialize<SaveData>(File.ReadAllText(FileName));

            if (data is null || string.IsNullOrWhiteSpace(data.PlayerName) || data.Turn < 0)
                return null;

            Player player = new(data.PlayerName);
            player.RestoreHealth(data.Health);

            if (data.Weapon is { } weapon)
                player.Equip(new Weapon(weapon.Name, weapon.Rarity, weapon.Power));

            if (data.Armor is { } armor)
                player.Equip(new Armor(armor.Name, armor.Rarity, armor.Power));

            if (data.IsFrozen)
                player.Freeze();

            return (player, data.Turn);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static void DeleteSave()
    {
        if (File.Exists(FileName))
            File.Delete(FileName);
    }
}
