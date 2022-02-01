using ProjectBank.Infrastructure.Entities;

namespace ProjectBank.Infrastructure;

public class ProjectRepository : IProjectRepository
{
    private readonly ProjectBankContext _dbcontext;

    public ProjectRepository(ProjectBankContext context)
    {
        _dbcontext = context;
    }

    public async Task<(Response, ProjectDTO)> CreateAsync(ProjectCreateDTO project)
    {
        var conflict = await _dbcontext.Projects
                        .Where(p => p.Author.Id == project.AuthorID)
                        .Where(p => p.Title == project.Title)
                        .Select(p => new ProjectDTO(p.Id, p.Author.Id, p.Title, p.Description, p.Status, p.MaxStudents, p.Category.Id,
                                                    p.Tags.Select(t => t.Id).ToList(), p.Tags.Select(t => t.Name).ToList(), p.Users.Select(u => u.Id).ToList(), p.Buckets.Select(b => b.Id).ToList()))
                        .FirstOrDefaultAsync();

        if (conflict != null)
        {
            return (Response.Conflict, conflict);
        }

        var author = await GetSupervisorAsync(project.AuthorID);
        var category = await GetCategoryAsync(project.CategoryID);
        if (author == null || category == null)
        {
            return (Response.NotFound, new ProjectDTO(-1, project.AuthorID, project.Title, project.Description, project.Status, project.MaxStudents, project.CategoryID, project.TagIDs, project.TagNames, project.UserIDs, project.BucketIDs));
        }

        /* var entity = new Project(author, project.Title, project.Description, project.Status, category,
            await GetTagsAsync(project.TagIDs).ToListAsync(),
            await GetUsersAsync(project.UserIDs).ToListAsync(),
            await GetBucketsAsync(project.BucketIDs).ToListAsync(), project.MaxStudents); */
        var entity = new Project
        {
            Author = author,
            Title = project.Title,
            Description = project.Description,
            Status = project.Status,
            MaxStudents = project.MaxStudents,
            Category = category,
            Tags = await GetTagsAsync(project.TagIDs).ToListAsync(),
            Users = await GetUsersAsync(project.UserIDs).ToListAsync(),
            Buckets = await GetBucketsAsync(project.BucketIDs).ToListAsync(),
        };

        _dbcontext.Projects.Add(entity);

        await _dbcontext.SaveChangesAsync();

        return (Response.Created, new ProjectDTO(entity.Id, entity.Author.Id, entity.Title, entity.Description, entity.Status, entity.MaxStudents, entity.Category.Id,
                                                 entity.Tags.Select(t => t.Id).ToList(), entity.Tags.Select(t => t.Name).ToList(), entity.Users.Select(u => u.Id).ToList(), entity.Buckets.Select(b => b.Id).ToList()));
    }
    public async Task<IReadOnlyCollection<ProjectDTO>> ReadAllAsync()
    {
        return (await _dbcontext.Projects
                        .Select(p => new ProjectDTO(p.Id, p.Author.Id, p.Title, p.Description, p.Status, p.MaxStudents, p.Category.Id,
                                                    p.Tags.Select(t => t.Id).ToList(), p.Tags.Select(t => t.Name).ToList(), p.Users.Select(u => u.Id).ToList(), p.Buckets.Select(b => b.Id).ToList()))
                        .ToListAsync())
                        .AsReadOnly();
    }

    public async Task<IReadOnlyCollection<ProjectDTO>> ReadAllAvailableAsync(int author) 
    {
        return (await _dbcontext.Projects
                        .Where(p => p.Status != ProjectStatus.DELETED)
                        .Where (p => p.Author.Id == author || p.Users.Select(u => u.Id).Contains(author))
                        .Select(p => new ProjectDTO(p.Id, p.Author.Id, p.Title, p.Description, p.Status, p.MaxStudents, p.Category.Id,
                                                    p.Tags.Select(t => t.Id).ToList(), p.Tags.Select(t => t.Name).ToList(), p.Users.Select(u => u.Id).ToList(), p.Buckets.Select(b => b.Id).ToList()))
                        .ToListAsync())
                        .AsReadOnly();
    }

