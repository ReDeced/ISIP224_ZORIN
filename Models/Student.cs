namespace ISIP224_ZORIN.Models;

public class Student : Person
{
    public const int MinStudentNumber = 100000;
    public const int MaxStudentNumber = 999999;
    public const int MinYearOfStudy = 1;
    public const int MaxYearOfStudy = 6;

    private readonly int studentNumber;
    private readonly int yearOfStudy;

    public Student(int id, string fullName, int age, string email, int studentNumber, int yearOfStudy)
        : base(id, fullName, age, email)
    {
        if (studentNumber < MinStudentNumber || studentNumber > MaxStudentNumber)
            throw new ArgumentOutOfRangeException(nameof(studentNumber), studentNumber,
                $"Номер студенческого билета должен быть от {MinStudentNumber} до {MaxStudentNumber}.");

        if (yearOfStudy < MinYearOfStudy || yearOfStudy > MaxYearOfStudy)
            throw new ArgumentOutOfRangeException(nameof(yearOfStudy), yearOfStudy,
                $"Курс обучения должен быть от {MinYearOfStudy} до {MaxYearOfStudy}.");

        this.studentNumber = studentNumber;
        this.yearOfStudy = yearOfStudy;
    }

    public int StudentNumber => studentNumber;

    public int YearOfStudy => yearOfStudy;

    public override string Role => "Студент";

    public override void PrintInfo()
    {
        base.PrintInfo();
        WriteRow("Номер билета", studentNumber.ToString());
        WriteRow("Год обучения", $"{yearOfStudy} курс");
        PrintCardFooter();
    }
}
