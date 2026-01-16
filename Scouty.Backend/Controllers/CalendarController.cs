using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scouty.Backend.Data;
using Scouty.Backend.Entities;
using Scouty.Shared.DTOs;

namespace Scouty.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CalendarController : ControllerBase
{
    private readonly ScoutyDbContext _context;

    public CalendarController(ScoutyDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<CalendarEventDto>>> GetMyEvents()
    {
        var user = await GetCurrentUser();
        if (user == null) return Unauthorized();

        // LOGIQUE DE FILTRAGE :
        // Je vois l'event SI :
        // 1. Je suis le créateur
        // 2. OU je suis dans la liste des participants directs
        // 3. OU ma section est dans la liste des sections invitées
        var events = await _context.CalendarEvents
            .Include(e => e.Creator)
            .Include(e => e.Participants).ThenInclude(p => p.User)
            .Include(e => e.InvitedSections)
            .Where(e =>
                e.CreatorId == user.Id ||
                e.Participants.Any(p => p.UserId == user.Id) ||
                e.InvitedSections.Any(s => s.Section == user.Section)
            )
            .OrderBy(e => e.StartDate)
            .ToListAsync();

        // Mapping vers DTO
        var dtos = events.Select(e => new CalendarEventDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            StartDate = e.StartDate,
            CreatorName = !string.IsNullOrEmpty(e.Creator.Totem) ? e.Creator.Totem : e.Creator.Firstname,
            ParticipantsDisplay = GenerateParticipantsString(e)
        }).ToList();

        return Ok(dtos);
    }

    [HttpPost]
    public async Task<ActionResult> CreateEvent([FromBody] CreateEventDto dto)
    {
        var user = await GetCurrentUser();

        var newEvent = new CalendarEventEntity
        {
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate.ToUniversalTime(),
            CreatorId = user.Id
        };

        // Ajout des sections
        foreach (var sec in dto.Sections)
            newEvent.InvitedSections.Add(new EventSectionEntity { Section = sec });

        // Ajout des utilisateurs spécifiques
        foreach (var uid in dto.UserIds)
            newEvent.Participants.Add(new EventParticipantEntity { UserId = uid });

        _context.CalendarEvents.Add(newEvent);
        await _context.SaveChangesAsync();
        return Ok();
    }

    // Récupérer tous les utilisateurs pour pouvoir les inviter
    [HttpGet("users")]
    public async Task<ActionResult<List<UserSelectDto>>> GetAllUsers()
    {
        var users = await _context.Users.Select(u => new UserSelectDto
        {
            Id = u.Id,
            DisplayName = !string.IsNullOrEmpty(u.Totem) ? u.Totem : u.Firstname
        }).ToListAsync();
        return Ok(users);
    }

    private Task<UserEntity?> GetCurrentUser()
    {
        var email = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
        return _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    private string GenerateParticipantsString(CalendarEventEntity e)
    {
        var parts = new List<string>();
        if (e.InvitedSections.Any())
            parts.Add("Sections: " + string.Join(", ", e.InvitedSections.Select(s => s.Section.ToString())));

        if (e.Participants.Any())
            parts.Add("Scouts: " + string.Join(", ", e.Participants.Select(p => !string.IsNullOrEmpty(p.User.Totem) ? p.User.Totem : p.User.Firstname)));

        return string.Join(" | ", parts);
    }
}