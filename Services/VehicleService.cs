using System.Net.Http.Json;
using System.Text.Json;
using MansuetoKarms.Models;

namespace MansuetoKarms.Services
{
    public class VehicleService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://69afee6dc63dd197febaa9a8.mockapi.io/vehicles";

        public VehicleService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET all vehicles
        public async Task<List<Vehicle>> GetVehiclesAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            var vehicles = await response.Content.ReadFromJsonAsync<List<Vehicle>>();
            return vehicles ?? new List<Vehicle>();
        }

        // GET single vehicle by ID
        public async Task<Vehicle?> GetVehicleAsync(string id)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Vehicle>();
        }

        // POST create new vehicle
        public async Task<Vehicle?> CreateVehicleAsync(Vehicle vehicle)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, vehicle);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Vehicle>();
        }

        // PUT update existing vehicle
        public async Task<Vehicle?> UpdateVehicleAsync(Vehicle vehicle)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{vehicle.Id}", vehicle);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Vehicle>();
        }

        // PUT soft delete (set isDeleted = true)
        public async Task<Vehicle?> SoftDeleteVehicleAsync(string id)
        {
            var vehicle = await GetVehicleAsync(id);
            if (vehicle == null) return null;
            vehicle.IsDeleted = true;
            return await UpdateVehicleAsync(vehicle);
        }

        // PUT restore (set isDeleted = false)
        public async Task<Vehicle?> RestoreVehicleAsync(string id)
        {
            var vehicle = await GetVehicleAsync(id);
            if (vehicle == null) return null;
            vehicle.IsDeleted = false;
            return await UpdateVehicleAsync(vehicle);
        }

        // DELETE hard delete (permanently remove)
        public async Task HardDeleteVehicleAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
