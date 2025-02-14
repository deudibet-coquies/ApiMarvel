using ApiMarvel.Servicios;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiMarvel.Modelos.Dto
{
    public class MarvelResponseDto
    {
        public Data Data { get; set; }

    }   

    public class Data
    {
        public ComicDto[] Results { get; set; }
    }
}
