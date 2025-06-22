using Microsoft.AspNetCore.Authorization;

namespace ApiUsers.Controllers
{
    // TODO. Add the auth attribute here to apply to all endpoints
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // TODO. Move to account controller
        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpRequest request, [FromServices] IValidator<SignUpRequest> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);

            return validationResult.IsValid
                ? Ok(await _userService.SignUpAsync(request, cancellationToken))
                : BadRequest(GetResponseFromWrongValidation(validationResult));
        }

        // TODO. Query string parameters [FromQuery] for each parameter/binding model or [FromUri] to wrap the query in a object
        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetAll([FromQuery] GetAllRequest request, [FromServices] IValidator<GetAllRequest> validator,  CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);

            return validationResult.IsValid
                ? Ok(await _userService.GetAllAsync(request, cancellationToken))
                : BadRequest(GetResponseFromWrongValidation(validationResult));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetAsync(id, cancellationToken);

            if (user is null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> Insert(InsertRequest request, [FromServices] IValidator<InsertRequest> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);

            return validationResult.IsValid
                ? Ok(await _userService.InsertAsync(request, cancellationToken))
                : BadRequest(GetResponseFromWrongValidation(validationResult));
        }

        // Update/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateRequest request, IValidator<UpdateRequest> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);

            return validationResult.IsValid
                ? Ok(await _userService.UpdateAsync(id, request, cancellationToken))
                : BadRequest(GetResponseFromWrongValidation(validationResult));
        }

        // TODO. Path parameters
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
             => Ok(await _userService.DeleteAsync(id, cancellationToken));

        [Authorize]
        [HttpPost("ImportByExcel")]
        public async Task<IActionResult> BulkByExcelLayout(IFormFile layout, CancellationToken cancellationToken)
            => Ok(await _userService.ImportFromExcelAsync(layout, cancellationToken));

        // TODO. Return a url for static file.
        [Authorize]
        [HttpGet("ExportToExcel")]
        public async Task<IActionResult> ExportToExcel(CancellationToken cancellationToken)
            => Ok(await _userService.ExportToExcelAsync(cancellationToken));

        private ApiResponse<string> GetResponseFromWrongValidation(ValidationResult validationResult)
            => ApiResponse<string>.FailResponse(validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
    }
}
