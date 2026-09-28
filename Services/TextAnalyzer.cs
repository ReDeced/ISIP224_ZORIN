using System;
using System.Collections.Generic;
using System.Text;
using ISIP224_ZORIN.Models;

namespace ISIP224_ZORIN.Services
{
    public static class TextAnalyzer
    {
        private static readonly char[] Vowels = { 'а', 'е', 'ё', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я',
                                                'a', 'e', 'i', 'o', 'u', 'y' };

        public static bool IsVowel(char letter)
        {
            char lower = char.ToLower(letter);

            for (int i = 0; i < Vowels.Length; i++)
            {
                if (Vowels[i] == lower)
                    return true;
            }

            return false;
        }

        public static bool IsLetter(char symbol)
        {
            return char.IsLetter(symbol);
        }

        public static List<string> SplitIntoWords(string text)
        {
            List<string> words = new List<string>();
            StringBuilder current = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                char symbol = text[i];

                if (IsLetter(symbol))
                {
                    current.Append(symbol);
                }
                else if (current.Length > 0)
                {
                    words.Add(current.ToString());
                    current.Length = 0;
                }
            }

            if (current.Length > 0)
                words.Add(current.ToString());

            return words;
        }

        public static void AnalyzeWords(string text, out int wordCount, out string longest, out string shortest)
        {
            List<string> words = SplitIntoWords(text);

            wordCount = words.Count;
            longest = string.Empty;
            shortest = string.Empty;

            if (wordCount == 0)
                return;

            longest = words[0];
            shortest = words[0];

            for (int i = 1; i < words.Count; i++)
            {
                if (words[i].Length > longest.Length)
                    longest = words[i];

                if (words[i].Length < shortest.Length)
                    shortest = words[i];
            }
        }

        public static int CountSentences(string text)
        {
            int count = 0;
            bool ended = false;

            for (int i = 0; i < text.Length; i++)
            {
                char symbol = text[i];

                if (symbol == '.' || symbol == '!' || symbol == '?')
                {
                    if (ended)
                        continue;

                    count++;
                    ended = true;
                }
                else if (IsLetter(symbol))
                {
                    ended = false;
                }
            }

            return count;
        }

        public static void AnalyzeLetters(string text, out int vowels, out int consonants, out List<LetterStat> frequency)
        {
            vowels = 0;
            consonants = 0;
            frequency = new List<LetterStat>();
            Dictionary<char, int> counts = new Dictionary<char, int>();

            for (int i = 0; i < text.Length; i++)
            {
                char symbol = text[i];

                if (!IsLetter(symbol))
                    continue;

                if (IsVowel(symbol))
                    vowels++;
                else
                    consonants++;

                char lower = char.ToLower(symbol);

                if (counts.ContainsKey(lower))
                    counts[lower] = counts[lower] + 1;
                else
                    counts[lower] = 1;
            }

            foreach (KeyValuePair<char, int> pair in counts)
                frequency.Add(new LetterStat(pair.Key, pair.Value));

            frequency.Sort(CompareByCount);
        }

        private static int CompareByCount(LetterStat first, LetterStat second)
        {
            if (first.Count != second.Count)
                return second.Count - first.Count;

            return first.Letter.CompareTo(second.Letter);
        }
    }
}
