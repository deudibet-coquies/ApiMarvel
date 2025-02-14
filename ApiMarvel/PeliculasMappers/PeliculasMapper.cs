using ApiMarvel.Modelos;
using ApiMarvel.Modelos.Dto;
using AutoMapper;

namespace ApiMarvel.PeliculasMappers
{
    public class PeliculasMapper : Profile
    {

        public PeliculasMapper()
        {          
            CreateMap<Favorito, FavoritosDto>().ReverseMap();
            CreateMap<AppUsuario, UsuarioDto>().ReverseMap();
            CreateMap<AppUsuario, UsuarioDatosDto>().ReverseMap();
        }

    }
}
