using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiMarvel.Modelos.Dto
{
    public class UsuarioUpdateDto
    {
        public string nombreUsuario { get; set; }
        public string nombre { get; set; }
    }
}
