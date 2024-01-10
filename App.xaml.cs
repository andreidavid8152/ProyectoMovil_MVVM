using ProyectoApp.Services;

namespace ProyectoApp
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        public App()
        {
            InitializeComponent();

            // Configurar el contenedor de servicios
            var services = new ServiceCollection();
            ConfigureServices(services);

            // Construir el proveedor de servicios
            _serviceProvider = services.BuildServiceProvider();

            // Establecer la página principal utilizando el servicio
            MainPage = new NavigationPage((Page)_serviceProvider.GetService(typeof(LoginPage)))
            {
                BarBackgroundColor = Color.FromHex("#14282f")
            };
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Registrar APIService como un singleton para que la misma instancia se utilice en toda la app
            services.AddSingleton<APIService>();

            // Registrar tus páginas con dependencias aquí
            services.AddTransient<LoginPage>();
            services.AddTransient<MainPage>();
            services.AddTransient<DetalleLocalPage>();
            services.AddTransient<MisLocalesPage>();

            // Si tienes más servicios, también deberías registrarlos aquí
        }

        public static IServiceProvider ServiceProvider => ((App)Current)._serviceProvider;
    }
}