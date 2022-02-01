namespace ProjectBank.Core.EF.DTO;
public record TagDTO(
    int Id,
    [Required, StringLength(100)]
    string Name
);

public record TagCreateDTO
{
    [Required]
    [StringLength(100)]
    public string Name {get; init;}
}