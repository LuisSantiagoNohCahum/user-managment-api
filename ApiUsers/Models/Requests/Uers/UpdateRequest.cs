namespace ApiUsers.Models.Requests.Uers
{
    public class UpdateRequest
    {
        public required int Id { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required bool IsActive { get; set; }
        public required int RolId { get; set; }
    }
}
