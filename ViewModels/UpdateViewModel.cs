using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MansuetoKarms.Models;
using MansuetoKarms.Services;

namespace MansuetoKarms.ViewModels
{
    [QueryProperty(nameof(VehicleId), "id")]
    public class UpdateViewModel : INotifyPropertyChanged
    {
        private readonly VehicleService _vehicleService;
        private string _title = "Edit Vehicle";
        private bool _isBusy;
        private string _vehicleId = string.Empty;
        private string _brand = string.Empty;
        private string _name = string.Empty;
        private string _type = string.Empty;
        private string _plateNumber = string.Empty;
        private string _color = string.Empty;
        private string _pricePerDay = string.Empty;
        private string _seats = string.Empty;
        private string _status = "Available";
        private string _description = string.Empty;

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public string VehicleId
        {
            get => _vehicleId;
            set
            {
                _vehicleId = value;
                OnPropertyChanged();
                _ = LoadVehicleAsync(value);
            }
        }

        public string Brand
        {
            get => _brand;
            set { _brand = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Type
        {
            get => _type;
            set { _type = value; OnPropertyChanged(); }
        }

        public string PlateNumber
        {
            get => _plateNumber;
            set { _plateNumber = value; OnPropertyChanged(); }
        }

        public string Color
        {
            get => _color;
            set { _color = value; OnPropertyChanged(); }
        }

        public string PricePerDay
        {
            get => _pricePerDay;
            set { _pricePerDay = value; OnPropertyChanged(); }
        }

        public string Seats
        {
            get => _seats;
            set { _seats = value; OnPropertyChanged(); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> VehicleTypes { get; } = new()
        {
            "Sedan", "SUV", "Truck", "Van", "Motorcycle", "Coupe", "Hatchback"
        };

        public ObservableCollection<string> StatusOptions { get; } = new()
        {
            "Available", "Rented", "Maintenance"
        };

        public ICommand UpdateCommand { get; }
        public ICommand CancelCommand { get; }

        public UpdateViewModel(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;

            UpdateCommand = new Command(async () =>
            {
                // Validation
                if (string.IsNullOrWhiteSpace(Brand))
                {
                    await Shell.Current.DisplayAlert("Validation", "Brand is required.", "OK");
                    return;
                }
                if (string.IsNullOrWhiteSpace(Name))
                {
                    await Shell.Current.DisplayAlert("Validation", "Model / Name is required.", "OK");
                    return;
                }
                if (string.IsNullOrWhiteSpace(PlateNumber))
                {
                    await Shell.Current.DisplayAlert("Validation", "Plate Number is required.", "OK");
                    return;
                }
                if (!decimal.TryParse(PricePerDay, out var price) || price <= 0)
                {
                    await Shell.Current.DisplayAlert("Validation", "Enter a valid Price Per Day.", "OK");
                    return;
                }
                if (!int.TryParse(Seats, out var seats) || seats <= 0)
                {
                    await Shell.Current.DisplayAlert("Validation", "Enter a valid number of Seats.", "OK");
                    return;
                }

                try
                {
                    IsBusy = true;
                    var vehicle = new Vehicle
                    {
                        Id = VehicleId,
                        Brand = Brand.Trim(),
                        Name = Name.Trim(),
                        Type = string.IsNullOrWhiteSpace(Type) ? "Sedan" : Type,
                        PlateNumber = PlateNumber.Trim(),
                        Color = Color.Trim(),
                        PricePerDay = price,
                        Seats = seats,
                        Status = string.IsNullOrWhiteSpace(Status) ? "Available" : Status,
                        Description = Description.Trim(),
                        IsDeleted = false
                    };

                    await _vehicleService.UpdateVehicleAsync(vehicle);
                    await Shell.Current.DisplayAlert("Success", "Vehicle updated successfully!", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Failed to update: {ex.Message}", "OK");
                }
                finally
                {
                    IsBusy = false;
                }
            });

            CancelCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("..");
            });
        }

        private async Task LoadVehicleAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            try
            {
                IsBusy = true;
                var vehicle = await _vehicleService.GetVehicleAsync(id);
                if (vehicle != null)
                {
                    Brand = vehicle.Brand;
                    Name = vehicle.Name;
                    Type = vehicle.Type;
                    PlateNumber = vehicle.PlateNumber;
                    Color = vehicle.Color;
                    PricePerDay = vehicle.PricePerDay.ToString();
                    Seats = vehicle.Seats.ToString();
                    Status = vehicle.Status;
                    Description = vehicle.Description;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load vehicle: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
