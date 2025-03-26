using ApiUsers.Interfaces;
using ApiUsers.Models.Common;
using ApiUsers.Models.Requests;
using ApiUsers.Models.Requests.Uers;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Authorize = Microsoft.AspNetCore.Authorization.AuthorizeAttribute;

namespace ApiUsers.Controllers
{
    //TODO. Change name to UsersController
    //SingUp Response "User successfully registered" || "There is already a registered user with the same email address" or enable the find by email

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService, 
            IValidator<SignUpRequest> signUpValidator)
        {
            _userService = userService;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpRequest request, [FromServices] IValidator<SignUpRequest> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);

            return validationResult.IsValid
                ? Ok(await _userService.SignUpAsync(request, cancellationToken))
                : BadRequest(GetResponseFromWrongValidation(validationResult));
        }

        [Authorize]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(GetAllRequest request, [FromServices] IValidator<GetAllRequest> validator,  CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);

            return validationResult.IsValid
                ? Ok(await _userService.GetAllAsync(request, cancellationToken))
                : BadRequest(GetResponseFromWrongValidation(validationResult));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id, CancellationToken ct)
            => Ok(await _userService.GetAsync(id, ct));


        //TODO. Validate rol, only admin rol, can be insert new complete user.
        [Authorize]
        [HttpPost("Insert")]
        public async Task<IActionResult> Insert(InsertRequest request, [FromServices] IValidator<InsertRequest> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);

            return validationResult.IsValid
                ? Ok(await _userService.InsertAsync(request, cancellationToken))
                : BadRequest(GetResponseFromWrongValidation(validationResult));
        }

        [Authorize]
        [HttpPost("Update")]
        public async Task<IActionResult> Update(UpdateRequest request, IValidator<UpdateRequest> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);

            return validationResult.IsValid
                ? Ok(await _userService.UpdateAsync(request, cancellationToken))
                : BadRequest(GetResponseFromWrongValidation(validationResult));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
             => Ok(await _userService.DeleteAsync(id, cancellationToken));

        [Authorize]
        [HttpPost("BulkByExcelLayout")]
        public async Task<IActionResult> BulkByExcelLayout(IFormFile layout, CancellationToken cancellationToken)
            => Ok(await _userService.ImportFromExcelAsync(layout, cancellationToken));

        [Authorize]
        [HttpGet("Export")]
        public async Task<IActionResult> ExportToExcel(CancellationToken cancellationToken)
            => Ok(await _userService.ExportToExcelAsync(cancellationToken));

        private ApiResponse<string> GetResponseFromWrongValidation(ValidationResult validationResult)
            => ApiResponse<string>.FailResponse(validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
    }
}
