namespace ProjectBank.Core.EF.DTO;
public record UserDTO(
    int Id,

    [Required, EmailAddress]
    string Email,

    [Required, StringLength(50)]
    string FirstName,

    [Required, StringLength(50)]
    string LastName,

    [Required]
    ICollection<int> ProjectIDs
);

