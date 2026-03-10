using MansuetoKarms.ViewModels;

namespace MansuetoKarms.Views
{
    public partial class MainView : ContentPage
    {
        private readonly MainViewModel _vm;

        public MainView(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = _vm.LoadVehiclesAsync();
        }
    }
}
