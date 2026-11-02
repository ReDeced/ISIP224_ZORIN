using ISIP224_ZORIN.Services;
using ISIP224_ZORIN.Utils;

namespace ISIP224_ZORIN;

internal class Program
{
    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("==========================================================");
            Console.WriteLine("              ТЕКСТОВЫЙ РОГАЛИК");
            Console.WriteLine("==========================================================");
            Console.WriteLine("Каждый ход героя ждёт сундук или враг, а каждый десятый — босс.");

            new Game().Run();
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
}
