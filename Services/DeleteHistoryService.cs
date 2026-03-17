using System.Text.Json;
using MansuetoKarms.Models;

namespace MansuetoKarms.Services
{
    public class DeleteHistoryService
    {
        private const string StorageKey = "deleted_vehicles";

        public List<DeletedVehicleItem> GetDeletedVehicles()
        {
            var json = Preferences.Get(StorageKey, string.Empty);
            if (string.IsNullOrWhiteSpace(json))
                return new List<DeletedVehicleItem>();

            try
            {
                return JsonSerializer.Deserialize<List<DeletedVehicleItem>>(json) ?? new List<DeletedVehicleItem>();
            }
            catch
            {
                return new List<DeletedVehicleItem>();
            }
        }

        public void AddDeletedVehicle(Vehicle vehicle)
        {
            var list = GetDeletedVehicles();
            list.Insert(0, new DeletedVehicleItem
            {
                Name = vehicle.Name,
                Brand = vehicle.Brand,
                Type = vehicle.Type,
                PlateNumber = vehicle.PlateNumber,
                Color = vehicle.Color,
                PricePerDay = vehicle.PricePerDay,
                Seats = vehicle.Seats,
                Status = vehicle.Status,
                Description = vehicle.Description,
                DeletedAt = DateTime.Now
            });

            SaveList(list);
        }

        public void RemoveDeletedVehicle(DeletedVehicleItem item)
        {
            var list = GetDeletedVehicles();
            list.Remove(item);
            SaveList(list);
        }

        private void SaveList(List<DeletedVehicleItem> list)
        {
            // Keep only the last 50 entries
            if (list.Count > 50)
                list = list.Take(50).ToList();

            var json = JsonSerializer.Serialize(list);
            Preferences.Set(StorageKey, json);
        }
    }
}
