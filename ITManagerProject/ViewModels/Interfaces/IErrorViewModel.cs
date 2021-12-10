namespace ITManagerProject.ViewModels.Interfaces;

public interface IErrorViewModel
{
    string RequestId { get; set; }
    bool ShowRequestId { get; }
}