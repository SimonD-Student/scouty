namespace Scouty.Shared.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty; // Le JWT
    public string UserEmail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    public bool IsChef { get; set; }
}