using ProyectoApp.Models;
using ProyectoApp.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Windows.Input;

namespace ProyectoApp.ViewModels
{
    public class RegistroViewModel : INotifyPropertyChanged
    {
        private readonly APIService _api = App.ServiceProvider.GetService<APIService>();
        public UserInput UserInput { get; set; }
        public ICommand RegistrarseCommand { get; private set; }

        public RegistroViewModel()
        {
            UserInput = new UserInput();
            RegistrarseCommand = new Command(async () => await OnClickRegistrarse());
        }

        private async Task OnClickRegistrarse()
        {
            if (IsValid(UserInput, out List<string> errorMessages))
            {
                try
                {
                    await _api.Registro(UserInput);
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Registro completado con éxito", "OK");
                    // Redirigir al usuario a la página de inicio de sesión
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
                }
            }
            else
            {
                foreach (var errorMessage in errorMessages)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
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

        // Implementación de INotifyPropertyChanged...
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
