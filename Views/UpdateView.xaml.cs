using MansuetoKarms.ViewModels;

namespace MansuetoKarms.Views
{
    public partial class UpdateView : ContentPage
    {
        public UpdateView(UpdateViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
