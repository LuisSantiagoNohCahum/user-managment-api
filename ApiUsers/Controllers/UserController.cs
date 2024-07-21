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
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Authorize = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace ApiUsers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserRepository _repository;
        public UserController(GeneralRepositoryContext dbContext)
        {
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

            ResponseDto response = new ResponseDto() { IsSuccess = success, DisplayMessage = displayMessage, Result = "" };

            return success ? Ok(response) : BadRequest(response);
        }


        [Authorize]
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetUsers()
        {
            //return response
            return Ok(await _repository.GetAllAsync());
        }

        [Authorize]
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

        [Authorize]
        [HttpPost]
        [Route("GetBy")]
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
                    IsSuccess = false, 
                    DisplayMessage = messageError
                });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id > 0)
            {
                bool success = await _repository.DeleteAsync(id);

                return success ? 
                    Ok(new ResponseDto() { IsSuccess = true , DisplayMessage = $"Usuario {id} eliminado correctamente"}) : 
                    NotFound();
            }
            else
            {
                return BadRequest("El id debe ser mayor a 0");
            }

            //return NoContent();
        }

        [Authorize]
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

                var response = new ResponseDto() { IsSuccess = success, DisplayMessage = displayMessage };

                return success ? Ok(response) : BadRequest(response);
            }
            catch (Exception ex)
            {
                //response false ex.message ex.stacktrace
                return StatusCode((int) HttpStatusCode.InternalServerError, $"Ocurrio un error al intentar actualizar: {ex.Message}");
            }
            
        }

        [Authorize]
        [HttpPost]
        [Route("BulkByExcelLayout")]
        public async Task<IActionResult> BulkByExcelLayout(IFormFile _file)
        {
            try
            {
                string messageError = string.Empty;
                //validate every object email and password
                List<User> users= (await ExcelHelper.SerializeToObject<User>( _file )).ToList();

                int cellIndex = 2;
                foreach (User _user in users)
                {
                    RequestUserDto _userDto = new RequestUserDto()
                    { 
                        UserName = _user.UserName,
                        FullName = _user.FullName,
                        Password = _user.Password,
                        RolType = _user.RolType
                    };

                    messageError = ValidateUser(_userDto);

                    if (string.IsNullOrEmpty(messageError))
                    {
                        User _userInDb = await _repository.GetByUserName(_userDto.UserName);

                        users[cellIndex - 2].Password = PasswordHasher.HashPassword(_userDto.Password);
                        users[cellIndex - 2].CreatedOn = DateTime.Now;

                        if (_userInDb != null) 
                            messageError = $"Error en la fila {cellIndex}: Ya existe un usuario registrado con el correo {_userDto.UserName}.";
                    }
                    else 
                        messageError = $"Error en el formato de la fila {cellIndex}: {messageError}";
                        
                    if (!string.IsNullOrEmpty(messageError)) throw new Exception(messageError);

                    cellIndex++;
                }

                //si todo esta ok se inserta
                bool success = await _repository.BulkInsertAsync(users);

                var response = new ResponseDto()
                {
                    IsSuccess = success,
                    DisplayMessage = $"Se importaron {cellIndex - 2} registros"
                };

                return Ok(response);
                //response num insrted register
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDto() { 
                    IsSuccess = false,
                    DisplayMessage = ex.Message,
                    Errors = new List<string> { ex.StackTrace ?? "An error has ocurred" }
                });
            }
        }

        [Authorize]
        [HttpGet("ExportToExcel/{filename}")]
        public async Task<IActionResult> ExportToExcel(string filename)
        {
            //add repo or class to manage the files and the excel getter data
            throw new NotImplementedException();
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


        /*
         * Ok
         * BadRequest
         * NotFound
         * NoContent
         * Content
         * StatusCode
         */
    }
}
