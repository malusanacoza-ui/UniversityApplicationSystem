using System.ComponentModel.DataAnnotations;

public class Application
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public int UniversityId { get; set; }

    public DateTime ApplicationDate { get; set; } = DateTime.Now;

    public string Status { get; set; } = "Pending";
}
