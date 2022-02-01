namespace ProjectBank.Infrastructure;

[Index(nameof(Email), IsUnique = true)]

public abstract class User 
{

    public int Id { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [StringLength(1000)]
    public Institution Institution { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }
    [Required]
    [StringLength(50)]
    public string LastName { get; set; }

    [Required]
    public ICollection<Project> Projects {get; set;} = null!;

    protected User(string Email, Institution Institution, string FirstName, string LastName, ICollection<Project> Projects)
    {
        this.Email = Email;
        this.Institution = Institution;
        this.FirstName = FirstName;
        this.LastName = LastName;
        this.Projects = Projects;   
    }

    protected User() {}
}