using ProjectTaskManagementAPI.DTOs;// Importing the DTOs namespace to use RegisterDto and LoginDto classes

namespace ProjectTaskManagementAPI.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);// Method for registering a new user, takes a RegisterDto object and returns a JWT token as a string

        Task<string> LoginAsync(LoginDto dto);// Method for logging in a user, takes a LoginDto object and returns a JWT token as a string
    }
}

//Interfaces define a contract between classes, reduce coupling, support Dependency Injection, and make the code easier to maintain and test.

// The IAuthService interface defines two methods: RegisterAsync and LoginAsync, which are responsible for user registration and login, respectively. Both methods return a JWT token as a string upon successful execution.

