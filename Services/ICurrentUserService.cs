
namespace ReviewDeck.Services;

public interface ICurrentUserService
{
    Task<int?> GetUserIdAsync();
}
