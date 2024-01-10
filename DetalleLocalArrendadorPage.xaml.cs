using ProyectoApp.Models;
using ProyectoApp.Services;
using System.IdentityModel.Tokens.Jwt;

namespace ProyectoApp;

public partial class DetalleLocalArrendadorPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private string _baseUrl = "https://api-proyecto20231210220132.azurewebsites.net/api/Reservas/verificarDisponibilidad";
    private readonly APIService _api;
    public Local LocalActual { get; set; }
    public DetalleLocalArrendadorPage(int localId)
    {
        InitializeComponent();
        _api = App.ServiceProvider.GetService<APIService>();

        // Cargar los detalles del local
        CargarDetallesLocal(localId);
    }

    private async void CargarDetallesLocal(int localId)
    {
        try
        {
            // Aquí asumo que tienes una manera de obtener el token
            string token = Preferences.Get("UserToken", defaultValue: string.Empty);

            LocalActual = await _api.ObtenerLocal(localId, token);

            if (LocalActual != null)
            {
                this.BindingContext = LocalActual;
            }
            else
            {
                await DisplayAlert("Error", "No se encontró el local", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error al obtener los detalles del local: " + ex.Message, "OK");
        }
    }

    private async void verificarDisponibilidad(object sender, EventArgs e)
    {
        stackHorarios.Children.Clear();
        string token = Preferences.Get("UserToken", defaultValue: string.Empty);
        DateTime fechaSeleccionada = fechaReserva.Date;

        foreach (var horario in LocalActual.Horarios)
        {
            string url = $"{_baseUrl}?localId={LocalActual.Id}&horarioId={horario.ID}&fecha={fechaSeleccionada.ToString("yyyy-MM-dd")}";

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var isDisponible = bool.Parse(content);

                    // Aquí actualizamos la interfaz de usuario
                    // Asegúrate de ejecutar en el hilo principal
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var grid = new Grid();
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                        var horarioLabel = new Label
                        {
                            Text = $"{horario.HoraInicio} - {horario.HoraFin}",
                            Margin = new Thickness(2),
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.StartAndExpand
                        };
                        Grid.SetColumn(horarioLabel, 0); // Establece la columna del Label
                        Grid.SetRow(horarioLabel, 0); // Establece la fila del Label (opcional si es la primera fila)
                        grid.Children.Add(horarioLabel); // Añade el Label al Grid

                        Button reservaButton;
                        if (isDisponible)
                        {
                            reservaButton = new Button
                            {
                                Text = "Reservar",
                                Margin = new Thickness(2),
                                BackgroundColor = Color.FromHex("#34C759"), // Verde para disponible
                                IsEnabled = false,
                                VerticalOptions = LayoutOptions.Center
                            };
                        }
                        else
                        {
                            reservaButton = new Button
                            {
                                Text = "No Disponible",
                                Margin = new Thickness(2),
                                BackgroundColor = Color.FromHex("#FF3B30"), // Rojo para no disponible
                                IsEnabled = false,
                                VerticalOptions = LayoutOptions.Center
                            };
                        }
                        Grid.SetColumn(reservaButton, 1); // Establece la columna del Button
                        Grid.SetRow(reservaButton, 0); // Establece la fila del Button (opcional si es la primera fila)
                        grid.Children.Add(reservaButton); // Añade el Button al Grid

                        stackHorarios.Children.Add(grid); // Añade el Grid al StackLayout principal
                    });

                }
                else
                {
                    // Manejar respuesta fallida
                    await DisplayAlert("Error", "No se pudo verificar la disponibilidad.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Manejar excepción
                await DisplayAlert("Error", $"Error al verificar la disponibilidad: {ex.Message}", "OK");
            }
        }
    }
}