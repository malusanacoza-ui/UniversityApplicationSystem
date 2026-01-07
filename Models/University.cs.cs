using System.ComponentModel.DataAnnotations;

public class University
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Location { get; set; }

    public string Description { get; set; }
}
