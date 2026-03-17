using MansuetoKarms.ViewModels;

namespace MansuetoKarms.Views
{
    public partial class ArchiveView : ContentPage
    {
        private readonly ArchiveViewModel _viewModel;

        public ArchiveView(ArchiveViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadDataAsync();
        }
    }
}
