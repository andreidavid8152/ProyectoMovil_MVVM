using ProyectoApp.Models;
using ProyectoApp.Services;

namespace ProyectoApp;

public partial class CrearHorariosLocalPage : ContentPage
{
    private readonly APIService _api;
    private int _localId;

    public CrearHorariosLocalPage(int localId)
    {
        InitializeComponent();
        _api = App.ServiceProvider.GetService<APIService>();
        _localId = localId;
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        try
        {
            // Crear una lista de horarios con los tiempos seleccionados
            var horarios = new List<Horario>
            {
                new Horario { HoraInicio = Inicio1.Time, HoraFin = Fin1.Time },
                new Horario { HoraInicio = Inicio2.Time, HoraFin = Fin2.Time },
                new Horario { HoraInicio = Inicio3.Time, HoraFin = Fin3.Time },
                new Horario { HoraInicio = Inicio4.Time, HoraFin = Fin4.Time }
            };

            string token = Preferences.Get("UserToken", string.Empty);
            // Asumiendo que tienes un método en tu API para crear los horarios
            bool resultado = await _api.AddHorarios(token, _localId, horarios);

            if (resultado)
            {
                await DisplayAlert("Éxito", "Horarios creados correctamente.", "OK");
                var crearImagenes = new CrearImagenesLocalPage(_localId);
                Navigation.InsertPageBefore(crearImagenes, this);
                await Navigation.PopAsync(); // Esto removerá la página actual de la pila
            }
            else
            {
                await DisplayAlert("Error", "No se pudieron crear los horarios.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error al crear los horarios: " + ex.Message, "OK");
        }
    }
}