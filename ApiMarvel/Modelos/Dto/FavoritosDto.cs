using System.ComponentModel.DataAnnotations;

namespace ApiMarvel.Modelos.Dto
{
    public class FavoritosDto
    {
        public int Id { get; set; }       
        public string IdUsuario { get; set; }
        public string IdComics { get; set; }
        public ComicDto? Comic { get; set; } 

    }


}
