using ProyectoApp.Models;
using ProyectoApp.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Windows.Input;

namespace ProyectoApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly APIService _api = App.ServiceProvider.GetService<APIService>();
        public Login UserLogin { get; set; }
        public ICommand LoginCommand { get; private set; }
        public ICommand RegisterCommand { get; private set; }

        public LoginViewModel()
        {
            UserLogin = new Login();
            LoginCommand = new Command(async () => await ExecuteLoginCommand());
            RegisterCommand = new Command(async () => await ExecuteRegisterCommand());
        }

        private async Task ExecuteLoginCommand()
        {
            if (IsValid(UserLogin, out List<string> errorMessages))
            {
                try
                {
                    var token = await _api.Login(UserLogin);
                    Preferences.Set("UserToken", token);
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Inicio de sesión exitoso", "OK");

                    var flyoutPage = new FlyoutPage
                    {
                        Flyout = new MenuPage(),
                        Detail = new NavigationPage(new MainPage())
                        {
                            BarBackgroundColor = Color.FromHex("#14282f")
                        }
                    };

                    Application.Current.MainPage = flyoutPage;
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

        private async Task ExecuteRegisterCommand()
        {
            await App.Current.MainPage.Navigation.PushAsync(new RegistroPage());
        }

        private bool IsValid(Login userLogin, out List<string> errorMessages)
        {
            var context = new ValidationContext(userLogin, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(userLogin, context, validationResults, true);
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
