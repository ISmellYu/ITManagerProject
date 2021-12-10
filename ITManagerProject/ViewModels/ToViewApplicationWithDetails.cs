using ITManagerProject.Models;

namespace ITManagerProject.ViewModels;

public class ToViewApplicationWithDetails
{
    public Application Application { get; set; }
    public Offer Offer { get; set; }
    public ToViewUser User { get; set; }
}