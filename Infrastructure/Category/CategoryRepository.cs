namespace ProjectBank.Infrastructure;

public class CategoryRepository : ICategoryRepository
{
    private readonly ProjectBankContext _dbcontext;

    public CategoryRepository(ProjectBankContext context)
    {
        _dbcontext = context;
    }

    public async Task<CategoryDTO> Read(int id)
    {
        var categories = from c in _dbcontext.Categories
                         where c.Id == id
                         select new CategoryDTO(c.Id, c.Title, c.Description);

        return await categories.FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<CategoryDTO>> Read()
        =>  (await _dbcontext.Categories
                .Select(c => new CategoryDTO(c.Id, c.Title, c.Description))
                .ToListAsync())
                .AsReadOnly();
}
