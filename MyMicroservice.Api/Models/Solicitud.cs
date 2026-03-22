using System.ComponentModel.DataAnnotations;

namespace MyMicroservice.Api.Models;

public class Solicitud {
    [Key]
    public Guid Id { get; set; }

    [Required] 
    public string Name { get; set; } = string.Empty; 

    [Required]
    public string Payload { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}