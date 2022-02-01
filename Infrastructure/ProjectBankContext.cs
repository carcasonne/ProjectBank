using ProjectBank.Infrastructure.Entities;
using ProjectBank.Infrastructure.ReferenceSystem;

namespace ProjectBank.Infrastructure;

public class ProjectBankContext : DbContext
{
    //User directory
    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Supervisor> Supervisors => Set<Supervisor>();

    //Category diretory
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CodedCategory> CodedCategories => Set<CodedCategory>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Faculty> Faculties => Set<Faculty>();
    public DbSet<Institution> Institutions => Set<Institution>();
    public DbSet<TeachingProgram>  Programs => Set<TeachingProgram>();

    //Project directory
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<Signature> Signatures => Set<Signature>();
    public DbSet<ProjectBucket> Buckets => Set<ProjectBucket>();

    //public ProjectLSH LSH;


    public ProjectBankContext(DbContextOptions<ProjectBankContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    
        // Create Table-Per-Type for the Category hierarchy
        // This is slower than Table-Per-Hierarchy, but it makes database updates work
        
        modelBuilder.Entity<Category>().ToTable("Categories");
        modelBuilder.Entity<CodedCategory>().ToTable("CodedCategories");
        modelBuilder.Entity<Institution>().ToTable("Institutions");
        modelBuilder.Entity<Faculty>().ToTable("Faculties");
        modelBuilder.Entity<Course>().ToTable("Courses");
        modelBuilder.Entity<TeachingProgram>().ToTable("Programs");

        //Save enumerator value as string
        modelBuilder
            .Entity<Project>()
            .Property(e => e.Status)
            .HasMaxLength(50)
            .HasConversion(new EnumToStringConverter<ProjectStatus>());

        //manually determine Project-User relationship, including unique for Supervisor
        modelBuilder.Entity<Project>().HasOne(p => p.Author).WithMany(u => u.AuthoredProjects);
        modelBuilder.Entity<Project>().HasMany(p => p.Users).WithMany(u => u.Projects);
        modelBuilder.Entity<Supervisor>().HasMany(s => s.Projects);

        //manually determine Student-Course relationship
        modelBuilder.Entity<Course>().HasMany(c => c.Students).WithMany(s => s.Courses);

        //manually determine Bucket-Project relationship
        modelBuilder.Entity<ProjectBucket>().HasMany(b => b.Projects).WithMany(p => p.Buckets);

        //manually determine Tag-Project relationship
        modelBuilder.Entity<Project>().HasMany(p => p.Tags).WithMany(t => t.Projects);

        //do not delete project when author is deleted
        modelBuilder.Entity<User>()
            .HasOne(u => u.Institution)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        //do not delete student when student is deleted
        modelBuilder.Entity<Student>()
            .HasOne(u => u.Program)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        //do not delete author supervisor when project is deleted
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Author)
            .WithMany(a => a.AuthoredProjects)
            .OnDelete(DeleteBehavior.Restrict);

        //do not delete bucket when project is deleted
        modelBuilder.Entity<ProjectBucket>()
            .HasMany(b => b.Projects)
            .WithMany(p => p.Buckets);
    }
}