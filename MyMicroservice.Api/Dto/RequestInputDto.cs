
using System.ComponentModel.DataAnnotations;

namespace MyMicroservice.Api.Dto;

public record RequestInputDto(
    [Required(ErrorMessage = "El nombre es obligatorio")] string Name, 
    [Required(ErrorMessage = "El payload no puede estar vacío")] string Payload
);