using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Scouty.Shared.Enums;

namespace Scouty.Backend.Entities;

public class EquipmentCategoryEntity
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } // ex: "Cuisine"
    public Section Section { get; set; } // Pour séparer par section

    // Relation : Une catégorie a plusieurs objets
    public List<EquipmentItemEntity> Items { get; set; } = new();
}

public class EquipmentItemEntity
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } // ex: "Spatules"
    public int Quantity { get; set; }

    [ForeignKey("Category")]
    public int CategoryId { get; set; }
    public EquipmentCategoryEntity Category { get; set; }
}