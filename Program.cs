using ISIP224_ZORIN.Enums;
using ISIP224_ZORIN.Models;
using ISIP224_ZORIN.Services;
using ISIP224_ZORIN.Utils;

namespace ISIP224_ZORIN;

internal class Program
{
    private static readonly UniversityRepository University = new();

    static void Main(string[] args)
    {
        try
        {
            University.FillWithTestData();

            PrintWelcome();
            RunMainMenu();
        }
        catch (InputClosedException)
        {
            Console.WriteLine("\nВвод закрыт, программа завершает работу.");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"\nНепредвиденная ошибка: {exception.Message}");
        }

        Console.WriteLine("\nДо свидания!");
    }

    private static void PrintWelcome()
    {
        Console.WriteLine("==========================================================");
        Console.WriteLine("              СИСТЕМА УПРАВЛЕНИЯ УНИВЕРСИТЕТОМ");
        Console.WriteLine("==========================================================");
        Console.WriteLine($"Студентов: {University.Students.Count}, преподавателей: {University.Teachers.Count}, курсов: {University.Courses.Count}");
    }

    private static void RunMainMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- ГЛАВНОЕ МЕНЮ ---");
            Console.WriteLine("1. Добавить студента");
            Console.WriteLine("2. Информация о студенте");
            Console.WriteLine("3. Добавить преподавателя");
            Console.WriteLine("4. Информация о преподавателе");
            Console.WriteLine("5. Создать курс");
            Console.WriteLine("6. Информация о курсе");
            Console.WriteLine("7. Записать студента на курс");
            Console.WriteLine("8. Назначить преподавателя на курс");
            Console.WriteLine("9. Список всех студентов");
            Console.WriteLine("10. Список всех преподавателей");
            Console.WriteLine("11. Список всех курсов");
            Console.WriteLine("12. Все участники учебного процесса");
            Console.WriteLine("0. Выход");

            string choice = Input.ReadText("Выберите команду: ");

            switch (choice)
            {
                case "1":
                    AddStudent();
                    break;

                case "2":
                    ShowStudent();
                    break;

                case "3":
                    AddTeacher();
                    break;

                case "4":
                    ShowTeacher();
                    break;

                case "5":
                    CreateCourse();
                    break;

                case "6":
                    ShowCourse();
                    break;

                case "7":
                    EnrollStudent();
                    break;

                case "8":
                    AssignTeacher();
                    break;

                case "9":
                    PrintStudentTable(University.Students);
                    break;

                case "10":
                    PrintTeacherTable(University.Teachers);
                    break;

                case "11":
                    PrintCourseTable(University.Courses);
                    break;

                case "12":
                    ShowAllPeople();
                    break;

                case "0":
                    return;

                default:
                    Input.ShowError("Такой команды нет. Введите номер из меню.");
                    break;
            }
        }
    }

    private static void AddStudent()
    {
        Console.WriteLine("\n--- ДОБАВЛЕНИЕ СТУДЕНТА ---");

        string fullName = Input.ReadText("  Имя: ");
        int age = Input.ReadAge("  Возраст: ");
        string email = Input.ReadEmail("  Почта: ");
        int studentNumber = Input.ReadInt("  Номер студенческого билета (6 цифр): ",
            Student.MinStudentNumber, Student.MaxStudentNumber);
        int yearOfStudy = Input.ReadInt("  Год обучения (1–6): ",
            Student.MinYearOfStudy, Student.MaxYearOfStudy);

        try
        {
            Student student = University.AddStudent(fullName, age, email, studentNumber, yearOfStudy);
            Console.WriteLine("\n  Студент добавлен:");
            student.PrintInfo();
        }
        catch (ArgumentException exception)
        {
            Input.ShowError($"Студент не добавлен: {exception.Message}");
        }
        catch (InvalidOperationException exception)
        {
            Input.ShowError(exception.Message);
        }
    }

    private static void AddTeacher()
    {
        Console.WriteLine("\n--- ДОБАВЛЕНИЕ ПРЕПОДАВАТЕЛЯ ---");

        string fullName = Input.ReadText("  Имя: ");
        int age = Input.ReadAge("  Возраст: ");
        string email = Input.ReadEmail("  Почта: ");
        AcademicDegree degree = ReadDegree();
        string department = Input.ReadText("  Кафедра: ");
        int experienceYears = Input.ReadInt($"  Стаж (0–{Teacher.MaxExperienceYears}): ", 0, Teacher.MaxExperienceYears);

        try
        {
            Teacher teacher = University.AddTeacher(fullName, age, email, degree, department, experienceYears);
            Console.WriteLine("\n  Преподаватель добавлен:");
            teacher.PrintInfo();
        }
        catch (ArgumentException exception)
        {
            Input.ShowError($"Преподаватель не добавлен: {exception.Message}");
        }
    }

    private static void CreateCourse()
    {
        Console.WriteLine("\n--- СОЗДАНИЕ КУРСА ---");

        string title = Input.ReadText("  Название курса: ");
        string description = Input.ReadText("  Описание: ");
        int creditHours = Input.ReadInt($"  Объём ({Course.MinCreditHours}–{Course.MaxCreditHours} ч.): ",
            Course.MinCreditHours, Course.MaxCreditHours);

        try
        {
            Course course = University.AddCourse(title, description, creditHours);
            Console.WriteLine("\n  Курс создан:");
            course.PrintInfo();
        }
        catch (ArgumentException exception)
        {
            Input.ShowError($"Курс не создан: {exception.Message}");
        }
    }

    private static void ShowStudent()
    {
        Console.WriteLine("\n--- ИНФОРМАЦИЯ О СТУДЕНТЕ ---");

        Student? student = University.FindStudent(Input.ReadId("  Идентификатор студента: "));

        if (student is null)
        {
            Input.ShowError("Студент с таким идентификатором не найден.");
            return;
        }

        student.PrintInfo();

        List<Course> courses = University.GetStudentCourses(student.Id);

        Console.WriteLine(courses.Count > 0
            ? $"\n  Студент записан на курсы ({courses.Count}):"
            : "\n  Студент не записан ни на один курс.");

        foreach (Course course in courses)
            Console.WriteLine($"    №{course.Id} — {course.Title} ({course.CreditHours} ч., {(course.Teacher?.FullName ?? "преподаватель не назначен")})");
    }

    private static void ShowTeacher()
    {
        Console.WriteLine("\n--- ИНФОРМАЦИЯ О ПРЕПОДАВАТЕЛЕ ---");

        Teacher? teacher = University.FindTeacher(Input.ReadId("  Идентификатор преподавателя: "));

        if (teacher is null)
        {
            Input.ShowError("Преподаватель с таким идентификатором не найден.");
            return;
        }

        teacher.PrintInfo();

        List<Course> courses = University.GetTeacherCourses(teacher.Id);

        Console.WriteLine(courses.Count > 0
            ? $"\n  Преподаватель ведёт курсы ({courses.Count}):"
            : "\n  Преподаватель не ведёт ни одного курса.");

        foreach (Course course in courses)
            Console.WriteLine($"    №{course.Id} — {course.Title} ({course.CreditHours} ч., студентов: {course.Students.Count})");
    }

    private static void ShowCourse()
    {
        Console.WriteLine("\n--- ИНФОРМАЦИЯ О КУРСЕ ---");

        Course? course = University.FindCourse(Input.ReadId("  Идентификатор курса: "));

        if (course is null)
        {
            Input.ShowError("Курс с таким идентификатором не найден.");
            return;
        }

        course.PrintInfo();

        if (course.Students.Count == 0)
        {
            Console.WriteLine("\n  На курс пока никто не записан.");
            return;
        }

        Console.WriteLine($"\n  Студенты курса ({course.Students.Count}):");
        PrintStudentTable(course.Students, withHeader: false);
    }

    private static void EnrollStudent()
    {
        Console.WriteLine("\n--- ЗАПИСЬ СТУДЕНТА НА КУРС ---");

        if (University.Students.Count == 0 || University.Courses.Count == 0)
        {
            Input.ShowError("Нужен хотя бы один студент и один курс.");
            return;
        }

        PrintStudentTable(University.Students);
        Console.WriteLine();
        PrintCourseTable(University.Courses);

        int studentId = Input.ReadId("  Идентификатор студента: ");
        int courseId = Input.ReadId("  Идентификатор курса: ");

        try
        {
            University.EnrollStudent(courseId, studentId);
            Student student = University.FindStudent(studentId)!;
            Course course = University.FindCourse(courseId)!;
            Console.WriteLine($"\n  {student.FullName} записан(а) на курс «{course.Title}».");
            Console.WriteLine($"  Всего на курсе студентов: {course.Students.Count}.");
        }
        catch (InvalidOperationException exception)
        {
            Input.ShowError(exception.Message);
        }
    }

    private static void AssignTeacher()
    {
        Console.WriteLine("\n--- НАЗНАЧЕНИЕ ПРЕПОДАВАТЕЛЯ НА КУРС ---");

        if (University.Teachers.Count == 0 || University.Courses.Count == 0)
        {
            Input.ShowError("Нужен хотя бы один преподаватель и один курс.");
            return;
        }

        PrintTeacherTable(University.Teachers);
        Console.WriteLine();
        PrintCourseTable(University.Courses);

        int teacherId = Input.ReadId("  Идентификатор преподавателя: ");
        int courseId = Input.ReadId("  Идентификатор курса: ");

        try
        {
            University.AssignTeacher(courseId, teacherId);
            Teacher teacher = University.FindTeacher(teacherId)!;
            Course course = University.FindCourse(courseId)!;
            Console.WriteLine($"\n  {teacher.FullName} теперь ведёт курс «{course.Title}».");
        }
        catch (InvalidOperationException exception)
        {
            Input.ShowError(exception.Message);
        }
    }

    // Полиморфизм: список выводится через базовый класс Person,
    // виртуальное свойство Role возвращает роль конкретного наследника.
    private static void ShowAllPeople()
    {
        Console.WriteLine("\n--- ВСЕ УЧАСТНИКИ УЧЕБНОГО ПРОЦЕССА ---");

        List<Person> people = University.GetAllPeople();

        if (people.Count == 0)
        {
            Console.WriteLine("  В университете пока никого нет.");
            return;
        }

        Console.WriteLine($"  {"ID",-5}{"Роль",-16}{"Имя",-30}{"Возраст",-9}Почта");

        foreach (Person person in people)
            Console.WriteLine($"  {person.Id,-5}{person.Role,-16}{Cut(person.FullName, 29),-30}{person.Age,-9}{person.Email}");

        int studentsCount = people.Count(person => person is Student);
        int teachersCount = people.Count(person => person is Teacher);
        Console.WriteLine($"\n  Всего участников: {people.Count}, из них студентов: {studentsCount}, преподавателей: {teachersCount}.");
    }

    private static void PrintStudentTable(IReadOnlyList<Student> students, bool withHeader = true)
    {
        if (withHeader)
        {
            Console.WriteLine("\n--- СПИСОК СТУДЕНТОВ ---");

            if (students.Count == 0)
            {
                Console.WriteLine("  Студентов в университете пока нет.");
                return;
            }
        }
        else if (students.Count == 0)
        {
            Console.WriteLine("  Студентов нет.");
            return;
        }

        Console.WriteLine($"  {"ID",-5}{"Имя",-26}{"Возраст",-9}{"Биллет",-9}{"Курс",-7}Почта");

        foreach (Student student in students)
            Console.WriteLine($"  {student.Id,-5}{Cut(student.FullName, 25),-26}{student.Age,-9}{student.StudentNumber,-9}{student.YearOfStudy,-7}{student.Email}");

        Console.WriteLine($"  Всего студентов: {students.Count}.");
    }

    private static void PrintTeacherTable(IReadOnlyList<Teacher> teachers)
    {
        Console.WriteLine("\n--- СПИСОК ПРЕПОДАВАТЕЛЕЙ ---");

        if (teachers.Count == 0)
        {
            Console.WriteLine("  Преподавателей в университете пока нет.");
            return;
        }

        Console.WriteLine($"  {"ID",-5}{"Имя",-26}{"Возраст",-9}{"Степень",-13}{"Стаж",-7}Кафедра");

        foreach (Teacher teacher in teachers)
            Console.WriteLine($"  {teacher.Id,-5}{Cut(teacher.FullName, 25),-26}{teacher.Age,-9}{teacher.Degree.GetTitle(),-13}{teacher.ExperienceYears,-7}{teacher.Department}");

        Console.WriteLine($"  Всего преподавателей: {teachers.Count}.");
    }

    private static void PrintCourseTable(IReadOnlyList<Course> courses)
    {
        Console.WriteLine("\n--- СПИСОК КУРСОВ ---");

        if (courses.Count == 0)
        {
            Console.WriteLine("  Курсов в университете пока нет.");
            return;
        }

        Console.WriteLine($"  {"ID",-5}{"Название",-32}{"Часы",-7}{"Студентов",-11}Преподаватель");

        foreach (Course course in courses)
            Console.WriteLine($"  {course.Id,-5}{Cut(course.Title, 31),-32}{course.CreditHours,-7}{course.Students.Count,-11}{course.Teacher?.FullName ?? "не назначен"}");

        Console.WriteLine($"  Всего курсов: {courses.Count}.");
    }

    private static AcademicDegree ReadDegree()
    {
        while (true)
        {
            Console.WriteLine("  Учёные степени:");
            foreach (AcademicDegree degree in AcademicDegreeExtensions.All)
                Console.WriteLine($"    {degree.GetNumberedTitle()}");

            string text = Input.ReadLine("  Выберите степень (номер или название): ").Trim();

            foreach (AcademicDegree degree in AcademicDegreeExtensions.All)
            {
                if (string.Equals(degree.GetTitle(), text, StringComparison.OrdinalIgnoreCase))
                    return degree;
            }

            if (int.TryParse(text, out int number) && Enum.IsDefined(typeof(AcademicDegree), number))
                return (AcademicDegree)number;

            Input.ShowError("Такой степени нет. Выберите номер или название из списка.");
        }
    }

    private static string Cut(string text, int maxLength)
        => text.Length <= maxLength ? text : text[..(maxLength - 1)] + "…";
}
