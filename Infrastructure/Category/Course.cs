namespace ProjectBank.Infrastructure;

public class Course : CodedCategory 
{
    [Required]
    public IEnumerable<TeachingProgram> Programs {get; set;}
    public IEnumerable<Student> Students {get; set;}

    public Course(string Title, string Description, Faculty Faculty, string Code, IEnumerable<TeachingProgram> Programs, IEnumerable<Student> Students) 
    : base(Title, Description, Faculty, Code)
    {
        this.Programs = Programs;
        this.Students = Students;
    }
    public Course() {}
}