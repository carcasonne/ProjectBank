using System.ComponentModel.DataAnnotations.Schema;
using ProjectBank.Infrastructure.Entities;
namespace ProjectBank.Infrastructure;

[Index(nameof(Title), IsUnique = true)]
public class Project : ITagable, IProject
{

    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [Required]
    [StringLength(10000)]
    public string Description { get; set; }

    [Required]
    public ProjectStatus Status { get; set; }

    [Required]
    public Category Category { get; set; }

    public IReadOnlyCollection<Tag> Tags
    {
        get{return tags;}

        set
        {
            tags = value;
            Signature = new Signature(Tags);
        }
    }

    [Required]
    private IReadOnlyCollection<Tag> tags = null!;

    //do not map to avoid functional dependency. Can be derived from tags
    [NotMapped]
    public Signature Signature {get; set;}

    public ICollection<User> Users {get; set;}  = null!;

    public ICollection<ProjectBucket> Buckets {get; set;}  = null!;

    [Required]
    public Supervisor Author {get; set;}

    [Required]
    public ICollection<Supervisor> Collaborators { get; set; } = null!;

    public int MaxStudents {get; set;}

    //ignore 'Signature' warning, since Signature is automatically set when Tags is set
    public Project(Supervisor Author, string Title, string Description, ProjectStatus Status, Category Category, IReadOnlyCollection<Tag> Tags, ICollection<User> Users, ICollection<ProjectBucket> Buckets, int MaxStudents)
    {
        this.Author = Author;
        this.Title = Title;
        this.Description = Description;
        this.Status = Status;
        this.Category = Category;
        this.Tags = Tags;
        this.Users = Users;
        this.Buckets = Buckets;
        this.MaxStudents = MaxStudents;
    }
    
    public Project(){}
}

 
