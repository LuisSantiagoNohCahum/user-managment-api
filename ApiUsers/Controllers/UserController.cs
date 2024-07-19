using ApiUsers.Classes;
using ApiUsers.DataBaseContext;
using ApiUsers.Models;
using ApiUsers.Models.Dto.Request;
using ApiUsers.Models.Dto.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiUsers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //Dependency Injection
        private readonly GeneralRepositoryContext _dbContext;
        public UserController(GeneralRepositoryContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Registration(RequestUserDto _userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool success = true;
            string displayMessage = string.Empty;
            string ErrorMessage = ValidateUser(_userDto);

            if (string.IsNullOrEmpty(ErrorMessage))
            {
                Models.User user = new Models.User()
                {
                    FullName = _userDto.FullName,
                    UserName = _userDto.UserName,
                    Password = PasswordHasher.HashPassword(_userDto.Password),
                    CreatedOn = DateTime.Now
                };

                //TODO. ASYNCRONUS PROCESS
                var _userTemp = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == _userDto.UserName);

                if (_userTemp == null)
                {
                    _dbContext.Users.Add(user);
                    await _dbContext.SaveChangesAsync();
                    displayMessage = "Usuario registrado correctamente.";
                }
                else
                {
                    displayMessage = "Ya existe un usuario registrado con este correo.";
                }
            }
            else
            {
                success = false;
                displayMessage = ErrorMessage;
            }

            ResponseDto response = new ResponseDto() { IsSucces = success, DisplayMessage = displayMessage, Result = "" };

            return success ? Ok(response) : BadRequest(response);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginUserDto _loginDTO)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == _loginDTO.UserName);

            bool success = true;
            string jwtToken = string.Empty;
            string displayMessage = string.Empty;

            if (user != null)
            {
                if (PasswordHasher.VerifyPassword(_loginDTO.Password, user.Password))
                {
                    displayMessage = "Ha iniciado sesión correctamente.";
                }
                return Ok(user);
            }
            //return response
            return NoContent();
        }

        [HttpGet]
        [Route("GetUsers")]
        public IActionResult GetUsers()
        {
            //return response
            return Ok(_dbContext.Users.ToList().FindAll(x => x.IsActive == 1));
        }

        [HttpGet]
        [Route("GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            if (id <= 0)
            {
                return NoContent();
            }
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

            //return response
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpPost]
        [Route("GetUsersBy")]
        public async Task<IActionResult> GetUsersBy(FilterUserDto _filter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            FilterUserDto filterTmp = new FilterUserDto();
            var users = _dbContext.Users.AsQueryable();

            if (_filter.UserName != filterTmp.UserName && !string.IsNullOrEmpty(_filter.UserName))
            {
                if (!CustomValidator.ValidateEmail(_filter.UserName)) return BadRequest("Ingrese un correo valido.");
                users = users.Where(x => x.UserName == _filter.UserName);
            }//else para messageerror para filtros obligatorios

            if (_filter.Type != filterTmp.Type)
            {
                users = users.Where(x => x.RolType == _filter.Type);
            }

            if (_filter.CreatedOn != filterTmp.CreatedOn)
            {
                users = users.Where(x => x.CreatedOn.Date.Equals(_filter.CreatedOn.Date));
            }

            var data = await users.ToListAsync();

            return Ok(data);

        }
        private static string ValidateUser(RequestUserDto _userDTO)
        {
            string MsgValidation = string.Empty;
            if (string.IsNullOrEmpty(_userDTO.FullName) || _userDTO.FullName == "string") MsgValidation = "Por favor, ingrese su Nombre Completo.";
            else if (!CustomValidator.ValidateEmail(_userDTO.UserName) || _userDTO.UserName == "string") MsgValidation = "Por favor, ingrese un correo electronico valido como usuario.";
            else if (!CustomValidator.ValidatePassword(_userDTO.Password) || _userDTO.Password == "string") MsgValidation = "Por favor, ingrese una contraseña valida de 10 caracteres (3 minusculas, 3 mayusculas, 2 numeros y 2 caracteres especiales)";
            return MsgValidation;
        }

        //get user by id and user name
    }
}
