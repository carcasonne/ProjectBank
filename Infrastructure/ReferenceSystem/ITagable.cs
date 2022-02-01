namespace ProjectBank.Infrastructure
{
    public interface ITagable
    {
        int Id {get; set;}
        IReadOnlyCollection<Tag> Tags {get; set;}
        Signature Signature {get;}
    }
}
