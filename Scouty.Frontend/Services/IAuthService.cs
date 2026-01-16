using Scouty.Shared.DTOs; // Important pour utiliser les DTOs

namespace Scouty.Frontend.Services;

public interface IAuthService
{
    // On garde bool pour simplifier le ViewModel pour l'instant
    Task<bool> LoginAsync(string email, string password);

    // On passe maintenant les bons paramètres ou le DTO directement
    Task<bool> RegisterAsync(string firstname, string lastname, string email, string password, string totem, Shared.Enums.Section section, bool isChef);
}