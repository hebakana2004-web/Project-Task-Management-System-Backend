using ProjectTaskManagementAPI.DTOs;
using ProjectTaskManagementAPI.Interfaces;
using ProjectTaskManagementAPI.Data;
using ProjectTaskManagementAPI.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace ProjectTaskManagementAPI.Services
{
    public class AuthService : IAuthService // Implementation of the IAuthService interface, providing methods for user registration and login
    {
        private readonly AppDbContext _context;//We use it to access the database.
        private readonly ILogger<AuthService> _logger;//We use it to log information, warnings, and errors,We use it to record events.
        private readonly IConfiguration _configuration; //We use it to read values ​​from: appsettings.json

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

        //hash password
        private string HashPassword(string password)//The HashPassword method takes a plain text password as input and returns a hashed version of it. It uses the SHA256 hashing algorithm to compute the hash of the password and then converts it to a Base64 string for storage.
        {
            using var sha256 = SHA256.Create();//We create an instance of the SHA256 hashing algorithm using the Create method. This allows us to compute the hash of the password.

            var bytes = Encoding.UTF8.GetBytes(password);

            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }

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

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),//We hash the provided password using the HashPassword method before storing it in the database. This ensures that the password is stored securely and cannot be easily retrieved in plain text.
                Role = dto.Role
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            _logger.LogInformation("User registered successfully");

            return "User registered successfully";
        }
        //The LoginAsync method is responsible for authenticating a user based on the provided email and password. It first checks if a user with the given email exists in the database. If not, it logs a warning and returns an error message. If the user exists, it hashes the provided password and compares it with the stored password hash. If they do not match, it logs a warning and returns an error message. If the login is successful, it logs the information and returns a success message.


        //login method
        public async Task<string> LoginAsync(LoginDto dto)//The LoginAsync method is responsible for authenticating a user based on the provided email and password. It first checks if a user with the given email exists in the database. If not, it logs a warning and returns an error message. If the user exists, it hashes the provided password and compares it with the stored password hash. If they do not match, it logs a warning and returns an error message. If the login is successful, it logs the information and returns a success message.
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);//We query the database to find a user with the provided email. If no user is found, we log a warning and return an error message indicating that the email or password is invalid.

            if (user == null)
            {
                _logger.LogWarning("User not found");

                return "Invalid Email or Password";
            }

            var hashedPassword = HashPassword(dto.Password);//We hash the provided password using the same hashing method as during registration. This ensures that we are comparing the hashed version of the provided password with the stored password hash.

            if (user.PasswordHash != hashedPassword) //We compare the hashed version of the provided password with the stored password hash. If they do not match, it means the password is incorrect.
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