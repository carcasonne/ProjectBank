namespace ProjectBank.Infrastructure;

public class Supervisor : User
{
    [Required]
    public Faculty Faculty {get;set;}

    [Required]
    public ICollection<Project> AuthoredProjects {get; set;}

    public Supervisor(string Email, Institution Institution, string FirstName, string LastName, ICollection<Project> Projects, Faculty Faculty, ICollection<Project> AuthoredProjects)
    : base(Email, Institution, FirstName, LastName, Projects)
    {
        this.Faculty = Faculty;
        this.AuthoredProjects = AuthoredProjects;
    }

    public Supervisor() {}
}

