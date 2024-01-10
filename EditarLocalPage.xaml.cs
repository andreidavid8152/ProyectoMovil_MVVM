using Microsoft.Maui.Media;
using ProyectoApp.Models;
using ProyectoApp.Services;

namespace ProyectoApp;

public partial class EditarLocalPage : ContentPage
{
    private readonly APIService _api = null;
    private int _localId;
    public EditarLocalPage(int localId)
    {
        InitializeComponent();
        _api = App.ServiceProvider.GetService<APIService>(); // Obtener la instancia de APIService del contenedor
        _localId = localId;
        CargarDatosLocal();
    }

    private async void CargarDatosLocal()
    {
        try
        {
            string token = Preferences.Get("UserToken", defaultValue: string.Empty);
            var local = await _api.ObtenerLocal(_localId, token);
            // Asigna los datos a los controles
            NombreEntry.Text = local.Nombre;
            DescripcionEditor.Text = local.Descripcion;
            DireccionEntry.Text = local.Direccion;
            CapacidadEntry.Text = local.Capacidad.ToString();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo cargar los datos del local: " + ex.Message, "OK");
        }
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        try
        {
            var localActualizado = new Local
            {
                Id = _localId,
                Nombre = NombreEntry.Text,
                Descripcion = DescripcionEditor.Text,
                Direccion = DireccionEntry.Text,
                Capacidad = int.Parse(CapacidadEntry.Text) // Asegúrate de manejar la conversión y validación correctamente
            };

            string token = Preferences.Get("UserToken", string.Empty);
            bool resultado = await _api.EditarLocal(_localId, localActualizado, token);

            if (resultado)
            {
                await DisplayAlert("Éxito", "Datos actualizados correctamente.", "OK");
                var editarHorariosPage = new EditarHorariosPage(_localId);
                Navigation.InsertPageBefore(editarHorariosPage, this);
                await Navigation.PopAsync(); // Esto removerá la página actual de la pila
            }
            else
            {
                await DisplayAlert("Error", "No se pudo actualizar los datos.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error al actualizar los datos: " + ex.Message, "OK");
        }
    }
}