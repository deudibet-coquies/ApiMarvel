using ApiMarvel.Migrations;
using ApiMarvel.Modelos;
using ApiMarvel.Modelos.Dto;
using ApiMarvel.Repositorio.IRepositorio;
using ApiMarvel.Servicios;
using ApiMarvel.Servicios.IServicios;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiMarvel.Controllers
{
   
//[Route("api/[controller]")]
    [Route("api/comics")]
    [ApiController]
    public class MarvelController : ControllerBase
    {
        private readonly IMarvelService _ctSer;
        private readonly IMapper _mapper;


        public MarvelController(IMarvelService ctSer, IMapper mapper)
        {
            _ctSer = ctSer;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        [ResponseCache(CacheProfileName = "PorDefecto30Segundos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetComics()
        {
            var ListaComics = await _ctSer.GetComicsAsync();
            var ListadoComicsDto = new List<ComicDto>();
            foreach (var lista in ListaComics)
            {
                ListadoComicsDto.Add(_mapper.Map<ComicDto>(lista));
            }
            return Ok(ListadoComicsDto);
        }

      
        [Route("favoritos")]
        [Authorize]
        [HttpGet]
        [ResponseCache(CacheProfileName = "PorDefecto30Segundos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetFavoritos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var listaFavorito = await _ctSer.GetFavoritos(userId);

            if (listaFavorito == null || !listaFavorito.Any())
            {
                return NotFound("No tienes favoritos registrados.");
            }

            var listaFavoritosDto = new List<FavoritosDto>();

            foreach (var favorito in listaFavorito)
            {
                var favoritoDto = _mapper.Map<FavoritosDto>(favorito);

                // Obtener información del cómic (simulación de API externa o BD)
                var comicInfo = await _ctSer.GetComicId(favorito.IdComics);

                if (comicInfo != null)
                {
                    favoritoDto.Comic = _mapper.Map<ComicDto>(comicInfo);
                }

                listaFavoritosDto.Add(favoritoDto);
            }

            return Ok(listaFavoritosDto);
        }


        [Route("favoritos")]
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ToggleFavorito([FromBody] FavoritosCrearDto favoritoDto)
        {
            var _respuestaApi = new RespuestaApi();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (favoritoDto == null || string.IsNullOrEmpty(favoritoDto.IdComics))
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMessages.Add("Datos inválidos.");
                return BadRequest(_respuestaApi);
            }

            // Buscar si ya existe el favorito en la base de datos
            var favoritoExistente = await _ctSer.GetFavoritoPorUsuarioYComic(userId, favoritoDto.IdComics);

            if (favoritoExistente != null)
            {
                // Si existe, eliminarlo (quitar de favoritos)
                var eliminado = await _ctSer.EliminarFavorito(favoritoExistente);

                if (!eliminado)
                {
                    _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                    _respuestaApi.IsSuccess = false;
                    _respuestaApi.ErrorMessages.Add("No se pudo eliminar de favoritos.");
                    return BadRequest(_respuestaApi);
                }

                _respuestaApi.StatusCode = HttpStatusCode.OK;
                _respuestaApi.IsSuccess = true;
                _respuestaApi.Result = "Eliminado de favoritos.";
                return Ok(_respuestaApi);
            }
            else
            {
                // Si no existe, agregarlo como favorito
                var nuevoFavorito = new Favorito
                {
                    IdUsuario = userId,
                    IdComics = favoritoDto.IdComics
                };

                var agregado = await _ctSer.AddFavorito(nuevoFavorito);

                if (!agregado)
                {
                    _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                    _respuestaApi.IsSuccess = false;
                    _respuestaApi.ErrorMessages.Add("No se pudo agregar a favoritos.");
                    return BadRequest(_respuestaApi);
                }

                _respuestaApi.StatusCode = HttpStatusCode.Created;
                _respuestaApi.IsSuccess = true;
                _respuestaApi.Result = "Agregado a favoritos.";
                return Ok(_respuestaApi);
            }
        }











    }
}
