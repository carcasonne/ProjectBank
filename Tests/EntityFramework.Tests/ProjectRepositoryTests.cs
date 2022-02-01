/* Testing code greatly 'inspired' by Rasmus Lystrøm
*  @ https://github.com/ondfisk/BDSA2021/blob/main/MyApp.Infrastructure.Tests/CharacterRepositoryTests.cs
*/

namespace EntityFramework.Tests;

public class ProjectRepositoryTests : IDisposable
{
    private readonly ProjectBankContext _context;
    private readonly ProjectRepository _repository;

    private ProjectCreateDTO TestProject;

    private User TestUser;
    private bool disposed;

    public ProjectRepositoryTests()
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
        TestUser = new Supervisor("Jan@itu.dk", ituInst, "Jan", "Jensen", new List<Project>(), ituFaculty, new List<Project>());

        var Tag1 = new Tag("Programming");
        var Tag2 = new Tag("Testing");

        context.Categories.AddRange(ituInst, ituFaculty);
        context.Users.AddRange(Supervisor1, TestUser);
        context.Tags.AddRange(Tag1, Tag2);
        
        context.Projects.AddRange(
            new Project{Id = 1, Title = "Best Project", Description = "Simply the best project to be a part of.", Status = ProjectStatus.PUBLIC, Category = ituInst, Tags = new List<Tag>(){Tag1}, Users = new List<User>(), Buckets = new List<ProjectBucket>(), Author = Supervisor1, MaxStudents = 5},
            new Project{Id = 2, Title = "Worst Project", Description = "Don't join this project.", Status = ProjectStatus.PUBLIC, Category = ituInst, Tags = new List<Tag>(){Tag2}, Users = new List<User>(), Buckets = new List<ProjectBucket>(), Author = Supervisor1, MaxStudents = 5},
            new Project{Id = 3, Title = "Deleted Project", Description = "It's deleted.", Status = ProjectStatus.DELETED, Category = ituInst, Tags = new List<Tag>(){Tag2}, Users = new List<User>(), Buckets = new List<ProjectBucket>(), Author = Supervisor1, MaxStudents = 5}
        
        );

        context.SaveChanges();
        _context = context;
        _repository = new ProjectRepository(_context);

