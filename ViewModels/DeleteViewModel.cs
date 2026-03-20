using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MansuetoKarms.Models;
using MansuetoKarms.Services;

namespace MansuetoKarms.ViewModels
{
    public class DeleteViewModel : INotifyPropertyChanged
    {
        private readonly VehicleService _vehicleService;
        private readonly DeleteHistoryService _deleteHistoryService;
        private string _title = "Deleted Vehicles";
        private bool _isBusy;
        private ObservableCollection<DeletedVehicleItem> _deletedVehicles = new();
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

        public ObservableCollection<DeletedVehicleItem> DeletedVehicles
        {
            get => _deletedVehicles;
            set { _deletedVehicles = value; OnPropertyChanged(); }
        }

        public int DeletedCount
        {
            get => _deletedCount;
            set { _deletedCount = value; OnPropertyChanged(); }
        }

        public ICommand RestoreDeletedCommand { get; }
        public ICommand GoBackCommand { get; }

        public DeleteViewModel(VehicleService vehicleService, DeleteHistoryService deleteHistoryService)
        {
            _vehicleService = vehicleService;
            _deleteHistoryService = deleteHistoryService;

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
                    IsBusy = false; // Allow LoadDataAsync to execute
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