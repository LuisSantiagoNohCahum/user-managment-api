using ApiUsers.Classes;
using ApiUsers.DataBaseContext;
using ApiUsers.Models.Dto.Request;
using ApiUsers.Models.Dto.Responses;
using ApiUsers.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ApiUsers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserRepository _repository;
        private readonly IConfiguration _configuration;
        public LoginController(GeneralRepositoryContext dbContext, IConfiguration configuration)
        {
            this._repository = new UserRepository(dbContext);
            this._configuration = configuration;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginUserDto _loginDTO)
        {
            if (!CustomValidator.ValidateEmail(_loginDTO.UserName))
            {
                return BadRequest("Ingrese un correo valido.");
            }

            var user = await _repository.GetByUserName(_loginDTO.UserName);

            bool success = true;
            string tokenValue = string.Empty;
            string displayMessage = string.Empty;

            if (user != null)
            {
                if (PasswordHasher.VerifyPassword(_loginDTO.Password, user.Password))
                {
                    tokenValue = JwtToken.GenerateToken(user, _configuration);

                    displayMessage = "Ha iniciado sesión correctamente.";
                    var response = new ResponseDto()
                    {
                        IsSucces = true,
                        DisplayMessage = displayMessage,
                        Result = new { Token = tokenValue, User = user }
                    };
                    return Ok(response);
                }
                else
                {
                    return BadRequest("Usuario o contraseña incorrecta");
                }

            }
            //return response
            return NoContent();
        }

        [Authorize]
        [HttpPost("Logout")]
        public Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();

            var response = new ResponseDto
            {
                IsSucces = true,
                DisplayMessage = "Usuario desconectado",
                Result = "Usuario desconectado"
            };

            return response.IsSucces
                ? Task.FromResult<IActionResult>(Ok(response))
                : Task.FromResult<IActionResult>(BadRequest(response));
        }
    }
}
