namespace libqbo;

public class QboAuthenticationRequiredException : Exception
{
    public QboAuthenticationRequiredException()
        : base("The QuickBooks Online client is not authenticated. Please authenticate.")
    {
    }

    public QboAuthenticationRequiredException(string? message) : base(message)
    {
    }
}
