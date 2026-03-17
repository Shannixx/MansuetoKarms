using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MansuetoKarms.Models;
using MansuetoKarms.Services;

namespace MansuetoKarms.ViewModels
{
    public class ArchiveViewModel : INotifyPropertyChanged
    {
        private readonly VehicleService _vehicleService;
        private readonly DeleteHistoryService _deleteHistoryService;
        private string _title = "Archive & History";
        private bool _isBusy;
        private ObservableCollection<Vehicle> _archivedVehicles = new();
        private ObservableCollection<DeletedVehicleItem> _deletedVehicles = new();
        private int _archivedCount;
        private int _deletedCount;

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

        public ObservableCollection<Vehicle> ArchivedVehicles
        {
            get => _archivedVehicles;
            set { _archivedVehicles = value; OnPropertyChanged(); }
        }

        public ObservableCollection<DeletedVehicleItem> DeletedVehicles
        {
            get => _deletedVehicles;
            set { _deletedVehicles = value; OnPropertyChanged(); }
        }

        public int ArchivedCount
        {
            get => _archivedCount;
            set { _archivedCount = value; OnPropertyChanged(); }
        }

        public int DeletedCount
        {
            get => _deletedCount;
            set { _deletedCount = value; OnPropertyChanged(); }
        }

        public ICommand UnarchiveCommand { get; }
        public ICommand RestoreDeletedCommand { get; }
        public ICommand GoBackCommand { get; }

        public ArchiveViewModel(VehicleService vehicleService, DeleteHistoryService deleteHistoryService)
        {
            _vehicleService = vehicleService;
            _deleteHistoryService = deleteHistoryService;

            // Unarchive = restore from soft-delete back to active
            UnarchiveCommand = new Command<Vehicle>(async (vehicle) =>
            {
                if (vehicle == null) return;
                try
                {
                    IsBusy = true;
                    await _vehicleService.RestoreVehicleAsync(vehicle.Id);
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Failed to unarchive: {ex.Message}", "OK");
                }
                finally
                {
                    IsBusy = false;
                }
            });

            // Restore deleted = re-create the vehicle in the API from saved local data
            RestoreDeletedCommand = new Command<DeletedVehicleItem>(async (item) =>
            {
                if (item == null) return;
                try
                {
                    IsBusy = true;
                    var vehicle = item.ToVehicle();
                    await _vehicleService.CreateVehicleAsync(vehicle);
                    _deleteHistoryService.RemoveDeletedVehicle(item);
                    await LoadDataAsync();
                    await Shell.Current.DisplayAlert("Restored", $"{item.DisplayName} has been restored.", "OK");
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

            GoBackCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync("..");
            });
        }

        public async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Load archived vehicles from API (soft-deleted)
                var allVehicles = await _vehicleService.GetVehiclesAsync();
                var archived = allVehicles.Where(v => v.IsDeleted).ToList();
                ArchivedVehicles = new ObservableCollection<Vehicle>(archived);
                ArchivedCount = archived.Count;

                // Load deleted vehicles from local storage
                var deleted = _deleteHistoryService.GetDeletedVehicles();
                DeletedVehicles = new ObservableCollection<DeletedVehicleItem>(deleted);
                DeletedCount = deleted.Count;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load data: {ex.Message}", "OK");
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
