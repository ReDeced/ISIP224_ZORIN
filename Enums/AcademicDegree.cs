namespace ISIP224_ZORIN.Enums;

public enum AcademicDegree
{
    Ассистент = 1,
    Доцент = 2,
    Профессор = 3
}

public static class AcademicDegreeExtensions
{
    public static readonly AcademicDegree[] All = (AcademicDegree[])Enum.GetValues(typeof(AcademicDegree));

    public static string GetTitle(this AcademicDegree degree) => degree switch
    {
        AcademicDegree.Ассистент => "Ассистент",
        AcademicDegree.Доцент => "Доцент",
        AcademicDegree.Профессор => "Профессор",
        _ => "Степень не определена"
    };

    public static string GetNumberedTitle(this AcademicDegree degree)
        => $"{(int)degree}. {degree.GetTitle()}";
}
