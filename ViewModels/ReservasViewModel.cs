using ProyectoApp.Models;
using ProyectoApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ProyectoApp.ViewModels
{
    public class ReservasViewModel : INotifyPropertyChanged
    {
        private readonly APIService _api = App.ServiceProvider.GetService<APIService>();
        public ObservableCollection<Reserva> Reservas { get; private set; }
        public ICommand EliminarReservaCommand { get; private set; }
        public ICommand ComentarCommand { get; private set; }

        public ReservasViewModel()
        {
            Reservas = new ObservableCollection<Reserva>();
            EliminarReservaCommand = new Command<Reserva>(async (reserva) => await EliminarReserva(reserva));
            ComentarCommand = new Command<Reserva>(async (reserva) => await ComentarReserva(reserva));
            CargarReservas();
        }

        private async void CargarReservas()
        {
            try
            {
                string token = Preferences.Get("UserToken", string.Empty);
                var reservasCliente = await _api.ObtenerReservasCliente(token);
                Reservas.Clear();
                foreach (var reserva in reservasCliente)
                {
                    Reservas.Add(reserva);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudieron cargar las reservas: " + ex.Message, "OK");
            }
        }

        private async Task EliminarReserva(Reserva reserva)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Eliminar Reserva",
                "¿Estás seguro de que quieres eliminar esta reserva?",
                "Sí", "No");

            if (!confirm) return;

            string token = Preferences.Get("UserToken", string.Empty);
            try
            {
                bool eliminado = await _api.EliminarReserva(reserva.Id, token);
                if (eliminado)
                {
                    Reservas.Remove(reserva);
                    await Application.Current.MainPage.DisplayAlert("Reserva Eliminada", "La reserva ha sido eliminada correctamente.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo eliminar la reserva: " + ex.Message, "OK");
            }
        }

        private async Task ComentarReserva(Reserva reserva)
        {
            string calificacionString = await Application.Current.MainPage.DisplayPromptAsync(
                "Calificación",
                "Ingresa tu calificación (1-5):",
                keyboard: Keyboard.Numeric);

            if (!int.TryParse(calificacionString, out int calificacion) || calificacion < 1 || calificacion > 5)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Debes ingresar un número entre 1 y 5.",
                    "OK");
                return;
            }

            string textoComentario = await Application.Current.MainPage.DisplayPromptAsync(
                "Comentario",
                "Ingresa tu comentario sobre la reserva:");

            if (string.IsNullOrWhiteSpace(textoComentario))
            {
                return;
            }

            Comentario comentario = new Comentario
            {
                LocalID = reserva.LocalID,
                UsuarioID = reserva.UsuarioID,
                Texto = textoComentario,
                Fecha = DateTime.Now,
                Calificacion = calificacion
            };

            string token = Preferences.Get("UserToken", string.Empty);
            try
            {
                bool resultado = await _api.ComentarReserva(comentario, token);
                if (resultado)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Tu comentario y calificación han sido enviados.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo enviar el comentario y la calificación: " + ex.Message, "OK");
            }
        }

        // Implementación de INotifyPropertyChanged...
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
