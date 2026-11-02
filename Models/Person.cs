namespace ISIP224_ZORIN.Models;

// Абстракция: общие данные любого участника учебного процесса.
// Класс абстрактный — «просто человека» создать нельзя, только студента или преподавателя.
// Все поля закрыты от прямого доступа (инкапсуляция), чтение возможно через свойства.
public abstract class Person
{
    public const int MinAge = 16;
    public const int MaxAge = 100;

    private readonly string fullName;
    private readonly string email;

    protected Person(int id, string fullName, int age, string email)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Имя не может быть пустым.", nameof(fullName));

        if (age < MinAge || age > MaxAge)
            throw new ArgumentOutOfRangeException(nameof(age), age, $"Возраст должен быть от {MinAge} до {MaxAge} лет.");

        if (!IsValidEmail(email))
            throw new ArgumentException("Электронная почта указана неверно.", nameof(email));

        Id = id;
        this.fullName = fullName.Trim();
        Age = age;
        this.email = email.Trim().ToLowerInvariant();
    }

    public int Id { get; }

    public string FullName => fullName;

    public int Age { get; }

    public string Email => email;

    // Название роли: у студента и преподавателя разное (полиморфизм).
    public abstract string Role { get; }

    // Каждый наследник дополняет карточку своей информацией.
    public virtual void PrintInfo()
    {
        Console.WriteLine("  +------------------------------------------------------+");
        WriteRow("Роль", Role);
        WriteRow("Идентификатор", Id.ToString());
        WriteRow("Имя", FullName);
        WriteRow("Возраст", $"{Age} л.");
        WriteRow("Почта", Email);
    }

    protected void WriteRow(string label, string value)
        => Console.WriteLine($"  | {label,-18}| {value,-52}|");

    protected void PrintCardFooter()
        => Console.WriteLine("  +------------------------------------------------------+");

    public static bool IsValidEmail(string email)
        => !string.IsNullOrWhiteSpace(email)
           && email.Contains('@')
           && email.IndexOf('@') < email.LastIndexOf('.')
           && !email.Contains(' ');
}
