/* Testing code greatly 'inspired' by Rasmus Lystrøm
*  @ https://github.com/ondfisk/BDSA2021/blob/main/MyApp.Server.Tests/Controllers/CharactersControllerTests.cs
*/

namespace ProjectBank.Tests.Controllers.Tests;

public class UsersControllers
{
    [Fact]
    public async Task Get_given_existing_returns_given()
    {
        // Arrange
        var logger = new Mock<ILogger<UsersController>>();
        var repository = new Mock<IUserRepository>();

        var user = new StudentDTO(1, "jens@gmail.com", "Jens", "Jensen", "SWU2020", "ITU", new List<int>(), new List<int>());
        repository.Setup(m => m.ReadByID(1)).ReturnsAsync(user);

        var controller = new UsersController(logger.Object, repository.Object);

        // Act
        var response = await controller.Get(1);

        // Assert
        Assert.Equal(user, response.Value);
    }

    [Fact]
    public async Task Get_given_non_existing_returns_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<UsersController>>();
        var repository = new Mock<IUserRepository>();
        var controller = new UsersController(logger.Object, repository.Object);

        // Act
        var response = await controller.Get(100);

        // Assert
        Assert.IsType<NotFoundResult>(response.Result);
    }

    [Fact]
    public async Task Get_given_supervisor_non_existing_returns_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<UsersController>>();
        var repository = new Mock<IUserRepository>();
        var controller = new UsersController(logger.Object, repository.Object);

        var user = new StudentDTO(1, "jens@gmail.com", "Jens", "Jensen", "SWU2020", "ITU", new List<int>(), new List<int>());
        repository.Setup(m => m.ReadByID(1)).ReturnsAsync(user);

        // Act
        var response = await controller.GetSupervisor(1);

        // Assert
        Assert.IsType<NotFoundResult>(response.Result);
    }

    [Fact]
    public async Task Get_given_student_non_existing_returns_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<UsersController>>();
        var repository = new Mock<IUserRepository>();
        var controller = new UsersController(logger.Object, repository.Object);

        var user = new SupervisorDTO(1, "jens@gmail.com", "Jens", "Jensen", "Computer Sciecne", "ITU", new List<int>(), new List<int>());
        repository.Setup(m => m.ReadByID(1)).ReturnsAsync(user);

        // Act
        var response = await controller.GetStudent(1);

        // Assert
        Assert.IsType<NotFoundResult>(response.Result);
    }

    [Fact]
    public async Task Get_given_student_existing_returns_given()
    {
        // Arrange
        var logger = new Mock<ILogger<UsersController>>();
        var repository = new Mock<IUserRepository>();
        var controller = new UsersController(logger.Object, repository.Object);

        var user = new StudentDTO(1, "jens@gmail.com", "Jens", "Jensen", "SWU2020", "ITU", new List<int>(), new List<int>());
        repository.Setup(m => m.ReadStudent(1)).ReturnsAsync(user);

        // Act
        var response = await controller.GetStudent(1);

        // Assert
        Assert.Equal(user, response.Value);
    }

    [Fact]
    public async Task Get_given_Supervisor_existing_returns_given()
    {
        // Arrange
        var logger = new Mock<ILogger<UsersController>>();
        var repository = new Mock<IUserRepository>();
        var controller = new UsersController(logger.Object, repository.Object);

        var user = new SupervisorDTO(1, "jens@gmail.com", "Jens", "Jensen", "Computer Science", "ITU", new List<int>(), new List<int>());
        repository.Setup(m => m.ReadSupervisor(1)).ReturnsAsync(user);

        // Act
        var response = await controller.GetSupervisor(1);

        // Assert
        Assert.Equal(user, response.Value);
    }

    [Fact]
    public async Task Get_given_email_returns_given()
    {
        // Arrange
        var logger = new Mock<ILogger<UsersController>>();
        var repository = new Mock<IUserRepository>();

        var user = new StudentDTO(1, "jens@gmail.com", "Jens", "Jensen", "SWU2020", "ITU", new List<int>(), new List<int>());
        repository.Setup(m => m.ReadByEmail("jens@gmail.com")).ReturnsAsync(user);

        var controller = new UsersController(logger.Object, repository.Object);

        // Act
        var response = await controller.Get("jens@gmail.com");

        // Assert
        Assert.Equal(user, response.Value);
    }

    [Fact]
    public async Task Get_given_email_non_existing_returns_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<UsersController>>();
        var repository = new Mock<IUserRepository>();

        var user = new StudentDTO(1, "jens@gmail.com", "Jens", "Jensen", "SWU2020", "ITU", new List<int>(), new List<int>());
        repository.Setup(m => m.ReadByEmail("jens@gmail.com")).ReturnsAsync(user);

        var controller = new UsersController(logger.Object, repository.Object);

        // Act
        var response = await controller.Get("bo@gmail.com");

        // Assert
        Assert.IsType<NotFoundResult>(response.Result);
    }

    [Fact]
    public async Task Post_Adds_Student()
    {
        // Arrange
        var logger = new Mock<ILogger<UsersController>>();
        var repository = new Mock<IUserRepository>();

        var toCreate = new StudentCreateDTO();
        var created = new StudentDTO(1, "jens@gmail.com", "Jens", "Jensen", "SWU2020", "ITU", new List<int>{}, new List<int>{});
        
        repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync((Response.Created, created));
        var controller = new UsersController(logger.Object, repository.Object);

        // Act
        var result = await controller.Post(toCreate) as CreatedAtActionResult;

        // Assert
        Assert.Equal(created, result?.Value);
        Assert.Equal("Get", result?.ActionName);
    }

    [Fact]
    public async Task Post_Existing_Student_returns_Conflict()
    {
        // Arrange
        var logger = new Mock<ILogger<UsersController>>();
        var repository = new Mock<IUserRepository>();

        var toCreate = new StudentCreateDTO();
        var created = new StudentDTO(1, "jens@gmail.com", "Jens", "Jensen", "SWU2020", "ITU", new List<int>{}, new List<int>{});
        
        repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync((Response.Conflict, created));
        var controller = new UsersController(logger.Object, repository.Object);

        // Act
        var result = await controller.Post(toCreate);

        // Assert
        Assert.IsType<ConflictResult>(result);
    }

    [Fact]
    public async Task Post_Student_Institution_non_existing_returns_Conflict()
    {
        // Arrange
        var logger = new Mock<ILogger<UsersController>>();
        var repository = new Mock<IUserRepository>();

        var toCreate = new StudentCreateDTO();
        var created = new StudentDTO(1, "jens@gmail.com", "Jens", "Jensen", "SWU2020", "ITU", new List<int>{}, new List<int>{});
        
        repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync((Response.NotFound, created));
        var controller = new UsersController(logger.Object, repository.Object);

        // Act
        var result = await controller.Post(toCreate);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Post_Adds_Supervisor()
    {
        // Arrange
        var logger = new Mock<ILogger<UsersController>>();
        var repository = new Mock<IUserRepository>();

        var toCreate = new SupervisorCreateDTO();
        var created = new SupervisorDTO(1, "jens@gmail.com", "Jens", "Jensen", "Computer Science", "ITU", new List<int>{}, new List<int>{});
        
        repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync((Response.Created, created));
        var controller = new UsersController(logger.Object, repository.Object);

        // Act
        var result = await controller.Post(toCreate) as CreatedAtActionResult;

        // Assert
        Assert.Equal(created, result?.Value);
        Assert.Equal("Get", result?.ActionName);
    }

    [Fact]
    public async Task Post_Existing_Supervisor_returns_Conflict()
    {
        // Arrange
        var logger = new Mock<ILogger<UsersController>>();
        var repository = new Mock<IUserRepository>();

        var toCreate = new SupervisorCreateDTO();
        var created = new SupervisorDTO(1, "jens@gmail.com", "Jens", "Jensen", "Computer Science", "ITU", new List<int>{}, new List<int>{});
        
        repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync((Response.Conflict, created));
        var controller = new UsersController(logger.Object, repository.Object);

        // Act
        var result = await controller.Post(toCreate);

        // Assert
        Assert.IsType<ConflictResult>(result);
    }

    [Fact]
    public async Task Post_Supervisor_Institution_non_existing_returns_Conflict()
    {
        // Arrange
        var logger = new Mock<ILogger<UsersController>>();
        var repository = new Mock<IUserRepository>();

        var toCreate = new SupervisorCreateDTO();
        var created = new SupervisorDTO(1, "jens@gmail.com", "Jens", "Jensen", "Computer Science", "ITU", new List<int>{}, new List<int>{});
        
        repository.Setup(m => m.CreateAsync(toCreate)).ReturnsAsync((Response.NotFound, created));
        var controller = new UsersController(logger.Object, repository.Object);

        // Act
        var result = await controller.Post(toCreate);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}