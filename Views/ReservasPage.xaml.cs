using ProyectoApp.ViewModels;

namespace ProyectoApp;

public partial class ReservasPage : ContentPage
{
    public ReservasPage()
    {
        InitializeComponent();
        BindingContext = new ReservasViewModel();
    }

}