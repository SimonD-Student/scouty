using Scouty.Shared.Enums;
using System.ComponentModel.DataAnnotations; // Permet de valider les données automatiquement

namespace Scouty.Shared.DTOs;

public class RegisterRequestDto
{
    [Required]
    public string Firstname { get; set; } = string.Empty;

    [Required]
    public string Lastname { get; set; } = string.Empty;

    public string Totem { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public Section Section { get; set; }

    public bool IsChef { get; set; }
}