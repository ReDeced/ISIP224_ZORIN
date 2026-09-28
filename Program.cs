using System;
using System.Collections.Generic;
using ISIP224_ZORIN.Models;
using ISIP224_ZORIN.Services;
using ISIP224_ZORIN.Utils;

namespace ISIP224_ZORIN
{
    internal class Program
    {
        private static readonly List<TextStatistics> History = new List<TextStatistics>();

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("==================================================");
                Console.WriteLine("        АНАЛИЗ ТЕКСТА");
                Console.WriteLine("==================================================");

                AnalyzeNewText();
                RunMainMenu();
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

        private static void AnalyzeNewText()
        {
            string text = TextInput.ReadText();

            TextAnalyzer.AnalyzeWords(text, out int wordCount, out string longest, out string shortest);
            int sentenceCount = TextAnalyzer.CountSentences(text);
            TextAnalyzer.AnalyzeLetters(text, out int vowels, out int consonants, out List<LetterStat> frequency);

            TextStatistics statistics = new(History.Count + 1, text);
            statistics.SetWords(wordCount, longest, shortest);
            statistics.SetSentences(sentenceCount);
            statistics.SetLetters(vowels, consonants, frequency);

            History.Add(statistics);

            Console.WriteLine();
            Console.WriteLine($"--- РЕЗУЛЬТАТЫ АНАЛИЗА (текст №{statistics.Number}) ---");
            statistics.PrintResults();
            statistics.PrintLetterFrequency();
        }

        private static void RunMainMenu()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("--- МЕНЮ ---");
                Console.WriteLine("1. Ввести новый текст");
                Console.WriteLine("2. Показать статистику по всем текстам");
                Console.WriteLine("3. Показать статистику по выбранному тексту");
                Console.WriteLine("0. Выход");

                string choice = TextInput.ReadRequiredLine("Выберите команду: ");

                switch (choice)
                {
                    case "1":
                        AnalyzeNewText();
                        break;

                    case "2":
                        ShowAllStatistics();
                        break;

                    case "3":
                        ShowSelectedStatistics();
                        break;

                    case "0":
                        return;

                    default:
                        TextInput.ShowError("Такой команды нет. Введите номер из меню.");
                        break;
                }
            }
        }

        private static void ShowAllStatistics()
        {
            Console.WriteLine();
            Console.WriteLine("--- СТАТИСТИКА ПО ВСЕМ ТЕКСТАМ ---");

            if (History.Count == 0)
            {
                Console.WriteLine("  Анализ ещё не выполнялся.");
                return;
            }

            Console.WriteLine($"  {"№",-5}{"Симв.",-9}{"Слов",-7}{"Предл.",-9}{"Глас.",-8}{"Согл.",-8}{"Самое длинное слово",-25}Самое короткое слово");
            Console.WriteLine($"  {"",-5}{"",-9}{"",-7}{"",-9}{"",-8}{"",-8}{"",-25}");

            int totalSymbols = 0;
            int totalWords = 0;
            int totalSentences = 0;
            int totalVowels = 0;
            int totalConsonants = 0;

            for (int i = 0; i < History.Count; i++)
            {
                TextStatistics item = History[i];

                totalSymbols += item.Symbols;
                totalWords += item.WordCount;
                totalSentences += item.SentenceCount;
                totalVowels += item.VowelCount;
                totalConsonants += item.ConsonantCount;

                Console.WriteLine($"  {item.Number,-5}{item.Symbols,-9}{item.WordCount,-7}{item.SentenceCount,-9}" +
                                  $"{item.VowelCount,-8}{item.ConsonantCount,-8}{Cut(item.LongestWord, 24),-25}{item.ShortestWord}");
            }

            Console.WriteLine($"  {"",-5}{"",-9}{"",-7}{"",-9}{"",-8}{"",-8}{"",-25}");
            Console.WriteLine($"  {"ИТОГО",-6}{totalSymbols,-8}{totalWords,-7}{totalSentences,-9}{totalVowels,-8}{totalConsonants,-8}" +
                              $"Текстов обработано: {History.Count}");
        }

        private static void ShowSelectedStatistics()
        {
            if (History.Count == 0)
            {
                TextInput.ShowError("Список текстов пуст. Сначала введите текст.");
                return;
            }

            int number = TextInput.ReadInt("Номер текста: ", 1, History.Count);
            TextStatistics item = History[number - 1];

            Console.WriteLine();
            item.PrintResults();
            item.PrintLetterFrequency();
        }

        private static string Cut(string text, int maxLength)
        {
            if (text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength - 1) + "…";
        }
    }
}
