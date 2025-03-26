namespace ApiUsers.Models.Requests
{
    public class SignUpRequest
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }

        //TODO. Two factor code when first signup
    }
}
