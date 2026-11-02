using ISIP224_ZORIN.Models;
using ISIP224_ZORIN.Utils;

namespace ISIP224_ZORIN.Services;

// Игровой цикл: каждый ход случайно (50 на 50) выпадает сундук или враг,
// каждый десятый ход встречает случайного босса.
public class Game
{
    private const double ChestEventChance = 0.5;
    private const int BossPeriod = 10;

    private readonly Random random = new();
    private readonly ItemFactory items;
    private readonly Chest chest;

    private Player player = new("Герой");
    private int turn;

    public Game()
    {
        items = new ItemFactory(random);
        chest = new Chest(items);
    }

    public void RunNewGame()
    {
        player = CreatePlayer();
        turn = 0;
        SaveManager.DeleteSave();

        Console.WriteLine($"\n  {player.Name} спускается в подземелье.");
        player.PrintStatus();

        Play();
    }

    public void RunFromSave()
    {
        (Player, int)? state = SaveManager.Load();

        if (state is not { } saved)
        {
            Input.ShowError("Сохранение не найдено или повреждено. Начинается новая игра.");
            RunNewGame();
            return;
        }

        player = saved.Item1;
        turn = saved.Item2;

        Console.WriteLine($"\n  Прогресс загружен: ход {turn}.");
        player.PrintStatus();

        Play();
    }

    private void Play()
    {
        while (player.IsAlive)
        {
            turn++;
            PrintTurnHeader();

            if (turn % BossPeriod == 0)
                StartBattle(CreateBoss());
            else if (random.NextDouble() < ChestEventChance)
                chest.Open(player);
            else
                StartBattle(CreateEnemy());

            if (!player.IsAlive)
                break;

            player.PrintStatus();
            SaveManager.Save(player, turn);

            if (!Input.ReadYesNo("\n  Продолжить поход? (д/н): "))
                break;
        }

        PrintGameOver();
    }

    private Player CreatePlayer()
    {
        string name = Input.ReadText("Введите имя героя: ");
        return new Player(name);
    }

    private void PrintTurnHeader()
    {
        Console.WriteLine();
        Console.WriteLine("==========================================================");
        Console.WriteLine($"  ХОД {turn}");
        Console.WriteLine("==========================================================");
    }

    private void StartBattle(Enemy enemy)
    {
        new Battle(player, enemy, random).Run();
    }

    // Обычный враг выбирается случайно из трёх рас.
    private Enemy CreateEnemy()
    {
        return random.Next(3) switch
        {
            0 => new Goblin(),
            1 => new Skeleton(),
            _ => new Mage(),
        };
    }

    // Каждый десятый ход приходит случайный босс.
    private Enemy CreateBoss()
    {
        Console.WriteLine("\n  Земля дрожит… навстречу идёт босс!");

        return random.Next(4) switch
        {
            0 => new Vvg(),
            1 => new Kovalsky(),
            2 => new ArchmageCpp(),
            _ => new Pestov(),
        };
    }

    private void PrintGameOver()
    {
        Console.WriteLine();
        Console.WriteLine("==========================================================");

        if (player.IsAlive)
            Console.WriteLine($"  {player.Name} выходит из подземелья живым(ой).");
        else
        {
            Console.WriteLine($"  Игра окончена. {player.Name} пал(а) в бою…");
            SaveManager.DeleteSave();
        }

        Console.WriteLine($"  Пройдено ходов: {turn}.");
        Console.WriteLine("==========================================================");
    }
}
