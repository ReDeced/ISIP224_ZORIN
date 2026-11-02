using ISIP224_ZORIN.Models;
using ISIP224_ZORIN.Utils;

namespace ISIP224_ZORIN.Services;

// Один бой. Игрок всегда ходит первым: атака или защита.
// Защита даёт 40% шанс полностью уклониться от следующей атаки врага,
// иначе блок уменьшает урон на 70–100% характеристики защиты.
// После действия игрока враг атакует, применяя свои особенности
// (крит, игнор брони, заморозка) — полиморфизм через базовый Enemy.
public class Battle
{
    private const double DodgeChance = 0.40;
    private const double MinBlockShare = 0.70;
    private const double MaxBlockShare = 1.00;
    private const double CritMultiplier = 2.0;

    private readonly Player player;
    private readonly Enemy enemy;
    private readonly Random random;

    // Защита игрока действует только на ближайшую атаку врага.
    private bool isDefending;

    public Battle(Player player, Enemy enemy, Random random)
    {
        this.player = player;
        this.enemy = enemy;
        this.random = random;
    }

    public void Run()
    {
        Console.WriteLine("\n  >>> БОЙ <<<");
        enemy.PrintInfo();
        player.PrintStatus();

        while (player.IsAlive && enemy.IsAlive)
        {
            if (player.IsFrozen)
            {
                Console.WriteLine($"\n  {player.Name} заморожен и пропускает ход!");
                player.Unfreeze();
            }
            else
            {
                PlayerTurn();
            }

            if (enemy.IsAlive)
                EnemyTurn();
        }

        PrintResult();
    }

    private void PlayerTurn()
    {
        Console.WriteLine("\n  Ваш ход:");
        Console.WriteLine("    1. Атака");
        Console.WriteLine("    2. Защита (40% уклонения, иначе блок 70–100% защиты)");

        if (Input.ReadInt("  Выберите действие: ", 1, 2) == 1)
        {
            int damage = Math.Max(1, player.AttackPower - enemy.Defense);
            enemy.TakeDamage(damage);
            Console.WriteLine($"  Игрок бьёт: {damage} урона. У {enemy.Name} {enemy.Health}/{enemy.MaxHealth} HP.");
        }
        else
        {
            isDefending = true;
            Console.WriteLine("  Игрок встаёт в защиту.");
        }
    }

    private void EnemyTurn()
    {
        // Скелеты игнорируют защиту игрока полностью.
        int damage = enemy.IgnoresDefense
            ? enemy.AttackPower
            : Math.Max(1, enemy.AttackPower - player.DefensePower);

        if (random.NextDouble() < enemy.CritChance)
        {
            damage = (int)Math.Round(damage * CritMultiplier);
            Console.WriteLine("  Критический удар врага!");
        }

        if (isDefending)
        {
            isDefending = false;

            if (random.NextDouble() < DodgeChance)
            {
                Console.WriteLine($"  {player.Name} полностью уклонился от атаки!");
                damage = 0;
            }
            else
            {
                // Блок: урон снижается на случайные 70–100% характеристики защиты.
                double blockShare = MinBlockShare + (MaxBlockShare - MinBlockShare) * random.NextDouble();
                int blocked = (int)Math.Round(player.DefensePower * blockShare);
                damage = Math.Max(0, damage - blocked);
                Console.WriteLine($"  Блок снижает урон на {blocked}.");
            }
        }

        if (damage > 0)
        {
            player.TakeDamage(damage);
            Console.WriteLine($"  {enemy.Name} наносит {damage} урона. У игрока {player.Health}/{Player.MaxHealth} HP.");
        }

        if (enemy.IsAlive && enemy.FreezeChance > 0 && random.NextDouble() < enemy.FreezeChance)
        {
            player.Freeze();
            Console.WriteLine($"  {enemy.Name} замораживает игрока — следующий ход будет пропущен!");
        }
    }

    private void PrintResult()
    {
        Console.WriteLine(player.IsAlive
            ? $"\n  Победа! {enemy.Name} повержен(а)."
            : $"\n  {player.Name} повержен(а) противником {enemy.Name}…");
    }
}
