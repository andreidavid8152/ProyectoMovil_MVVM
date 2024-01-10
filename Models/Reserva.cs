using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoApp.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        public int LocalID { get; set; }

        public int UsuarioID { get; set; }

        public int HorarioID { get; set; }

        public DateTime Fecha { get; set; }


        public Local? Local { get; set; }
        public Horario? Horario { get; set; }
    }
}
