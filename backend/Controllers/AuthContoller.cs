// Controllers/AuthController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;



[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

 
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User model)
    {
        var user = await _userService.AuthenticateAsync(model.UserName, model.PasswordHash);

        if (user == null)
            return Unauthorized();

        var token = GenerateJwtToken(user);

        return Ok(new { Token = token });
    }

   
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User model)
    {
        var user = new User { UserName = model.UserName };
        var result = await _userService.CreateAsync(user, model.PasswordHash);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok();
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("super secretkey with 128 bits length"); 
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
