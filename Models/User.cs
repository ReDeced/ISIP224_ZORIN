namespace ISIP224_ZORIN.Models;

// Таблица User: зарегистрированный пользователь маркетплейса.
public class User
{
    public User(int id, string nickname, string passwordHash, DateTime createdAt)
    {
        Id = id;
        Nickname = nickname;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    public int Id { get; }

    public string Nickname { get; }

    public string PasswordHash { get; }

    public DateTime CreatedAt { get; }
}
