using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiUsers.Models.Base
{
    public class Entity : IEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime? CreatedOn { get; set; }

        [MaxLength(200)]
        public string? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        [MaxLength(200)]
        public string? UpdatedBy { get; set; }
    }
}
