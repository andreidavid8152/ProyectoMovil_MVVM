using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoApp.Models
{
    public class Horario
    {
        public int ID { get; set; }

        public int LocalID { get; set; }

        [Required(ErrorMessage = "La hora de inicio es requerida.")]

        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es requerida.")]
        [GreaterThanStart("HoraInicio", ErrorMessage = "La hora de fin debe ser al menos 2 horas después de la hora de inicio.")]
        public TimeSpan HoraFin { get; set; }

        // Relaciones
        public Local? Local { get; set; }
    }

    public class GreaterThanStartAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        // Constructor que recibe el nombre de la propiedad contra la que vamos a comparar (HoraInicio en este caso)
        public GreaterThanStartAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        // Este método se encarga de hacer la validación real
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            // Obtiene el valor de la propiedad HoraInicio del objeto que se está validando (HoraInicio)
            var startTime = (TimeSpan)validationContext.ObjectType.GetProperty(_comparisonProperty).GetValue(validationContext.ObjectInstance);

            // Obtiene el valor de la propiedad que se está validando (HoraFin)
            var endTime = (TimeSpan)value;

            // Comprueba si la hora de fin es al menos 2 horas después que la hora de inicio
            if (endTime - startTime < TimeSpan.FromHours(2))
            {
                return new ValidationResult(ErrorMessage);
            }

            // Si todo está bien, devuelve Success
            return ValidationResult.Success;
        }
    }
}
