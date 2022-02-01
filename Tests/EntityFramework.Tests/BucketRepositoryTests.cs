/* Testing code greatly 'inspired' by Rasmus Lystrøm
*  @ https://github.com/ondfisk/BDSA2021/blob/main/MyApp.Infrastructure.Tests/CharacterRepositoryTests.cs
*/

namespace EntityFramework.Tests;

public class BucketRepositoryTests : IDisposable
{
    private readonly ProjectBankContext _context;
    private readonly BucketRepository _repository;
    private readonly ProjectRepository _ProjectRepository;
    private bool disposed;
    private Project project1;
    private Project project2;

    public BucketRepositoryTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var builder = new DbContextOptionsBuilder<ProjectBankContext>();
        builder.UseSqlite(connection);
        builder.EnableSensitiveDataLogging();

        var context = new ProjectBankContext(builder.Options);
        context.Database.EnsureCreated();
        
        var ituInst = new Institution("ITU", "IT-Universitetet i København");
        var ituFaculty = new Faculty("Computer Science", "Computers and Science", ituInst);
        var Supervisor1 = new Supervisor("troe@itu.dk", ituInst, "Troels", "Jyde", new List<Project>(), ituFaculty, new List<Project>());
        var Tag1 = new Tag("Algorithm");
        var Tag2 = new Tag("Agriculture");

        context.Categories.AddRange(ituInst, ituFaculty);
        context.Users.AddRange(Supervisor1);
        context.Tags.AddRange(Tag1, Tag2);

        project1 = new Project { Id = 1, Title = "Best Project", Description = "Simply the best project to be a part of.", Status = ProjectStatus.PUBLIC, Category = ituInst, Tags = new List<Tag>() { Tag1 }, Users = new List<User>(), Buckets = new List<ProjectBucket>(), Author = Supervisor1, MaxStudents = 5 };
        project2 = new Project { Id = 2, Title = "Worst Project", Description = "Don't join this project.", Status = ProjectStatus.PUBLIC, Category = ituInst, Tags = new List<Tag>() { Tag2 }, Users = new List<User>(), Buckets = new List<ProjectBucket>(), Author = Supervisor1, MaxStudents = 5 };

        context.Projects.AddRange(project1, project2);

        var Bucket1 = new ProjectBucket() { Projects = new HashSet<Project>(), Key = "Algorithm", Id = 1 };
        var Bucket2 = new ProjectBucket() { Projects = new HashSet<Project>(), Key = "Agriculture", Id = 2 }; ;

        context.Buckets.Add(Bucket1);
        context.Buckets.Add(Bucket2);

        context.SaveChanges();
        _context = context;

