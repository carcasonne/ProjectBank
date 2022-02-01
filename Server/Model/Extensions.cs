namespace ProjectBank.Server.Model;

public static class Extensions
{
    public static IActionResult ToActionResult(this Response status) => status switch
    {
        Created => new OkResult(),
        Updated => new OkResult(),
        Deleted => new NoContentResult(),
        NotFound => new NotFoundResult(),
        BadRequest => new BadRequestResult(),
        Conflict => new ConflictResult(),
        _ => throw new NotSupportedException($"{status} not supported")
    };

    public static ActionResult<T> ToActionResult<T>(this Option<T> option) where T : class
        => option.IsSome ? option.Value : new NotFoundResult();
}