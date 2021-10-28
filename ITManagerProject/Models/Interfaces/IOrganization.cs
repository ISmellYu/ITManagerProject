namespace ITManagerProject.Models.Interfaces
{
    public interface IOrganization
    {
        int Id { get; set; }
        string Name { get; set; }
        string NormalizedName { get; set; }
        string ConcurrencyStamp { get; set; }
        string ToString();
    }
}