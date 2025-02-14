using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiMarvel.Modelos.Dto
{
    public class UsuarioRegistroDto
    {
        [Required(ErrorMessage = "El nombreUsuario es obligatorio")]
        public string nombreUsuario { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string nombre { get; set; }
        [Required(ErrorMessage = "El password es obligatorio")]
        public string password { get; set; }
        [Required(ErrorMessage = "La identificación es obligatoria")]
        public string identificacion { get; set; }
        public string role { get; set; }

    }
}
