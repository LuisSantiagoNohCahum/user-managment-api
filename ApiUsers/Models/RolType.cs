namespace ApiUsers.Models
{
    public class RolType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; } //= DateTime.Now;
    }
}
