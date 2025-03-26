namespace ApiUsers.Models.Requests
{
    public class GetAllRequest
    {
        public required string Status { get; set; }
        public required string Email { get; set; }
        public required DateTime CreatedOn { get; set; }
        public required int RolId { get; set; }
    }
}
