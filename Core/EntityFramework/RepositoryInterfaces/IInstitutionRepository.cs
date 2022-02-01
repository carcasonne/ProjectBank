namespace ProjectBank.Core.EF.Repository;

public interface IInstitutionRepository 
{
    Task<(Response, InstitutionDTO)> CreateAsync(InstitutionCreateDTO institution);
    Task<InstitutionDTO> ReadByIDAsync(int insitutionID);
    Task<IReadOnlyCollection<InstitutionDTO>> ReadAllAsync();
}