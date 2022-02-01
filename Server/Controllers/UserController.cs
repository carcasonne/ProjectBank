namespace ProjectBank.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUserRepository _repository;

    public UsersController(ILogger<UsersController> logger, IUserRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IReadOnlyCollection<UserDTO>> Get()
        => await _repository.ReadAllAsync();

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> Get(int id)
        => (await _repository.ReadByID(id)).ToActionResult();

    [AllowAnonymous]
    [Route("Mail/{email}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult<UserDTO>> Get(string email)
        => (await _repository.ReadByEmail(email)).ToActionResult();

    [AllowAnonymous]
    [Route("Supervisors/{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult<UserDTO>> GetSupervisor(int id)
        => (await _repository.ReadSupervisor(id)).ToActionResult();

    [AllowAnonymous]
    [Route("Students/{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult<UserDTO>> GetStudent(int id)
        => (await _repository.ReadStudent(id)).ToActionResult();

    [Authorize]
    [Route("Students")]
    [ProducesResponseType(typeof(StudentDTO), 201)]
    [HttpPost]
    public async Task<IActionResult> Post(StudentCreateDTO student)
    {
        var response = await _repository.CreateAsync(student);
        var status = response.Item1;
        var created = response.Item2;

        if(status == Core.Response.NotFound)
        {
            return new NotFoundResult();
        }

        if(status == Core.Response.Conflict || status == Core.Response.BadRequest)
        {
            return new ConflictResult();
        }

        return CreatedAtAction(nameof(Get), created.Id, created);
    }

    [Authorize]
    [Route("Supervisors")]
    [ProducesResponseType(typeof(SupervisorDTO), 201)]
    [HttpPost]
    public async Task<IActionResult> Post(SupervisorCreateDTO supervisor)
    {
        var response = await _repository.CreateAsync(supervisor);
        var status = response.Item1;
        var created = response.Item2;

        if(status == Core.Response.NotFound)
        {
            return new NotFoundResult();
        }

        if(status == Core.Response.Conflict || status == Core.Response.BadRequest)
        {
            return new ConflictResult();
        }

        return CreatedAtAction(nameof(Get), created.Id, created);
    }
}