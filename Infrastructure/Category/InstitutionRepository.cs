namespace ProjectBank.Infrastructure;

public class InstitutionRepository : IInstitutionRepository
{
    private readonly ProjectBankContext _dbcontext;

    public InstitutionRepository(ProjectBankContext context)
    {
        _dbcontext = context;
    }

    public async Task<(Response, InstitutionDTO)> CreateAsync(InstitutionCreateDTO institution)
    {
        //assume that all institutions with the same name are equal
        var conflict =
            await _dbcontext.Institutions
                            .Where(i => i.Title == institution.Title)
                            .Select(i => new InstitutionDTO(i.Id, i.Title, i.Description))
                            .FirstOrDefaultAsync();

        if (conflict != null)
        {
            return (Response.Conflict, conflict);
        }

        var entity = new Institution
        (
            institution.Title,
            institution.Description
        );

        _dbcontext.Institutions.Add(entity);

        await _dbcontext.SaveChangesAsync();

        return (Response.Created, new InstitutionDTO(entity.Id, entity.Title, entity.Description));
    }
    public async Task<InstitutionDTO> ReadByIDAsync(int insitutionID)
    {
        var institutions = from i in _dbcontext.Institutions
                           where i.Id == insitutionID
                           select new InstitutionDTO(i.Id, i.Title, i.Description);

        return await institutions.FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<InstitutionDTO>> ReadAllAsync() =>
        (await _dbcontext.Institutions
                        .Select(i => new InstitutionDTO(i.Id, i.Title, i.Description))
                        .ToListAsync())
                        .AsReadOnly();
}