    public async Task<IReadOnlyCollection<ProjectDTO>> ReadFirstHundred_PrioritozeAuthored(int author)
    {
        var AuthoredProjects = (await _dbcontext.Projects
                                .Where(p => p.Author.Id == author)
                                .Where(p => p.Status == ProjectStatus.PUBLIC)
                                .Take(100)
                                .Select(p => new ProjectDTO(p.Id, p.Author.Id, p.Title, p.Description, p.Status, p.MaxStudents, p.Category.Id,
                                                            p.Tags.Select(t => t.Id).ToList(), p.Tags.Select(t => t.Name).ToList(), p.Users.Select(u => u.Id).ToList(), p.Buckets.Select(b => b.Id).ToList()))
                                .ToListAsync());

        if (AuthoredProjects.Count() < 100)
        {
            var take = 100 - AuthoredProjects.Count();
            var FillerProjects = (await _dbcontext.Projects
                                    .Where(p => p.Status == ProjectStatus.PUBLIC)
                                    .Take(take)
                                    .Select(p => new ProjectDTO(p.Id, p.Author.Id, p.Title, p.Description, p.Status, p.MaxStudents, p.Category.Id,
                                                                p.Tags.Select(t => t.Id).ToList(), p.Tags.Select(t => t.Name).ToList(), p.Users.Select(u => u.Id).ToList(), p.Buckets.Select(b => b.Id).ToList()))
                                    .ToListAsync());

            AuthoredProjects.AddRange(FillerProjects);
        }

        return AuthoredProjects.AsReadOnly();
    }
        
 
    public async Task<IReadOnlyCollection<ProjectDTO>> ReadAllAuthoredAsync(int authorID)
    {
        return (await _dbcontext.Projects
                        .Where(p => p.Author.Id == authorID)
                        .Select(p => new ProjectDTO(p.Id, p.Author.Id, p.Title, p.Description, p.Status, p.MaxStudents, p.Category.Id,
                                                    p.Tags.Select(t => t.Id).ToList(), p.Tags.Select(t => t.Name).ToList(), p.Users.Select(u => u.Id).ToList(), p.Buckets.Select(b => b.Id).ToList()))
                        .ToListAsync())
                        .AsReadOnly();
    }

    public async Task<IReadOnlyCollection<ProjectDTO>> ReadAllByTagAsync(int tagID)
    {
        return (await _dbcontext.Projects
                        .Where(p => p.Tags
                                    .Any(t => t.Id == tagID))
                        .Select(p => new ProjectDTO(p.Id, p.Author.Id, p.Title, p.Description, p.Status, p.MaxStudents, p.Category.Id,
                                                    p.Tags.Select(t => t.Id).ToList(), p.Tags.Select(t => t.Name).ToList(), p.Users.Select(u => u.Id).ToList(), p.Buckets.Select(b => b.Id).ToList()))
                        .ToListAsync())
                        .AsReadOnly();
    }

    public async Task<Option<ProjectDTO>> ReadByIDAsync(int projectID)
    {
        var users = from p in _dbcontext.Projects
                    where p.Id == projectID
                    select new ProjectDTO(p.Id, p.Author.Id, p.Title, p.Description, p.Status, p.MaxStudents, p.Category.Id,
                                          p.Tags.Select(t => t.Id).ToList(), p.Tags.Select(t => t.Name).ToList(), p.Users.Select(u => u.Id).ToList(), p.Buckets.Select(b => b.Id).ToList());

        return await users.FirstOrDefaultAsync(); 
    }

    public async Task<ProjectDTO> ReadByKeyAsync(string ProjectTitle, int authorID)
    {
        var users = from p in _dbcontext.Projects
                    where p.Author.Id == authorID
                    where p.Title == ProjectTitle
                    select new ProjectDTO(p.Id, p.Author.Id, p.Title, p.Description, p.Status, p.MaxStudents, p.Category.Id,
                                                    p.Tags.Select(t => t.Id).ToList(), p.Tags.Select(t => t.Name).ToList(), p.Users.Select(u => u.Id).ToList(), p.Buckets.Select(b => b.Id).ToList());

        return await users.FirstOrDefaultAsync(); 
    }

    public async Task<IReadOnlyCollection<ProjectDTO>> ReadCollectionAsync(ICollection<int> projectIDs)
    {
        var projects = (await _dbcontext.Projects
                        .Where(p => projectIDs
                                    .Any(inP => inP == p.Id))
                        .Select(p => new ProjectDTO(p.Id, p.Author.Id, p.Title, p.Description, p.Status, p.MaxStudents, p.Category.Id,
                                                    p.Tags.Select(t => t.Id).ToList(), p.Tags.Select(t => t.Name).ToList(), p.Users.Select(u => u.Id).ToList(), p.Buckets.Select(b => b.Id).ToList()))
                        .ToListAsync())
                        .AsReadOnly();
        
        return projects;
    }

    public async Task<Response> AddUserAsync(ProjectKeyDTO projectKey, int userID)
    {
        var user = await GetUserAsync(userID);

        var project = await _dbcontext.Projects
                            .Where(p => p.Author.Id == projectKey.AuthorID)
                            .Where(p => p.Title == projectKey.Title)
                            .Select(p => p)
                            .FirstOrDefaultAsync();

        if(user == null || project == null)
        {
            return Response.NotFound;
        }

        user.Projects.Add(project);

        await _dbcontext.SaveChangesAsync();

        return Response.Updated;
    }

    public async Task<Response> RemoveUserAsync(ProjectKeyDTO projectKey, int userID)
    {
        var user = await GetUserAsync(userID);

        var project = await _dbcontext.Projects
                            .Where(p => p.Author.Id == projectKey.AuthorID)
                            .Where(p => p.Title == projectKey.Title)
                            .Select(p => p)
                            .FirstOrDefaultAsync();

        if(user == null || project == null)
        {
            return Response.NotFound;
        }

        var removed = user.Projects.Remove(project);

        if(removed) 
        {
            await _dbcontext.SaveChangesAsync();
            return Response.Updated;
        } 

        return Response.NotFound;
    }

