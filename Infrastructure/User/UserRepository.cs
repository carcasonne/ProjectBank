namespace ProjectBank.Infrastructure;

public class UserRepository : IUserRepository
{
    private readonly ProjectBankContext _dbcontext;

    public UserRepository(ProjectBankContext context)
    {
        _dbcontext = context;
    }

    public async Task<(Response, StudentDTO)> CreateAsync(StudentCreateDTO user)
    {
        var conflict = await _dbcontext.Users.OfType<Student>()
                        .Where(u => u.Email == user.Email)
                        .Select(u => new StudentDTO(u.Id, u.Email, u.FirstName, u.LastName, u.Program.Code, u.Program.Faculty.Institution.Title, 
                                                    u.Projects.Select(p => p.Id).ToList(), u.Courses.Select(c => c.Id).ToList()))
                        .FirstOrDefaultAsync();

        if (conflict != null)
        {
            return (Response.Conflict, conflict);
        }

        var institution = await _dbcontext.Institutions
                              .Where(i => i.Title == user.InstitutionName)
                              .Select(i => i)
                              .FirstOrDefaultAsync();

        var program = await _dbcontext.Programs
                              .Where(p => p.Faculty.Institution == institution)
                              .Where(p => p.Code == user.ProgramCode)
                              .Select(p => p)
                              .FirstOrDefaultAsync();

        //institution or program doesn't exists
        if(institution == null || program == null)
        {
            return (Response.NotFound, new StudentDTO(-1, user.Email, user.FirstName, user.LastName, user.ProgramCode, user.InstitutionName, user.ProjectIDs, user.CourseIDs));
        }

        var entity = new Student
        {
            Email = user.Email,
            Institution = institution,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Projects = await GetProjectsAsync(user.ProjectIDs).ToListAsync(),
            Courses = await GetCoursesAsync(user.CourseIDs).ToListAsync(),
            Program = program
        };

        _dbcontext.Users.Add(entity);

        await _dbcontext.SaveChangesAsync();

        return (Response.Created, new StudentDTO(entity.Id, entity.Email, entity.FirstName, entity.LastName, entity.Program.Code, 
                                                 entity.Institution.Title, entity.Projects.Select(p => p.Id).ToList(), entity.Courses.Select(c => c.Id).ToList()));
    }

    public async Task<(Response, SupervisorDTO)> CreateAsync(SupervisorCreateDTO user)
    {
        var conflict = await _dbcontext.Users.OfType<Supervisor>()
                        .Where(u => u.Email == user.Email)
                        .Select(u => new SupervisorDTO(u.Id, u.Email, u.FirstName, u.LastName, u.Faculty.Title, u.Institution.Title, 
                                                       u.Projects.Select(p => p.Id).ToList(), u.AuthoredProjects.Select(p => p.Id).ToList()))
                        .FirstOrDefaultAsync();

        if (conflict != null)
        {
            return (Response.Conflict, conflict);
        }

        var institution = await _dbcontext.Institutions
                              .Where(i => i.Title == user.InstitutionName)
                              .Select(i => i)
                              .FirstOrDefaultAsync();

        var faculty = await _dbcontext.Faculties
                              .Where(f => f.Institution.Id == institution.Id)
                              .Where(f => f.Title == user.FacultyName)
                              .Select(f => f)
                              .FirstOrDefaultAsync();

        //institution or faculty doesn't exists
        if(institution == null || faculty == null)
        {
            return (Response.NotFound, new SupervisorDTO(-1, user.Email, user.FirstName, user.LastName, user.FacultyName, user.InstitutionName, user.ProjectIDs, user.AuthoredProjectIDs));
        }

        var entity = new Supervisor
        {
            Email = user.Email,
            Institution = institution,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Projects = await GetProjectsAsync(user.ProjectIDs).ToListAsync(),
            AuthoredProjects = await GetProjectsAsync(user.AuthoredProjectIDs).ToListAsync(),
            Faculty = faculty
        };

        _dbcontext.Users.Add(entity);

        await _dbcontext.SaveChangesAsync();

        return (Response.Created, new SupervisorDTO(entity.Id, entity.Email, entity.FirstName, entity.LastName, entity.Faculty.Title, 
                                                    entity.Institution.Title, entity.Projects.Select(p => p.Id).ToList(), entity.AuthoredProjects.Select(p => p.Id).ToList()));
    }