        var TestInstitution = new Institution("ITU", "IT-Universitetet i København");
        var TestFaculty = new Faculty("Computer Science", "Computers and Science", TestInstitution);
        var TestSupervisor = new Supervisor("troe@itu.dk", TestInstitution, "Troels", "Jyde", new List<Project>(), TestFaculty, new List<Project>());
        TestProject = new ProjectCreateDTO{
            AuthorID = 1,
            Title = "Test Project",
            Description = "This is a test project",
            Status = ProjectStatus.PUBLIC,
            MaxStudents = 3,
            CategoryID = 1,
            TagIDs = new List<int>(){1},
            UserIDs = new List<int>(),
            BucketIDs = new List<int>()
        };
    }

     [Fact]
    public async void CreateAsync_adds_a_the_new_project_to_the_repository() 
    {
        var created = await _repository.CreateAsync(TestProject);

        if(created.Item1 == Response.Conflict) 
        {
            Assert.False(true);
        }

        var i = created.Item2;
        
        var option = await _repository.ReadByIDAsync(4);
        var created2 = option.Value;

        Assert.Equal(Response.Created, created.Item1);
        Assert.Equal(1, i.AuthorID);
        Assert.Equal(4, i.Id);
        Assert.Equal("Test Project", i.Title);
        Assert.Equal("This is a test project",i.Description);
        Assert.Equal(ProjectStatus.PUBLIC, i.Status);
        Assert.Equal(3, i.MaxStudents);
        Assert.Equal(1, i.CategoryID);
        Assert.Equal(new List<int>(){1}, i.TagIDs);
        Assert.Equal(new List<int>(), i.UserIDs);
        Assert.Equal(new List<int>(), i.BucketIDs);

        Assert.Equal(1, created2.AuthorID);
        Assert.Equal(4, created2.Id);
        Assert.Equal("Test Project", created2.Title);
        Assert.Equal("This is a test project",created2.Description);
        Assert.Equal(ProjectStatus.PUBLIC, created2.Status);
        Assert.Equal(3, created2.MaxStudents);
        Assert.Equal(1, created2.CategoryID);
        Assert.Equal(new List<int>(){1}, created2.TagIDs);
        Assert.Equal(new List<int>(), created2.UserIDs);
        Assert.Equal(new List<int>(), created2.BucketIDs);
    }

    [Fact]
    public async void CreateAsync_adding_exisisting_project_returns_Conflict() 
    {
        await _repository.CreateAsync(TestProject);
        var created = await _repository.CreateAsync(TestProject); 

        var i = created.Item2;

        Assert.Equal(Response.Conflict, created.Item1); 
    }
    
    [Fact]
    public async void ReadAllAsync_returns_all_projects()
    {
        var project1 = new ProjectDTO(1, 1, "Best Project", "Simply the best project to be a part of.", ProjectStatus.PUBLIC, 5, 2, new List<int>(){1}, new List<string>(){"Programming"}, new List<int>(), new List<int>());
        var project2 = new ProjectDTO(2, 1, "Worst Project", "Don't join this project.", ProjectStatus.PUBLIC, 5, 2, new List<int>(){2}, new List<string>(){"Testing"}, new List<int>(), new List<int>());
        var project3 = new ProjectDTO(3, 1, "Deleted Project", "It's deleted.", ProjectStatus.DELETED, 5, 2, new List<int>(){2}, new List<string>(){"Testing"}, new List<int>(), new List<int>());
        var projects = await _repository.ReadAllAsync();

        Assert.Collection(projects,
           project => 
           {
                Assert.Equal(project1.Id, project.Id);
                Assert.Equal(project1.AuthorID, project.AuthorID);
                Assert.Equal(project1.Title, project.Title);
                Assert.Equal(project1.Description, project.Description);
                Assert.Equal(project1.Status, project.Status);
                Assert.Equal(project1.MaxStudents, project.MaxStudents);
                Assert.Equal(project1.CategoryID, project.CategoryID);
                Assert.Equal(project1.TagIDs, project.TagIDs);
                Assert.Equal(project1.UserIDs, project.UserIDs);
                Assert.Equal(project1.BucketIDs, project.BucketIDs);
           },
           project => 
           {
                Assert.Equal(project2.Id, project.Id);
                Assert.Equal(project2.AuthorID, project.AuthorID);
                Assert.Equal(project2.Title, project.Title);
                Assert.Equal(project2.Description, project.Description);
                Assert.Equal(project2.Status, project.Status);
                Assert.Equal(project2.MaxStudents, project.MaxStudents);
                Assert.Equal(project2.CategoryID, project.CategoryID);
                Assert.Equal(project2.TagIDs, project.TagIDs);
                Assert.Equal(project2.UserIDs, project.UserIDs);
                Assert.Equal(project2.BucketIDs, project.BucketIDs);
           },
           project => 
           {
                Assert.Equal(project3.Id, project.Id);
                Assert.Equal(project3.AuthorID, project.AuthorID);
                Assert.Equal(project3.Title, project.Title);
                Assert.Equal(project3.Description, project.Description);
                Assert.Equal(project3.Status, project.Status);
                Assert.Equal(project3.MaxStudents, project.MaxStudents);
                Assert.Equal(project3.CategoryID, project.CategoryID);
                Assert.Equal(project3.TagIDs, project.TagIDs);
                Assert.Equal(project3.UserIDs, project.UserIDs);
                Assert.Equal(project3.BucketIDs, project.BucketIDs);
           }
        );
    }

    [Fact]
    public async void ReadAllAvailableAsync_returns_projects_with_correct_authorID_and_not_deleted()
    {
        var TestProjectDeleted = new ProjectCreateDTO{
            AuthorID = 1,
            Title = "Test Project Deleted",
            Description = "This is a test project",
            Status = ProjectStatus.DELETED,
            MaxStudents = 3,
            CategoryID = 1,
            TagIDs = new List<int>(){1},
            UserIDs = new List<int>(),
            BucketIDs = new List<int>()
        };

        var TestProjectAuthor = new ProjectCreateDTO{
            AuthorID = 2,
            Title = "Test Project Author",
            Description = "This is a test project",
            Status = ProjectStatus.DRAFT,
            MaxStudents = 3,
            CategoryID = 1,
            TagIDs = new List<int>(){1},
            UserIDs = new List<int>(),
            BucketIDs = new List<int>()
        };
        
        await _repository.CreateAsync(TestProjectDeleted);
        await _repository.CreateAsync(TestProjectAuthor);

        var project1 = new ProjectDTO(1, 1, "Best Project", "Simply the best project to be a part of.", ProjectStatus.PUBLIC, 5, 2, new List<int>(){1}, new List<string>(){"Programming"}, new List<int>(), new List<int>());
        var project2 = new ProjectDTO(2, 1, "Worst Project", "Don't join this project.", ProjectStatus.PUBLIC, 5, 2, new List<int>(){2}, new List<string>(){"Testing"}, new List<int>(), new List<int>());
        var projects = await _repository.ReadAllAvailableAsync(1);

        Assert.Collection(projects,
           project => 
           {
                Assert.Equal(project1.Id, project.Id);
                Assert.Equal(project1.AuthorID, project.AuthorID);
                Assert.Equal(project1.Title, project.Title);
                Assert.Equal(project1.Description, project.Description);
                Assert.Equal(project1.Status, project.Status);
                Assert.Equal(project1.MaxStudents, project.MaxStudents);
                Assert.Equal(project1.CategoryID, project.CategoryID);
                Assert.Equal(project1.TagIDs, project.TagIDs);
                Assert.Equal(project1.UserIDs, project.UserIDs);
                Assert.Equal(project1.BucketIDs, project.BucketIDs);
           },
           project => 
           {
                Assert.Equal(project2.Id, project.Id);
                Assert.Equal(project2.AuthorID, project.AuthorID);
                Assert.Equal(project2.Title, project.Title);
                Assert.Equal(project2.Description, project.Description);
                Assert.Equal(project2.Status, project.Status);
                Assert.Equal(project2.MaxStudents, project.MaxStudents);
                Assert.Equal(project2.CategoryID, project.CategoryID);
                Assert.Equal(project2.TagIDs, project.TagIDs);
                Assert.Equal(project2.UserIDs, project.UserIDs);
                Assert.Equal(project2.BucketIDs, project.BucketIDs);
           }
        );
    }

    [Fact]
    public async void ReadAllAvailableAsync_returns_empty_list_if_provided_authorID_does_not_exist()
    {
        var Emptylist = await _repository.ReadAllAvailableAsync(500);

        Assert.Equal(new List<ProjectDTO>(), Emptylist);
    }

    [Fact]
    public async void ReadFirstHundred_PrioritizeAuthored_returns_only_100_project()
    {
        for(int i = 0; i < 150; i++){
            var TestProject = new ProjectCreateDTO{
            AuthorID = 1,
            Title = "Test Project Deleted - " + i,
            Description = "This is a test project",
            Status = ProjectStatus.PUBLIC,
            MaxStudents = 3,
            CategoryID = 1,
            TagIDs = new List<int>(){1},
            UserIDs = new List<int>(),
            BucketIDs = new List<int>()
            };

            await _repository.CreateAsync(TestProject);

        }

        var TestProjectAuthor2 = new ProjectCreateDTO{
            AuthorID = 2,
            Title = "Test Project Author",
            Description = "This is a test project",
            Status = ProjectStatus.PUBLIC,
            MaxStudents = 3,
            CategoryID = 1,
            TagIDs = new List<int>(){1},
            UserIDs = new List<int>(),
            BucketIDs = new List<int>()
        };

        await _repository.CreateAsync(TestProjectAuthor2);
        

        var project1 = new ProjectDTO(1, 1, "Best Project", "Simply the best project to be a part of.", ProjectStatus.PUBLIC, 5, 2, new List<int>(){1}, new List<string>(){"Programming"}, new List<int>(), new List<int>());
        var project2 = new ProjectDTO(2, 1, "Worst Project", "Don't join this project.", ProjectStatus.PUBLIC, 5, 2, new List<int>(){2}, new List<string>(){"Testing"}, new List<int>(), new List<int>());
        var projects_ID_1 = await _repository.ReadFirstHundred_PrioritozeAuthored(1);
        var projects_ID_2 = await _repository.ReadFirstHundred_PrioritozeAuthored(2);
        var projects_ID_not_in_database = await _repository.ReadFirstHundred_PrioritozeAuthored(999);

        Assert.Equal(100, projects_ID_1.Count());
        Assert.Equal(100, projects_ID_2.Count());
        Assert.Equal(100, projects_ID_not_in_database.Count());
        Assert.Equal(TestProjectAuthor2.Description, projects_ID_2.ElementAt(0).Description);   
        Assert.Equal(TestProjectAuthor2.Status, projects_ID_2.ElementAt(0).Status);
        Assert.Equal(TestProjectAuthor2.MaxStudents, projects_ID_2.ElementAt(0).MaxStudents);
        Assert.Equal(TestProjectAuthor2.Title, projects_ID_2.ElementAt(0).Title);     
    }

   [Fact]
    public async void ReadAllAuthoredAsync_returns_all_projects_from_chosen_author()
    {
        var TestProjectAuthor = new ProjectCreateDTO{
            AuthorID = 2,
            Title = "Test Project",
            Description = "This is a test project",
            Status = ProjectStatus.PUBLIC,
            MaxStudents = 3,
            CategoryID = 1,
            TagIDs = new List<int>(){1},
            UserIDs = new List<int>(),
            BucketIDs = new List<int>()
        };
        
        await _repository.CreateAsync(TestProjectAuthor);

        var project1 = new ProjectDTO(1, 1, "Best Project", "Simply the best project to be a part of.", ProjectStatus.PUBLIC, 5, 2, new List<int>(){1}, new List<string>(){"Programming"}, new List<int>(), new List<int>());
        var project2 = new ProjectDTO(2, 1, "Worst Project", "Don't join this project.", ProjectStatus.PUBLIC, 5, 2, new List<int>(){2}, new List<string>(){"Testing"}, new List<int>(), new List<int>());
        var project3 = new ProjectDTO(3, 1, "Deleted Project", "It's deleted.", ProjectStatus.DELETED, 5, 2, new List<int>(){2}, new List<string>(){"Testing"}, new List<int>(), new List<int>());
        var project4 = new ProjectDTO(4, 2, "Test Project", "This is a test project", ProjectStatus.PUBLIC, 3, 1, new List<int>(){1}, new List<string>(){"Programming"}, new List<int>(), new List<int>());
        
        var projects = await _repository.ReadAllAuthoredAsync(1);

        Assert.Collection(projects,
           project => 
           {
                Assert.Equal(project1.Id, project.Id);
                Assert.Equal(project1.AuthorID, project.AuthorID);
                Assert.Equal(project1.Title, project.Title);
                Assert.Equal(project1.Description, project.Description);
                Assert.Equal(project1.Status, project.Status);
                Assert.Equal(project1.MaxStudents, project.MaxStudents);
                Assert.Equal(project1.CategoryID, project.CategoryID);
                Assert.Equal(project1.TagIDs, project.TagIDs);
                Assert.Equal(project1.UserIDs, project.UserIDs);
                Assert.Equal(project1.BucketIDs, project.BucketIDs);
           },
           project => 
           {
                Assert.Equal(project2.Id, project.Id);
                Assert.Equal(project2.AuthorID, project.AuthorID);
                Assert.Equal(project2.Title, project.Title);
                Assert.Equal(project2.Description, project.Description);
                Assert.Equal(project2.Status, project.Status);
                Assert.Equal(project2.MaxStudents, project.MaxStudents);
                Assert.Equal(project2.CategoryID, project.CategoryID);
                Assert.Equal(project2.TagIDs, project.TagIDs);
                Assert.Equal(project2.UserIDs, project.UserIDs);
                Assert.Equal(project2.BucketIDs, project.BucketIDs);
           },
           project => 
           {
                Assert.Equal(project3.Id, project.Id);
                Assert.Equal(project3.AuthorID, project.AuthorID);
                Assert.Equal(project3.Title, project.Title);
                Assert.Equal(project3.Description, project.Description);
                Assert.Equal(project3.Status, project.Status);
                Assert.Equal(project3.MaxStudents, project.MaxStudents);
                Assert.Equal(project3.CategoryID, project.CategoryID);
                Assert.Equal(project3.TagIDs, project.TagIDs);
                Assert.Equal(project3.UserIDs, project.UserIDs);
                Assert.Equal(project3.BucketIDs, project.BucketIDs);
           }
        );

        var projects2 = await _repository.ReadAllAuthoredAsync(2);

        Assert.Collection(projects2,
           project => 
           {
                Assert.Equal(project4.Id, project.Id);
                Assert.Equal(project4.AuthorID, project.AuthorID);
                Assert.Equal(project4.Title, project.Title);
                Assert.Equal(project4.Description, project.Description);
                Assert.Equal(project4.Status, project.Status);
                Assert.Equal(project4.MaxStudents, project.MaxStudents);
                Assert.Equal(project4.CategoryID, project.CategoryID);
                Assert.Equal(project4.TagIDs, project.TagIDs);
                Assert.Equal(project4.UserIDs, project.UserIDs);
                Assert.Equal(project4.BucketIDs, project.BucketIDs);
           }
        );
    }

    [Fact]
    public async void ReadAllAuthoredAsync_returns_empty_list_if_provided_authorID_does_not_exist()
    {
        var Emptylist = await _repository.ReadAllAuthoredAsync(500);

        Assert.Equal(new List<ProjectDTO>(), Emptylist);
    }

    [Fact]
    public async void ReadAllByTagAsync_returns_all_projects_with_specified_tag()
    {
        var option1 = await _repository.ReadByIDAsync(1);
        var project1 = option1.Value;
        
        var projects = await _repository.ReadAllByTagAsync(1);

        Assert.Collection(projects,
           project => 
           {
                Assert.Equal(project1.Id, project.Id);
                Assert.Equal(project1.AuthorID, project.AuthorID);
                Assert.Equal(project1.Title, project.Title);
                Assert.Equal(project1.Description, project.Description);
                Assert.Equal(project1.Status, project.Status);
                Assert.Equal(project1.MaxStudents, project.MaxStudents);
                Assert.Equal(project1.CategoryID, project.CategoryID);
                Assert.Equal(project1.TagIDs, project.TagIDs);
                Assert.Equal(project1.UserIDs, project.UserIDs);
                Assert.Equal(project1.BucketIDs, project.BucketIDs);
           }
        );
    }

    [Fact]
    public async void ReadAllByTagAsync_returns_empty_list_if_provided_tag_does_not_exist()
    {
        var Emptylist = await _repository.ReadAllByTagAsync(999);

        Assert.Equal(new List<ProjectDTO>(), Emptylist);
    }

    [Fact]
    public async void ReadAsync_provided_ID_exists_returns_Project()
    {
        var option = await _repository.ReadByIDAsync(2);
        var project = option.Value;

        Assert.Equal(2, project.Id);
        Assert.Equal(1, project.AuthorID);
        Assert.Equal("Worst Project", project.Title);
        Assert.Equal("Don't join this project.", project.Description);
        Assert.Equal(ProjectStatus.PUBLIC, project.Status);
        Assert.Equal(5, project.MaxStudents);
        Assert.Equal(2, project.CategoryID);
        Assert.Equal(new List<int>(){2}, project.TagIDs);
        Assert.Equal(new List<int>(), project.UserIDs);
        Assert.Equal(new List<int>(), project.BucketIDs);
    }

    [Fact]
    public async void ReadByIDAsync_provided_ID_does_not_exist_returns_Null()
    {
        var nonExisting = await _repository.ReadByIDAsync(1000);

        Assert.Equal(null, nonExisting);
    }

    [Fact]
    public async void ReadByKeyAsync_returns_project_with_specified_Title_and_authorID()
    {
        var project = await _repository.ReadByKeyAsync("Worst Project", 1);

        Assert.Equal(2, project.Id);
        Assert.Equal(1, project.AuthorID);
        Assert.Equal("Worst Project", project.Title);
        Assert.Equal("Don't join this project.", project.Description);
        Assert.Equal(ProjectStatus.PUBLIC, project.Status);
        Assert.Equal(5, project.MaxStudents);
        Assert.Equal(2, project.CategoryID);
        Assert.Equal(new List<int>(){2}, project.TagIDs);
        Assert.Equal(new List<int>(), project.UserIDs);
        Assert.Equal(new List<int>(), project.BucketIDs);
    }

    [Fact]
    public async void ReadByKeyAsync_returns_null_if_provided_Title_and_authorID_combination_does_not_exist()
    {
        var NonExistingTitle = await _repository.ReadByKeyAsync("Not a Project", 1);
        var NonExistingAuthor = await _repository.ReadByKeyAsync("Worst Project", 300);

        Assert.Null(NonExistingTitle);
        Assert.Null(NonExistingAuthor);
    }

    [Fact]
    public async void ReadCollectionAsync_returns_list_with_projects_from_provided_projectIDs()
    {
        var option1 = await _repository.ReadByIDAsync(1);
        var project1 = option1.Value;
        var option2 = await _repository.ReadByIDAsync(2);
        var project2 = option2.Value;

        var projects = await _repository.ReadCollectionAsync(new List<int>(){1,2});

        Assert.Collection(projects,
           project => 
           {
                Assert.Equal(project1.Id, project.Id);
                Assert.Equal(project1.AuthorID, project.AuthorID);
                Assert.Equal(project1.Title, project.Title);
                Assert.Equal(project1.Description, project.Description);
                Assert.Equal(project1.Status, project.Status);
                Assert.Equal(project1.MaxStudents, project.MaxStudents);
                Assert.Equal(project1.CategoryID, project.CategoryID);
                Assert.Equal(project1.TagIDs, project.TagIDs);
                Assert.Equal(project1.UserIDs, project.UserIDs);
                Assert.Equal(project1.BucketIDs, project.BucketIDs);
           },
           project => 
           {
                Assert.Equal(project2.Id, project.Id);
                Assert.Equal(project2.AuthorID, project.AuthorID);
                Assert.Equal(project2.Title, project.Title);
                Assert.Equal(project2.Description, project.Description);
                Assert.Equal(project2.Status, project.Status);
                Assert.Equal(project2.MaxStudents, project.MaxStudents);
                Assert.Equal(project2.CategoryID, project.CategoryID);
                Assert.Equal(project2.TagIDs, project.TagIDs);
                Assert.Equal(project2.UserIDs, project.UserIDs);
                Assert.Equal(project2.BucketIDs, project.BucketIDs);
           }
        );
    }

    [Fact]
    public async void ReadCollectionAsync_returns_emptylist_if_the_content_of_provided_list_of_IDs_does_not_exist()
    {
        var Emptylist = await _repository.ReadCollectionAsync(new List<int>(){998, 998});

        Assert.Equal(new List<ProjectDTO>(){}, Emptylist);
    }

    [Fact]
    public async void AddUserAsync_adds_specified_user_to_specified_project()
    {
        var Key = new ProjectKeyDTO(1, "Best Project");
        var response = await _repository.AddUserAsync(Key, 2);

        var option = await _repository.ReadByIDAsync(1);
        var created = option.Value;

        Assert.Equal(Response.Updated, response);
        Assert.Equal(new List<int>(){2}, created.UserIDs);
    }

    [Fact]
    public async void AddUserAsync_returns_NotFound_if_provided_userID_or_ProjectKey_does_not_exist()
    {
        var RealKey = new ProjectKeyDTO(1, "Best Project");
        var FakeKey = new ProjectKeyDTO(1, "Shitty Project");
        var ResponseFromNonExistingKey = await _repository.AddUserAsync(FakeKey, 2);
        var ResponseFromNonExistingUserID = await _repository.AddUserAsync(RealKey, 999);

        Assert.Equal(Response.NotFound, ResponseFromNonExistingKey);
        Assert.Equal(Response.NotFound, ResponseFromNonExistingUserID);
    }

    [Fact]
    public async void RemoveUserAsync_removes_specified_user_from_specified_project()
    {
        var Key = new ProjectKeyDTO(1, "Best Project");
        var AddResponse = await _repository.AddUserAsync(Key, 2);
        Assert.Equal(Response.Updated, AddResponse);
        
        var response = await _repository.RemoveUserAsync(Key, 2);
        var option = await _repository.ReadByIDAsync(1);
        var created = option.Value;

        Assert.Equal(Response.Updated, response);
        Assert.Equal(new List<int>(){}, created.UserIDs);

    }

    [Fact]
    public async void RemoveUserAsync_returns_NotFound_if_provided_userID_or_ProjectKey_does_not_exist()
    {
        var RealKey = new ProjectKeyDTO(1, "Best Project");
        var FakeKey = new ProjectKeyDTO(1, "Shitty Project");
        var ResponseFromNonExistingKey = await _repository.RemoveUserAsync(FakeKey, 2);
        var ResponseFromNonExistingUserID = await _repository.RemoveUserAsync(RealKey, 999);

        Assert.Equal(Response.NotFound, ResponseFromNonExistingKey);
        Assert.Equal(Response.NotFound, ResponseFromNonExistingUserID);
    }   

    [Fact]
    public async void UpdateAsync_updates_project_succesfully_given_that_provided_ProjectUpdateDTO_is_valid()
    {
        //Arrange
        var UpdatedProject = new ProjectUpdateDTO
        {
           Id = 1,
           AuthorID = 1,
           Title = "Best Updated Project",
           Description = "Simply the updated project",
           Status = ProjectStatus.PUBLIC,
           MaxStudents = 3,
           CategoryID = 2,
           TagIDs = new List<int>(){1},
           UserIDs = new List<int>(),
           BucketIDs = new List<int>()
        };
        
        //Act
        var updated = await _repository.UpdateAsync(1,UpdatedProject);
        var option = await _repository.ReadByIDAsync(1);
        var actual = option.Value;

        //Assert.Equal
        Assert.Equal(Response.Updated, updated);
        Assert.Equal("Best Updated Project", actual.Title);
        Assert.Equal("Simply the updated project", actual.Description);
        Assert.Equal(3, actual.MaxStudents);
    }

    [Fact]
    public async void UpdateAsync_returns_NotFound_if_project_does_not_exist()
    {
        var NonExistingID = new ProjectUpdateDTO{Id = 999, AuthorID = 1, Title = "Best Updated Project", Description = "Simply the updated project", Status = ProjectStatus.PUBLIC, MaxStudents = 3, CategoryID = 2, TagIDs = new List<int>(){1}, UserIDs = new List<int>(), BucketIDs = new List<int>()};

        var CreatedNonExistingID = await _repository.UpdateAsync(-1,NonExistingID);

        Assert.Equal(Response.NotFound, CreatedNonExistingID);
    } 

    [Fact]
    public async void UpdateAsync_returns_NotFound_if_provided_category_does_not_exist()
    {
        var NonExistingCategory = new ProjectUpdateDTO{Id = 1, AuthorID = 1, Title = "Best Updated Project", Description = "Simply the updated project", Status = ProjectStatus.PUBLIC, MaxStudents = 3, CategoryID = 999, TagIDs = new List<int>(){1}, UserIDs = new List<int>(), BucketIDs = new List<int>()};

        var CreatedNonExistingCategory = await _repository.UpdateAsync(-1,NonExistingCategory);

        Assert.Equal(Response.NotFound, CreatedNonExistingCategory);
    }

    [Fact]
    public async void DeleteAsync_returns_NotFound_if_provided_project_does_not_exists()
    {
        //Arrange
        var NonExistingID = new ProjectUpdateDTO{Id = 99, AuthorID = 1, Title = "Best Updated Project", Description = "Simply the updated project", Status = ProjectStatus.PUBLIC, MaxStudents = 3, CategoryID = 2, TagIDs = new List<int>(){1}, UserIDs = new List<int>(), BucketIDs = new List<int>()};
        
        //Act
        var response = await _repository.DeleteAsync(99, NonExistingID);

        //Assert
        Assert.Equal(Response.NotFound, response);
    }

    [Fact]
    public async void DeleteAsync_returns_NotFound_if_provided_category_does_not_exists()
    {
        //Arrange
        var NonExistingCategory = new ProjectUpdateDTO{Id = 1, AuthorID = 1, Title = "Best Updated Project", Description = "Simply the updated project", Status = ProjectStatus.PUBLIC, MaxStudents = 3, CategoryID = 999, TagIDs = new List<int>(){1}, UserIDs = new List<int>(), BucketIDs = new List<int>()};
        
        //Act
        var response = await _repository.DeleteAsync(1,NonExistingCategory);

        //Assert
        Assert.Equal(Response.NotFound, response);
    }

    [Fact]
    public async void DeleteAsync_returns_Conflict_if_provided_project_is_already_deleted_exists()
    {
        //Arrange
        var Deletedproject = new ProjectUpdateDTO{Id = 3, AuthorID = 1, Title = "Deleted Project", Description = "It's deleted.", Status = ProjectStatus.DELETED, MaxStudents = 5, CategoryID = 1, TagIDs = new List<int>(){1}, UserIDs = new List<int>(), BucketIDs = new List<int>()};
        
        //Act
        var response = await _repository.DeleteAsync(3, Deletedproject);
        
        //Assert
        Assert.Equal(Response.Conflict, response);
    }

    [Fact]
    public async void DeleteAsync_returns_Deleted_if_succes()
    {
        //Arrange
        var DeletedProject = new ProjectUpdateDTO
        {
           Id = 1,
           AuthorID = 1,
           Title = "Best Updated Project",
           Description = "Simply the updated project",
           Status = ProjectStatus.PUBLIC,
           MaxStudents = 3,
           CategoryID = 2,
           TagIDs = new List<int>(){1},
           UserIDs = new List<int>(),
           BucketIDs = new List<int>()
        };
        
        //Act
        var response = await _repository.DeleteAsync(1, DeletedProject);
        
        //Assert
        Assert.Equal(Response.Deleted, response);
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