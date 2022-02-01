namespace ProjectBank.Core.EF.DTO;

public record StudentDTO(
    int Id, 

    [Required, EmailAddress]
    string Email,

    [Required, StringLength(50)]
    string FirstName,

    [Required, StringLength(50)]
    string LastName,

    [Required]
    string ProgramCode,

    [Required]
    string InstitutionName,

    [Required]
    ICollection<int> ProjectIDs,

    [Required]
    ICollection<int> CourseIDs

) : UserDTO(Id, Email, FirstName, LastName, ProjectIDs);

public record StudentCreateDTO
{

    [EmailAddress]
    [Required]
    [StringLength(50)]
    public string Email { get; init; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; init; }

    [Required]
    [StringLength(50)]
    public string LastName { get; init; }

    public string ProgramCode {get; init; }
    public string InstitutionName {get; init; }
    public ICollection<int> ProjectIDs {get; init;}

    public ICollection<int> CourseIDs {get; init;}
}