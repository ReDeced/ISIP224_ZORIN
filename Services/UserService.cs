using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using ISIP224_ZORIN.Models;

namespace ISIP224_ZORIN.Services;

// Регистрация и вход пользователей. Пароль в базу не пишется —
// хранится только его SHA256-хеш.
public class UserService
{
    public const int MinPasswordLength = 4;

    private readonly MarketplaceDatabase database;

    public UserService(MarketplaceDatabase database)
    {
        this.database = database;
    }

    // Создаёт аккаунт с подтверждённым паролем и уникальным ником.
    public User Register(string nickname, string password, string passwordConfirmation)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            throw new ArgumentException("Ник не может быть пустым.", nameof(nickname));

        if (nickname.Contains(' '))
            throw new ArgumentException("Ник не должен содержать пробелов.", nameof(nickname));

        ValidatePassword(password);

        if (password != passwordConfirmation)
            throw new ArgumentException("Пароль и подтверждение не совпадают.");

        if (IsNicknameTaken(nickname))
            throw new InvalidOperationException($"Ник «{nickname}» уже занят.");

        DateTime createdAt = DateTime.Now;

        long id = database.Insert(
            "INSERT INTO User (Nickname, PasswordHash, CreatedAt) VALUES ($nickname, $passwordHash, $createdAt);",
            MarketplaceDatabase.P("$nickname", nickname),
            MarketplaceDatabase.P("$passwordHash", HashPassword(password)),
            MarketplaceDatabase.P("$createdAt", FormatDate(createdAt)));

        return new User((int)id, nickname, HashPassword(password), createdAt);
    }

    // Вход зарегистрированного пользователя, null — если данные неверны.
    public User? Login(string nickname, string password)
    {
        if (string.IsNullOrWhiteSpace(nickname) || string.IsNullOrWhiteSpace(password))
            return null;

        string passwordHash = HashPassword(password);

        return database
            .Query(
                "SELECT Id, Nickname, PasswordHash, CreatedAt FROM User WHERE Nickname = $nickname AND PasswordHash = $passwordHash;",
                MapUser,
                MarketplaceDatabase.P("$nickname", nickname),
                MarketplaceDatabase.P("$passwordHash", passwordHash))
            .FirstOrDefault();
    }

    public bool IsNicknameTaken(string nickname)
        => (long)database.Scalar(
            "SELECT COUNT(*) FROM User WHERE Nickname = $nickname;",
            MarketplaceDatabase.P("$nickname", nickname)) > 0;

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < MinPasswordLength)
            throw new ArgumentException($"Пароль должен содержать не менее {MinPasswordLength} символов.");

        if (password.Contains(' '))
            throw new ArgumentException("Пароль не должен содержать пробелов.");
    }

    public static string HashPassword(string password)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));

    private static User MapUser(SqliteDataReader reader)
        => new(
            reader.GetInt32(reader.GetOrdinal("Id")),
            reader.GetString(reader.GetOrdinal("Nickname")),
            reader.GetString(reader.GetOrdinal("PasswordHash")),
            ParseDate(reader.GetString(reader.GetOrdinal("CreatedAt"))));

    // Дата хранится в базе строкой ISO-формата: она сортируется лексикографически.
    public static string FormatDate(DateTime value)
        => value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

    public static DateTime ParseDate(string value)
        => DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}
