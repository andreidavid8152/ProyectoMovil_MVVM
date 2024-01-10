using ProyectoApp.Models;
using ProyectoApp.Services;
using System.ComponentModel.DataAnnotations;

namespace ProyectoApp;

public partial class LoginPage : ContentPage
{
    private readonly APIService _api;
    public LoginPage()
    {
        InitializeComponent();
        _api = App.ServiceProvider.GetService<APIService>(); // Obtener la instancia de APIService del contenedor
        this.BindingContext = new Login();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        string token = Preferences.Get("UserToken", string.Empty);
        if (!token.Equals(string.Empty))
        {
            // Crear la página de menú (Flyout) que actuará como el menú principal
            var flyoutPage = new FlyoutPage
            {
                Flyout = new MenuPage(), // Una nueva página que actuará como el menú
                Detail = new NavigationPage(new MainPage()) // La página principal dentro del menú
                {
                    BarBackgroundColor = Color.FromHex("#14282f"), // Estableces el color de fondo
                }
            };

            Application.Current.MainPage = flyoutPage; // Establecer la nueva página principal con el menú
        }
    }

    private async void OnClickLogin(object sender, EventArgs e)
    {
        var userLogin = (Login)this.BindingContext;
        if (IsValid(userLogin, out List<string> errorMessages))
        {
            try
            {
                var token = await _api.Login(userLogin);
                Preferences.Set("UserToken", token); // Guardar el token

                await DisplayAlert("Éxito", "Inicio de sesión exitoso", "OK");

                // Crear la página de menú (Flyout) que actuará como el menú principal
                var flyoutPage = new FlyoutPage
                {
                    Flyout = new MenuPage(), // Una nueva página que actuará como el menú
                    Detail = new NavigationPage(new MainPage()) // La página principal dentro del menú
                    {
                        BarBackgroundColor = Color.FromHex("#14282f"), // Estableces el color de fondo
                    }
                };

                Application.Current.MainPage = flyoutPage; // Establecer la nueva página principal con el menú


            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }
        else
        {
            foreach (var errorMessage in errorMessages)
            {
                await DisplayAlert("Error", errorMessage, "OK");
            }
        }
    }

    private bool IsValid(Login userLogin, out List<string> errorMessages)
    {
        var context = new ValidationContext(userLogin, serviceProvider: null, items: null);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(userLogin, context, validationResults, true);

        errorMessages = validationResults.Select(r => r.ErrorMessage).ToList();
        return isValid;
    }

    private async void OnClickRegistrarse(object sender, EventArgs e)
    {
        Navigation.PushAsync(new RegistroPage(_api));
    }
}