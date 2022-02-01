/* Testing code greatly 'inspired' by Rasmus Lystrøm
*  @ https://github.com/ondfisk/BDSA2021/blob/main/MyApp.Infrastructure.Tests/CharacterRepositoryTests.cs
*/

namespace EntityFramework.Tests;

public class InstitutionRepositoryTest : IDisposable
{
    private readonly ProjectBankContext _context;
    private readonly InstitutionRepository _repository;
    private bool disposed;

    public InstitutionRepositoryTest()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var builder = new DbContextOptionsBuilder<ProjectBankContext>();
        builder.UseSqlite(connection);
        builder.EnableSensitiveDataLogging();

        var context = new ProjectBankContext(builder.Options);
        context.Database.EnsureCreated();

        context.Institutions.AddRange(
            new Institution{Id = 1, Title = "ITU", Description = "Best university"},
            new Institution{Id = 2, Title = "KU", Description = "West university"}
        );

        context.SaveChanges();

        _context = context;
        _repository = new InstitutionRepository(_context);
    }

    [Fact]
    public async void CreateAsync_creates_new_institution_with_generated_id() 
    {
        //Arrange
        var institutionDTO = new InstitutionCreateDTO
        {
            Title = "DTU",
            Description = "Danmarks Tekniske Universitet"
        };


        //Act
        var created = await _repository.CreateAsync(institutionDTO);

        if(created.Item1 == Response.Conflict)
        {
            Assert.False(true);
        }

        var i = created.Item2;

        //Assert
        Assert.Equal(Response.Created, created.Item1);
        Assert.Equal(3, i.Id);
        Assert.Equal("DTU",i.Title);
        Assert.Equal("Danmarks Tekniske Universitet",i.Description);
    }

    [Fact]
    public async void ReadAllAsync_returns_all_institutions()
    {
        var institutions = await _repository.ReadAllAsync();

        Assert.Collection(institutions,
            institution => Assert.Equal(new InstitutionDTO(1, "ITU", "Best university"), institution),
            institution => Assert.Equal(new InstitutionDTO(2, "KU", "West university"), institution)
        );
    }

    [Fact]
    public async void ReadByIDAsync_provided_ID_does_not_exist_returns_Null()
    {
        var nonExisting = await _repository.ReadByIDAsync(42);

        Assert.Null(nonExisting);
    }

    [Fact]
    public async void ReadAsync_provided_ID_exists_returns_Institution()
    {
        var institution = await _repository.ReadByIDAsync(2);

        Assert.Equal(2, institution.Id);
        Assert.Equal("KU", institution.Title);
        Assert.Equal("West university", institution.Description);
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