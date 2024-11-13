namespace KiloMart.Domain.Locations.Models;

public class Location
{
    public int Id { get; set; }
    public float Longitude { get; set; }
    public float Latitude { get; set; }
    public string Name { get; set; } = null!;
    public int Party { get; set; }
    public bool IsActive { get; set; }
}