    public async Task<Response> UpdateAsync(int id, ProjectUpdateDTO project)
    {
        var projectEntity = await _dbcontext.Projects
                .Where(p => p.Id == project.Id)
                .FirstOrDefaultAsync();
        
        var category = await GetCategoryAsync(project.CategoryID);

        if(projectEntity == null || category == null)
        {
            return Response.NotFound;
        }

        projectEntity.Title = project.Title;
        projectEntity.Description = project.Description;
        projectEntity.MaxStudents = project.MaxStudents;
        projectEntity.Status = project.Status;
        projectEntity.Category = category;
        //changing project crashes readonly
        //projectEntity.Tags = (await GetTagsAsync(new List<int>() {1, 2, 3}).ToListAsync()).AsReadOnly();
        projectEntity.Buckets = await GetBucketsAsync(project.BucketIDs).ToListAsync();
        projectEntity.Users = await GetUsersAsync(project.UserIDs).ToListAsync();

        await _dbcontext.SaveChangesAsync();
        return Response.Updated;
    }

    public async Task<Response> DeleteAsync(int id, ProjectUpdateDTO project)
    {
        var projectEntity = await _dbcontext.Projects
                .Where(p => p.Id == project.Id)
                .FirstOrDefaultAsync();
        
        var category = await GetCategoryAsync(project.CategoryID);

        if(projectEntity == null || category == null)
        {
            return Response.NotFound;
        }

        if(projectEntity.Status == ProjectStatus.DELETED)
        {
            return Response.Conflict;
        }

        projectEntity.Status = ProjectStatus.DELETED;

        await _dbcontext.SaveChangesAsync();
        
        return Response.Deleted;
    }

    private async Task<Supervisor> GetSupervisorAsync(int authorID)
    {
        var users = from u in _dbcontext.Users.OfType<Supervisor>()
                    where u.Id == authorID
                    select u;
                           
        return await users.FirstOrDefaultAsync();
    }

    private async IAsyncEnumerable<Tag> GetTagsAsync(ICollection<int> inTags)
    {
        var existing = await _dbcontext.Tags
                        .Where(t => inTags
                                    .Any(inT => inT == t.Id))
                        .Select(t => t)
                        .ToListAsync();
                           
        foreach (var tag in existing)
        {
            yield return tag;
        }
    }

    private async IAsyncEnumerable<User> GetUsersAsync(ICollection<int> inUsers)
    {
        var existing = await _dbcontext.Users
                        .Where(u => inUsers
                                    .Any(inS => inS == u.Id))
                        .Select(u => u)
                        .ToListAsync();
                           
        foreach (var user in existing)
        {
            yield return user;
        }
    }

    private async Task<User> GetUserAsync(int userID)
    {
        return await _dbcontext.Users
                        .Where(u => u.Id == userID)
                        .Select(u => u)
                        .FirstOrDefaultAsync();
    }

    private async IAsyncEnumerable<Supervisor> GetCollaboratorsAsync(ICollection<int> inUsers)
    {
        var existing = await _dbcontext.Users.OfType<Supervisor>()
                        .Where(u => inUsers
                                    .Any(inS => inS == u.Id))
                        .Select(u => u)
                        .ToListAsync();
                           
        foreach (var user in existing)
        {
            yield return user;
        }
    }

    private async IAsyncEnumerable<ProjectBucket> GetBucketsAsync(ICollection<int> inBuckets)
    {
        var existing = await _dbcontext.Buckets
                        .Where(b => inBuckets
                                    .Any(inB => inB == b.Id))
                        .Select(b => b)
                        .ToListAsync();
                           
        foreach (var bucket in existing)
        {
            yield return bucket;
        }
    }

    private async Task<Category> GetCategoryAsync(int categoryID)
    {
        return await _dbcontext.Categories
                        .Where(c => c.Id == categoryID)
                        .Select(c => c)
                        .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyCollection<IProject>> ReadAllProjectReferenceAsync()
    {        
           return ((await _dbcontext.Projects
                        .Select(p => new ProjectReference{Id = p.Id, Tags = p.Tags.Select( t => new Tag(t.Name)).ToList(), Category = new Category{Id = p.Category.Id}, Signature = new Signature(p.Tags.Select( t => new Tag(t.Name)).ToList())})
                        .ToListAsync())
                        .AsReadOnly());
    }
    public async Task<IProject> ReadProjectReferenceAsync(int id)
    {        
           return ((await _dbcontext.Projects
                        .Where(a => a.Id == id)
                        .Select(p => new ProjectReference{Id = p.Id, Tags = p.Tags.Select( t => new Tag(t.Name)).ToList(), Category = new Category{Id = p.Category.Id}, Signature = new Signature(p.Tags.Select( t => new Tag(t.Name)).ToList())})
                        .ToListAsync())
                        .AsReadOnly()).FirstOrDefault();
    }
}