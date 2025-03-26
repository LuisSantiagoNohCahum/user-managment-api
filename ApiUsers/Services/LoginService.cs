namespace ApiUsers.Services
{
    public class LoginService : ILoginService
    {
        private readonly IJwtService _jwtService;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherHelper _passwordHasherHelper;

        public LoginService(IJwtService jwtService, IUserRepository userRepository, IPasswordHasherHelper passwordHasherHelper)
        {
            _jwtService = jwtService;
            _userRepository = userRepository;
            _passwordHasherHelper = passwordHasherHelper;
        }

        public async Task<string> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(u => !string.IsNullOrEmpty(u.Email) && u.Email.Equals(request), cancellationToken);

            if (user is null || !IsValidPassword(request, user))
                throw new Exception("Email or password is wrong, try again.");

            return await _jwtService.Generate(user);
        }

        private bool IsValidPassword(LoginRequest request, User user)
            => !string.IsNullOrEmpty(user.Password) && _passwordHasherHelper.VerifyHashedPassword(request.Password, user.Password);
    }
}