        _repository = new BucketRepository(_context);
        _ProjectRepository = new ProjectRepository(_context);

    }

    [Fact]
    public async void CreateAsync_creates_new_bucket_with_generated_id()
    {
        //Arrange
        var bucketDTO = new BucketCreateDTO
        {
            ProjectIds = new HashSet<int>(),
            Key = "S3CR3TC0D3"
        };

        //Act
        var created = await _repository.CreateAsync(bucketDTO);

        Assert.False(created.Item1 == Response.Conflict);

        var i = created.Item2;

        //Assert
        Assert.Equal(Response.Created, created.Item1);
        Assert.Equal(3, i.Id);
        Assert.Equal("S3CR3TC0D3", i.Key);
        Assert.Empty(i.ProjectIds);

    }

    [Fact]
    public async void ReadByKeyAsync_provided_Key_does_not_exist_returns_Null()
    {
        var nonExisting = await _repository.ReadBucketByKeyAsync("N0TF0UND");

        Assert.Null(nonExisting);
    }

    [Fact]
    public async void ReadByKeyAsync_returns_bucket_with_the_given_key()
    {
        //Arrange
        var Expected = new BucketDTO(new HashSet<int>(), "Algorithm", 1);

        //Act
        var Actual = await _repository.ReadBucketByKeyAsync("Algorithm");

        //Assert
        Assert.Equal(Expected.Id, Actual.Id);
        Assert.Equal(Expected.ProjectIds, Actual.ProjectIds);
        Assert.Equal(Expected.Key, Actual.Key);
    }

    [Fact]
    public async void ReadByIDAsync_provided_ID_does_not_exist_returns_Null()
    {
        var nonExisting = await _repository.ReadBucketByIDAsync(42);

        Assert.Null(nonExisting);
    }

    [Fact]
    public async void ReadAsync_provided_ID_exists_returns_Bucket()
    {
        var bucket = await _repository.ReadBucketByIDAsync(2);
        Assert.Equal(2, bucket.Id);
        Assert.Equal("Agriculture", bucket.Key);
        Assert.Empty(bucket.ProjectIds);
    }

    [Fact]
    public async void ReadAsync_provided_Key_exists_returns_Institution()
    {
        var institution = await _repository.ReadBucketByIDAsync(1);

        Assert.Equal(1, institution.Id);
    }

    [Fact]
    public async void ReadAllAsync_Returns_All_Buckets()
    {
        //Arrange
        var ExpectedAllBuckets = new List<BucketDTO>() { new BucketDTO(new HashSet<int>(), "Algorithm", 1), new BucketDTO(new HashSet<int>(), "Agriculture", 2) }.AsReadOnly();

        //Act
        var ActualAllBuckets = await _repository.ReadAllAsync();

        //Assert
        for (int i = 0; i < ActualAllBuckets.Count; i++)
        {
            Assert.Equal(ExpectedAllBuckets.ElementAt(i).Key, ActualAllBuckets.ElementAt(i).Key);
            Assert.Equal(ExpectedAllBuckets.ElementAt(i).ProjectIds, ActualAllBuckets.ElementAt(i).ProjectIds);
            Assert.Equal(ExpectedAllBuckets.ElementAt(i).Id, ActualAllBuckets.ElementAt(i).Id);
        }
    }

    [Fact]
    public async void AddProjectAsync_Adds_Project_to_a_bucket()
    {
        //Arrange
        var insertProjectId = 1;
        var Expected = new BucketDTO(new HashSet<int>() { 1 }, "Algorithm", 1);
        var ExpectedResponse = Response.Updated;

        //Act
        var ActualResponse = await _repository.AddProjectAsync(Expected.Id, insertProjectId);
        var Actual = await _repository.ReadBucketByIDAsync(Expected.Id);

        //Assert
        Assert.Equal(ExpectedResponse, ActualResponse);
        Assert.Equal(Expected.Id, Actual.Id);
        Assert.Equal(Expected.ProjectIds, Actual.ProjectIds);
        Assert.Equal(Expected.Key, Actual.Key);
    }

    [Fact]
    public async void AddProjectAsync_given_already_existing_project_responds_with_conflict()
    {
        //Arrange
        var Expected = Response.Conflict;

        //Act
        await _repository.AddProjectAsync(1, 1);
        var Actual = await _repository.AddProjectAsync(1, 1);

        //Assert
        Assert.Equal(Expected, Actual);
    }

    [Fact]
    public async void AddProjectAsync_given_non_existing_bucket_id_returns_notfound()
    {
        //Arrange
        var Expected = Response.NotFound;

        //Act
        var Actual = await _repository.AddProjectAsync(4, 1);

        //Assert
        Assert.Equal(Expected, Actual);
    }

    [Fact]
    public async void AddProjectAsync_given_non_existing_project_id_returns_notfound()
    {
        //Arrange
        var Expected = Response.NotFound;

        //Act
        var Actual = await _repository.AddProjectAsync(1, 4);

        //Assert
        Assert.Equal(Expected, Actual);
    }

    [Fact]
    public async void RemoveProjectAsync_removes_a_project_from_all_buckets_containing_the_project()
    {
        //Arrange
        var insertAndRemoveProjectId = 1;
        var Expected = new BucketDTO(new HashSet<int>() { }, "Algorithm", 1);
        var ExpectedResponse = Response.Updated;

        //Act
        await _repository.AddProjectAsync(Expected.Id, insertAndRemoveProjectId);
        var ActualRepsonse = await _repository.RemoveProjectAsync(Expected.Id, insertAndRemoveProjectId);
        var Actual = await _repository.ReadBucketByIDAsync(Expected.Id);


        //Assert
        Assert.Equal(ExpectedResponse, ActualRepsonse);
        Assert.Equal(Expected.Id, Actual.Id);
        Assert.Equal(Expected.ProjectIds, Actual.ProjectIds);
        Assert.Equal(Expected.Key, Actual.Key);
    }

    [Fact]
    public async void RemoveProjectAsync_given_id_of_project_that_is_not_in_any_bucket_returns_notfound()
    {
        //Arrange
        var Expected = Response.NotFound;

        //Act
        var Actual = await _repository.RemoveProjectAsync(1, 1);

        //Assert
        Assert.Equal(Expected, Actual);
    }

    [Fact]
    public async void RemoveProjectAsync_given_non_existing_bucket_id_returns_notfound()
    {
        //Arrange
        var Expected = Response.NotFound;

        //Act
        var Actual = await _repository.RemoveProjectAsync(4, 1);

        //Assert
        Assert.Equal(Expected, Actual);
    }

    [Fact]
    public async void UpdateAllProjectAsync_updates_the_projectIds_set()
    {
        //Arrange
        var newProjectIds = new List<int>() { 1, 2 };
        var Expected = new BucketDTO(newProjectIds.ToHashSet(), "Agriculture", 2);
        var ExpectedResponse = Response.Updated;

        //Act
        var ActualResponse = await _repository.UpdateAllProjectAsync(2, newProjectIds);
        var Actual = await _repository.ReadBucketByIDAsync(2);

        //Assert
        Assert.Equal(ExpectedResponse, ActualResponse);
        Assert.Equal(Expected.Id, Actual.Id);
        Assert.Equal(Expected.ProjectIds, Actual.ProjectIds);
        Assert.Equal(Expected.Key, Actual.Key);
    }

    [Fact]
    public async void UpdateAllProjectAsync_given_non_existing_id_returns_notfound()
    {
        //Arrange
        var Expected = Response.NotFound;

        //Act
        var Actual = await _repository.UpdateAllProjectAsync(4, new List<int>());

        //Assert
        Assert.Equal(Expected, Actual);
    }

    [Fact]
    public async void UpdateAllProjectAsync_given_empty_ICollection_of_projectIds_returns_BadRequest()
    {
        //Arrange
        var Expected = Response.BadRequest;

        //Act
        var Actual = await _repository.UpdateAllProjectAsync(1, new List<int>());

        //Assert
        Assert.Equal(Expected, Actual);
    }

    [Fact]
    public async void ClearBucketAsync()
    {
        //Arrange
        var Expected = new BucketDTO(new HashSet<int>(), "Algorithm", 1);
        var insertAndRemoveProjectId = 1;
        var ExpectedResponse = Response.Updated;

        //Act
        await _repository.AddProjectAsync(Expected.Id, insertAndRemoveProjectId);
        var ActualResponse = await _repository.ClearBucketAsync(Expected.Id);
        var Actual = await _repository.ReadBucketByIDAsync(Expected.Id);

        //Assert
        Assert.Equal(ExpectedResponse, ActualResponse);
        Assert.Equal(Expected.Id, Actual.Id);
        Assert.Equal(Expected.ProjectIds, Actual.ProjectIds);
        Assert.Equal(Expected.Key, Actual.Key);
    }

    [Fact]
    public async void ClearBucketAsync_given_non_existing_id_returns_notfound()
    {
        //Arrange
        var Expected = Response.NotFound;

        //Act
        var Actual = await _repository.ClearBucketAsync(4);

        //Assert
        Assert.Equal(Expected, Actual);
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