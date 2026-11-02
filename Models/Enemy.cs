namespace ISIP224_ZORIN.Models;

// Базовый враг: здоровье, атака и защита.
// Особые способности включаются переопределением виртуальных свойств
// в наследниках: бой работает с врагами через базовый класс (полиморфизм).
public abstract class Enemy
{
    private int health;

    protected Enemy(string name, int health, int attackPower, int defense)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя врага не может быть пустым.", nameof(name));

        if (health <= 0)
            throw new ArgumentOutOfRangeException(nameof(health), health, "Здоровье врага должно быть больше нуля.");

        if (attackPower < 0)
            throw new ArgumentOutOfRangeException(nameof(attackPower), attackPower, "Атака врага не может быть отрицательной.");

        if (defense < 0)
            throw new ArgumentOutOfRangeException(nameof(defense), defense, "Защита врага не может быть отрицательной.");

        Name = name;
        this.health = health;
        MaxHealth = health;
        AttackPower = attackPower;
        Defense = defense;
    }

    public string Name { get; }

    public int Health => health;

    public int MaxHealth { get; }

    public int AttackPower { get; }

    public int Defense { get; }

    // Шанс критического удара (гоблин и его босс).
    public virtual double CritChance => 0.0;

    // Полное игнорирование защиты игрока (скелет и его боссы).
    public virtual bool IgnoresDefense => false;

    // Шанс наложить заморозку на игрока (маг и его боссы).
    public virtual double FreezeChance => 0.0;

    public bool IsAlive => health > 0;

    public void TakeDamage(int amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), amount, "Урон не может быть отрицательным.");

        health = Math.Max(0, health - amount);
    }

    public virtual void PrintInfo()
    {
        Console.WriteLine("  +------------------------------------------------------+");
        WriteRow("Противник", Name);
        WriteRow("Здоровье", $"{health}/{MaxHealth}");
        WriteRow("Атака", AttackPower.ToString());
        WriteRow("Защита", Defense.ToString());

        if (CritChance > 0)
            WriteRow("Особенность", $"критический удар ({FormatChance(CritChance)})");

        if (IgnoresDefense)
            WriteRow("Особенность", "игнорирует защиту игрока");

        if (FreezeChance > 0)
            WriteRow("Особенность", $"заморозка ({FormatChance(FreezeChance)})");

        Console.WriteLine("  +------------------------------------------------------+");
    }

    protected void WriteRow(string label, string value)
        => Console.WriteLine($"  | {label,-18}| {value,-52}|");

    protected static string FormatChance(double chance)
        => Math.Round(chance * 100) + "%";
}
