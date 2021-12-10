using ITManagerProject.Models;

namespace ITManagerProject.HelperTypes;

public class ApplicationWithDetails
{
    public Application Application { get; set; }
    public User User { get; set; }
    public Offer Offer { get; set; }
        
}