namespace ProjectBank.Core.EF.Repository;

public interface ICourseRepository 
{
    Task<(Response, CourseDTO)> CreateAsync(CourseCreateDTO course);
    Task<CourseDTO> ReadCourseByIDAsync(int courseID);
    Task<IReadOnlyCollection<CourseDTO>> ReadAllAsync();
}