    public async Task<Option<UserDTO>> ReadSupervisor(int userID)
    {
        var users = from u in _dbcontext.Users.OfType<Supervisor>()
                           where u.Id == userID
                           select new UserDTO(u.Id, u.Email, u.FirstName, u.LastName, u.Projects.Select(p => p.Id).ToList());

        return await users.FirstOrDefaultAsync(); 
    }

    public async Task<Option<UserDTO>> ReadStudent(int userID)
    {
        var users = from u in _dbcontext.Users.OfType<Student>()
                           where u.Id == userID
                           select new UserDTO(u.Id, u.Email, u.FirstName, u.LastName, u.Projects.Select(p => p.Id).ToList());

        return await users.FirstOrDefaultAsync(); 
    }

    public async Task<Option<UserDTO>> ReadByID(int userID)
    {
        var users = from u in _dbcontext.Users
                    where u.Id == userID
                    select new UserDTO(u.Id, u.Email, u.FirstName, u.LastName, u.Projects.Select(p => p.Id).ToList());

        return await users.FirstOrDefaultAsync(); 
    }

    public async Task<Option<UserDTO>> ReadByEmail(string Email)
    {
        var users = from u in _dbcontext.Users
                           where u.Email == Email
                           select new UserDTO(u.Id, u.Email, u.FirstName, u.LastName, u.Projects.Select(p => p.Id).ToList());

        return await users.FirstOrDefaultAsync(); 
    }

    public async Task<IReadOnlyCollection<UserDTO>> ReadAllAsync()
    {
        return (await _dbcontext.Users
                        .Select(u => new UserDTO(u.Id, u.Email, u.FirstName, u.LastName, u.Projects.Select(p => p.Id).ToList()))
                        .ToListAsync())
                        .AsReadOnly();
    }

    public async Task<IReadOnlyCollection<StudentDTO>> ReadAllStudentsAsync()
    {
        return (await _dbcontext.Users.OfType<Student>()
                        .Select(u => new StudentDTO(u.Id, u.Email, u.FirstName, u.LastName, u.Program.Code, u.Institution.Title, u.Projects.Select(p => p.Id).ToList(), u.Courses.Select(c => c.Id).ToList()))
                        .ToListAsync())
                        .AsReadOnly();
    }

    public async Task<IReadOnlyCollection<SupervisorDTO>> ReadAllSupervisorsAsync()
    {
        return (await _dbcontext.Users.OfType<Supervisor>()
                        .Select(u => new SupervisorDTO(u.Id, u.Email, u.FirstName, u.LastName, u.Faculty.Title, u.Institution.Title, 
                                                       u.Projects.Select(p => p.Id).ToList(), u.AuthoredProjects.Select(p => p.Id).ToList()))
                        .ToListAsync())
                        .AsReadOnly();
    }

    private async IAsyncEnumerable<Project> GetProjectsAsync(ICollection<int> inProjects) 
    {
        var existing = await _dbcontext.Projects
                        .Where(p => inProjects
                                    .Any(inP => inP == p.Id))
                        .Select(p => p)
                        .ToListAsync();

        foreach (var project in existing)
        {
            yield return project;
        }
    }

    private async IAsyncEnumerable<Course> GetCoursesAsync(ICollection<int> inCourses) 
    {
        var existing = await _dbcontext.Courses
                        .Where(c => inCourses
                                    .Any(inC => inC == c.Id))
                        .Select(c => c)
                        .ToListAsync();

        foreach (var course in existing)
        {
            yield return course;
        }
    }
}