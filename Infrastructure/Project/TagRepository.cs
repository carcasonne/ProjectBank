namespace ProjectBank.Infrastructure.Entities;

public class TagRepository : ITagRepository
{
    private readonly ProjectBankContext _dbcontext;

    public TagRepository(ProjectBankContext context)
    {
        _dbcontext = context;
    }

    public async Task<(Response, TagDTO)> CreateAsync(TagCreateDTO tag)
    {
        var conflict =
            await _dbcontext.Tags
                            .Where(t => t.Name == tag.Name)
                            .Select(t => new TagDTO(t.Id, t.Name))
                            .FirstOrDefaultAsync();

        if (conflict != null)
        {
            return (Response.Conflict, conflict);
        }

        var entity = new Tag(tag.Name);

        _dbcontext.Tags.Add(entity);
        
        await _dbcontext.SaveChangesAsync();

        return (Response.Created, new TagDTO(entity.Id, entity.Name));
    }
    public async Task<Option<TagDTO>> ReadTagByIDAsync(int TagID)
    {
        var tags = from t in _dbcontext.Tags
                        where t.Id == TagID
                        select new TagDTO(t.Id, t.Name);

        return await tags.FirstOrDefaultAsync();
    }

     public async Task<TagDTO> ReadTagByNameAsync(string TagName)
    {
        var tags = from t in _dbcontext.Tags
                        where t.Name == TagName
                        select new TagDTO(t.Id, t.Name);

        return await tags.FirstOrDefaultAsync();
    }


    public async Task<IReadOnlyCollection<TagDTO>> ReadAllAsync() =>
        (await _dbcontext.Tags
                        .Select(t => new TagDTO(t.Id, t.Name))
                        .ToListAsync())
                        .AsReadOnly(); 

    public async Task<ICollection<TagDTO>> ReadCollectionAsync(ICollection<int> tagIDs) =>
        (await _dbcontext.Tags
                        .Where(t => tagIDs
                            .Any( inT => inT == t.Id))
                        .Select(t => new TagDTO(t.Id, t.Name))
                        .ToListAsync())
                        .AsReadOnly();
}