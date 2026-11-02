using ISIP224_ZORIN.Services;
using ISIP224_ZORIN.Utils;

namespace ISIP224_ZORIN;

internal class Program
{
    static void Main(string[] args)
    {
        try
        {
            RunStartMenu();
        }
        catch (InputClosedException)
        {
            Console.WriteLine("\nВвод закрыт, игра завершает работу.");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"\nНепредвиденная ошибка: {exception.Message}");
        }

        Console.WriteLine("\nДо свидания!");
    }

    private static void RunStartMenu()
    {
        Console.WriteLine("==========================================================");
        Console.WriteLine("              ТЕКСТОВЫЙ РОГАЛИК");
        Console.WriteLine("==========================================================");
        Console.WriteLine("Каждый ход героя ждёт сундук или враг, а каждый десятый — босс.");

        while (true)
        {
            Console.WriteLine("\n--- ГЛАВНОЕ МЕНЮ ---");
            Console.WriteLine("1. Новая игра");

            if (SaveManager.HasSave())
                Console.WriteLine("2. Продолжить");

            Console.WriteLine("0. Выход");

            string choice = Input.ReadText("Выберите действие: ");

            switch (choice)
            {
                case "1":
                    new Game().RunNewGame();
                    return;

                case "2" when SaveManager.HasSave():
                    new Game().RunFromSave();
                    return;

                case "0":
                    return;

                default:
                    Input.ShowError("Такого действия нет. Введите номер из меню.");
                    break;
            }
        }
    }
}
