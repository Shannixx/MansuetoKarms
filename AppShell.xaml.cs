using MansuetoKarms.Views;

namespace MansuetoKarms
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(CreateView), typeof(CreateView));
            Routing.RegisterRoute(nameof(UpdateView), typeof(UpdateView));
        }
    }
}
