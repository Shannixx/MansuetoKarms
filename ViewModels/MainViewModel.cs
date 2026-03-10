using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MansuetoKarms.Models;
using MansuetoKarms.Services;
using MansuetoKarms.Views;

namespace MansuetoKarms.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly VehicleService _vehicleService;
        private string _title = "Rent A Vehicle";
        private bool _isBusy;
        private bool _showDeleted;
        private ObservableCollection<Vehicle> _vehicles = new();
        private List<Vehicle> _allVehicles = new();

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

        public bool ShowDeleted
        {
            get => _showDeleted;
            set
            {
                _showDeleted = value;
                OnPropertyChanged();
                FilterVehicles();
            }
        }

        public ObservableCollection<Vehicle> Vehicles
        {
            get => _vehicles;
            set { _vehicles = value; OnPropertyChanged(); }
        }

        public ICommand NavigateToCreateCommand { get; }
        public ICommand NavigateToUpdateCommand { get; }
        public ICommand SoftDeleteCommand { get; }
        public ICommand RestoreCommand { get; }
        public ICommand HardDeleteCommand { get; }

        public MainViewModel(VehicleService vehicleService)
        {
            _vehicleService = vehicleService;

            NavigateToCreateCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(CreateView));
            });

            NavigateToUpdateCommand = new Command<Vehicle>(async (vehicle) =>
            {
                if (vehicle == null) return;
                await Shell.Current.GoToAsync($"{nameof(UpdateView)}?id={vehicle.Id}");
            });

            SoftDeleteCommand = new Command<Vehicle>(async (vehicle) =>
            {
                if (vehicle == null) return;
                bool confirm = await Shell.Current.DisplayAlert(
                    "Archive Vehicle",
                    $"Archive {vehicle.DisplayName}? It can be restored later.",
                    "Archive", "Cancel");
                if (!confirm) return;

                try
                {
                    IsBusy = true;
                    await _vehicleService.SoftDeleteVehicleAsync(vehicle.Id);
                    await LoadVehiclesAsync();
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Failed to archive: {ex.Message}", "OK");
                }
                finally
                {
                    IsBusy = false;
                }
            });

            RestoreCommand = new Command<Vehicle>(async (vehicle) =>
            {
                if (vehicle == null) return;
                try
                {
                    IsBusy = true;
                    await _vehicleService.RestoreVehicleAsync(vehicle.Id);
                    await LoadVehiclesAsync();
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Failed to restore: {ex.Message}", "OK");
                }
                finally
                {
                    IsBusy = false;
                }
            });

            HardDeleteCommand = new Command<Vehicle>(async (vehicle) =>
            {
                if (vehicle == null) return;
                bool confirm = await Shell.Current.DisplayAlert(
                    "Delete Permanently",
                    $"Permanently delete {vehicle.DisplayName}? This cannot be undone.",
                    "Delete", "Cancel");
                if (!confirm) return;

                try
                {
                    IsBusy = true;
                    await _vehicleService.HardDeleteVehicleAsync(vehicle.Id);
                    await LoadVehiclesAsync();
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Failed to delete: {ex.Message}", "OK");
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        public async Task LoadVehiclesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                _allVehicles = await _vehicleService.GetVehiclesAsync();
                FilterVehicles();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load vehicles: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void FilterVehicles()
        {
            var filtered = ShowDeleted
                ? _allVehicles
                : _allVehicles.Where(v => !v.IsDeleted).ToList();

            Vehicles = new ObservableCollection<Vehicle>(filtered);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
