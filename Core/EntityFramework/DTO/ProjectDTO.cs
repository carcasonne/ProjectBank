namespace ProjectBank.Core.EF.DTO;

public record ProjectKeyDTO(int AuthorID, String Title);

public record ProjectReferenceDTO (
    int Id,

    int CategoryID,
    
    ICollection<int> TagIDs
);


public record ProjectDTO (
    int Id,

    [Required]
    int AuthorID,

    [Required, StringLength(100)]
    string Title,

    [Required,StringLength(10000)]
    string Description,

    [Required]
    ProjectStatus Status,

    int MaxStudents,

    int CategoryID,

    ICollection<int> TagIDs,

    ICollection<string> TagNames,

    ICollection<int> UserIDs,

    ICollection<int> BucketIDs

) : ProjectKeyDTO(AuthorID, Title);

public record ProjectCreateDTO
{
    [Required]
    public int AuthorID {get; set;}

    [Required, StringLength(100)]
    public string Title {get; set;}

    [Required,StringLength(10000)]
    public string Description {get; set;}

    [Required]
    public ProjectStatus Status {get; set;}
    
    public int MaxStudents {get; set;}

    public int CategoryID {get; set;}
    
    public ICollection<int> TagIDs {get; set;}

    public ICollection<string> TagNames {get; set;}

    public ICollection<int> UserIDs {get; set;}

    public ICollection<int> BucketIDs {get; set;}
};

public record ProjectUpdateDTO : ProjectCreateDTO
{
     public int Id { get; init; }
}