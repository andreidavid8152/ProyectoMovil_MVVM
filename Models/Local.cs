using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoApp.Models
{
    public class Local
    {
        public int Id { get; set; }

        public int PropietarioID { get; set; }


        [Required(ErrorMessage = "El nombre del local es requerido.")]
        public string Nombre { get; set; }


        [Required(ErrorMessage = "La descripción es requerida.")]
        public string Descripcion { get; set; }


        [Required(ErrorMessage = "La dirección es requerida.")]
        public string Direccion { get; set; }


        [Required(ErrorMessage = "La capacidad es requerida.")]
        [Range(5, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor o igual que 5.")]
        public int? Capacidad { get; set; }

        public List<Horario>? Horarios { get; set; }
        public List<ImagenLocal>? Imagenes { get; set; }
        public List<Comentario>? Comentarios { get; set; }

    }
}
