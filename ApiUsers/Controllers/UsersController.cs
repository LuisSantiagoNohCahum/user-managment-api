using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        [HttpPost("InsertGuest")]
        public async Task<IActionResult> InsertGuest(SignUpRequest request, CancellationToken cancellationToken)
        {
            bool inserted = await _userService.InsertGuestAsync(request, cancellationToken);

            var response = ApiResponse<string>.SuccessResponse("Usuario creado correctamente");

            return Ok(response);
        }

        [Authorize]
        [HttpGet()]
        public async Task<IActionResult> GetAll([FromQuery] GetAllRequest request, CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllAsync(request, cancellationToken);

            var response = ApiResponse<IEnumerable<UserDto>>.SuccessResponse(users);

            return Ok(response);   
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetAsync(id, cancellationToken);
            
            var response = ApiResponse<UserDto>.SuccessResponse(user);

            return Ok(response);
        }

        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> Insert(InsertRequest request, CancellationToken cancellationToken)
        {
            var insertedId = await _userService.InsertAsync(request, cancellationToken);

            var response = ApiResponse<int>.SuccessResponse(insertedId);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateRequest request, CancellationToken cancellationToken)
        {
            var updated = await _userService.UpdateAsync(id, request, cancellationToken);

            var response = ApiResponse<int>.SuccessResponse(updated);

            return Ok(response);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            int deleted = await _userService.DeleteAsync(id, cancellationToken);

            var response = ApiResponse<int>.SuccessResponse(deleted);

            return  Ok(response);
        }

        [Authorize]
        [HttpPost("ImportFromFile")]
        public async Task<IActionResult> ImportFromFile([FromForm] ImportFromFileRequest request, CancellationToken cancellationToken)
        {
            var count = await _userService.ImportFromFileAsync(request, cancellationToken);

            var response = ApiResponse<string>.SuccessResponse($"Se importaron {count} usuarios.");

            return Ok(response);
        }
         


        [Authorize]
        [HttpGet("ExportToExcel")]
        public async Task<IActionResult> ExportToExcel([FromQuery] GetAllRequest request, CancellationToken cancellationToken)
        {
            var fileUrl = await _userService.ExportToExcelAsync(request, cancellationToken);

            var response = ApiResponse<string>.SuccessResponse(fileUrl);

            return Ok(response);
        }
        
    }
}
