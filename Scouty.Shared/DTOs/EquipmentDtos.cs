namespace Scouty.Shared.DTOs;

public class EquipmentCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<EquipmentItemDto> Items { get; set; } = new();
}

public class EquipmentItemDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public int CategoryId { get; set; }
}

public class CreateEquipmentItemDto
{
    public string Name { get; set; }
    public int Quantity { get; set; }
    public int CategoryId { get; set; }
}