namespace ApiUsers.Models.Common
{
    public class Filter
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public required string Operator { get; set; }
        public required string Value { get; set; }
    }
}
