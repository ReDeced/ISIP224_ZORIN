using ISIP224_ZORIN.Models;

namespace ISIP224_ZORIN.Utils;

public sealed class InputClosedException : Exception
{
    public InputClosedException() : base("Поток ввода закрыт.")
    {
    }
}

// Чтение данных с клавиатуры с проверкой каждого значения.
public static class Input
{
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
}
