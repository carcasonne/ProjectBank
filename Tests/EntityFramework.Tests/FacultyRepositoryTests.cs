/* Testing code greatly 'inspired' by Rasmus Lystrøm
*  @ https://github.com/ondfisk/BDSA2021/blob/main/MyApp.Infrastructure.Tests/CharacterRepositoryTests.cs
*/

namespace ProjectBank.Infrastructure;

public class FacultyRepositoryTest : IDisposable
{
    
    private readonly ProjectBankContext _context;
    private readonly FacultyRepository _repository;
    private bool disposed;

    public FacultyRepositoryTest()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var builder = new DbContextOptionsBuilder<ProjectBankContext>();
        builder.UseSqlite(connection);
        builder.EnableSensitiveDataLogging();

        var context = new ProjectBankContext(builder.Options);
        context.Database.EnsureCreated();

        Institution I1 = new Institution{Id = 3, Title = "IT-Universitetet", Description = "Informations Teknologiens Universitet"};
        Institution I2 = new Institution{Id = 4, Title = "University of Copenhagen", Description = "Jødernes Universitet"};

        context.Institutions.AddRange(
            I1, I2
        );

        context.Faculties.AddRange(
            new Faculty{Id = 1, Title = "Jura", Description = "Det Juridiske Fakulitet", Institution = I1},
            new Faculty{Id = 2, Title = "Software", Description = "Det IT-tekniske Fakulitet", Institution = I2}
        );

        context.SaveChanges();

        _context = context;
        _repository = new FacultyRepository(_context);
    }

    [Fact]
    public async void CreateAsync_creates_new_faculty_with_generated_id() 
    {
        //Arrange
        var facultyDTO = new FacultyCreateDTO
        {
            Title = "Maths",
            Description = "Det Matematiske Fakulitet",
            InstitutionName = "University of Copenhagen"
        };


        //Act
        var created = await _repository.CreateAsync(facultyDTO);

        if(created.Item1 == Response.Conflict)
        {
            Assert.False(true);
        }

        var i = created.Item2;

        //Assert
        Assert.Equal(Response.Created, created.Item1);
        Assert.Equal(5, i.Id);
        Assert.Equal("Maths",i.Title);
        Assert.Equal("Det Matematiske Fakulitet",i.Description);
    }

    [Fact]
    public async void ReadAllAsync_returns_all_faculties()
    {

        var faculties = await _repository.ReadAllAsync();

    Assert.Collection(faculties, 
        Faculty => Assert.Equal(new FacultyDTO(1, "Jura", "Det Juridiske Fakulitet", "IT-Universitetet"), Faculty),
        Faculty => Assert.Equal(new FacultyDTO(2, "Software", "Det IT-tekniske Fakulitet", "University of Copenhagen"), Faculty)
    );
    }

    [Fact]
    public async void ReadByIDAsync_provided_ID_does_not_exist_returns_Null()
    {
        var nonExisting = await _repository.ReadByIDAsync(404);

        Assert.Null(nonExisting);
    }

    [Fact]
    public async void ReadAsync_provided_ID_exists_returns_Institution()
    {
        var faculty = await _repository.ReadByIDAsync(2);

        Assert.Equal(2, faculty.Id);
        Assert.Equal("Software", faculty.Title);
        Assert.Equal("Det IT-tekniske Fakulitet", faculty.Description);
        Assert.Equal("University of Copenhagen", faculty.InstitutionName);

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