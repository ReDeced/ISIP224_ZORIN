using System;
using System.Collections.Generic;
using System.Globalization;
using ISIP224_ZORIN.Enums;

namespace ISIP224_ZORIN.Utils;

public sealed class InputClosedException : Exception
{
    public InputClosedException() : base("Поток ввода закрыт.")
    {
    }
}

public static class Input
{
    public const decimal MaxPrice = 1_000_000m;

    public static string ReadLine(string prompt)
    {
        Console.Write(prompt);
        string? line = Console.ReadLine();

        if (line is null)
            throw new InputClosedException();

        return line;
    }

    public static string ReadText(string prompt)
    {
        while (true)
        {
            string text = ReadLine(prompt).Trim();
            if (text.Length > 0)
                return text;

            ShowError("Значение не может быть пустым. Попробуйте ещё раз.");
        }
    }

    public static int ReadId(string prompt = "Идентификатор книги: ")
        => ReadInt(prompt, 1, 1_000_000);

    public static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            string text = ReadLine(prompt).Trim();

            if (int.TryParse(text, out int value) && value >= min && value <= max)
                return value;

            ShowError($"Введите целое число от {min} до {max}.");
        }
    }

    public static int ReadYear(string prompt)
        => ReadInt(prompt, 1450, DateTime.Now.Year + 1);

    public static decimal ReadPrice(string prompt)
    {
        while (true)
        {
            string text = ReadLine(prompt).Trim();

            if (TryParsePrice(text, out decimal value))
                return value;

            ShowError($"Введите цену числом больше нуля (от 0,01 до {MaxPrice:0.##}).");
        }
    }

    public static int ReadPositiveInt(string prompt)
        => ReadInt(prompt, 1, 1_000_000);

    public static bool TryParseDecimal(string text, out decimal value)
    {
        string normalized = text.Replace(',', '.').Replace(" ", string.Empty);

        return decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
    }

    public static bool TryParseInt(string text, out int value)
        => int.TryParse(text, out value);

    public static bool TryParseYear(string text, out int year)
        => int.TryParse(text, out year) && year >= 1450 && year <= DateTime.Now.Year + 1;

    public static bool TryParsePrice(string text, out decimal price)
        => TryParseDecimal(text, out price) && price > 0m && price <= MaxPrice;

    public static bool ReadYesNo(string prompt)
    {
        while (true)
        {
            string answer = ReadLine(prompt).Trim();

            if (answer.StartsWith("д", StringComparison.OrdinalIgnoreCase)
                || answer.StartsWith("y", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (answer.StartsWith("н", StringComparison.OrdinalIgnoreCase)
                || answer.StartsWith("n", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            ShowError("Введите «д» или «н».");
        }
    }

    public static void ShowError(string message)
        => Console.WriteLine($"  [Ошибка] {message}");

    public static string FormatMoney(decimal value)
        => value.ToString("0.00", CultureInfo.InvariantCulture);

    public static List<string> ReadLines(string prompt, string terminator)
    {
        Console.WriteLine(prompt);
        Console.WriteLine($"Введите строки по одной. Для завершения ввода наберите с новой строки: {terminator}");

        List<string> lines = new List<string>();

        while (true)
        {
            string line = ReadLine("  ").TrimEnd();

            if (line.Trim() == terminator)
                return lines;

            if (line.Trim().Length > 0)
                lines.Add(line);
        }
    }
}