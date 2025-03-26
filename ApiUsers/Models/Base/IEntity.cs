namespace ApiUsers.Models.Base
{
    public interface IEntity
    {
        int Id { get; set; }
        DateTime? CreatedOn { get; set; }
        string? CreatedBy { get; set; }
        DateTime? UpdatedOn { get; set; }
        string? UpdatedBy { get; set; }
    }
}
