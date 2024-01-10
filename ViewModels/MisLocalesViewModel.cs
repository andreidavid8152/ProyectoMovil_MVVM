using ProyectoApp.Models;
using ProyectoApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ProyectoApp.ViewModels
{
    public class MisLocalesViewModel : INotifyPropertyChanged
    {
        private readonly APIService _api = App.ServiceProvider.GetService<APIService>();
        public ObservableCollection<Local> Locales { get; private set; }
        public ICommand EliminarLocalCommand { get; private set; }
        public ICommand DetalleLocalCommand { get; private set; }
        public ICommand EditarLocalCommand { get; private set; }
        public ICommand AgregarLocalCommand { get; private set; }

        public MisLocalesViewModel()
        {
            Locales = new ObservableCollection<Local>();
            EliminarLocalCommand = new Command<int>(async (localId) => await EliminarLocal(localId));
            DetalleLocalCommand = new Command<int>(async (localId) => await DetalleLocal(localId));
            EditarLocalCommand = new Command<int>(async (localId) => await EditarLocal(localId));
            AgregarLocalCommand = new Command(async () => await AgregarLocal());
            CargarLocales();
        }

        private async void CargarLocales()
        {
            try
            {
                string token = Preferences.Get("UserToken", string.Empty);
                var locales = await _api.ObtenerLocalesArrendador(token);
                Locales.Clear();
                foreach (var local in locales)
                {
                    Locales.Add(local);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar los locales: {ex.Message}", "OK");
            }
        }

        private async Task EliminarLocal(int localId)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmar", "¿Desea eliminar este local?", "Sí", "No");
            if (!confirm)
            {
                return; // El usuario canceló la operación
            }

            try
            {
                string token = Preferences.Get("UserToken", string.Empty);
                bool eliminado = await _api.EliminarLocal(localId, token);
                if (eliminado)
                {
                    var localAEliminar = Locales.FirstOrDefault(l => l.Id == localId);
                    if (localAEliminar != null)
                    {
                        Locales.Remove(localAEliminar);
                    }
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Local eliminado correctamente", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar el local", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al eliminar el local: {ex.Message}", "OK");
            }
        }

        private async Task DetalleLocal(int localId)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new DetalleLocalArrendadorPage(localId));
        }

        private async Task EditarLocal(int localId)
        {
            await App.Current.MainPage.Navigation.PushModalAsync(new EditarLocalPage(localId));
        }

        private async Task AgregarLocal()
        {
            await App.Current.MainPage.Navigation.PushModalAsync(new NuevoLocalPage());
        }

        // Implementación de INotifyPropertyChanged...
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
