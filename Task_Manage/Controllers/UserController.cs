using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using Task_Manage.Data;
using Task_Manage.Models;
using Microsoft.EntityFrameworkCore;

namespace Task_Manage.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UserController : ControllerBase
  {
    private readonly Task_Context _context;
    IConfiguration _configuration;

    public UserController(Task_Context context,IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(tblUser user)
    {
      // Check if the username is already taken
      if (await _context.tblUsers.AnyAsync(u => u.Username == user.Username))
      {
        return BadRequest("Username is already taken");
      }

      // Hash the password (you should use a secure password hashing algorithm)
      // For simplicity, we are using a plain text password here, which is not secure.
      user.Password = HashPassword(user.Password);

      // Add the user to the database
      _context.tblUsers.Add(user);
      await _context.SaveChangesAsync();

      return StatusCode(201); // Created
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(tblUser loginUser)
    {
      // Find the user with the given username
      var user = await _context.tblUsers.FirstOrDefaultAsync(u => u.Username == loginUser.Username);

      // Check if the user exists and the password is correct
      if (user == null || !VerifyPassword(loginUser.Password, user.Password))
      {
        return Unauthorized("Invalid username or password");
      }

      // Generate a JWT token
      var token = GenerateJwtToken(user);
      //user.AccessToken = GenerateJwtToken(user);
      return Ok(new { Token = token });
      //return Ok(user);
    }

    private string GenerateJwtToken(tblUser user)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes("your_secret_key_here");

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new[]
          {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            }),
        Expires = DateTime.UtcNow.AddDays(1), // Token expiration time
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
      };

      var token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }


    //public string GenerateJwtToken(tblUser user)
    //{
    //  var claims = new[]
    //  {
    //    new Claim(JwtRegisteredClaimNames.Sub,_configuration["Jwt:Subject"]),
    //    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
    //    new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString()),
    //    new Claim("Id",user.Id.ToString()),
    //    new Claim("UserName",user.Username),
    //    new Claim("Email",user.Email),
    //    new Claim("PhoneNumber",user.PhoneNumber)
    //  };
    //  var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

    //  var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    //  var token = new JwtSecurityToken(
    //    _configuration["Jwt:Issuer"],
    //    _configuration["Jwt:Audience"],
    //    claims,
    //    expires: DateTime.UtcNow.AddMinutes(10),
    //    signingCredentials: signIn);

    //    string Token=new JwtSecurityTokenHandler().WriteToken(token);
    //    return Token;
    //}

    private string HashPassword(string password)
    {
      // Implement a secure password hashing algorithm (e.g., using BCrypt)
      // For simplicity, we are using a plain text password here, which is not secure.
      return password;
    }

    private bool VerifyPassword(string inputPassword, string hashedPassword)
    {
      
      return inputPassword == hashedPassword;
    }

  }
}
