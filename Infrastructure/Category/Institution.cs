namespace ProjectBank.Infrastructure;


public class Institution : Category {

    public IEnumerable<Faculty> Faculties {get; set;}  = null!;

    public Institution(string Title, string Description) 
    : base(Title, Description)
    {

    }

    public Institution() {}
}