using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoApp.Models
{
    public class ImagenLocal
    {
        public string Url { get; set; }

        public int LocalID { get; set; }

        public Local? Local { get; set; }

    }
}
