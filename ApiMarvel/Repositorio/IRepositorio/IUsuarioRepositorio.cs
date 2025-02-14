using ApiMarvel.Modelos;
using ApiMarvel.Modelos.Dto;

namespace ApiMarvel.Repositorio.IRepositorio
{
    public interface IUsuarioRepositorio
    {
       // ICollection<Usuario> GetUsuarios();

        ICollection<AppUsuario> GetUsuarios();

        //Usuario GetUsuario(int usuarioId);
        AppUsuario GetUsuario(string usuarioId);
        bool IsUniqueUser(string usuario);

        //Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto);
        Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto);


       // Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto);
       Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuarioRegistroDto);

       
        Task<bool> UpdateUsuario(string usuarioId, UsuarioUpdateDto usuarioUpdateDto);   

        Task<bool> DeleteUsuario(string usuarioId);


    }
}
