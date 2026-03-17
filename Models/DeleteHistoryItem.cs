using System.Text.Json.Serialization;

namespace MansuetoKarms.Models
{
    /// <summary>
    /// Stores a deleted vehicle's full data locally so it can be restored later.
    /// </summary>
    public class DeletedVehicleItem
    {
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

        [JsonPropertyName("deletedAt")]
        public DateTime DeletedAt { get; set; }

        [JsonIgnore]
        public string DisplayName => string.IsNullOrWhiteSpace(Brand)
            ? Name
            : $"{Brand} {Name}";

        [JsonIgnore]
        public string DeletedAtFormatted => DeletedAt.ToString("MMM dd, yyyy  h:mm tt");

        /// <summary>
        /// Converts back to a Vehicle for re-creating in the API.
        /// </summary>
        public Vehicle ToVehicle() => new Vehicle
        {
            Brand = Brand,
            Name = Name,
            Type = Type,
            PlateNumber = PlateNumber,
            Color = Color,
            PricePerDay = PricePerDay,
            Seats = Seats,
            Status = Status,
            Description = Description,
            IsDeleted = false
        };
    }
}
