namespace ApiUsers.Models.Dto.Request
{
    public class FilterUserDto
    {
        public string? UserName {  get; set; }
        public DateTime CreatedOn { get; set; }
        public int? Type { get; set; }
    }
}
