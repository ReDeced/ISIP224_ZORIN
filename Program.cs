using MyConsoleApp.Enums;
using MyConsoleApp.Models;

namespace MyConsoleApp;

/// <summary>
/// Точка входа. Пока реализована только модель товара: создаём два товара
/// и выводим полную информацию о них.
/// </summary>
internal class Program
{
    static void Main(string[] args)
    {
        Product bread = new(1, "Хлеб «Бородинский»", 45.00m, 30, Category.Продукты);
        Product tea = new(2, "Чай чёрный 100 г", 210.00m, 0, Category.Напитки);

        Console.WriteLine("=== Модель товара ===");
        bread.PrintInfo();
        tea.PrintInfo();
    }
}
