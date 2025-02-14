using System.ComponentModel.DataAnnotations;

namespace ApiMarvel.Modelos
{
    public class Favorito
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string IdUsuario { get; set; }

        [Required]
        public string IdComics { get; set; }
    }
}
