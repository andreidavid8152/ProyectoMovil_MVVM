using ProyectoApp.Models;
using ProyectoApp.Services;

namespace ProyectoApp;

public partial class EditarImagenesLocalPage : ContentPage
{
    private readonly APIService _api;
    private int _localId;
    private List<ImagenLocal> _imagenesLocal;
    public EditarImagenesLocalPage(int localId)
    {
        InitializeComponent();
        _api = App.ServiceProvider.GetService<APIService>(); // Obtener la instancia de APIService
        _localId = localId;
        CargarDatosImagenes();
    }

    private async void CargarDatosImagenes()
    {
        try
        {
            string token = Preferences.Get("UserToken", string.Empty);
            _imagenesLocal = await _api.ObtenerImagenesLocal(_localId, token);

            // Asignar los URL a los Entry correspondientes
            if (_imagenesLocal != null && _imagenesLocal.Count >= 3)
            {
                imagen1Entry.Text = _imagenesLocal[0].Url;
                imagen2Entry.Text = _imagenesLocal[1].Url;
                imagen3Entry.Text = _imagenesLocal[2].Url;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo cargar las imágenes: " + ex.Message, "OK");
        }
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        try
        {
            if (_imagenesLocal != null && _imagenesLocal.Count >= 3)
            {
                // Actualizar los URL de las imágenes
                _imagenesLocal[0].Url = imagen1Entry.Text;
                _imagenesLocal[1].Url = imagen2Entry.Text;
                _imagenesLocal[2].Url = imagen3Entry.Text;

                string token = Preferences.Get("UserToken", string.Empty);

                bool resultado = await _api.EditarImagenesLocal(_localId, _imagenesLocal, token);

                if (resultado)
                {
                    await DisplayAlert("Éxito", "Imágenes actualizadas correctamente.", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo actualizar las imágenes.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "No hay suficientes imágenes para actualizar.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error al guardar las imágenes: " + ex.Message, "OK");
        }
    }
}