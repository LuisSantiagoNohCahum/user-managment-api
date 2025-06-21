namespace ApiUsers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request, [FromServices] IValidator<LoginRequest> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            return validationResult.IsValid
                ? Ok(new { Token = await _loginService.LoginAsync(request, cancellationToken) })
                : BadRequest(GetResponseFromWrongValidation(validationResult));
        }

        private ApiResponse<string> GetResponseFromWrongValidation(ValidationResult validationResult)
            => ApiResponse<string>.FailResponse(validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
    }
}
