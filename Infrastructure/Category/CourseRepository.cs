namespace ProjectBank.Infrastructure;

public class CourseRepository : ICourseRepository
{
    private readonly ProjectBankContext _dbcontext;

    public CourseRepository(ProjectBankContext context)
    {
        _dbcontext = context;
    }

    public async Task<(Response, CourseDTO)> CreateAsync(CourseCreateDTO course)
    {

        var conflict =
                await _dbcontext.Courses
                .Where(c => c.Faculty.Title == course.FacultyName)
                .Where(c => c.Code == course.Code)
                .Select(c => new CourseDTO(c.Id, c.Title, c.Description, c.Faculty.Title, c.Code, c.Programs.Select(p => p.Code), c.Students.Select(s => s.Email)))
                .FirstOrDefaultAsync();

        if (conflict != null)
        {
            return (Response.Conflict, conflict);
        }

        //find related faculty
        var EntityFaculty =
            await _dbcontext.Faculties
                            .Where(f => f.Title == course.FacultyName)
                            .Where(f => f.Institution.Title == course.InstitutionName)
                            .Select(f => f)
                            .FirstOrDefaultAsync();

        //if the faculty doesn't exist, this course cant exist
        if(EntityFaculty == null) 
        {
            return (Response.NotFound, new CourseDTO(-1, course.Title, course.Description, course.FacultyName, course.Code, course.ProgramCodes, course.StudentEmails));
        }

        var ProgramList = await GetProgramsAsync(course.ProgramCodes, course.FacultyName).ToListAsync();
        var StudentList = await GetStudentsAsync(course.StudentEmails).ToListAsync();

        var entity = new Course
        {
            Title = course.Title,
            Description = course.Description,
            Faculty = EntityFaculty,
            Code = course.Code,
            Programs = ProgramList,
            Students = StudentList
        };

        _dbcontext.Courses.Add(entity);

        await _dbcontext.SaveChangesAsync();

        return (Response.Created, new CourseDTO(entity.Id, entity.Title, entity.Description, entity.Faculty.Title, entity.Code,
                                                ProgramList.Select(p => p.Code), StudentList.Select(s => s.Email)));
    }
    public async Task<CourseDTO> ReadCourseByIDAsync(int CourseID)
    {
        var courses = from c in _dbcontext.Courses
                        where c.Id == CourseID
                        select new CourseDTO(c.Id, c.Title, c.Description, c.Faculty.Title, c.Code, c.Programs.Select(p => p.Code), c.Students.Select(s => s.Email));

        return await courses.FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<CourseDTO>> ReadAllAsync() =>
        (await _dbcontext.Courses
                        .Select(c => new CourseDTO(c.Id, c.Title, c.Description, c.Faculty.Title, c.Code, c.Programs.Select(p => p.Code), c.Students.Select(s => s.Email)))
                        .ToListAsync())
                        .AsReadOnly();

    //used to get existing Programs based on prgoram codes and FacultyName given in DTO
    private async IAsyncEnumerable<TeachingProgram> GetProgramsAsync(ICollection<string> inPrograms, string FacultyName) 
    {
        //code is unique for each institution
        var existing = await _dbcontext.Programs
                        .Where(p => inPrograms
                                    .Any(inP => inP == p.Code))
                        .Where(p => p.Faculty.Title == FacultyName)
                        .Select(p => p)
                        .ToListAsync();

        foreach (var program in existing)
        {
            yield return program;
        }
    }

        private async IAsyncEnumerable<Student> GetStudentsAsync(ICollection<string> inStudents) 
    {
        var existing = await _dbcontext.Users.OfType<Student>()
                        .Where(s => inStudents
                                    .Any(inS => inS == s.Email))
                        .Select(s => s)
                        .ToListAsync();

        foreach (var student in existing)
        {
            yield return student;
        }
    }
}