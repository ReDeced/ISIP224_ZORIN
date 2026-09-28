using System;
using ISIP224_ZORIN.Models;
using ISIP224_ZORIN.Services;
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

                TextAnalyzer.AnalyzeWords(text, out int wordCount, out string longest, out string shortest);
                int sentenceCount = TextAnalyzer.CountSentences(text);
                TextAnalyzer.AnalyzeLetters(text, out int vowels, out int consonants, out var frequency);

                Console.WriteLine();
                Console.WriteLine("--- РЕЗУЛЬТАТЫ АНАЛИЗА ---");
                Console.WriteLine($"Количество символов: {text.Length}");
                Console.WriteLine($"Количество слов: {wordCount}");
                Console.WriteLine($"Самое длинное слово: {longest}");
                Console.WriteLine($"Самое короткое слово: {shortest}");
                Console.WriteLine($"Количество предложений: {sentenceCount}");
                Console.WriteLine($"Гласных букв: {vowels}");
                Console.WriteLine($"Согласных букв: {consonants}");
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
