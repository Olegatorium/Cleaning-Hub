using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using RepositoryContracts;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenRepository _tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequestDto)
        {
            registerRequestDto.Role = registerRequestDto.Role.ToLower();

            if (registerRequestDto.Role != "client" && registerRequestDto.Role != "company") 
            {
                return BadRequest("Something went wrong");
            }

            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Name,
                Email = registerRequestDto.Email,
                PhoneNumber = registerRequestDto.PhoneNumber
            };

            var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (identityResult.Succeeded)
            {
                identityResult = await _userManager.AddToRolesAsync(identityUser, new[] { registerRequestDto.Role});

                if (identityResult.Succeeded)
                {
                    return Ok("User was registred! Please login.");
                }
            }

            var errors = identityResult.Errors.Select(e => e.Description);
            return BadRequest(new { Message = "Registration failed.", Errors = errors });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);

            bool isPasswordCorrect = false;

            if (user != null)
            {
                isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            }

            if (isPasswordCorrect && user != null)
            {
                IList<string>? roles = await _userManager.GetRolesAsync(user);

                if (roles != null)
                {
                    //Create Token

                    string jwtToken = _tokenRepository.CreateJWTToken(user, roles.ToList());

                    LoginResponse response = new LoginResponse
                    {
                        JwtToken = jwtToken
                    };

                    return Ok(response);
                }
            }

            return BadRequest("Email or password incorrect");
        }
    }
}
