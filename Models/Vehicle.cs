using System.Text.Json.Serialization;

namespace MansuetoKarms.Models
{
    public class Vehicle
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("brand")]
        public string Brand { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("plateNumber")]
        public string PlateNumber { get; set; } = string.Empty;

        [JsonPropertyName("color")]
        public string Color { get; set; } = string.Empty;

        [JsonPropertyName("pricePerDay")]
        public decimal PricePerDay { get; set; }

        [JsonPropertyName("seats")]
        public int Seats { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Available";

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        // Computed display properties (not serialized to API)
        [JsonIgnore]
        public string DisplayName => string.IsNullOrWhiteSpace(Brand)
            ? Name
            : $"{Brand} {Name}";

        [JsonIgnore]
        public Color StatusBg => Status switch
        {
            "Available" => Microsoft.Maui.Graphics.Color.FromArgb("#E8F5E9"),
            "Rented" => Microsoft.Maui.Graphics.Color.FromArgb("#E3F2FD"),
            "Maintenance" => Microsoft.Maui.Graphics.Color.FromArgb("#FFF3E0"),
            _ => Colors.Transparent
        };

        [JsonIgnore]
        public Color StatusColor => Status switch
        {
            "Available" => Microsoft.Maui.Graphics.Color.FromArgb("#2E7D32"),
            "Rented" => Microsoft.Maui.Graphics.Color.FromArgb("#1565C0"),
            "Maintenance" => Microsoft.Maui.Graphics.Color.FromArgb("#E65100"),
            _ => Colors.Black
        };
    }
}
