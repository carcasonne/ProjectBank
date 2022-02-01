/* Testing code greatly 'inspired' by Rasmus Lystrøm
*  @ https://github.com/ondfisk/BDSA2021/blob/main/MyApp.Infrastructure.Tests/CharacterRepositoryTests.cs
*/

namespace EntityFramework.Tests;

public class TagRepositoryTests: IDisposable
{
    private readonly ProjectBankContext _context;
    private readonly TagRepository _repository;
    private bool disposed;

    public TagRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var builder = new DbContextOptionsBuilder<ProjectBankContext>();
        builder.UseSqlite(connection);
        builder.EnableSensitiveDataLogging();

        var context = new ProjectBankContext(builder.Options);
        context.Database.EnsureCreated();

        context.Tags.AddRange(
            new Tag("Algo"),
            new Tag("Java")
        );

        context.SaveChanges();

        _context = context;
        _repository = new TagRepository(_context);
    }

    [Fact]
    public async void CreateAsync_creates_new_tag_with_generated_id() 
    {
        //Arrange
        var tagDTO = new TagCreateDTO
        {
            Name = "Code"
        };

        //Act
        var created = await _repository.CreateAsync(tagDTO);

        if(created.Item1 == Response.Conflict)
        {
            Assert.False(true);
        }

        var t = created.Item2;

        //Assert
        Assert.Equal(Response.Created, created.Item1);
        Assert.Equal(3, t.Id);
        Assert.Equal("Code",t.Name);
    }

    [Fact]
    public async void ReadAllAsync_returns_all_Tags()
    {
        var tags = await _repository.ReadAllAsync();

        Assert.Collection(tags,
            tag => Assert.Equal(new TagDTO(1, "Algo"), tag),
            tag => Assert.Equal(new TagDTO(2, "Java"), tag)
        );
    }

    [Fact]
    public async void ReadByIDAsync_provided_ID_does_not_exist_returns_Null()
    {
        var nonExisting = await _repository.ReadTagByIDAsync(42);

        Assert.Equal(null, nonExisting);
    }

    [Fact]
    public async void ReadTagAsync_provided_ID_exists_returns_Tag()
    {
        var option = await _repository.ReadTagByIDAsync(2);
        var tag = option.Value;

        Assert.Equal(2, tag.Id);
        Assert.Equal("Java",tag.Name);
    }

    [Fact]
     public async void ReadTagAsync_provided_Name_exists_returns_Tag()
    {
        var tag = await _repository.ReadTagByNameAsync("Algo");

        Assert.Equal(1, tag.Id);
        Assert.Equal("Algo",tag.Name);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}