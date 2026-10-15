namespace  ReviewDeck.Services;

using ReviewDeck.Models;

public interface IUserService
{
    //Fetch all users
    Task<List<User>> GetAllUserAsync();
    
    // Get  a single user by its Id
    Task<User?> GetUserByIdAsync(string slug);

    // Register a new user
    Task<User> UserRegisterAsync(UserRegisterModel user);

    // Update an existing user
    Task<User?> UpdateUserAsync(User user, string slug);

    Task<bool> DeleteUserAsync(string slug);

    Task<User?> LoginUserAsync(LoginRequest request);

}
