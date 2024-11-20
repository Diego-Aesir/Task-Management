using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Interface;
using UserManagementAPI.DTO;

namespace UserManagementAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IUserServices _userServices;
        public UserController(IUserServices userServices) { 
            _userServices = userServices;
        }

        [HttpGet("{userId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetUser(string userId)
        {
            var user = await _userServices.GetUserASync(userId);

            if (user == null)
            {
                return NotFound("User couldn't be found");
            }

            return Ok(user);
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RegisterUser([FromBody] UserRequest user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"User information is incomplete. {ModelState}");
            }

            User newUser = new User
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            try
            {
                var userResponse = await _userServices.RegisterUserAsync(newUser, user.Password);
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> LoginUser([FromBody] LoginUserRequest user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"User information is incomplete. {ModelState}");
            }

            var userLogged = await _userServices.AuthenticateUserAsync(user.username, user.password);
            if(userLogged == null)
            {
                return BadRequest("User not couldn't be authenticated");
            }

            return Ok(userLogged);
        }

        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdatedUser([FromBody] UpdateUserRequest user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"User information is incomplete. {ModelState}");
            }

            User updatedUser = new User
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            try
            {
                var updated = await _userServices.UpdateUserAsync(updatedUser, user.Password);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteUser([FromBody] DeleteUserRequest deleteUserRequest)
        {
            if(string.IsNullOrEmpty(deleteUserRequest.Id))
            {
                return BadRequest("User ID is required.");
            };
            
            var userToBeDeleted = new User { Id = deleteUserRequest.Id, FirstName = "", LastName = ""};
            try
            {
                var result = await _userServices.DeleteUserAsync(userToBeDeleted);

                if (result)
                {
                    return Ok("User deleted successfully.");
                }
                else
                {
                    return StatusCode(500, "Failed to delete user.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting user: {ex.Message}");
            }
        }
    }
}
