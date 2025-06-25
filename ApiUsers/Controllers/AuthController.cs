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
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            string token = await _loginService.LoginAsync(request, cancellationToken);

            var response = ApiResponse<object>.SuccessResponse(new { Token = token });

            return Ok(response);
        }
    }
}
