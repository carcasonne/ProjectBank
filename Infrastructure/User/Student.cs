namespace ProjectBank.Infrastructure;

public class Student : User 
{
    [Required]
    public TeachingProgram Program {get; set;}

    [Required]
    public ICollection<Course> Courses {get; set;}

    public Student(string Email, Institution Institution, string FirstName, string LastName, ICollection<Project> Projects, TeachingProgram Program, ICollection<Course> Courses)
    : base(Email, Institution, FirstName, LastName, Projects)
    {
        this.Program = Program;
        this.Courses = Courses;
    }

    public Student() {}

}