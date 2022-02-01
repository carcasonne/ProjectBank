namespace ProjectBank.Infrastructure;
public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title {get; set;}

    [StringLength(1000)]
    public string Description {get; set;}

    public Category(string Title, string Description) 
    {
        this.Title = Title;
        this.Description = Description;
    }

    public Category() {}
}