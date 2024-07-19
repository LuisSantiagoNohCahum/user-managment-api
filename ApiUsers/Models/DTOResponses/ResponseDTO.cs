using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace ApiUsers.Models.DTOResponses
{
    public class ResponseDTO
    {
        public bool IsSucces { get; set; } = true;
        public object Result { get; set; }
        public string DisplayMessage { get; set; }
        public List<string> Errors { get; set; }

            
    }
}
