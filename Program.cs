using System;
using ISIP224_ZORIN.Utils;

namespace ISIP224_ZORIN
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("==================================================");
                Console.WriteLine("        АНАЛИЗ ТЕКСТА");
                Console.WriteLine("==================================================");

                string text = TextInput.ReadText();

                Console.WriteLine();
                Console.WriteLine($"Введён текст: {text.Length} символов.");
            }
            catch (InputClosedException)
            {
                Console.WriteLine("\nВвод закрыт, программа завершает работу.");
            }
            catch (Exception exception)
            {
                Console.WriteLine($"\nНепредвиденная ошибка: {exception.Message}");
            }

            Console.WriteLine("\nДо свидания!");
        }
    }
}
