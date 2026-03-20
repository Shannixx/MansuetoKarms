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
        private readonly DeleteHistoryService _deleteHistoryService;
        private string _title = "Monki Car Rental Vehicle";
        private bool _isBusy;
        private ObservableCollection<Vehicle> _vehicles = new();
        private List<Vehicle> _allVehicles = new();
        private int _archivedCount;

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

        public ObservableCollection<Vehicle> Vehicles
        {
            get => _vehicles;
            set { _vehicles = value; OnPropertyChanged(); }
        }

        public int ArchivedCount
        {
            get => _archivedCount;
            set { _archivedCount = value; OnPropertyChanged(); }
        }

        public ICommand NavigateToCreateCommand { get; }
        public ICommand NavigateToUpdateCommand { get; }
        public ICommand NavigateToArchiveCommand { get; }
        public ICommand NavigateToDeleteCommand { get; }
        public ICommand SoftDeleteCommand { get; }
        public ICommand HardDeleteCommand { get; }

        public MainViewModel(VehicleService vehicleService, DeleteHistoryService deleteHistoryService)
        {
            _vehicleService = vehicleService;
            _deleteHistoryService = deleteHistoryService;

            NavigateToCreateCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(CreateView));
            });

            NavigateToUpdateCommand = new Command<Vehicle>(async (vehicle) =>
            {
                if (vehicle == null) return;
                await Shell.Current.GoToAsync($"{nameof(UpdateView)}?id={vehicle.Id}");
            });

            NavigateToArchiveCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(ArchiveView));
            });

            NavigateToDeleteCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync(nameof(DeleteView));
            });

            // Archive = soft delete, moves to Archive section
            SoftDeleteCommand = new Command<Vehicle>(async (vehicle) =>
            {
                if (vehicle == null) return;
                bool confirm = await Shell.Current.DisplayAlert(
                    "Archive Vehicle",
                    $"Archive {vehicle.DisplayName}? You can unarchive it later.",
                    "Archive", "Cancel");
                if (!confirm) return;

                try
                {
                    IsBusy = true;
                    await _vehicleService.SoftDeleteVehicleAsync(vehicle.Id);
                    IsBusy = false; // Allow LoadVehiclesAsync to execute
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

            // Delete = hard delete from API, but save locally so it can be restored
            HardDeleteCommand = new Command<Vehicle>(async (vehicle) =>
            {
                if (vehicle == null) return;
                bool confirm = await Shell.Current.DisplayAlert(
                    "Delete Vehicle",
                    $"Delete {vehicle.DisplayName}? You can restore it from the Deleted section.",
                    "Delete", "Cancel");
                if (!confirm) return;

                try
                {
                    IsBusy = true;
                    _deleteHistoryService.AddDeletedVehicle(vehicle);
                    await _vehicleService.HardDeleteVehicleAsync(vehicle.Id);
                    IsBusy = false; // Allow LoadVehiclesAsync to execute
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
                var active = _allVehicles.Where(v => !v.IsDeleted).ToList();
                Vehicles = new ObservableCollection<Vehicle>(active);
                ArchivedCount = _allVehicles.Count(v => v.IsDeleted);
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
