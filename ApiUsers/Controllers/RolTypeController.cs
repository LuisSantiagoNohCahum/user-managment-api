using ApiUsers.DataBaseContext;
using ApiUsers.Models.DTORequest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiUsers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolTypeController : ControllerBase
    {
        private readonly GeneralRepositoryContext _dbContext;
        public RolTypeController(GeneralRepositoryContext dbContext)
        {
            this._dbContext = dbContext;
        }

        [HttpPost]
        [Route("Role")]
        public async Task<IActionResult> Create(RolType_DTO _role)
        { 
            if (!ModelState.IsValid) 
            {
                return NoContent();
            }

            if (string.IsNullOrEmpty(_role.Name) || string.IsNullOrEmpty(_role.Type))
            {
                return BadRequest("Debe ingresar valores validos.");
            }

            var roleType = new Models.RolType() { Name = _role.Name, Type = _role.Type, CreatedOn = DateTime.Now };

            var roleTemp = await _dbContext.RolTypes.FirstOrDefaultAsync(x => x.Type == _role.Type);

            if (roleTemp == null) 
            {
                _dbContext.RolTypes.Add(roleType);
                _dbContext.SaveChanges();

                return Ok("Elemento guardado correctamente.");
            }

            return BadRequest("Ya existe un rol con la clave de tipo espefificada.");
        }
    }
}
