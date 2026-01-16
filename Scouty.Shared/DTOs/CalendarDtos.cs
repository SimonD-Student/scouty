using Scouty.Shared.Enums;

namespace Scouty.Shared.DTOs;

public class CalendarEventDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public string CreatorName { get; set; } // Totem du créateur

    // Liste des participants (Noms/Totems concaténés pour l'affichage)
    public string ParticipantsDisplay { get; set; }
}

public class CreateEventDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }

    public List<int> UserIds { get; set; } = new(); // Liste des IDs des scouts invités
    public List<Section> Sections { get; set; } = new(); // Liste des sections invitées
}

// Pour afficher la liste des utilisateurs lors de la création
public class UserSelectDto
{
    public int Id { get; set; }
    public string DisplayName { get; set; } // Totem ou Prénom
    public bool IsSelected { get; set; } // Pour la CheckBox
}