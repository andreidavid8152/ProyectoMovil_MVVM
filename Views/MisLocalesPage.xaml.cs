using ProyectoApp.ViewModels;

namespace ProyectoApp;

public partial class MisLocalesPage : ContentPage
{
    public MisLocalesPage()
    {
        InitializeComponent();
        BindingContext = new MisLocalesViewModel();
    }
}