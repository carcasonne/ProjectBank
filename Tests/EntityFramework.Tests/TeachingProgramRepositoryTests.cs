/* Testing code greatly 'inspired' by Rasmus Lystrøm
*  @ https://github.com/ondfisk/BDSA2021/blob/main/MyApp.Infrastructure.Tests/CharacterRepositoryTests.cs
*/

namespace EntityFramework.Tests;

public class TeachingProgramRepositoryTests : IDisposable
{
    private readonly ProjectBankContext _context;
    private readonly TeachingProgramRepository _repository;
    private bool disposed;

    public TeachingProgramRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var builder = new DbContextOptionsBuilder<ProjectBankContext>();
        builder.UseSqlite(connection);
        builder.EnableSensitiveDataLogging();

        var context = new ProjectBankContext(builder.Options);
        context.Database.EnsureCreated();

        Institution institution = new Institution("ITU","Best university") {Id = 1};
        Faculty faculty = new Faculty("Comp Sci","comp",institution) {Id = 2};

        TeachingProgram software =  new TeachingProgram("SWU","Softwareudvikling",faculty,"SWU2021",new List<Course>()) {Id = 3};
        TeachingProgram data =  new TeachingProgram("DDS","Data Design",faculty,"DDS2021",new List<Course>()) {Id = 4};


        Course bdsa = new Course{Id = 5, Title = "BDSA", Description = "Software Design and Architecture", Faculty = faculty, Code ="BDSA2021", Programs = new List<TeachingProgram> {software}};
        Course idbs = new Course{Id = 6, Title = "IDBS", Description = "Databases", Faculty = faculty, Code ="IDBS2021", Programs = new List<TeachingProgram> {software}};

     
        context.Institutions.Add(institution);
        context.Faculties.Add(faculty);

        

        context.Courses.AddRange(
            bdsa,
            idbs
        );

        context.Programs.AddRange(
           software,
           data
        );

        context.SaveChanges();
        _context = context;
        _repository = new TeachingProgramRepository(_context);
    }

    [Fact]
    public async void CreateAsync_creates_new_program_with_generated_id() 
    {
        //Arrange
        var programDTO = new TeachingProgramCreateDTO
        {
            Title ="DDIT",
            Description ="Det der IT",
            FacultyName ="Comp Sci",
            Code="DDIT2021",
            CourseCodes = new List<string>()
        };

        //Act
        var created = await _repository.CreateAsync(programDTO);

        if(created.Item1 == Response.Conflict)
        {
            Assert.False(true);
        }

        var p = created.Item2;

        //Assert
        Assert.Equal(Response.Created, created.Item1);
        Assert.Equal(7, p.Id);
        Assert.Equal("DDIT",p.Title);
        Assert.Equal("Det der IT",p.Description);
        Assert.Equal("Comp Sci",p.FacultyName);
        Assert.Equal("DDIT2021",p.Code);
    }

    [Fact]
     public async void ReadAllAsync_returns_all_programs()
    {
        var programs = await _repository.ReadAllAsync();

        var testList = new List<string>() {"BDSA2021","IDBS2021"};
         
            Assert.Collection(programs,
                program =>
                 {
                     Assert.Equal("SWU",program.Title);
                     Assert.Equal("Softwareudvikling",program.Description);
                     Assert.Equal("Comp Sci",program.FacultyName);
                     Assert.Equal("SWU2021",program.Code);
                     Assert.Equal(testList,program.CourseCodes);
                 },
                program =>
                {
                        Assert.Equal("DDS",program.Title);
                        Assert.Equal("Data Design",program.Description);
                        Assert.Equal("Comp Sci",program.FacultyName);
                        Assert.Equal("DDS2021",program.Code);
                        Assert.Empty(program.CourseCodes);
                }
            );
    } 

    public async void ReadByIDAsync_provided_ID_does_not_exist_returns_Null()
    {
        var nonExisting = await _repository.ReadProgramByIDAsync(34);

        Assert.Null(nonExisting);
    }

    [Fact]
    public async void ReadAsync_provided_ID_exists_returns_Program()
    {
        var program = await _repository.ReadProgramByIDAsync(3);
        Assert.Equal(3, program.Id);
        Assert.Equal("SWU", program.Title);
        Assert.Equal("Softwareudvikling", program.Description);
        Assert.Equal("Comp Sci",program.FacultyName);
        Assert.Equal("SWU2021",program.Code);
        Assert.Equal(2,program.CourseCodes.Count());
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

