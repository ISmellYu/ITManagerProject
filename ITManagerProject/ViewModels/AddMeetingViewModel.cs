using System;
using System.ComponentModel.DataAnnotations;

namespace ITManagerProject.ViewModels;

public class AddMeetingViewModel
{
    [Required(ErrorMessage = "Musisz podać tytuł spotkania")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Musisz podać opis spotkania")]
    public string Description { get; set; }
    [Required(ErrorMessage = "Musisz podać lokalizację spotkania")]
    public string Location { get; set; }
    [Required(ErrorMessage = "Musisz podać datę rozpoczecia spotkania")]
    public DateTime Start { get; set; }
    [Required(ErrorMessage = "Musisz podać datę zakończenia spotkania")]
    public DateTime End { get; set; }
    [Required(ErrorMessage = "Musisz wybrac kolor spotkania")]
    public string Color { get; set; }
    [Required(ErrorMessage = "Wybierz czy spotkanie bedzie calodniowe")]
    public bool IsFullDay { get; set; }
}