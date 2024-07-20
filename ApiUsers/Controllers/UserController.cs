using ApiUsers.Classes;
using ApiUsers.DataBaseContext;
using ApiUsers.Models;
using ApiUsers.Models.Dto.Request;
using ApiUsers.Models.Dto.Responses;
using ApiUsers.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace ApiUsers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //Dependency Injection
        //private readonly GeneralRepositoryContext _dbContext;
        private readonly UserRepository _repository;
        public UserController(GeneralRepositoryContext dbContext)
        {
            //this._dbContext = dbContext;
            this._repository = new UserRepository(dbContext);
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
                var _userTemp = await _repository.GetByUserName(_userDto.UserName);

                if (_userTemp == null)
                {
                    success = await _repository.SaveAsync(user);
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
            if (!CustomValidator.ValidateEmail(_loginDTO.UserName))
            {
                return BadRequest("Ingrese un correo valido.");
            }

            var user = await _repository.GetByUserName(_loginDTO.UserName);

            bool success = true;
            string jwtToken = string.Empty;
            string displayMessage = string.Empty;

            if (user != null)
            {
                if (PasswordHasher.VerifyPassword(_loginDTO.Password, user.Password))
                {
                    displayMessage = "Ha iniciado sesión correctamente.";
                    var response = new ResponseDto()
                    {
                        IsSucces = true,
                        DisplayMessage = displayMessage,
                        Result = user
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

        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            //return response
            return Ok(await _repository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            if (id <= 0)
            {
                return NoContent();
            }
            var user = await _repository.GetByIdAsync(id);

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

            string messageError = string.Empty;

            bool existRequiredFilter = (_filter.UserName != filterTmp.UserName && !string.IsNullOrEmpty(_filter.UserName))
                ||(_filter.Type != filterTmp.Type) 
                || (_filter.CreatedOn != filterTmp.CreatedOn);

            if (!existRequiredFilter)
            {
                messageError = "Debe ingresar al menos un valor en algun filtro.";
            }
            else
            {
                if (!string.IsNullOrEmpty(_filter.UserName) && !CustomValidator.ValidateEmail(_filter.UserName))
                {
                    messageError = "Ingrese un correo valido.";
                }
            }
            bool success = existRequiredFilter && string.IsNullOrEmpty(messageError);

            IEnumerable<Models.User> data = new List<User>();

            if (success) data = await _repository.GetAllByAsync(_filter);

            return success ? 
                Ok(data) : 
                BadRequest(new ResponseDto() { 
                    IsSucces = false, 
                    DisplayMessage = messageError
                });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id > 0)
            {
                bool success = await _repository.DeleteAsync(id);

                return success ? 
                    Ok(new ResponseDto() { IsSucces = true , DisplayMessage = $"Usuario {id} eliminado correctamente"}) : 
                    NotFound();
            }
            else
            {
                return BadRequest("El id debe ser mayor a 0");
            }

            //return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, RequestUserDto entity)
        {
            try
            {
                bool success = false;
                string displayMessage = string.Empty;
                string ErrorMessage = ValidateUser(entity);

                var userToUpdate = await _repository.GetByIdAsync(id);
                if (string.IsNullOrEmpty(ErrorMessage))
                {
                    if (userToUpdate != null)
                    {
                        //No actualizar con correos duplicados
                        userToUpdate.UserName = entity.UserName;
                        userToUpdate.FullName = entity.FullName;
                        userToUpdate.RolType = entity.RolType;
                        userToUpdate.Password = PasswordHasher.HashPassword(entity.Password);
                        userToUpdate.UpdatedOn = DateTime.Now;
                        success = await _repository.SaveAsync(userToUpdate);
                        displayMessage = $"Usuario {userToUpdate.Id} actualizado correctamente.";
                    }
                    else
                    {
                        displayMessage = "No se encontro el usuario que desea actualizar";
                    }
                }
                else
                {
                    displayMessage = ErrorMessage;
                }

                var response = new ResponseDto() { IsSucces = success, DisplayMessage = displayMessage };

                return success ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                //response false ex.message ex.stacktrace
                return StatusCode((int) HttpStatusCode.InternalServerError, $"Ocurrio un error al intentar actualizar: {ex.Message}");
            }
            
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

        //set statuc code in try catch process
    }
}
