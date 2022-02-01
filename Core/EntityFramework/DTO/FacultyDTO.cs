namespace ProjectBank.Core.EF.DTO;

public record FacultyDTO(
    int Id,

    [Required, StringLength(100)]
    string Title,

    [StringLength(1000)]
    string Description,

    [Required]
    string InstitutionName

) : CategoryDTO(Id, Title, Description);

public record FacultyCreateDTO 
{
    [Required, StringLength(100)] 
    public string Title { get; init; }

    [StringLength(1000)]
    public string Description { get; init; }

    [Required]
    public string InstitutionName { get; init; }
};
