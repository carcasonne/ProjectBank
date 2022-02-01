/* Testing code greatly 'inspired' by Rasmus Lystrøm
*  @ https://github.com/ondfisk/BDSA2021/blob/main/MyApp.Infrastructure.Tests/CharacterRepositoryTests.cs
*/

namespace EntityFramework.Tests;

public class CourseRepositoryTests : IDisposable
{
    
    private readonly ProjectBankContext _context;
    private readonly CourseRepository _repository;
    private bool disposed;

    public CourseRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var builder = new DbContextOptionsBuilder<ProjectBankContext>();
        builder.UseSqlite(connection);
        builder.EnableSensitiveDataLogging();

        var context = new ProjectBankContext(builder.Options);
        context.Database.EnsureCreated();

        

        var ITU = new Institution("ITU", "IT-University of Copenhagen") {Id = 1};
        var ComputerScience = new Faculty("Computer Science", "Related to computers", ITU) {Id = 2};

        TeachingProgram Software = new TeachingProgram("Software Development", "The best line at ITU", ComputerScience, "SWU2021", new List<Course>()) {Id = 3};

        Student Anton = new Student("antbr@itu.dk", ITU, "Anton", "Breinholt", new List<Project>(), Software, new List<Course>()) {Id = 4};
        Student Villum = new Student("vson@itu.dk", ITU, "Villum", "Sonne", new List<Project>(), Software, new List<Course>()) {Id = 5};

        Course Bdsa = new Course("BDSA", "Software Design and Architecture", ComputerScience, "BDSA2021", new List<TeachingProgram>{Software}, new List<Student>() {Anton} ) {Id = 6};
        Course Idbs = new Course("IDBS","Databases", ComputerScience, "IDBS2021", new List<TeachingProgram>{Software}, new List<Student>() {Anton, Villum}) {Id = 7};

        context.Institutions.Add(ITU);
        context.Faculties.Add(ComputerScience);
          
        context.Programs.AddRange(
            Software
        );

        context.Courses.AddRange(
            Bdsa,
            Idbs
        );

        context.Users.AddRange(
            Anton,
            Villum
        );

        context.SaveChanges();
        _context = context;
        _repository = new CourseRepository(_context);
    }

    [Fact]
    public async void CreateAsync_creates_new_course_with_generated_id() 
    {
        //Arrange
        var courseDTO = new CourseCreateDTO
        {
            Title = "DISYS",
            Description = "Distributed Systems",
            InstitutionName = "ITU",
            FacultyName = "Computer Science",
            Code = "DISYS2021",
            ProgramCodes = new List<string>() {"SWU2021"},
            StudentEmails = new List<string>(),
        };

        //Act
        var created = await _repository.CreateAsync(courseDTO);

        if(created.Item1 == Response.Conflict)
        {
            Assert.False(true);
        }

        var c = created.Item2;

        //Assert
        Assert.Equal(Response.Created, created.Item1);
        Assert.Equal(8, c.Id);
        Assert.Equal("DISYS", c.Title);
        Assert.Equal("Distributed Systems",c .Description);
        Assert.Equal("DISYS2021", c.Code);
        Assert.Equal(new List<string>() {"SWU2021"}, c.ProgramCodes);
    }
    
    [Fact]
    public async void ReadAllAsync_returns_all_courses()
    {
        
        var courses = await _repository.ReadAllAsync();

        var TeachingProgramList = new List<string>() {"SWU2021"};

        var StudentList_BDSA = new List<string>() {"antbr@itu.dk"};
        var StudentList_IDBS = new List<string>() {"antbr@itu.dk", "vson@itu.dk"};

        Assert.Collection(courses,
            course => 
            {
                Assert.Equal(6, course.Id);
                Assert.Equal("BDSA", course.Title);
                Assert.Equal("Software Design and Architecture", course.Description);
                Assert.Equal("Computer Science", course.FacultyName);
                Assert.Equal("BDSA2021", course.Code);
                Assert.Equal(TeachingProgramList, course.ProgramCodes);
                Assert.Equal(StudentList_BDSA, course.StudentEmails);
            },
            course =>
            {
                Assert.Equal(7, course.Id);
                Assert.Equal("IDBS", course.Title);
                Assert.Equal("Databases", course.Description);
                Assert.Equal("Computer Science", course.FacultyName);
                Assert.Equal("IDBS2021", course.Code);
                Assert.Equal(TeachingProgramList, course.ProgramCodes);
                Assert.Equal(StudentList_IDBS, course.StudentEmails);
            }
        );
    }

    [Fact]
    public async void ReadByIDAsync_provided_ID_does_not_exist_returns_Null()
    {
        var nonExisting = await _repository.ReadCourseByIDAsync(34);

        Assert.Null(nonExisting);
    }

    [Fact] 
    public async void ReadAsync_provided_ID_exists_returns_Course()
    {
        var course = await _repository.ReadCourseByIDAsync(6);
        Assert.Equal(6, course.Id);
        Assert.Equal("BDSA", course.Title);
        Assert.Equal("Software Design and Architecture", course.Description);
        Assert.Equal("Computer Science",course.FacultyName);
        Assert.Equal("BDSA2021",course.Code);
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

