using ProyectoApp.Models;
using ProyectoApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProyectoApp.ViewModels
{
    public class ComentariosViewModel : INotifyPropertyChanged
    {
        private readonly APIService _api = App.ServiceProvider.GetService<APIService>();
        public ObservableCollection<Comentario> Comentarios { get; private set; }
        public ICommand EliminarComentarioCommand { get; private set; }

        public ComentariosViewModel()
        {
            Comentarios = new ObservableCollection<Comentario>();
            EliminarComentarioCommand = new Command<Comentario>(async (comentario) => await EliminarComentario(comentario));
            CargarComentarios();
        }

        private async void CargarComentarios()
        {
            try
            {
                string token = Preferences.Get("UserToken", string.Empty);
                int usuarioId = GetUserIdFromToken(token);
                var comentariosObtenidos = await _api.ObtenerComentariosPorUsuario(usuarioId, token);
                Comentarios.Clear();
                foreach (var comentario in comentariosObtenidos)
                {
                    Comentarios.Add(comentario);
                }
            }
            catch (Exception ex)
            {
                // Manejo de la excepción mostrando un mensaje al usuario
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Ocurrió un problema al cargar los comentarios: " + ex.Message,
                    "OK");
            }
        }

        private async Task EliminarComentario(Comentario comentarioAEliminar)
        {
            // Confirmar con el usuario si realmente desea eliminar el comentario
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Eliminar Comentario",
                "¿Estás seguro de que quieres eliminar este comentario?",
                "Sí",
                "No");

            if (!confirm)
            {
                return; // El usuario canceló la operación
            }

            try
            {
                string token = Preferences.Get("UserToken", string.Empty);
                await _api.EliminarComentario(comentarioAEliminar.Id, token);
                Comentarios.Remove(comentarioAEliminar);
                await Application.Current.MainPage.DisplayAlert(
                    "Comentario Eliminado",
                    "El comentario ha sido eliminado correctamente.",
                    "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "No se pudo eliminar el comentario: " + ex.Message,
                    "OK");
            }
        }

        private int GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            var userIdClaim = jwtToken?.Claims.FirstOrDefault(claim => claim.Type == "nameid")?.Value;
            return int.Parse(userIdClaim);
        }

        // Implementación de INotifyPropertyChanged...
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
