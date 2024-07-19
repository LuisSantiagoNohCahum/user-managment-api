namespace ApiUsers.Models.Dto.Responses
{
    public class ResponseDto
    {
        public bool IsSucces { get; set; } = true;
        public object Result { get; set; }
        public string DisplayMessage { get; set; }
        public List<string> Errors { get; set; }


    }
}
