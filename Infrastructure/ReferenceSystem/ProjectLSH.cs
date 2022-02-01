using System.Net.Http;
using ProjectBank.Infrastructure.Entities;

namespace ProjectBank.Infrastructure.ReferenceSystem
{
    public class ProjectLSH : LocalitySensitiveHashTable<IProject>, IProjectLSH
    {
        private readonly ProjectRepository _projectRepository;

        public ProjectLSH(IProjectRepository projectRepository)
        {
            _projectRepository = (ProjectRepository) projectRepository;
        }
        public override async Task<Response> Insert(IProject project)
        {
            if (project.Signature == null || project.Category == null) return Response.Conflict;
            return await base.Insert(project);           
        }

        public async Task<IReadOnlyCollection<IProject>> GetSortedInCategory(IProject tagable)
        {
            var category = tagable.Category;
            var allCategories = await GetSorted(tagable);
            var sorted = new List<IProject>();

            await foreach (var tag in allCategories.ToAsyncEnumerable())
            {
                if (tag.Category.Id == category.Id) sorted.Add(tag);
            }
            return sorted.AsReadOnly();
        }

        public async Task<Response> InsertAll()
        {
            var allProjects = await _projectRepository.ReadAllProjectReferenceAsync();
            Console.WriteLine("DONE");
            foreach (var project in allProjects)
            {
                var response = await Insert(project);
                if (response != Response.Created) return Response.Conflict;
            }
            return Response.Created;
        }
        public async Task<IReadOnlyCollection<IProject>> GetSorted(IProject tagable)
        {
            var NotSortedTagables = await Get(tagable);

            var jaccardList = new List<(float, ITagable)>();
            var tagNames = new List<string>();
            foreach(var tag in tagable.Tags) tagNames.Add(tag.Name);
            foreach (var project in NotSortedTagables)
            {
                var tags = project.Tags;
                int inCommon = 0;
                foreach (var tag in tags)
                {
                    if (tagNames.Contains(tag.Name))
                    {
                        inCommon++;
                    }
                }

                int total = tags.Count + tagable.Tags.Count - inCommon;
                float jaccardindex = inCommon / (float)total;

                jaccardList.Add((jaccardindex, project));
            }
            jaccardList.Sort((x, y) => y.Item1.CompareTo(x.Item1));
            var SortedTagables = new List<IProject>();
            foreach ((float, IProject) tagger in jaccardList)
            {
                SortedTagables.Add(tagger.Item2);
            }
            return SortedTagables.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<ProjectReferenceDTO>> GetSorted(IProject project, int size)
        {
            
            var sorted = (await GetSorted(project)).Take(size);
            var limited = new List<ProjectReferenceDTO>();

            foreach (var p in sorted) limited.Add(new ProjectReferenceDTO(p.Id, p.Category.Id,
                                                     p.Tags.Select(t => t.Id).ToList()));
            return limited.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<ProjectReferenceDTO>> GetSortedInCategory(IProject project, int size)
        {
            var sorted = (await GetSortedInCategory(project)).Take(size);
            var limited = new List<ProjectReferenceDTO>();

            foreach (var p in sorted) limited.Add(new ProjectReferenceDTO(p.Id, p.Category.Id,
                                                     p.Tags.Select(t => t.Id).ToList()));
            return limited.AsReadOnly(); 
        }

    }

}
