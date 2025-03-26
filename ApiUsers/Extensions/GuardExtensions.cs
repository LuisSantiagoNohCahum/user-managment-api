namespace ApiUsers.Extensions
{
    public static class GuardExtensions
    {
        public static void Guard(this string? source, string fieldName, string? message = null)
        {
            if (source is null)
            {
                throw new ArgumentNullException(message ?? $"The value of the {fieldName} field cannot be null.");
            }

            if (string.IsNullOrEmpty(source))
            {
                throw new Exception(message ?? $"The value of the {fieldName} field cannot be empty.");
            }
        }
    }
}
