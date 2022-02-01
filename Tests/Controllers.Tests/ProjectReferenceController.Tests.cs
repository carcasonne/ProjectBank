using Microsoft.Data.Sqlite;

namespace ProjectBank.Tests.Controllers.Tests;

public class ProjectReferenceControllerTests
{

    Tag Agriculture = new Tag("Agriculture");
    Tag ComputerScience = new Tag("Computer Science");
    Tag Security = new Tag("Security");
    Tag Algorithms = new Tag("Algorithms");
    Tag Simulation = new Tag("Simulation");
    Tag Food = new Tag("Food");
    Tag Farming = new Tag("Farming");


    Project AgricultureFood;
    Project ComputerScienceSimulationAlgorithmsAgriculture;
    Project ComputerScienceAlgorithmsSecurity;
    Project AgricultureFarming;
    Project ComputerScienceAlgorithmsSimulationSecurity;

    static Institution ITU = new Institution { Id = 1, Title = "ITU", Description = "ITU" };
    static Institution DTU = new Institution { Id = 2, Title = "DTU", Description = "ITU" };
    static Faculty ComputerScienceFaculty = new Faculty { Title = "Computer Science", Description = "ITU", Institution = ITU, Id = 3};
    Supervisor Supervisor1 = new Supervisor("troe@itu.dk", ITU, "Troels", "Jyde", new List<Project>(), ComputerScienceFaculty, new List<Project>());

    ProjectLSH _LSH;

    public ProjectBankContext _context;


    public ProjectRepository _projectRepository;
    public TagRepository _tagRepository;
    public CategoryRepository _categoryRepository;


    public ProjectReferenceControllerTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var builder = new DbContextOptionsBuilder<ProjectBankContext>();
        builder.UseSqlite(connection);
        builder.EnableSensitiveDataLogging();

        var context = new ProjectBankContext(builder.Options);
        context.Database.EnsureCreated();
        context.Categories.Add(ITU);
        context.Categories.Add(DTU);
        context.Categories.Add(ComputerScienceFaculty);

        context.Tags.Add(Agriculture);
        context.Tags.Add(ComputerScience);
        context.Tags.Add(Algorithms);
        context.Tags.Add(Security);
        context.Tags.Add(Simulation);
        context.Tags.Add(Food);
        context.Tags.Add(Farming);
        context.SaveChanges();

        AgricultureFood = new Project { Category = ITU, Tags = new List<Tag> { Agriculture, Food }, Id = 1, Author = Supervisor1, Title = "AgricultureFood", Description = "AgricultureFood", Status = ProjectBank.Core.ProjectStatus.PUBLIC, Buckets = new List<ProjectBucket>(), Users = new List<User>(), Collaborators = new List<Supervisor>(), MaxStudents = 5 };
        ComputerScienceSimulationAlgorithmsAgriculture = new Project { Category = ITU, Tags = new List<Tag> { ComputerScience, Simulation, Algorithms }, Id = 2, Author = Supervisor1, Title = "ComputerScienceSimulationAlgorithms", Description = "ComputerScienceSimulationAlgorithmsAgriculture", Status = ProjectBank.Core.ProjectStatus.PUBLIC, Buckets = new List<ProjectBucket>(), Users = new List<User>(), Collaborators = new List<Supervisor>(), MaxStudents = 5 };
        ComputerScienceAlgorithmsSecurity = new Project { Category = ITU, Tags = new List<Tag> { ComputerScience, Agriculture, Food }, Id = 3, Author = Supervisor1, Title = "ComputerScienceAlgorithmsSecurity", Description = "ComputerScienceAlgorithmsSecurity", Status = ProjectBank.Core.ProjectStatus.PUBLIC, Buckets = new List<ProjectBucket>(), Users = new List<User>(), Collaborators = new List<Supervisor>(), MaxStudents = 5 };
        AgricultureFarming = new Project { Category = DTU, Tags = new List<Tag> { Agriculture, Farming, Food }, Id = 4, Author = Supervisor1, Title = "AgricultureFarming", Description = "AgricultureFarming", Status = ProjectBank.Core.ProjectStatus.PUBLIC, Buckets = new List<ProjectBucket>(), Users = new List<User>(), Collaborators = new List<Supervisor>(), MaxStudents = 5 };
        ComputerScienceAlgorithmsSimulationSecurity = new Project { Category = DTU, Tags = new List<Tag> { Security }, Id = 5, Author = Supervisor1, Title = "ComputerScienceAlgorithmsSimulationSecurity", Description = "ComputerScienceAlgorithmsSimulationSecurity", Status = ProjectBank.Core.ProjectStatus.PUBLIC, Buckets = new List<ProjectBucket>(), Users = new List<User>(), Collaborators = new List<Supervisor>(), MaxStudents = 5 };

        context.Projects.Add(AgricultureFood);
        context.Projects.Add(ComputerScienceSimulationAlgorithmsAgriculture);
        context.Projects.Add(ComputerScienceAlgorithmsSecurity);
        context.Projects.Add(AgricultureFarming);
        context.Projects.Add(ComputerScienceAlgorithmsSimulationSecurity);
        context.SaveChanges();
        
        _context = context;
        _projectRepository = new ProjectRepository(_context);
        _tagRepository = new TagRepository(_context);
        _categoryRepository = new CategoryRepository(_context);
    }

    [Fact]
    public async Task GetSorted_Returns_Sorted()
    {
       // Arrange
        var logger = new Mock<ILogger<ProjectReferenceController>>();
        var controller = new ProjectReferenceController(logger.Object, _projectRepository);

        // Act
        var expected = new List<int>{3, 4};
        var actualDTOs = await controller.GetSorted(1, 5);
        var actual = new List<int>();
        foreach(var dto in actualDTOs) actual.Add(dto.Id);
        
        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetSortedInCategory_Returns_SortedInCategory()
    {
        // Arrange
        var logger = new Mock<ILogger<ProjectReferenceController>>();
        var controller = new ProjectReferenceController(logger.Object, _projectRepository);

        // Act
        var expected = new List<int>{3};
        var actualDTOs = await controller.GetSortedInCategory(1, 5);
        var actual = new List<int>();
        foreach(var dto in actualDTOs) actual.Add(dto.Id);
        
        // Assert
        Assert.Equal(expected, actual);
    }

     [Fact]
    public async Task GetSorted_Returns_EmptyList_If_No_Related()
    {
        // Arrange
        var logger = new Mock<ILogger<ProjectReferenceController>>();
        var controller = new ProjectReferenceController(logger.Object, _projectRepository);

        // Act
        var expected = new List<int>{};
        var actualDTOs = await controller.GetSortedInCategory(5, 5);
        var actual = new List<int>();
        foreach(var dto in actualDTOs) actual.Add(dto.Id);
        
        // Assert
        Assert.Equal(expected, actual);
    }
}