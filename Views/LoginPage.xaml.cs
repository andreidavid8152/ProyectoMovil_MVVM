using ProyectoApp.ViewModels;

namespace ProyectoApp;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();
    }
}