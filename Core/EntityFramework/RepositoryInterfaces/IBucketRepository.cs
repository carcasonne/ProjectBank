namespace ProjectBank.Core.EF.Repository;

public interface IBucketRepository
{
    Task<(Response, BucketDTO)> CreateAsync(BucketCreateDTO course);
    Task<BucketDTO> ReadBucketByIDAsync(int bucketId);
    Task<BucketDTO> ReadBucketByKeyAsync(string key);
    Task<Response> AddProjectAsync(int bucketID, int projectID); 
    Task<Response> RemoveProjectAsync(int bucketID, int projectID);
    Task<Response> UpdateAllProjectAsync(int bucketID, ICollection<int> projectIDs);  
    Task<Response> ClearBucketAsync(int bucketID);
    Task<IReadOnlyCollection<BucketDTO>> ReadAllAsync();
}