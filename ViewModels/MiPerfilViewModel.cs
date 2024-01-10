using ProyectoApp.Models;
using ProyectoApp.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Windows.Input;

namespace ProyectoApp.ViewModels
{
    public class MiPerfilViewModel : INotifyPropertyChanged
    {
        private readonly APIService _api = App.ServiceProvider.GetService<APIService>();
        private UserInput _perfil;
        public UserInput Perfil
        {
            get => _perfil;
            set
            {
                _perfil = value;
                OnPropertyChanged(nameof(Perfil));
            }
        }

        public ICommand GuardarCommand { get; private set; }

        public MiPerfilViewModel()
        {
            Perfil = new UserInput();
            GuardarCommand = new Command(async () => await GuardarPerfil());
            CargarPerfil();
        }

        private async void CargarPerfil()
        {
            try
            {
                string token = Preferences.Get("UserToken", string.Empty);
                Perfil = await _api.GetPerfil(token);
            }
            catch (Exception ex)
            {
                // Manejar el error
                Console.WriteLine(ex.Message);
            }
        }

        private async Task GuardarPerfil()
        {
            try
            {
                if (IsValid(Perfil, out List<string> errorMessages))
                {
                    string token = Preferences.Get("UserToken", string.Empty);
                    var result = await _api.EditarPerfil(Perfil, token);
                    if (result)
                    {
                        await Application.Current.MainPage.DisplayAlert("Éxito", "Perfil actualizado correctamente.", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "No se pudo actualizar el perfil.", "OK");
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
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al actualizar el perfil: " + ex.Message, "OK");
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
