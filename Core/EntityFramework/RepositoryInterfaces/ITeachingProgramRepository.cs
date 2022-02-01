namespace ProjectBank.Core.EF.Repository;

public interface ITeachingProgramRepository 
{
    Task<(Response, TeachingProgramDTO)> CreateAsync(TeachingProgramCreateDTO program);
    Task<TeachingProgramDTO> ReadProgramByIDAsync(int programID);
    Task<IReadOnlyCollection<TeachingProgramDTO>> ReadAllAsync();
}

