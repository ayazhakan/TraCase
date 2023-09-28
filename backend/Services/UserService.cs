using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

public interface IUserService
{
    Task<User> AuthenticateAsync(string username, string password);
    Task<IdentityResult> CreateAsync(User user, string password);
}

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserService(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<User> AuthenticateAsync(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
        if (result.Succeeded)
            return user;

        return null;
    }

    public async Task<IdentityResult> CreateAsync(User user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        return result;
    }
}
