using MansuetoKarms.ViewModels;

namespace MansuetoKarms.Views
{
    public partial class CreateView : ContentPage
    {
        public CreateView(CreateViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
