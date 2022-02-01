using ProjectBank.Infrastructure.ReferenceSystem;

namespace ProjectBank.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]

    public class ProjectReferenceController : ControllerBase
    {
        private readonly ILogger<ProjectReferenceController> _logger;

        private ProjectRepository _projectRepository;



        public ProjectReferenceController(ILogger<ProjectReferenceController> logger, IProjectRepository projectRepository)
        {
            _logger = logger;
            if (ProjectReferenceData._LSH == null)
            {
                ProjectReferenceData._LSH = new ProjectLSH(projectRepository);
                Task.Run(() => ProjectReferenceData._LSH.InsertAll()).Wait();
            }
            _projectRepository = (ProjectRepository) projectRepository;
        }

        [Authorize]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IReadOnlyCollection<ProjectReferenceDTO>), StatusCodes.Status200OK)]
        [HttpGet("{projectID},{size}")]
        public async Task<IReadOnlyCollection<ProjectReferenceDTO>> GetSortedInCategory(int projectID, int size)
        {
            var project = await _projectRepository.ReadProjectReferenceAsync(projectID);
            return await ProjectReferenceData._LSH.GetSortedInCategory(project, size);
        }

        [Authorize]
        [Route("Delete/{projectID}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPut]
        public async Task<Response> Delete(int projectID)
        {
            var project = await _projectRepository.ReadProjectReferenceAsync(projectID);
            return ProjectReferenceData._LSH.Delete(project);
        }

        [Authorize]
        [Route("Sorted/{projectID},{size}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IReadOnlyCollection<ProjectReferenceDTO>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IReadOnlyCollection<ProjectReferenceDTO>> GetSorted(int projectID, int size)
        {
            var project = await _projectRepository.ReadProjectReferenceAsync(projectID);
            return await ProjectReferenceData._LSH.GetSorted(project, size);
        }
    }
}