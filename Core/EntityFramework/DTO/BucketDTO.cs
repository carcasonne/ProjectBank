public class BucketDTO
{
    public int Id { get; init; }
    public ISet<int> ProjectIds { get; set; }

    [Required]
    public string Key { get; set; }

    public BucketDTO(ISet<int> ProjectIds, string Key)
    {
        this.ProjectIds = ProjectIds;
        this.Key = Key;
    }

    public BucketDTO(ISet<int> ProjectIds, string Key, int Id)
    {
        this.ProjectIds = ProjectIds;
        this.Key = Key;
        this.Id = Id;
    }
}
public record BucketCreateDTO()
{
    public ISet<int> ProjectIds { get; init; }

    [Required]
    public string Key { get; init; }
}