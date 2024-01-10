using ProyectoApp.ViewModels;

namespace ProyectoApp;

public partial class ComentariosPage : ContentPage
{
    public ComentariosPage()
    {
        InitializeComponent();
        BindingContext = new ComentariosViewModel();
    }
}