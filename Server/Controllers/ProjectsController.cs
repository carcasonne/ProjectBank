namespace ProjectBank.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class ProjectsController : ControllerBase
{
    private readonly ILogger<ProjectsController> _logger;
    private readonly IProjectRepository _repository;

    public ProjectsController(ILogger<ProjectsController> logger, IProjectRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [AllowAnonymous]
    [Route("Own/{id}")]
    [HttpGet]
    public async Task<IReadOnlyCollection<ProjectDTO>> GetOwn(int id)
        => await _repository.ReadAllAvailableAsync(id);

    [AllowAnonymous]
    [Route("Count/100/{id}")]
    [HttpGet]
    public async Task<IReadOnlyCollection<ProjectDTO>> GetFirstHundred(int id)
        => await _repository.ReadFirstHundred_PrioritozeAuthored(id);

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProjectDTO), StatusCodes.Status200OK)]
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDTO>> Get(int id)
        => (await _repository.ReadByIDAsync(id)).ToActionResult();

    [Authorize]
    [Route("Post")]
    [ProducesResponseType(typeof(ProjectDTO), 201)]
    [HttpPost]
    public async Task<IActionResult> Post(ProjectCreateDTO project)
    {
        var response = await _repository.CreateAsync(project);

        var created = response.Item2;
        var status = response.Item1;

        if(status == Core.Response.Conflict)
        {
            return new ConflictResult();
        }
        if(status == Core.Response.NotFound)
        {
            return new NotFoundResult();
        }

        return CreatedAtAction(nameof(Get), created, created);
    }

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] ProjectUpdateDTO project)
    {
        if(id != project.Id)
        {
            return BadRequest("Id mismatch");
        }

        var projectToReturn = await _repository.UpdateAsync(id, project);
        
        return projectToReturn.ToActionResult();
    }

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [Route("Delete/{id}")]
    [HttpPut]
    public async Task<IActionResult> Delete(int id, ProjectUpdateDTO project)
    {
        if(id != project.Id)
        {
            return BadRequest("Id mismatch");
        }

        var projectToReturn = await _repository.DeleteAsync(id, project); 
        
        return projectToReturn.ToActionResult();
        
    }
}