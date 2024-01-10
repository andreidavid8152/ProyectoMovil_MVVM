using ProyectoApp.Models;
using ProyectoApp.Services;
using System.ComponentModel.DataAnnotations;

namespace ProyectoApp;

public partial class RegistroPage : ContentPage
{
    private readonly APIService _api;
    public RegistroPage(APIService _apiService)
	{
		InitializeComponent();
        _api = _apiService;
        this.BindingContext = new UserInput();
    }

    private async void OnClickRegistrarse(object sender, EventArgs e)
    {
        var userInput = (UserInput)this.BindingContext;
        if (IsValid(userInput, out List<string> errorMessages))
        {
            try
            {
                await _api.Registro(userInput);
                await DisplayAlert("�xito", "Registro completado con �xito", "OK");

                // Redirigir al usuario a la p�gina de inicio de sesi�n
                await Navigation.PopAsync();
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

    private bool IsValid(UserInput userInput, out List<string> errorMessages)
    {
        var context = new ValidationContext(userInput, serviceProvider: null, items: null);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(userInput, context, validationResults, true);

        errorMessages = validationResults.Select(r => r.ErrorMessage).ToList();
        return isValid;
    }

}