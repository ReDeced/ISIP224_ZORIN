using ISIP224_ZORIN.Enums;

namespace ISIP224_ZORIN.Models;

public class Course
{
    public const int MinCreditHours = 1;
    public const int MaxCreditHours = 300;

    private readonly string title;
    private readonly string description;
    private readonly int creditHours;
    private readonly List<Student> students = [];

    // Преподаватель может быть ещё не назначен — курс сначала создают, потом закрепляют за ним ведущего.
    private Teacher? teacher;

    public Course(int id, string title, string description, int creditHours)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Название курса не может быть пустым.", nameof(title));

        if (creditHours < MinCreditHours || creditHours > MaxCreditHours)
            throw new ArgumentOutOfRangeException(nameof(creditHours), creditHours,
                $"Объём курса должен быть от {MinCreditHours} до {MaxCreditHours} часов.");

        Id = id;
        this.title = title.Trim();
        this.description = description.Trim();
        this.creditHours = creditHours;
    }

    public int Id { get; }

    public string Title => title;

    public string Description => description;

    public int CreditHours => creditHours;

    public Teacher? Teacher => teacher;

    public IReadOnlyList<Student> Students => students;

    // Назначить ведущего (переназначение тоже разрешено).
    public void AssignTeacher(Teacher teacher)
    {
        this.teacher = teacher ?? throw new ArgumentNullException(nameof(teacher), "Преподаватель не найден.");
    }

    public void AddStudent(Student student)
    {
        if (student is null)
            throw new ArgumentNullException(nameof(student), "Студент не найден.");

        if (HasStudent(student.Id))
            throw new InvalidOperationException(
                $"Студент {student.FullName} уже записан на курс «{title}».");

        students.Add(student);
    }

    public bool HasStudent(int studentId)
        => students.Any(student => student.Id == studentId);

    public void PrintInfo()
    {
        Console.WriteLine("  +------------------------------------------------------+");
        WriteRow("Идентификатор", Id.ToString());
        WriteRow("Название", title);
        WriteRow("Описание", description.Length > 0 ? description : "—");
        WriteRow("Объём", $"{creditHours} ч.");
        WriteRow("Преподаватель", teacher is null ? "не назначен" : $"{teacher.FullName} ({teacher.Degree.GetTitle()})");
        WriteRow("Студентов", students.Count.ToString());
        Console.WriteLine("  +------------------------------------------------------+");
    }

    private void WriteRow(string label, string value)
        => Console.WriteLine($"  | {label,-18}| {value,-52}|");
}
