using ISIP224_ZORIN.Enums;

namespace ISIP224_ZORIN.Models;

public class Teacher : Person
{
    public const int MaxExperienceYears = 70;

    private readonly AcademicDegree degree;
    private readonly string department;
    private readonly int experienceYears;

    public Teacher(int id, string fullName, int age, string email,
        AcademicDegree degree, string department, int experienceYears)
        : base(id, fullName, age, email)
    {
        if (string.IsNullOrWhiteSpace(department))
            throw new ArgumentException("Кафедра не может быть пустой.", nameof(department));

        if (experienceYears < 0 || experienceYears > MaxExperienceYears)
            throw new ArgumentOutOfRangeException(nameof(experienceYears), experienceYears,
                $"Стаж должен быть от 0 до {MaxExperienceYears} лет.");

        this.degree = degree;
        this.department = department.Trim();
        this.experienceYears = experienceYears;
    }

    public AcademicDegree Degree => degree;

    public string Department => department;

    public int ExperienceYears => experienceYears;

    public override string Role => "Преподаватель";

    public override void PrintInfo()
    {
        base.PrintInfo();
        WriteRow("Степень", degree.GetTitle());
        WriteRow("Кафедра", department);
        WriteRow("Стаж", $"{experienceYears} л.");
        PrintCardFooter();
    }
}
