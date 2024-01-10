using ProyectoApp.ViewModels;

namespace ProyectoApp;

public partial class MiPerfilPage : ContentPage
{
    public MiPerfilPage()
    {
        InitializeComponent();
        BindingContext = new MiPerfilViewModel();
    }
}