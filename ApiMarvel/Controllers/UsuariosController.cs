using ApiMarvel.Modelos;
using ApiMarvel.Modelos.Dto;
using ApiMarvel.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Runtime.InteropServices;

namespace ApiMarvel.Controllers
{
    [Route("api/Usuarios")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {

        private readonly IUsuarioRepositorio _usRepo;
        protected RespuestaApi _respuestaApi;
        private readonly IMapper _mapper;

        public UsuariosController(IUsuarioRepositorio usRepo, IMapper mapper)
        {
            _usRepo = usRepo;
            this._respuestaApi = new();
            _mapper = mapper;
        }


        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetUsuarios()
        {
            var listaUsuarios = _usRepo.GetUsuarios();
            var listaUsuariosDto = new List<UsuarioDto>();
            foreach (var lista in listaUsuarios)
            {
                listaUsuariosDto.Add(_mapper.Map<UsuarioDto>(lista));
            }
            return Ok(listaUsuariosDto);
        }


        [Authorize]
        [HttpGet("{usuarioId}", Name = "GetUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetUsuario(string usuarioId)
        {
            var itenUsuario = _usRepo.GetUsuario(usuarioId);

            if (itenUsuario == null)
            {
                return NotFound();
            }
            var itenUsuarioDto = _mapper.Map<UsuarioDto>(itenUsuario);
            return Ok(itenUsuarioDto);
        }

        [AllowAnonymous]
        [HttpPost("registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDto usuarioRegistroDto)
        {
            bool validarUsuarioUnico = _usRepo.IsUniqueUser(usuarioRegistroDto.nombreUsuario);
            if (!validarUsuarioUnico)
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.Result = false;
                _respuestaApi.ErrorMessages.Add("el susuario ya existe en el sistema");
                return BadRequest(_respuestaApi);
            }        

            var usuario = await _usRepo.Registro(usuarioRegistroDto);
            if (usuario == null)
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.Result = false;
                _respuestaApi.ErrorMessages.Add("erroer en el registro");
                return BadRequest(_respuestaApi);
            }

            _respuestaApi.StatusCode = HttpStatusCode.OK;
            _respuestaApi.IsSuccess = true;
            return Ok(_respuestaApi);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> login([FromBody] UsuarioLoginDto usuarioLoginDto)
        {
            var respuestaLogin = await _usRepo.Login(usuarioLoginDto);
            if (respuestaLogin.usuario == null || string.IsNullOrEmpty(respuestaLogin.token))
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.Result = false;
                _respuestaApi.ErrorMessages.Add("el nombreUsuario o passwod son incorrectos");
                return BadRequest(_respuestaApi);
            }

            _respuestaApi.StatusCode = HttpStatusCode.OK;
            _respuestaApi.IsSuccess = true;
            _respuestaApi.Result = respuestaLogin;
            return Ok(_respuestaApi);
        }


        [Authorize]
        [HttpPut("{usuarioId}", Name = "ActualizarUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateUsuario(string usuarioId, [FromBody] UsuarioUpdateDto usuarioUpdateDto)
        {
            if (usuarioUpdateDto == null || string.IsNullOrEmpty(usuarioId))
            {
                return BadRequest(new RespuestaApi
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Datos inválidos" }
                });
            }

            var resultado = await _usRepo.UpdateUsuario(usuarioId, usuarioUpdateDto);
            if (!resultado)
            {
                return NotFound(new RespuestaApi
                {
                    StatusCode = HttpStatusCode.NotFound,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Usuario no encontrado" }
                });
            }

            return Ok(new RespuestaApi
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Result = "Usuario actualizado exitosamente"
            });
        }



        [Authorize]
        [HttpDelete("{usuarioId}", Name = "BorrarUsuarios")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteUsuario(string usuarioId)
        {
            var usuario = _usRepo.GetUsuario(usuarioId);
            if (usuario == null)
            {
                return NotFound(new RespuestaApi
                {
                    StatusCode = HttpStatusCode.NotFound,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Usuario no encontrado" }
                });
            }

            var resultado = await _usRepo.DeleteUsuario(usuario.Id);
            if (!resultado)
            {
                return BadRequest(new RespuestaApi
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Error al eliminar el usuario" }
                });
            }

            return Ok(new RespuestaApi
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Result = "Usuario eliminado exitosamente"
            });
        }





    }
}
