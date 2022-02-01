namespace ProjectBank.Infrastructure;

public abstract class CodedCategory : Category
{
    
    [Required]
    public Faculty Faculty {get; set;}

    [Required]
    public string Code {get; set;}

    protected CodedCategory(string Title, string Description, Faculty Faculty, string Code) 
    : base(Title, Description) 
    {
        this.Faculty = Faculty;
        this.Code = Code;
    }

    protected CodedCategory() {}
}