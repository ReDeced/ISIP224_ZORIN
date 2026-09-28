using System.Globalization;
using MyConsoleApp.Enums;

namespace MyConsoleApp.Utils;

public sealed class InputClosedException : Exception
{
    public InputClosedException() : base("Поток ввода закрыт.")
    {
    }
}

public static class Input
{
    public const int MaxQuantity = 1_000_000;

    public const decimal MaxPrice = 1_000_000m;

    public static string? ReadLine(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    public static string ReadText(string prompt)
    {
        while (true)
        {
            string text = ReadLineOrExit(prompt).Trim();
            if (text.Length > 0)
                return text;

            ShowError("Значение не может быть пустым. Попробуйте ещё раз.");
        }
    }

    public static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            string text = ReadLineOrExit(prompt).Trim();
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                && value >= min && value <= max)
            {
                return value;
            }

            ShowError($"Введите целое число от {min} до {max}.");
        }
    }

    public static int ReadPositiveInt(string prompt)
    {
        while (true)
        {
            string text = ReadLineOrExit(prompt).Trim();
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                && value > 0 && value <= MaxQuantity)
            {
                return value;
            }

            ShowError($"Введите целое число от 1 до {MaxQuantity}.");
        }
    }

    public static int ReadNonNegativeInt(string prompt)
    {
        while (true)
        {
            string text = ReadLineOrExit(prompt).Trim();
            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                && value >= 0 && value <= MaxQuantity)
            {
                return value;
            }

            ShowError($"Введите целое число от 0 до {MaxQuantity}.");
        }
    }

    public static decimal ReadPositiveDecimal(string prompt)
    {
        while (true)
        {
            string text = ReadLineOrExit(prompt).Trim();
            if (TryParseDecimal(text, out decimal value) && value > 0m && value <= MaxPrice)
                return value;

            ShowError($"Введите цену числом больше нуля (от 0,01 до {FormatNumber(MaxPrice)}).");
        }
    }

    public static bool TryParseDecimal(string text, out decimal value)
    {
        string normalized = text.Replace(',', '.').Replace(" ", string.Empty);

        return decimal.TryParse(
            normalized,
            NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
            CultureInfo.InvariantCulture,
            out value);
    }

    public static int ReadCode(string prompt = "Код товара: ")
        => ReadPositiveInt(prompt);

    public static Category ReadCategory()
    {
        while (true)
        {
            Console.WriteLine("  Категории товаров:");
            foreach (Category category in CategoryExtensions.All)
                Console.WriteLine($"    {category.GetNumberedTitle()}");

            string text = ReadLineOrExit("  Выберите категорию (номер или название): ").Trim();

            foreach (Category category in CategoryExtensions.All)
            {
                if (string.Equals(category.GetTitle(), text, StringComparison.OrdinalIgnoreCase))
                    return category;
            }

            if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int number)
                && Enum.IsDefined(typeof(Category), number))
            {
                return (Category)number;
            }

            ShowError("Такой категории нет. Выберите номер или название из списка.");
        }
    }

    private static string ReadLineOrExit(string prompt)
        => ReadLine(prompt) ?? throw new InputClosedException();

    public static void ShowError(string message)
        => Console.WriteLine($"  [Ошибка] {message}");

    public static string FormatNumber(decimal value)
        => value.ToString("0.00", CultureInfo.InvariantCulture);
}
