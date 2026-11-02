namespace ISIP224_ZORIN.Models;

// Состояние героя: здоровье и две ячейки экипировки.
// Поле здоровья закрыто, изменять его можно только через методы.
public class Player
{
    public const int MaxHealth = 100;
    public const int BaseAttackPower = 10;
    public const int BaseDefense = 0;

    private int health;

    public Player(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя героя не может быть пустым.", nameof(name));

        Name = name.Trim();
        health = MaxHealth;
    }

    public string Name { get; }

    public int Health => health;

    public Weapon? Weapon { get; private set; }

    public Armor? Armor { get; private set; }

    public bool IsFrozen { get; private set; }

    public bool IsAlive => health > 0;

    // Итоговая атака: базовое значение плюс бонус оружия.
    public int AttackPower => BaseAttackPower + (Weapon?.AttackBonus ?? 0);

    // Итоговая защита: базовое значение плюс бонус доспеха.
    public int DefensePower => BaseDefense + (Armor?.DefenseBonus ?? 0);

    public void TakeDamage(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Урон не может быть отрицательным.");

        health = Math.Max(0, health - amount);
    }

    // Лечебное зелье мгновенно полностью восстанавливает здоровье.
    public void HealFully()
        => health = MaxHealth;

    // Восстановление здоровья при загрузке сохранения.
    public void RestoreHealth(int value)
        => health = Math.Clamp(value, 0, MaxHealth);

    public void Equip(Weapon weapon)
        => Weapon = weapon ?? throw new ArgumentNullException(nameof(weapon), "Оружие не найдено.");

    public void Equip(Armor armor)
        => Armor = armor ?? throw new ArgumentNullException(nameof(armor), "Доспех не найден.");

    public void Freeze()
        => IsFrozen = true;

    public void Unfreeze()
        => IsFrozen = false;

    public void PrintStatus()
    {
        Console.WriteLine($"  {Name}: HP {health}/{MaxHealth} | атака {AttackPower} | защита {DefensePower}");
        Console.WriteLine($"  Оружие: {(Weapon is null ? "нет" : $"{Weapon.Name} ({Weapon.Stats})")}");
        Console.WriteLine($"  Доспех: {(Armor is null ? "нет" : $"{Armor.Name} ({Armor.Stats})")}");
    }
}
