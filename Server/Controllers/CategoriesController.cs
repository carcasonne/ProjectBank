namespace ProjectBank.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class CategoriesController : ControllerBase
{
    private readonly ILogger<CategoriesController> _logger;
    private readonly ICategoryRepository _repository;

    public CategoriesController(ILogger<CategoriesController> logger, ICategoryRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IReadOnlyCollection<CategoryDTO>> Get()
        => await _repository.Read();

    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status200OK)]
    [HttpGet("{id}")] 
    public async Task<CategoryDTO> Get(int id)
        => await _repository.Read(id);
}