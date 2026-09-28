using System;
using System.Collections.Generic;
using System.Text;

namespace ISIP224_ZORIN.Models
{
    public class LetterStat
    {
        public char Letter { get; }

        public int Count { get; }

        public LetterStat(char letter, int count)
        {
            Letter = letter;
            Count = count;
        }
    }

    public class TextStatistics
    {
        public int Number { get; }

        public string Text { get; }

        public int Symbols { get; }

        public int WordCount { get; private set; }

        public string LongestWord { get; private set; }

        public string ShortestWord { get; private set; }

        public int SentenceCount { get; private set; }

        public int VowelCount { get; private set; }

        public int ConsonantCount { get; private set; }

        public List<LetterStat> LetterFrequency { get; private set; }

        public TextStatistics(int number, string text)
        {
            Number = number;
            Text = text;
            Symbols = text.Length;
            WordCount = 0;
            LongestWord = string.Empty;
            ShortestWord = string.Empty;
            SentenceCount = 0;
            VowelCount = 0;
            ConsonantCount = 0;
            LetterFrequency = new List<LetterStat>();
        }

        public void SetWords(int wordCount, string longestWord, string shortestWord)
        {
            WordCount = wordCount;
            LongestWord = longestWord;
            ShortestWord = shortestWord;
        }

        public void SetSentences(int sentenceCount)
        {
            SentenceCount = sentenceCount;
        }

        public void SetLetters(int vowelCount, int consonantCount, List<LetterStat> frequency)
        {
            VowelCount = vowelCount;
            ConsonantCount = consonantCount;
            LetterFrequency = frequency;
        }

        public void PrintResults()
        {
            Console.WriteLine($"Текст №{Number}: {GetPreview(60)}");
            Console.WriteLine($"  Количество символов: {Symbols}");
            Console.WriteLine($"  Количество слов: {WordCount}");
            Console.WriteLine($"  Самое длинное слово: {LongestWord}");
            Console.WriteLine($"  Самое короткое слово: {ShortestWord}");
            Console.WriteLine($"  Количество предложений: {SentenceCount}");
            Console.WriteLine($"  Гласных букв: {VowelCount}");
            Console.WriteLine($"  Согласных букв: {ConsonantCount}");
        }

        public void PrintLetterFrequency()
        {
            if (LetterFrequency.Count == 0)
            {
                Console.WriteLine("  Букв в тексте не найдено.");
                return;
            }

            int total = 0;

            for (int i = 0; i < LetterFrequency.Count; i++)
                total += LetterFrequency[i].Count;

            Console.WriteLine($"  Частота встречаемости букв (всего букв: {total}):");
            Console.WriteLine($"  {"Буква",-8}{"Количество",-14}{"Доля, %",-10}");

            for (int i = 0; i < LetterFrequency.Count; i++)
            {
                LetterStat stat = LetterFrequency[i];
                double percent = (double)stat.Count / total * 100d;

                Console.WriteLine($"  {stat.Letter,-8}{stat.Count,-14}{percent.ToString("0.00"),-10}");
            }
        }

        public string GetPreview(int maxSymbols)
        {
            string single = Text.Replace('\n', ' ');

            if (single.Length <= maxSymbols)
                return single;

            return single.Substring(0, maxSymbols) + "...";
        }
    }
}
