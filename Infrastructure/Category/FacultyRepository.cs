namespace ProjectBank.Infrastructure;

 public class FacultyRepository : IFacultyRepository
    {
        private readonly ProjectBankContext _dbcontext;

        public FacultyRepository(ProjectBankContext context)
        {
            _dbcontext = context;
        }

        public async Task<(Response, FacultyDTO)> CreateAsync(FacultyCreateDTO faculty)
        {
            var conflict =
                await _dbcontext.Faculties
                              .Where(f => f.Title == faculty.Title)
                              .Where(f => f.Institution.Title == faculty.InstitutionName)
                              .Select(f => new FacultyDTO(f.Id, f.Title, f.Description, f.Institution.Title))
                              .FirstOrDefaultAsync();

            if (conflict != null)
            {
                return (Response.Conflict, conflict);
            }

            //get related institution by name
            var institution =
                await _dbcontext.Institutions
                              .Where(i => i.Title == faculty.InstitutionName)
                              .Select(i => i)
                              .FirstOrDefaultAsync();

            //no institution exists by given name, but each faculty must be related to an institution
            if(institution == null) 
            {
                return (Response.NotFound, new FacultyDTO(-1, faculty.Title, faculty.Description, faculty.InstitutionName));
            }

            var entity = new Faculty(faculty.Title, faculty.Description, institution);

            _dbcontext.Faculties.Add(entity);

            await _dbcontext.SaveChangesAsync();

            return (Response.Created, new FacultyDTO(entity.Id, entity.Title, entity.Description, entity.Institution.Title));
        }
        public async Task<FacultyDTO> ReadByIDAsync(int facultyID)
        {
            var faculties = from f in _dbcontext.Faculties
                         where f.Id == facultyID
                         select new FacultyDTO(f.Id, f.Title, f.Description, f.Institution.Title);

            return await faculties.FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyCollection<FacultyDTO>> ReadAllAsync() =>
            (await _dbcontext.Faculties
                           .Select(f => new FacultyDTO(f.Id, f.Title, f.Description, f.Institution.Title))
                           .ToListAsync())
                           .AsReadOnly();
}