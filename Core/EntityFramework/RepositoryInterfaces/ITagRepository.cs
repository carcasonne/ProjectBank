namespace ProjectBank.Core.EF.Repository;

public interface ITagRepository
{
    Task<(Response, TagDTO)> CreateAsync(TagCreateDTO program);
    Task<Option<TagDTO>> ReadTagByIDAsync(int TagID);

    Task<TagDTO> ReadTagByNameAsync(string TagName);
    Task<IReadOnlyCollection<TagDTO>> ReadAllAsync();

    Task<ICollection<TagDTO>> ReadCollectionAsync(ICollection<int> tagIDs);
}