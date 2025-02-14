using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using XAct;

namespace ApiMarvel.Modelos.Dto
{
    public class ComicDto
    {
        public int id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public Thumbnail thumbnail { get; set; }

        [NotMapped] // Indica que no es un campo de la base de datos
        public string ImageUrl => thumbnail != null ? $"{thumbnail.path}.{thumbnail.extension}" : null;
    }

    public class Thumbnail
    {
        public string path { get; set; }
        public string extension { get; set; }

    }


}
