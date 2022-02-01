/* Testing code greatly 'inspired' by Rasmus Lystrøm
*  @ https://github.com/ondfisk/BDSA2021/blob/main/MyApp.Server.Tests/Controllers/CharactersControllerTests.cs
*/

namespace ProjectBank.Tests.Controllers.Tests;

public class ProjectControllersTests
{
    [Fact]
    public async Task Get_given_non_existing_returns_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<ProjectsController>>();
        var repository = new Mock<IProjectRepository>();
        repository.Setup(m => m.ReadByIDAsync(42)).ReturnsAsync(default(ProjectDTO));
        var controller = new ProjectsController(logger.Object, repository.Object);

        // Act
        var response = await controller.Get(42);

        // Assert
        Assert.IsType<NotFoundResult>(response.Result);
    }

    [Fact]
    public async Task Get_given_existing_returns_given()
    {
        // Arrange
        var logger = new Mock<ILogger<ProjectsController>>();
        var repository = new Mock<IProjectRepository>();
        var project = new ProjectDTO(1, 1, "Project: ", "Description", ProjectStatus.PUBLIC, 3, 1, new List<int>{1}, new List<string>(){""}, new List<int>{2}, new List<int>{1, 2, 3});
        repository.Setup(m => m.ReadByIDAsync(42)).ReturnsAsync(project);
        var controller = new ProjectsController(logger.Object, repository.Object);

        // Act
        var response = await controller.Get(42);

        // Assert
        Assert.Equal(project, response.Value);
    }

    [Fact]
    public async Task GetOwn_returns_public_Projects()
    {
        // Arrange
        var logger = new Mock<ILogger<ProjectsController>>();
        var repository = new Mock<IProjectRepository>();
        var expected = Array.Empty<ProjectDTO>();
        repository.Setup(m => m.ReadAllAvailableAsync(1)).ReturnsAsync(expected);
        var controller = new ProjectsController(logger.Object, repository.Object);

        // Act
        var actual = await controller.GetOwn(1);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Post_Adds_Project()
    {
        // Arrange
        var logger = new Mock<ILogger<ProjectsController>>();
        var repository = new Mock<IProjectRepository>();
        var toCreate = new ProjectCreateDTO();
        var created = new ProjectDTO(1, 1, "Project: ", "Description", ProjectStatus.PUBLIC, 3, 1, new List<int>{1}, new List<string>(){""}, new List<int>{2}, new List<int>{1, 2, 3});
        repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync((Response.Created, created));
        var controller = new ProjectsController(logger.Object, repository.Object);

        // Act
        var result = await controller.Post(toCreate) as CreatedAtActionResult;

        // Assert
        Assert.Equal(created, result?.Value);
        Assert.Equal("Get", result?.ActionName);
    }

    [Fact]
    public async Task Post_Existing_Project_Returns_Conflict()
    {
        // Arrange
        var logger = new Mock<ILogger<ProjectsController>>();
        var repository = new Mock<IProjectRepository>();
        var existing = new ProjectCreateDTO();
        var created = new ProjectDTO(1, 1, "Project: ", "Description", ProjectStatus.PUBLIC, 3, 1, new List<int>{1}, new List<string>(){""}, new List<int>{2}, new List<int>{1, 2, 3});
        repository.Setup(m => m.CreateAsync(existing)).ReturnsAsync((Response.Conflict, created));
        var controller = new ProjectsController(logger.Object, repository.Object);

        // Act
        var response = await controller.Post(existing);

        // Assert
        Assert.IsType<ConflictResult>(response);
    }

    [Fact]
    public async Task Post_Project_With_Author_non_existing_Returns_Conflict()
    {
        // Arrange
        var logger = new Mock<ILogger<ProjectsController>>();
        var repository = new Mock<IProjectRepository>();
        var existing = new ProjectCreateDTO();
        var created = new ProjectDTO(1, -10, "Project: ", "Description", ProjectStatus.PUBLIC, 3, 1, new List<int>{1}, new List<string>(){""}, new List<int>{2}, new List<int>{1, 2, 3});
        repository.Setup(m => m.CreateAsync(existing)).ReturnsAsync((Response.NotFound, created));
        var controller = new ProjectsController(logger.Object, repository.Object);

        // Act
        var response = await controller.Post(existing);

        // Assert
        Assert.IsType<NotFoundResult>(response);
    }

    [Fact]
    public async Task Get_First_Hundred_returns_Max_Hundred_Projects()
    {
        // Arrange
        var logger = new Mock<ILogger<ProjectsController>>();
        var repository = new Mock<IProjectRepository>();
        var projects = new List<ProjectDTO>();
        for (int i = 0; i < 100; i++)
        {
            projects.Add(default(ProjectDTO));
        }

        repository.Setup(m => m.ReadFirstHundred_PrioritozeAuthored(1)).ReturnsAsync(projects);
        var controller = new ProjectsController(logger.Object, repository.Object);

        // Act
        var response = await controller.GetFirstHundred(1);

        // Assert
        Assert.Equal(100, response.Count());
    }

    [Fact]
    public async Task Put_updates_Project()
    {
        // Arrange
        var logger = new Mock<ILogger<ProjectsController>>();
        var project = new ProjectUpdateDTO{Id = 1};
        var repository = new Mock<IProjectRepository>();
        repository.Setup(m => m.UpdateAsync(1, project)).ReturnsAsync(Response.Updated);
        var controller = new ProjectsController(logger.Object, repository.Object);

        // Act
        var response = await controller.Put(1, project);

        // Assert
        Assert.IsType<OkResult>(response);
    }
    [Fact]
    public async Task Put_given_unknown_id_returns_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<ProjectsController>>();
        var project = new ProjectUpdateDTO { Id = 2 };
        var repository = new Mock<IProjectRepository>();
        repository.Setup(m => m.UpdateAsync(2, project)).ReturnsAsync(Response.NotFound);
        var controller = new ProjectsController(logger.Object, repository.Object);

        // Act
        var response = await controller.Put(2, project);

        // Assert
        Assert.IsType<NotFoundResult>(response);
    }

    [Fact]
    public async Task Delete_deletes_project()
    {
        // Arrange
        var logger = new Mock<ILogger<ProjectsController>>();
        var project = new ProjectUpdateDTO { Id = 1 };
        var repository = new Mock<IProjectRepository>();
        repository.Setup(m => m.DeleteAsync(1, project)).ReturnsAsync(Response.Deleted);
        var controller = new ProjectsController(logger.Object, repository.Object);

        // Act
        var response = await controller.Delete(1, project);

        // Assert
        Assert.IsType<NoContentResult>(response);
    }

    [Fact]
    public async Task Delete_given_already_deleted_project_returns_conflict()
    { 
        // Arrange
        var logger = new Mock<ILogger<ProjectsController>>();
        var project = new ProjectUpdateDTO { Id = 1 };
        var repository = new Mock<IProjectRepository>();
        repository.Setup(m => m.DeleteAsync(1, project)).ReturnsAsync(Response.Conflict);
        var controller = new ProjectsController(logger.Object, repository.Object);

        // Act
        var response = await controller.Delete(1, project);

        // Assert
        Assert.IsType<ConflictResult>(response);
    }
}