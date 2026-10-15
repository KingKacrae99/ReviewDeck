namespace  ReviewDeck.Services;
using Microsoft.AspNetCore.Identity;

using ReviewDeck.Models;
using ReviewDeck.Data;
using Microsoft.EntityFrameworkCore;

public class UserService: IUserService
{
    private readonly ReviewDeckContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(ReviewDeckContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<List<User>> GetAllUserAsync()
    {
        try
        {
            var users = await _context.Users.ToListAsync();

            if (!users.Any())
            {
                throw new Exception("No users found");
            }
            return users;
        }
        catch (System.Exception)
        {
            
            throw;
        }
    }

    // Get a single user by Id
    public async Task<User?> GetUserByIdAsync(string slug)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                throw new ArgumentException("Slug cannot be null or empty");
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Slug == slug);
            if (user == null)
            {
                throw new Exception($"User with slug '{slug}' not found");
            }
            return user;
        }
        catch (System.Exception)
        {
            
            throw;
        }
    }
    public async Task<User> UserRegisterAsync(UserRegisterModel user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null");
        }
        if (string.IsNullOrWhiteSpace(user.Firstname)|| 
        string.IsNullOrWhiteSpace(user.Lastname)||
        string.IsNullOrWhiteSpace(user.Email)||
        string.IsNullOrWhiteSpace(user.Password)||
        string.IsNullOrWhiteSpace(user.ConfirmPassword))
        {
            throw new ArgumentException("All required fields must be filled out");
        }
        if (user.Password != user.ConfirmPassword)
        {
            throw new ArgumentException("Passwords do not match");
        }
        var email = user.Email.Trim().ToLowerInvariant();
        var existingUser = await _context.Users.FirstOrDefaultAsync(u=> u.Email == email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("A user with this email already exists");
        }
        var newUser = new User
        {
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Email = email,
            Phone = user.Phone,
            Role = "client"
        };
        newUser.PasswordHash = _passwordHasher.HashPassword(newUser,user.Password);
        newUser.SetForCreation();
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
        return newUser;
    }

    public async Task<User?> UpdateUserAsync(User user, string slug)
    {
        if (user == null || string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("User or slug cannot be null or empty and slug cannot have whitespace.");
        }
        var checkUser = await _context.Users.FirstOrDefaultAsync(u => u.Slug == slug);
        if (checkUser == null)
        {
            throw new Exception($"User with slug '{slug}' not found");
        }
        if (string.IsNullOrWhiteSpace(user.Firstname)||
        string.IsNullOrWhiteSpace(user.Lastname)||
        string.IsNullOrWhiteSpace(user.Email))
        {
            throw new ArgumentException("Required fields cannot be null or empty");
        }
        var emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email && u.Id != checkUser.Id);
        if (emailExists)
        {
            throw new InvalidOperationException("A user with this email already exists");
        }

        checkUser.Firstname = user.Firstname;
        checkUser.Lastname = user.Lastname;
        checkUser.Email = user.Email;
        checkUser.Phone = user.Phone;

        await _context.SaveChangesAsync();
        return checkUser;
    }

    public async Task<bool> DeleteUserAsync(string slug)
    {
        try{
            if (string.IsNullOrEmpty(slug))
            {
                Console.WriteLine("slug cannot be null or empty");
                throw new ArgumentException("slug cannot be null or empty", nameof(slug));
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Slug == slug);
            if (user == null)
            {
                Console.WriteLine($"User not found");
                throw new Exception($"User not found");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while deleting the user: {ex.Message}");
            throw;
        }
    }

    public async Task<User?> LoginUserAsync(LoginRequest request)
    {
        try
        {
            // Ensures request object is not empty
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request), "Fields cannot be empty");
            }
            // checks that the fields are not empty
            if (string.IsNullOrWhiteSpace(request.Email)|| 
            string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Required fields cannot be null or empty"); 
            }

            var email = request.Email.Trim().ToLowerInvariant();

            // Check if information exists
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if(user == null)
            {
                return null;
            }
            var result = _passwordHasher.VerifyHashedPassword(user,user.PasswordHash,request.Password);

            if(result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return user;
            
        }
        catch (System.Exception)
        {
            
            throw;
        }
    }
    
}