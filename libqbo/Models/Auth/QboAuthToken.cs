namespace libqbo.Models.Auth;

public class QboAuthToken
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? RealmId { get; set; }
    public DateTimeOffset AccessExpiry { get; set; }
    public DateTimeOffset RefreshExpiry { get; set; }
}
