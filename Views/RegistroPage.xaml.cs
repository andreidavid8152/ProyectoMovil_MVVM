using ProyectoApp.ViewModels;

namespace ProyectoApp;

public partial class RegistroPage : ContentPage
{
    public RegistroPage()
    {
        InitializeComponent();
        BindingContext = new RegistroViewModel();
    }
}