namespace ApiUsers.Models.Requests
{
    public class GetAllRequest
    {
        public string Status { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }
        public int RolId { get; set; }
    }
}
