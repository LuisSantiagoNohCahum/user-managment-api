namespace ApiUsers.Models.Common
{
    public class ApiException : Exception
    {
        public string DisplayMessage { get; set; }
        public ApiException(string displayMessage) : base(displayMessage)
        {
            DisplayMessage = displayMessage;
        }

        public ApiException(string displayMessage, Exception innerException) : base(displayMessage, innerException)
        {
            DisplayMessage = displayMessage;
        }
    }
}
