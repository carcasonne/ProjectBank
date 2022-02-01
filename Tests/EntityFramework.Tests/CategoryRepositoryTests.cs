/* Testing code greatly 'inspired' by Rasmus Lystrøm
*  @ https://github.com/ondfisk/BDSA2021/blob/main/MyApp.Infrastructure.Tests/CharacterRepositoryTests.cs
*/

namespace EntityFramework.Tests;

public class CategoryRepositoryTests : IDisposable
{
    private readonly ProjectBankContext _context;
    private readonly CategoryRepository _repository;
    private bool disposed;

    public CategoryRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var builder = new DbContextOptionsBuilder<ProjectBankContext>();
        builder.UseSqlite(connection);
        builder.EnableSensitiveDataLogging();

        var context = new ProjectBankContext(builder.Options);
        context.Database.EnsureCreated();

        var Inst1 = new Institution{Id = 1, Title = "ITU", Description = "Best university"};
        var Inst2 = new Institution{Id = 2, Title = "KU", Description = "West university"};

        context.Institutions.AddRange(
            Inst1,
            Inst2
        );

        var Fac1 = new Faculty{Id = 3, Title = "Computer Science", Description = "Something something", Institution = Inst1};
        var Fac2 = new Faculty{Id = 4, Title = "Humanities", Description = "Something something human", Institution = Inst2};

        context.Faculties.AddRange(
            Fac1,
            Fac2
        );

        context.Programs.AddRange(
            new TeachingProgram{Id = 5, Title = "Software Design and Architecture", Description = "idk i didn't pay attention?", Faculty = Fac1, Code = "BDSA2020", Courses = new List<Course>()}
        );

        context.SaveChanges();

        _context = context;
        _repository = new CategoryRepository(_context);
    }

    [Fact]
    public async void ReadByIDAsync_provided_ID_does_not_exist_returns_Null()
    {
        var nonExisting = await _repository.Read(34);

        Assert.Null(nonExisting);
    }

    [Fact] 
    public async void ReadAsync_provided_ID_exists_returns_Category()
    {
        var course = await _repository.Read(2);

        Assert.Equal(2, course.Id);
        Assert.Equal("KU", course.Title);
        Assert.Equal("West university", course.Description);
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