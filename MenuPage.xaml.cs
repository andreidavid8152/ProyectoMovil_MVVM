namespace ProyectoApp;

public partial class MenuPage : ContentPage
{
    public MenuPage()
    {
        InitializeComponent();
        menuItemsCollectionView.ItemsSource = new List<MenuItem>
        {
            new MenuItem { Title = "Dashboard", TargetType = typeof(MainPage), Icon = "\uD83D\uDCCA" },
            new MenuItem { Title = "Mis Locales", TargetType = typeof(MisLocalesPage), Icon = "\uD83C\uDFEA" },
            new MenuItem { Title = "Mis Reservas", TargetType = typeof(ReservasPage), Icon = "\uD83D\uDCC5" },
            new MenuItem { Title = "Mis Comentarios", TargetType = typeof(ComentariosPage), Icon = "\uD83D\uDCAC" }
        };

        menuItemsCollectionView.SelectionChanged += OnSelectionChanged;
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var collectionView = (CollectionView)sender;
        if (collectionView.SelectedItem is MenuItem item)
        {
            var page = (Page)Activator.CreateInstance(item.TargetType);
            var flyoutPage = this.Parent as FlyoutPage; // Obtiene la referencia del FlyoutPage actual
            if (flyoutPage != null)
            {
                flyoutPage.Detail = new NavigationPage(page)
                {
                    BarBackgroundColor = Color.FromHex("#14282f")
                };
                flyoutPage.IsPresented = false; // Esto cierra el menú después de la selección de un elemento
            }
            collectionView.SelectedItem = null; // Desmarca el elemento seleccionado
        }
    }

    private async void OnPerfilTapped(object sender, EventArgs e)
    {
        var flyoutPage = this.Parent as FlyoutPage;
        if (flyoutPage != null)
        {
            var perfilPage = new MiPerfilPage(); // Asegúrate de tener una página llamada MiPerfilPage
            flyoutPage.Detail = new NavigationPage(perfilPage)
            {
                BarBackgroundColor = Color.FromHex("#14282f")
            };
            flyoutPage.IsPresented = false;
        }
    }

    private async void OnCerrarSesionTapped(object sender, EventArgs e)
    {
        // Eliminar el token guardado
        Preferences.Remove("UserToken");
        // Navegar al usuario a la pantalla de inicio de sesión o a la pantalla principal
        // Asumiendo que tienes una LoginPage o una página similar para manejar el inicio de sesión
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }

}

public class MenuItem
{
    public string Title { get; set; }
    public Type TargetType { get; set; }
    public string Icon { get; set; }
}