namespace ApiUsers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _loginService;
        public AuthController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginRequest request, [FromServices] IValidator<LoginRequest> validator, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (validationResult.IsValid)
            {
                return Ok(new
                {
                    Token = await _loginService.LoginAsync(request, cancellationToken)
                });
            }

            return BadRequest(GetResponseFromWrongValidation(validationResult));
        }

        private ApiResponse<string> GetResponseFromWrongValidation(ValidationResult validationResult)
            => ApiResponse<string>.FailResponse(validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
    }
}
