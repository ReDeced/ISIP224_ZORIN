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

                TextStatistics statistics = new(1, text);
                statistics.SetWords(wordCount, longest, shortest);
                statistics.SetSentences(sentenceCount);
                statistics.SetLetters(vowels, consonants, frequency);

                Console.WriteLine();
                Console.WriteLine("--- РЕЗУЛЬТАТЫ АНАЛИЗА ---");
                statistics.PrintResults();
                statistics.PrintLetterFrequency();
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
