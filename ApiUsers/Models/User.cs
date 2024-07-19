namespace ApiUsers.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int IsActive { get; set; } = 1;

        public DateTime CreatedOn { get; set; } //= DateTime.Now;

        public DateTime UpdatedOn { get; set;}

        public int RolType { get; set; } = 0;
    }
}
