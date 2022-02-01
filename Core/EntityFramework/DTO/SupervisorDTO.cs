namespace ProjectBank.Core.EF.DTO;

public record SupervisorDTO(
    int Id, 

    [Required, EmailAddress]
    string Email,

    [Required, StringLength(50)]
    string FirstName,

    [Required, StringLength(50)]
    string LastName,

    [Required]
    string FacultyName,

    [Required]
    string InstitutionName,

    [Required]
    ICollection<int> ProjectIDs,

    [Required]
    ICollection<int> AuthoredProjectsIDs

) : UserDTO(Id, Email, FirstName, LastName, ProjectIDs);

public record SupervisorCreateDTO
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

    public string FacultyName {get; init;}

    public string InstitutionName {get; init;}

    public ICollection<int> ProjectIDs {get; init;}

    public ICollection<int> AuthoredProjectIDs {get; init;}
}