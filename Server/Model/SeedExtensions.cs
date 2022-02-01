namespace ProjectBank.Server.Model;

public static class SeedExtensions
{
    public static async Task<IHost> SeedAsync(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ProjectBankContext>();

          
            await SeedProjectsAsync(context);
        }
        return host;
    }

    private static async Task SeedProjectsAsync(ProjectBankContext context)
    {
        await context.Database.MigrateAsync();

        if (!await context.Projects.AnyAsync())
        {
            context.Institutions.AddRange(Seeding.Institutions);

            context.Faculties.AddRange(Seeding.Faculties);

            context.Programs.AddRange(Seeding.TeachingPrograms);

            context.Supervisors.AddRange(Seeding.Supervisors);

            context.Students.AddRange(Seeding.Students);

            context.Tags.AddRange(Seeding.Tags);
            
            await context.SaveChangesAsync();

            context.Projects.AddRange(Seeding.Projects);

            await context.SaveChangesAsync();
        }
    }
}