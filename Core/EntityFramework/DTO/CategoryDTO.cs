namespace ProjectBank.Core.EF.DTO;
public record CategoryDTO(
   
    int Id,
    
    [Required, StringLength(100)]
    string Title,

    [StringLength(1000)]
    string Description
);

public record CodedCategoryDTO (
    int Id,

    [Required, StringLength(100)]
    string Title,

    [StringLength(1000)]
    string Description,

    [Required]
    string FacultyName,

    [Required]
    string Code

    
) : CategoryDTO(Id, Title, Description);

