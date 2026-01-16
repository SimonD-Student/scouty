using Scouty.Shared.Enums; 
using System.ComponentModel.DataAnnotations;

namespace Scouty.Backend.Entities;

public class UserEntity
{
    [Key]
    public int Id { get; set; } // Clé primaire auto-incrémentée

    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;

    public string? Totem { get; set; }

    [Required]
    public string Email { get; set; } = string.Empty;

    // ATTENTION : Ici c'est le HASH, pas le mot de passe brut
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    // On stocke l'enum sous forme de texte ou d'entier dans la DB (EF gère ça)
    public Section Section { get; set; }

    public bool IsChef { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}