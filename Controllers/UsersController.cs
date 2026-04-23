using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// GET all users
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            _logger.LogInformation("Fetching all users");
            var users = _userService.GetAllUsers();
            return Ok(new
            {
                message = "Users retrieved successfully",
                count = users.Count,
                data = users
            });
        }

        /// <summary>
        /// GET a user by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<User> GetUserById(int id)
        {
            _logger.LogInformation("Fetching user with ID {Id}", id);
            var user = _userService.GetUserById(id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found", id);
                return NotFound(new { error = $"User with ID {id} not found" });
            }

            return Ok(new { message = "User retrieved successfully", data = user });
        }

        /// <summary>
        /// POST - Create a new user
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<User> CreateUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for CreateUser");
                return BadRequest(new
                {
                    error = "Validation failed",
                    details = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            // Validate unique email
            if (_userService.EmailExists(user.Email))
            {
                return Conflict(new { error = $"A user with email '{user.Email}' already exists" });
            }

            var createdUser = _userService.CreateUser(user);
            _logger.LogInformation("Created new user with ID {Id}", createdUser.Id);

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = createdUser.Id },
                new { message = "User created successfully", data = createdUser }
            );
        }

        /// <summary>
        /// PUT - Update an existing user
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<User> UpdateUser(int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    error = "Validation failed",
                    details = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            if (!_userService.UserExists(id))
            {
                return NotFound(new { error = $"User with ID {id} not found" });
            }

            // Check if email belongs to another user
            if (_userService.EmailExists(user.Email, excludeId: id))
            {
                return Conflict(new { error = $"Email '{user.Email}' is already in use by another user" });
            }

            var updatedUser = _userService.UpdateUser(id, user);
            _logger.LogInformation("Updated user with ID {Id}", id);

            return Ok(new { message = "User updated successfully", data = updatedUser });
        }

        /// <summary>
        /// DELETE - Remove a user
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteUser(int id)
        {
            if (!_userService.UserExists(id))
            {
                return NotFound(new { error = $"User with ID {id} not found" });
            }

            _userService.DeleteUser(id);
            _logger.LogInformation("Deleted user with ID {Id}", id);

            return Ok(new { message = $"User with ID {id} deleted successfully" });
        }
    }
}
