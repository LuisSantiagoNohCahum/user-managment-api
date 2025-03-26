using ApiUsers.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace ApiUsers.Models
{
    public class Rol : Entity
    {
        [MaxLength(50)]
        public string? Code { get; set; }

        [MaxLength(100)]
        public string? Name { get; set; }
    }
}
