using Microsoft.EntityFrameworkCore;
using Scouty.Backend.Entities;

namespace Scouty.Backend.Data;

public class ScoutyDbContext : DbContext
{
    public ScoutyDbContext(DbContextOptions<ScoutyDbContext> options) : base(options)
    {
    }

    // Cette ligne dit : "Crée une table 'Users' basée sur le modèle 'UserEntity'"
    public DbSet<UserEntity> Users { get; set; }

    public DbSet<TransactionEntity> Transactions { get; set; }  //Table pour les transactions et le budget

    public DbSet<EquipmentCategoryEntity> EquipmentCategories { get; set; }
    public DbSet<EquipmentItemEntity> EquipmentItems { get; set; }

    public DbSet<CalendarEventEntity> CalendarEvents { get; set; }
    public DbSet<EventParticipantEntity> EventParticipants { get; set; }
    public DbSet<EventSectionEntity> EventSections { get; set; }


}