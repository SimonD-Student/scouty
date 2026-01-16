using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Scouty.Shared.Enums;

namespace Scouty.Backend.Entities;

public class CalendarEventEntity
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }

    // Le créateur (participe d'office)
    public int CreatorId { get; set; }
    public UserEntity Creator { get; set; }

    // Invitations ciblées (ex: juste "Simon" et "Julie")
    public List<EventParticipantEntity> Participants { get; set; } = new();

    // Invitations par section (ex: "Tous les Eclaireurs")
    public List<EventSectionEntity> InvitedSections { get; set; } = new();
}

// Table de liaison Utilisateurs <-> Event
public class EventParticipantEntity
{
    [Key]
    public int Id { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; } // L'invité
    public UserEntity User { get; set; } // Pour récupérer son Totem
}

// Table de liaison Sections <-> Event
public class EventSectionEntity
{
    [Key]
    public int Id { get; set; }
    public int EventId { get; set; }
    public Section Section { get; set; }
}