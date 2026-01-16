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
public class EquipmentController : ControllerBase
{
    private readonly ScoutyDbContext _context;

    public EquipmentController(ScoutyDbContext context)
    {
        _context = context;
    }

    // Récupérer tout le matériel de la section
    [HttpGet]
    public async Task<ActionResult<List<EquipmentCategoryDto>>> GetEquipment()
    {
        var user = (await _context.Users.FirstOrDefaultAsync(u => u.Email == User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value));
        if (user == null) return Unauthorized();

        var categories = await _context.EquipmentCategories
            .Where(c => c.Section == user.Section)
            .Include(c => c.Items)
            .Select(c => new EquipmentCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Items = c.Items.Select(i => new EquipmentItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Quantity = i.Quantity,
                    CategoryId = i.CategoryId
                }).ToList()
            })
            .ToListAsync();

        return Ok(categories);
    }

    // Créer une catégorie (ex: "Tentes")
    [HttpPost("category")]
    public async Task<ActionResult> CreateCategory([FromBody] string name)
    {
        var user = (await _context.Users.FirstOrDefaultAsync(u => u.Email == User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value));

        var category = new EquipmentCategoryEntity { Name = name, Section = user.Section };
        _context.EquipmentCategories.Add(category);
        await _context.SaveChangesAsync();
        return Ok();
    }

    // Supprimer une catégorie
    [HttpDelete("category/{id}")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        var cat = await _context.EquipmentCategories.FindAsync(id);
        if (cat == null) return NotFound();

        _context.EquipmentCategories.Remove(cat);
        await _context.SaveChangesAsync();
        return Ok();
    }

    // Ajouter un objet dans une catégorie
    [HttpPost("item")]
    public async Task<ActionResult> AddItem([FromBody] CreateEquipmentItemDto dto)
    {
        var item = new EquipmentItemEntity
        {
            Name = dto.Name,
            Quantity = dto.Quantity,
            CategoryId = dto.CategoryId
        };
        _context.EquipmentItems.Add(item);
        await _context.SaveChangesAsync();
        return Ok();
    }

    // Supprimer un objet
    [HttpDelete("item/{id}")]
    public async Task<ActionResult> DeleteItem(int id)
    {
        var item = await _context.EquipmentItems.FindAsync(id);
        if (item == null) return NotFound();

        _context.EquipmentItems.Remove(item);
        await _context.SaveChangesAsync();
        return Ok();
    }
}