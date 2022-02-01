namespace ProjectBank.Core.EF.Repository;

public interface IProjectRepository 
{
    Task<(Response, ProjectDTO)> CreateAsync(ProjectCreateDTO project);
    Task<Option<ProjectDTO>> ReadByIDAsync(int projectID);
    Task<ProjectDTO> ReadByKeyAsync(string ProjectTitle, int authorID);
    Task<IReadOnlyCollection<ProjectDTO>> ReadAllAsync();
    Task<IReadOnlyCollection<ProjectDTO>> ReadAllAvailableAsync(int author);
    Task<IReadOnlyCollection<ProjectDTO>> ReadFirstHundred_PrioritozeAuthored(int author);
    Task<IReadOnlyCollection<ProjectDTO>> ReadAllAuthoredAsync(int authorID);
    Task<IReadOnlyCollection<ProjectDTO>> ReadAllByTagAsync(int tagID);
    Task<IReadOnlyCollection<ProjectDTO>> ReadCollectionAsync(ICollection<int> projectIDs);
    Task<Response> UpdateAsync(int id, ProjectUpdateDTO project);
    Task<Response> DeleteAsync(int id, ProjectUpdateDTO project);
    Task<Response> AddUserAsync(ProjectKeyDTO projectKey, int userID);
    Task<Response> RemoveUserAsync(ProjectKeyDTO projectKey, int userID);
}