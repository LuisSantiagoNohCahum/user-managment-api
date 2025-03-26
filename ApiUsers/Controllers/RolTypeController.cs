using ApiUsers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiUsers.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RolTypeController : ControllerBase
    {
        private readonly IRolService _rolService;
        public RolTypeController(IRolService rolService)
        {
            _rolService = rolService;
        }

    }
}
