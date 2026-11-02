using ISIP224_ZORIN.Models;

namespace ISIP224_ZORIN.Services;

// Хранит всех участников учебного процесса и курсы, выдаёт уникальные
// идентификаторы и выполняет поиск и выборки через LINQ.
public class UniversityRepository
{
    private readonly List<Student> students = [];
    private readonly List<Teacher> teachers = [];
    private readonly List<Course> courses = [];

    private int nextId = 1;

    public IReadOnlyList<Student> Students => students;

    public IReadOnlyList<Teacher> Teachers => teachers;

    public IReadOnlyList<Course> Courses => courses;

    public Student AddStudent(string fullName, int age, string email, int studentNumber, int yearOfStudy)
    {
        if (students.Any(student => student.StudentNumber == studentNumber))
            throw new InvalidOperationException($"Студенческий билет №{studentNumber} уже зарегистрирован.");

        Student student = new(nextId++, fullName, age, email, studentNumber, yearOfStudy);
        students.Add(student);
        return student;
    }

    public Teacher AddTeacher(string fullName, int age, string email,
        Enums.AcademicDegree degree, string department, int experienceYears)
    {
        Teacher teacher = new(nextId++, fullName, age, email, degree, department, experienceYears);
        teachers.Add(teacher);
        return teacher;
    }

    public Course AddCourse(string title, string description, int creditHours)
    {
        Course course = new(nextId++, title, description, creditHours);
        courses.Add(course);
        return course;
    }

    // Закрепить преподавателя за курсом.
    public void AssignTeacher(int courseId, int teacherId)
    {
        Course course = FindCourse(courseId)
            ?? throw new InvalidOperationException($"Курс с идентификатором {courseId} не найден.");

        Teacher teacher = FindTeacher(teacherId)
            ?? throw new InvalidOperationException($"Преподаватель с идентификатором {teacherId} не найден.");

        course.AssignTeacher(teacher);
    }

    // Записать студента на курс.
    public void EnrollStudent(int courseId, int studentId)
    {
        Course course = FindCourse(courseId)
            ?? throw new InvalidOperationException($"Курс с идентификатором {courseId} не найден.");

        Student student = FindStudent(studentId)
            ?? throw new InvalidOperationException($"Студент с идентификатором {studentId} не найден.");

        course.AddStudent(student);
    }

    public Student? FindStudent(int studentId)
        => students.FirstOrDefault(student => student.Id == studentId);

    public Teacher? FindTeacher(int teacherId)
        => teachers.FirstOrDefault(teacher => teacher.Id == teacherId);

    public Course? FindCourse(int courseId)
        => courses.FirstOrDefault(course => course.Id == courseId);

    // Все курсы, на которые записан студент.
    public List<Course> GetStudentCourses(int studentId)
        => courses.Where(course => course.HasStudent(studentId)).ToList();

    // Все курсы, которые ведёт преподаватель.
    public List<Course> GetTeacherCourses(int teacherId)
        => courses.Where(course => course.Teacher is not null && course.Teacher.Id == teacherId).ToList();

    // Полиморфизм: и студенты, и преподаватели возвращаются как базовый тип Person.
    public List<Person> GetAllPeople()
        => students.Cast<Person>().Concat(teachers).OrderBy(person => person.Id).ToList();

    // Наполняет университет демонстрационными данными при запуске.
    public void FillWithTestData()
    {
        Student ivanov = AddStudent("Иванов Иван", 18, "ivanov@example.com", 224001, 1);
        Student petrova = AddStudent("Петрова Анна", 19, "petrova@example.com", 224002, 1);
        AddStudent("Сидоров Сергей", 20, "sidorov@example.com", 224003, 2);

        Teacher kuznetsov = AddTeacher("Кузнецов Пётр", 42, "kuznetsov@example.com",
            Enums.AcademicDegree.Доцент, "Кафедра информатики", 12);
        Teacher smirnova = AddTeacher("Смирнова Ольга", 55, "smirnova@example.com",
            Enums.AcademicDegree.Профессор, "Кафедра математики", 25);

        Course programming = AddCourse("Программирование на C#", "Классы, коллекции и LINQ", 72);
        Course algebra = AddCourse("Линейная алгебра", "Матрицы и системы уравнений", 108);
        AddCourse("Базы данных", "Проектирование и SQL", 72);

        AssignTeacher(programming.Id, kuznetsov.Id);
        AssignTeacher(algebra.Id, smirnova.Id);

        EnrollStudent(programming.Id, ivanov.Id);
        EnrollStudent(programming.Id, petrova.Id);
        EnrollStudent(algebra.Id, ivanov.Id);
    }
}
