using ApiMarvel.Modelos;
using ApiMarvel.Modelos.Dto;

namespace ApiMarvel.Servicios.IServicios
{
    public interface IMarvelService
    {
        

        Task<ICollection<ComicDto>> GetComicsAsync();

        // todo el tema de favoritos

        Task<ICollection<Favorito>> GetFavoritos(string usuarioId);

        Task<Favorito> GetFavoritoPorUsuarioYComic(string userId, string comicId);
        Task<bool> EliminarFavorito(Favorito favorito);
        Task<bool> AddFavorito(Favorito favorito);

        Task<ComicDto> GetComicId(string comicId);





    }
}
