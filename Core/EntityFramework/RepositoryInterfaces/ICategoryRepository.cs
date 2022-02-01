namespace ProjectBank.Core.EF.Repository;

public interface ICategoryRepository
{
    Task<CategoryDTO> Read(int id);
    Task<IReadOnlyCollection<CategoryDTO>> Read();
}