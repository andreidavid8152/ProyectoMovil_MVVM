using Microsoft.Maui.Media;
using ProyectoApp.Models;
using ProyectoApp.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ProyectoApp;

public partial class ReservasPage : ContentPage
{
    public ICommand EliminarReservaCommand { get; }
    public ICommand ComentarCommand { get; }
    private readonly APIService _api;
    public ObservableCollection<Reserva> Reservas { get; set; }
    public ReservasPage()
    {
        InitializeComponent();
        _api = App.ServiceProvider.GetService<APIService>();
        Reservas = new ObservableCollection<Reserva>();
        EliminarReservaCommand = new Command<Reserva>(async (reserva) => await EliminarReserva(reserva));
        ComentarCommand = new Command<Reserva>(async (reserva) => await ComentarReserva(reserva));
        this.BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarReservas();
    }

    private async Task CargarReservas()
    {
        try
        {
            string token = Preferences.Get("UserToken", string.Empty);
            var reservasCliente = await _api.ObtenerReservasCliente(token);

            Reservas.Clear();
            if (reservasCliente.Any()) // Verifica si hay reservas
            {
                foreach (var reserva in reservasCliente)
                {
                    Reservas.Add(reserva);
                }
            }
            else
            {
                // Mostrar mensaje si no hay reservas
                await DisplayAlert("Sin Reservas", "No tienes reservas en este momento.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudieron cargar las reservas: " + ex.Message, "OK");
        }
    }

    private async Task EliminarReserva(Reserva reserva)
    {
        // Confirmar con el usuario si realmente desea eliminar la reserva
        bool confirm = await DisplayAlert("Eliminar Reserva", "�Est�s seguro de que quieres eliminar esta reserva?", "S�", "No");
        if (!confirm)
        {
            return; // El usuario cancel� la operaci�n
        }

        string token = Preferences.Get("UserToken", string.Empty);

        try
        {
            bool eliminado = await _api.EliminarReserva(reserva.Id, token);

            if (eliminado)
            {
                // Elimina la reserva de la colecci�n para actualizar la UI
                Reservas.Remove(reserva);

                // Mensaje de confirmaci�n
                await DisplayAlert("Reserva Eliminada", "La reserva ha sido eliminada correctamente.", "OK");
            }
        }
        catch (Exception ex)
        {
            // Muestra un mensaje de error si algo sale mal
            await DisplayAlert("Error", $"No se pudo eliminar la reserva: {ex.Message}", "OK");
        }
    }


    private async Task ComentarReserva(Reserva reserva)
    {
        // Pedir al usuario que ingrese una calificaci�n
        string calificacionString = await DisplayPromptAsync("Calificaci�n", "Ingresa tu calificaci�n (1-5):", keyboard: Keyboard.Numeric);
        if (!int.TryParse(calificacionString, out int calificacion) || calificacion < 1 || calificacion > 5)
        {
            // El usuario no ingres� una calificaci�n v�lida o cancel� el prompt
            await DisplayAlert("Error", "Debes ingresar un n�mero entre 1 y 5.", "OK");
            return;
        }

        // Pedir al usuario que ingrese un comentario
        string textoComentario = await DisplayPromptAsync("Comentario", "Ingresa tu comentario sobre la reserva:");
        if (string.IsNullOrWhiteSpace(textoComentario))
        {
            // El usuario no ingres� un comentario o cancel� el prompt
            return;
        }

        // Crear un objeto Comentario y completar con los datos
        Comentario comentario = new Comentario
        {
            LocalID = reserva.LocalID,
            UsuarioID = reserva.UsuarioID,
            Texto = textoComentario,
            Fecha = DateTime.Now, // Puedes ajustar la fecha si es necesario
            Calificacion = calificacion
        };

        // Obtener el token de usuario para la autorizaci�n
        string token = Preferences.Get("UserToken", string.Empty);

        try
        {
            // Llamar al m�todo del API para enviar el comentario y la calificaci�n
            bool resultado = await _api.ComentarReserva(comentario, token);
            if (resultado)
            {
                await DisplayAlert("�xito", "Tu comentario y calificaci�n han sido enviados.", "OK");
                // Obtiene la referencia de la FlyoutPage que es la MainPage actual
                if (Application.Current.MainPage is FlyoutPage flyoutPage)
                {
                    // Crea una nueva instancia de la comentariosPage
                    var comentariosPage = new ComentariosPage();

                    // Configura la comentariosPage como la nueva p�gina de detalle
                    flyoutPage.Detail = new NavigationPage(comentariosPage);

                    // Cierra el men� lateral
                    flyoutPage.IsPresented = false;
                }
            }
        }
        catch (Exception ex)
        {
            // Mostrar mensaje de error si algo falla
            await DisplayAlert("Error", $"No se pudo enviar el comentario y la calificaci�n: {ex.Message}", "OK");
        }
    }
}