namespace ProjectBank.Core.EF.Repository;

public interface IStudentRepository 
{
    Task<(Response, StudentDTO)> CreateAsync(StudentCreateDTO student);
    Task<StudentDTO> ReadStudentByIDAsync(int studentID);
    Task<IReadOnlyCollection<StudentDTO>> ReadAllAsync();
}