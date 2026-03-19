using MansuetoKarms.ViewModels;

namespace MansuetoKarms.Views
{
    public partial class DeleteView : ContentPage
    {
        private readonly DeleteViewModel _viewModel;

        public DeleteView(DeleteViewModel viewModel)
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