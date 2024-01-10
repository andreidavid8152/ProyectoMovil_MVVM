using ProyectoApp.Models;
using ProyectoApp.Services;
using System.Collections.ObjectModel;

namespace ProyectoApp;

public partial class MisLocalesPage : ContentPage
{
    private readonly APIService _api;
    public ObservableCollection<Local> Locales { get; set; }
    public MisLocalesPage()
    {
        InitializeComponent();
        _api = App.ServiceProvider.GetService<APIService>(); // Obtener la instancia de APIService del contenedor
        Locales = new ObservableCollection<Local>();
        this.BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarLocales();
    }


    private async void OnClickNuevoLocal(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NuevoLocalPage());
    }


    private async Task CargarLocales()
    {
        try
        {
            string token = Preferences.Get("UserToken", defaultValue: string.Empty);
            var locales = await _api.ObtenerLocalesArrendador(token);

            Locales.Clear();
            foreach (var local in locales)
            {
                Locales.Add(local);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo obtener los locales: " + ex.Message, "OK");
        }
    }

    private async void OnDetailsClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int localId)
        {
            // Navegar a la página de detalles del local
            await Navigation.PushAsync(new DetalleLocalArrendadorPage(localId));
        }
    }


    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int localId)
        {
            await Navigation.PushAsync(new EditarLocalPage(localId));
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int localId)
        {
            // Confirmar con el usuario si realmente desea eliminar el local
            bool confirm = await DisplayAlert("Eliminar Local", "¿Estás seguro de que quieres eliminar este local?", "Sí", "No");
            if (!confirm)
            {
                return; // El usuario canceló la operación
            }

            string token = Preferences.Get("UserToken", string.Empty);

            try
            {
                bool eliminado = await _api.EliminarLocal(localId, token);

                if (eliminado)
                {
                    // Elimina el local de la colección para actualizar la UI
                    var localAEliminar = Locales.FirstOrDefault(l => l.Id == localId);
                    if (localAEliminar != null)
                    {
                        Locales.Remove(localAEliminar);
                    }

                    // Mensaje de confirmación
                    await DisplayAlert("Local Eliminado", "El local ha sido eliminado correctamente.", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo eliminar el local porque tiene reservaciones", "OK");
                }
            }
            catch (Exception ex)
            {
                // Muestra un mensaje de error si algo sale mal
                await DisplayAlert("Error", $"No se pudo eliminar el local: {ex.Message}", "OK");
            }
        }
    }

}