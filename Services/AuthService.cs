using ProjectTaskManagementAPI.DTOs;
using ProjectTaskManagementAPI.Interfaces;
using ProjectTaskManagementAPI.Data;
using ProjectTaskManagementAPI.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

namespace ProjectTaskManagementAPI.Services
{
    public class AuthService : IAuthService // Implementation of the IAuthService interface, providing methods for user registration and login
    {
        private readonly AppDbContext _context;//We use it to access the database.
        private readonly ILogger<AuthService> _logger;//We use it to log information, warnings, and errors,We use it to record events.
        private readonly IConfiguration _configuration; //We use it to read values ​​from: appsettings.json
        private readonly PasswordHasher<User> _passwordHasher = new();//We use it to hash passwords securely.


        public AuthService(
            AppDbContext context,
            ILogger<AuthService> logger,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }
        //Constructor Injection
        //When .NET creates: AuthService It automatically sends: AppDbContext ILogger IConfiguration to the Constructor.

    

        

        //generate jwt token method
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.FullName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role),
       
    };
            //The token it holds the user's information.

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //register method
        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);//We check if a user with the provided email already exists in the database. If such a user is found, we log a warning and return an error message indicating that the email already exists.

            if (existingUser != null)//If a user with the provided email already exists, we log a warning and return an error message indicating that the email already exists. This prevents duplicate registrations with the same email address.
            {
                _logger.LogWarning("Email already exists");

                return "Email already exists";
            }
            //If the email is unique, we create a new User object and populate its properties with the data from the RegisterDto. We then hash the provided password using the PasswordHasher and store the hashed password in the PasswordHash property of the User object. After that, we add the new user to the database context and save the changes asynchronously. Finally, we log an information message indicating that the user was registered successfully and return a success message.
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Role = dto.Role
            };

            //We use the PasswordHasher to hash the provided password securely.
            //The hashed password is stored in the PasswordHash property of the User object,
            //ensuring that the actual password is not stored in plain text in the database.
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            _logger.LogInformation("User registered successfully");

            return "User registered successfully";
        }
        

        //login method
        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);//We query the database to find a user with the provided email. If no user is found, we log a warning and return an error message indicating that the email or password is invalid.
            
            if (user == null)
            {
                _logger.LogWarning("User not found");

                return "Invalid Email or Password";
            }

            //We use the PasswordHasher to verify the provided password against the stored password hash.
            //If the verification fails, we log a warning and return an error message indicating that the email or password is invalid.
            var verificationResult =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    dto.Password);

            if (verificationResult == PasswordVerificationResult.Failed)//If the password verification fails, we log a warning and return an error message indicating that the email or password is invalid.
            {
                _logger.LogWarning("Invalid password");

                return "Invalid Email or Password";
            }


            //If the user is found and the password matches, we log an information message indicating that the login was successful. We then generate a JWT token for the authenticated user using the GenerateJwtToken method and return it.
            _logger.LogInformation("Login successful");

            var token = GenerateJwtToken(user);

            return token; 
        }
    }
}

//The LoginAsync method authenticates the user by verifying
//the entered password against the stored password hash using
//ASP.NET Core PasswordHasher.b