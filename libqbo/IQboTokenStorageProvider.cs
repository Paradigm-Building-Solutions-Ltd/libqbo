using libqbo.Models.Auth;

namespace libqbo;

public interface IQboTokenStorageProvider
{
    Task<QboAuthToken?> GetToken();
    Task SaveToken(QboAuthToken token);
}
