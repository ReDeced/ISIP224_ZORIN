using ISIP224_ZORIN.Enums;
using ISIP224_ZORIN.Models;
using ISIP224_ZORIN.Utils;

namespace ISIP224_ZORIN.Services;

public class BatchImportResult
{
    public int Added { get; }

    public List<string> Errors { get; }

    public BatchImportResult(int added, List<string> errors)
    {
        Added = added;
        Errors = errors;
    }
}

public static class BatchImporter
{
    public static BatchImportResult Import(LibraryRepository repository, List<string> lines)
    {
        int added = 0;
        List<string> errors = new List<string>();

        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i];
            string[] parts = line.Split(';');

            if (parts.Length != 5)
            {
                errors.Add($"строка {i + 1}: ожидалось 5 полей через «;», получено {parts.Length}");
                continue;
            }

            string title = parts[0].Trim();
            string author = parts[1].Trim();
            string genreText = parts[2].Trim();
            string yearText = parts[3].Trim();
            string priceText = parts[4].Trim();

            if (title.Length == 0 || author.Length == 0)
            {
                errors.Add($"строка {i + 1}: название и автор не могут быть пустыми");
                continue;
            }

            if (!Input.TryParseInt(yearText, out int year))
            {
                errors.Add($"строка {i + 1}: год «{yearText}» не является числом");
                continue;
            }

            if (!Book.IsValidYear(year))
            {
                errors.Add($"строка {i + 1}: год «{year}» должен быть от {Book.MinYear} до {Book.MaxYear}");
                continue;
            }

            if (!Input.TryParsePrice(priceText, out decimal price))
            {
                errors.Add($"строка {i + 1}: цена «{priceText}» должна быть числом больше нуля");
                continue;
            }

            Genre genre;
            try
            {
                genre = GenreExtensions.FromText(genreText);
            }
            catch (FormatException)
            {
                errors.Add($"строка {i + 1}: жанр «{genreText}» не найден");
                continue;
            }

            try
            {
                repository.Add(title, author, genre, year, price);
                added++;
            }
            catch (ArgumentException exception)
            {
                errors.Add($"строка {i + 1}: {exception.Message}");
            }
        }

        return new BatchImportResult(added, errors);
    }
}