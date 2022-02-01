namespace ProjectBank.Infrastructure;
public class ProjectReference : IProject
{
    public int Id { get; set; }
    public Category Category {get; set;}
    public IReadOnlyCollection<Tag> Tags {get; set;}
    public Signature Signature {get; set;} 
 
}