using System;

namespace ITManagerProject.Models;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; }
    public string Color { get; set; }
    public bool IsFullDay { get; set; }
}