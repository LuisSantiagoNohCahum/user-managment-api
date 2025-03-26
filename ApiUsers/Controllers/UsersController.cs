using Microsoft.AspNetCore.Authorization;

namespace ApiUsers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
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
        public async Task<IActionResult> GetUser(int id, CancellationToken cancellationToken)
            => Ok(await _userService.GetAsync(id, cancellationToken));


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
