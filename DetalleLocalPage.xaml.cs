using ProyectoApp.Models;
using ProyectoApp.Services;
using System.IdentityModel.Tokens.Jwt;

namespace ProyectoApp;

public partial class DetalleLocalPage : ContentPage
{
    private readonly HttpClient _httpClient = new HttpClient();
    private string _baseUrl = "http://10.0.2.2:5260/api/Reservas/verificarDisponibilidad";
    private readonly APIService _api;
    public Local LocalActual { get; set; }

    public DetalleLocalPage(int localId)
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
                                Command = new Command(() => HacerReserva(horario.ID)), // Comando para hacer la reserva
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
    private async void HacerReserva(int horarioId)
    {
        // Obtén la fecha seleccionada por el usuario y el token de autenticación
        DateTime fechaSeleccionada = fechaReserva.Date;
        string token = Preferences.Get("UserToken", defaultValue: string.Empty);

        // Crea el objeto de reserva
        var reserva = new Reserva
        {
            LocalID = LocalActual.Id,
            HorarioID = horarioId,
            Fecha = fechaSeleccionada,
            UsuarioID = GetUserIdFromToken(token)
        };

        try
        {
            // Haz la llamada a la API para hacer la reserva
            var esReservado = await _api.Reservar(reserva, token);

            if (esReservado)
            {
                // Si la reserva fue exitosa, informa al usuario
                await DisplayAlert("Éxito", "La reserva ha sido realizada con éxito.", "OK");
                // Obtiene la referencia de la FlyoutPage que es la MainPage actual
                if (Application.Current.MainPage is FlyoutPage flyoutPage)
                {
                    // Crea una nueva instancia de la ReservasPage
                    var reservasPage = new ReservasPage();

                    // Configura la ReservasPage como la nueva página de detalle
                    flyoutPage.Detail = new NavigationPage(reservasPage);

                    // Cierra el menú lateral
                    flyoutPage.IsPresented = false;
                }
            }
        }
        catch (Exception ex)
        {
            // Si algo falla, muestra el error al usuario
            await DisplayAlert("Error", $"No se pudo realizar la reserva: {ex.Message}", "OK");
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