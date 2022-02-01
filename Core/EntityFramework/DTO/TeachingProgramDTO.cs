namespace ProjectBank.Core.EF.DTO;
public record TeachingProgramDTO(
    int Id,

    [Required, StringLength(100)]
    string Title,

    [StringLength(1000)]
    string Description,

    [Required]
    string FacultyName,

    [Required]
    string Code,

    ICollection<string> CourseCodes

) : CodedCategoryDTO(Id, Title, Description, FacultyName, Code);

public record TeachingProgramCreateDTO 
{
 
    [Required, StringLength(100)]
    public string Title {get; init;}

    [StringLength(1000)]
    public string Description {get; init;}

    [Required]
    public string FacultyName {get; init;}

    [Required]
    public string Code {get; init;}
    public ICollection<string> CourseCodes {get; init;}
}