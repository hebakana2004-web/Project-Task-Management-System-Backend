using Microsoft.AspNetCore.Mvc; // import types for controllers, attributes and IActionResult
using ProjectTaskManagementAPI.DTOs; // import DTO types like `RegisterDto` and `LoginDto`
using ProjectTaskManagementAPI.Interfaces; // import service interfaces like `IAuthService`

namespace ProjectTaskManagementAPI.Controllers // define the namespace for controller classes
{ // start namespace
    [Route("api/[controller]")] // route attribute: base route becomes "api/auth" for this controller
    [ApiController] // marks class as an API controller (model-binding, validation, etc.)
    public class AuthController : ControllerBase // controller that provides auth endpoints; derives from `ControllerBase`
    { // start class
        private readonly IAuthService _authService; // injected service handling auth logic
        private readonly ILogger<AuthController> _logger; // logger for recording errors and info; trailing // in original line is harmless

        public AuthController( // constructor used by DI to create controller instances
            IAuthService authService, // DI-injected `IAuthService` implementation
            ILogger<AuthController> logger) // DI-injected logger instance
        { // start constructor
            _authService = authService; // assign injected auth service to private field
            _logger = logger; // assign injected logger to private field
        } // end constructor

        [HttpPost("register")]  // maps POST api/auth/register to this action
        public async Task<IActionResult> Register(RegisterDto dto) // action to register a new user; accepts `RegisterDto`
        { // start Register
            try // begin try block to catch unexpected errors and log them
            { // start try
                var result = await _authService.RegisterAsync(dto); // call service to register and await result (string message or token)

                if (result == "Email already exists") // service returned a known failure message
                { // start if
                    return BadRequest(new // return 400 Bad Request with a small payload
                    {
                        Success = false, // indicate operation failed
                        Message = result // include the failure reason from service
                    });
                } // end if

                return Ok(new // on success return 200 Ok with success message
                {
                    Success = true, // indicate operation succeeded
                    Message = result // include success message (or token) from service
                });
            } // end try
            catch (Exception ex) // catch any exception thrown during registration flow
            { // start catch
                _logger.LogError(ex, "Error occurred while registering user"); // log the exception and a contextual message

                return StatusCode(500, new // return 500 Internal Server Error with a small payload
                {
                    Success = false, // indicate operation failed
                    Message = "An unexpected error occurred." // generic message for clients
                });
            } // end catch
        } // end Register

        [HttpPost("login")] // maps POST api/auth/login to this action
        public async Task<IActionResult> Login(LoginDto dto) // action to authenticate a user; accepts `LoginDto`
        { // start Login
            try // begin try block to catch errors during login
            { // start try
                var token = await _authService.LoginAsync(dto); // call service to login and await returned token or error message

                if (token == "Invalid Email or Password") // service returned a known failure indication
                { // start if
                    return Unauthorized(new // return 401 Unauthorized with a small payload
                    {
                        Success = false, // operation failed
                        Message = token // pass the failure message back to client
                    });
                } // end if

                return Ok(new // on success return 200 Ok with the token
                {
                    Success = true, // indicate success
                    Token = token // include JWT or token string returned by service
                });
            } // end try
            catch (Exception ex) // catch unexpected exceptions during login
            { // start catch
                _logger.LogError(ex, "Error occurred while logging in"); // log exception with context

                return StatusCode(500, new // return 500 Internal Server Error for unexpected cases
                {
                    Success = false, // indicate failure
                    Message = "An unexpected error occurred." // generic client message
                });
            } // end catch
        } // end Login
    } // end class
} // end namespace