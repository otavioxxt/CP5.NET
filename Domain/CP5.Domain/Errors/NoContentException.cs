namespace CP5.Domain.Errors;

public class NoContentException : Exception
{
    public NoContentException()
    {
    }

    public NoContentException(string? message) : base(message)
    {
    }
}
