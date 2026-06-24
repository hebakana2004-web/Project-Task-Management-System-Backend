namespace ProjectTaskManagementAPI.DTOs
{
    public class LoginDto// Data Transfer Object for user login, containing the user's email and password
    {
        public string Email { get; set; } = string.Empty;// User's email address

        public string Password { get; set; } = string.Empty;// User password
    }
}
