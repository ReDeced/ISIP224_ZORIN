using System;

namespace ISIP224_ZORIN.Utils
{
    public sealed class InputClosedException : Exception
    {
        public InputClosedException() : base("Поток ввода закрыт.")
        {
        }
    }

    public static class TextInput
    {
        public const int MinLength = 100;

        public static string ReadLine(string prompt)
        {
            Console.Write(prompt);
            string? line = Console.ReadLine();

            if (line is null)
                throw new InputClosedException();

            return line;
        }

        public static string ReadRequiredLine(string prompt)
        {
            while (true)
            {
                string line = ReadLine(prompt).Trim();

                if (line.Length > 0)
                    return line;

                ShowError("Строка не может быть пустой. Введите текст.");
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

        public static string ReadText()
        {
            Console.WriteLine($"\nВведите текст (минимум {MinLength} символов).");
            Console.WriteLine("Введите текст и нажмите Enter. Для завершения ввода наберите с новой строки: !done");

            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            while (true)
            {
                string line = ReadLine("  ");

                if (line.Trim() == "!done")
                {
                    if (builder.Length < MinLength)
                    {
                        ShowError($"Текст слишком короткий: {builder.Length} символов из {MinLength}. Добавьте ещё текст.");
                        continue;
                    }

                    return builder.ToString();
                }

                if (builder.Length > 0)
                    builder.Append(' ');

                builder.Append(line);
            }
        }

        public static void ShowError(string message)
        {
            Console.WriteLine($"  [Ошибка] {message}");
        }
    }
}
