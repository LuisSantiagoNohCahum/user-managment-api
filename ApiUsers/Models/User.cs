using System.ComponentModel.DataAnnotations;

namespace ApiUsers.Models
{
    public class User : Entity
    {
        // TODO. Add the rol relationship as a SQL relation.

        // TODO. Add account table model.
        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(200)]
        public string Password { get; set; }

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        public bool IsActive { get; set; } //= 1;

        public int RolId { get; set; } //= 0;
    }
}
