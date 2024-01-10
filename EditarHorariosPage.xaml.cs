using Microsoft.Maui.Media;
using ProyectoApp.Models;
using ProyectoApp.Services;

namespace ProyectoApp;

public partial class EditarHorariosPage : ContentPage
{
    private readonly APIService _api;
    private int _localId;
    private List<Horario> _horarios;
    public EditarHorariosPage(int localId)
    {
        InitializeComponent();
        _api = App.ServiceProvider.GetService<APIService>(); // Obtener la instancia de APIService
        _localId = localId;
        CargarDatosHorarios();
    }

    private async void CargarDatosHorarios()
    {
        try
        {
            string token = Preferences.Get("UserToken", string.Empty);
            _horarios = await _api.ObtenerHorariosLocal(_localId, token);

            // Asegúrate de que hay suficientes horarios en la lista antes de asignarlos
            if (_horarios != null && _horarios.Count >= 4)
            {
                Inicio1.Time = _horarios[0].HoraInicio;
                Fin1.Time = _horarios[0].HoraFin;

                Inicio2.Time = _horarios[1].HoraInicio;
                Fin2.Time = _horarios[1].HoraFin;

                Inicio3.Time = _horarios[2].HoraInicio;
                Fin3.Time = _horarios[2].HoraFin;

                Inicio4.Time = _horarios[3].HoraInicio;
                Fin4.Time = _horarios[3].HoraFin;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo cargar los horarios: " + ex.Message, "OK");
        }
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        try
        {
            // Asegúrate de que los horarios han sido previamente cargados
            if (_horarios != null && _horarios.Count >= 4)
            {
                // Actualiza los horarios con los nuevos tiempos seleccionados por el usuario
                _horarios[0].HoraInicio = Inicio1.Time;
                _horarios[0].HoraFin = Fin1.Time;

                _horarios[1].HoraInicio = Inicio2.Time;
                _horarios[1].HoraFin = Fin2.Time;

                _horarios[2].HoraInicio = Inicio3.Time;
                _horarios[2].HoraFin = Fin3.Time;

                _horarios[3].HoraInicio = Inicio4.Time;
                _horarios[3].HoraFin = Fin4.Time;

                string token = Preferences.Get("UserToken", string.Empty);
                // Asumiendo que tienes un método en tu API para actualizar los horarios
                bool resultado = await _api.EditarHorariosLocal(_localId, _horarios, token);

                if (resultado)
                {
                    await DisplayAlert("Éxito", "Horarios actualizados correctamente.", "OK");
                    var editarImagenes = new EditarImagenesLocalPage(_localId);
                    Navigation.InsertPageBefore(editarImagenes, this);
                    await Navigation.PopAsync(); // Esto removerá la página actual de la pila
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo actualizar los horarios.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "No hay suficientes horarios para actualizar.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error al guardar los horarios: " + ex.Message, "OK");
            var editarImagenes = new EditarImagenesLocalPage(_localId);
            Navigation.InsertPageBefore(editarImagenes, this);
            await Navigation.PopAsync(); // Esto removerá la página actual de la pila
        }
    }
}