using System.ComponentModel.DataAnnotations;

namespace ITManagerProject.ViewModels
{
    public class ApplicationViewModel
    {
        [Required]
        public string Cv { get; set; }
    }
}