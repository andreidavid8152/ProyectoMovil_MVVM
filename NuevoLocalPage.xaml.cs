using ProyectoApp.Models;
using ProyectoApp.Services;
using System.IdentityModel.Tokens.Jwt;

namespace ProyectoApp;

public partial class NuevoLocalPage : ContentPage
{
    private readonly APIService _api;
    private Local _nuevoLocal;
    public NuevoLocalPage()
    {
        InitializeComponent();
        _api = App.ServiceProvider.GetService<APIService>(); // Obtener la instancia de APIService del contenedor
        _nuevoLocal = new Local(); // Crear una nueva instancia de Local
        this.BindingContext = _nuevoLocal; // Asignar la instancia al BindingContext de la página
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        try
        {
            string token = Preferences.Get("UserToken", string.Empty);
            _nuevoLocal.PropietarioID = GetUserIdFromToken(token);
            var localCreado = await _api.CrearLocal(_nuevoLocal, token);

            if (localCreado != null)
            {
                await DisplayAlert("Éxito", "Local creado correctamente.", "OK");
                var crearHorarios = new CrearHorariosLocalPage(localCreado.Id);
                Navigation.InsertPageBefore(crearHorarios, this);
                await Navigation.PopAsync(); // Esto removerá la página actual de la pila
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error al crear el local: " + ex.Message, "OK");
        }
    }

    private int GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
        var userIdClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == "nameid")?.Value;
        return int.Parse(userIdClaim);
    }
}