namespace ApiUsers.Models.Requests.Uers
{
    public class InsertRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required bool IsActive { get; set; }
        public required int RolId { get; set; }
    }
}
