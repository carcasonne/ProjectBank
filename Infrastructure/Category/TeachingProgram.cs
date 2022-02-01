namespace ProjectBank.Infrastructure;

public class TeachingProgram : CodedCategory
{
    [Required]
    public ICollection<Course> Courses {get; set;}
    public TeachingProgram(string Title, string Description, Faculty Faculty, string Code, ICollection<Course> Courses) 
    : base(Title, Description, Faculty, Code)
    {
        this.Courses = Courses;
    }

    public TeachingProgram() {}
}