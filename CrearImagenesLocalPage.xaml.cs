using ProyectoApp.Models;
using ProyectoApp.Services;

namespace ProyectoApp;

public partial class CrearImagenesLocalPage : ContentPage
{
    private readonly APIService _api;
    private int _localId;
    public CrearImagenesLocalPage(int localId)
    {
        InitializeComponent();
        _api = App.ServiceProvider.GetService<APIService>(); // Obtener la instancia de APIService
        _localId = localId;
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        try
        {
            // Crear una lista de ImagenLocal con los URLs de los Entry
            var imagenesLocal = new List<ImagenLocal>
            {
                new ImagenLocal { Url = imagen1Entry.Text },
                new ImagenLocal { Url = imagen2Entry.Text },
                new ImagenLocal { Url = imagen3Entry.Text }
            }.Where(imagen => !string.IsNullOrWhiteSpace(imagen.Url)).ToList(); // Filtrar las imágenes con URL vacío

            if (imagenesLocal.Any())
            {
                string token = Preferences.Get("UserToken", string.Empty);
                bool resultado = await _api.AddImagenes(token, _localId, imagenesLocal);

                if (resultado)
                {
                    await DisplayAlert("Éxito", "Imágenes creadas correctamente.", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "No se pudieron crear las imágenes.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "No hay imágenes para agregar.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error al crear las imágenes: " + ex.Message, "OK");
        }
    }
}