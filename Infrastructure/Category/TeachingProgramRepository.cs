namespace ProjectBank.Infrastructure;

 public class TeachingProgramRepository : ITeachingProgramRepository
    {
        private readonly ProjectBankContext _dbcontext;

        public TeachingProgramRepository(ProjectBankContext context)
        {
            _dbcontext = context;
        }

        public async Task<(Response, TeachingProgramDTO)> CreateAsync(TeachingProgramCreateDTO program)
        {
            var conflict =
                    await _dbcontext.Programs
                    .Where(p => p.Title == program.Title)
                    .Where(p => p.Description == program.Description)
                    .Where(p => p.Faculty.Title == program.FacultyName)
                    .Where(p => p.Code == program.Code)
                    .Select(p => new TeachingProgramDTO(p.Id, p.Title, p.Description, p.Faculty.Title, p.Code,program.CourseCodes))
                    .FirstOrDefaultAsync();

            if (conflict != null)
            {
                return (Response.Conflict, conflict);
            }

            //finds the Faculty related to the institution by its id
            var EntityFaculty =
                await _dbcontext.Faculties
                              .Where(f => f.Title == program.FacultyName)
                              .Select(f => f)
                              .FirstOrDefaultAsync();
                              
            if(EntityFaculty == null)
            {
                return (Response.NotFound, new TeachingProgramDTO(-1, program.Title, program.Description, program.FacultyName, program.Code, program.CourseCodes));
            }

            var entity = new TeachingProgram
            {
                Title = program.Title,
                Description = program.Description,
                Faculty = EntityFaculty, 
                Code = program.Code,
                Courses = await GetCoursesAsync(program.CourseCodes, EntityFaculty.Institution.Title).ToListAsync()
            };

            _dbcontext.Programs.Add(entity);

            await _dbcontext.SaveChangesAsync();

            return (Response.Created, new TeachingProgramDTO(entity.Id, entity.Title,entity.Description,entity.Faculty.Title,entity.Code,program.CourseCodes));
        }
        public async Task<TeachingProgramDTO> ReadProgramByIDAsync(int ProgramID)
        {
            var programs = from p in _dbcontext.Programs
                           where p.Id == ProgramID
                           select new TeachingProgramDTO(p.Id, p.Title, p.Description, p.Faculty.Title, p.Code, p.Courses.Select(p=> p.Code).ToList());

            return await programs.FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<TeachingProgramDTO>> ReadAllAsync() =>
            (await _dbcontext.Programs
                           .Select(p => new TeachingProgramDTO(p.Id, p.Title, p.Description, p.Faculty.Title, p.Code, p.Courses.Select(p=> p.Code).ToList()))
                           .ToListAsync())
                           .AsReadOnly();

        //used to get existing courses based on Title and FacultyName given in DTO
        private async IAsyncEnumerable<Course> GetCoursesAsync(ICollection<string> inCourses, string InstitutionName) 
        {
            var existing = await _dbcontext.Courses
                            .Where(c => c.Faculty.Institution.Title == InstitutionName)
                            .Where(c => inCourses
                                        .Any(inC => inC == c.Code))
                            .Select(c => c)
                            .ToListAsync();

            foreach (var course in existing)
            {
                yield return course;
            }
        }
}