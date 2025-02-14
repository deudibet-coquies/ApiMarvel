using ApiMarvel.Data;
using ApiMarvel.Modelos;
using ApiMarvel.Modelos.Dto;
using ApiMarvel.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XAct;
using XAct.Library.Settings;
using XSystem.Security.Cryptography;

namespace ApiMarvel.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {

        private readonly ApplicationDbContext _applicationDbContext;
        private string claveSecreta;
        private readonly UserManager<AppUsuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UsuarioRepositorio(ApplicationDbContext applicationDbContext,
            IConfiguration config,
            RoleManager<IdentityRole> roleManager, UserManager<AppUsuario> userManager, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        public AppUsuario GetUsuario(string usuarioId)
        {
            return _applicationDbContext.AppUsuario.FirstOrDefault(u => u.Id == usuarioId);
        }

        public ICollection<AppUsuario> GetUsuarios()
        {
            return _applicationDbContext.AppUsuario.OrderBy(u => u.UserName).ToList();
        }

        public bool IsUniqueUser(string usuario)
        {
            var usuarioDb = _applicationDbContext.AppUsuario.FirstOrDefault(u => u.UserName == usuario);
            if (usuarioDb == null)
            {
                return true;
            }
            else
            {
                return false;
            }           
        }


        public async Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
         
            AppUsuario usuario = new AppUsuario()
            {
                UserName = usuarioRegistroDto.nombreUsuario,
                identificacion = usuarioRegistroDto.identificacion,
                Email = usuarioRegistroDto.nombreUsuario,
                NormalizedEmail = usuarioRegistroDto.nombreUsuario.ToUpper(),
                nombre = usuarioRegistroDto.nombre,
            };

            var result = await _userManager.CreateAsync(usuario, usuarioRegistroDto.password);
            if (result.Succeeded)
            {
                // pro primera vez y creacion de roles
                if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole("admin"));
                    await _roleManager.CreateAsync(new IdentityRole("registrado"));
                }

                await _userManager.AddToRoleAsync(usuario, "registrado");
                var usuarioRetornado = _applicationDbContext.AppUsuario.FirstOrDefault( u => u.UserName == usuarioRegistroDto.nombreUsuario);
                
                return _mapper.Map<UsuarioDatosDto>(usuarioRetornado);
            }


            return new UsuarioDatosDto();
        }






        public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {
           // var passwordEncriptada = obtenermd5(usuarioLoginDto.password);

            var usuarios = _applicationDbContext.AppUsuario.FirstOrDefault(
                u => u.UserName.ToLower() == usuarioLoginDto.nombreUsuario.ToLower());
           
            bool isValidate = await _userManager.CheckPasswordAsync(usuarios, usuarioLoginDto.password);
            
            // se valida si el usuario existe
            if (usuarios == null || isValidate == false)
            {
                return new UsuarioLoginRespuestaDto()
                {
                    token = "",
                    usuario = null
                };
            }

            // usuario encontrado
            var roles = await _userManager.GetRolesAsync(usuarios);

            var manejadorToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(claveSecreta);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, usuarios.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    new Claim(ClaimTypes.NameIdentifier,usuarios.Id.ToString() )
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = manejadorToken.CreateToken(tokenDescription);

            UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
            {
                token = manejadorToken.WriteToken(token),
                usuario = _mapper.Map<UsuarioDatosDto>(usuarios)
            };

            return usuarioLoginRespuestaDto;
        }

        //Método para encriptar contraseña con MD5 se usa tanto en el Acceso como en el Registro
        public static string obtenermd5(string valor)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
            data = x.ComputeHash(data);
            string resp = "";
            for (int i = 0; i < data.Length; i++)
                resp += data[i].ToString("x2").ToLower();
            return resp;
        }


        public async Task<bool> UpdateUsuario(string usuarioId, UsuarioUpdateDto usuarioUpdateDto)
        {
            var usuario = await _userManager.FindByIdAsync(usuarioId);
            if (usuario == null)
            {
                return false; // Usuario no encontrado
            }

            bool isUpdated = false;

            // Solo actualiza los campos si fueron enviados y son diferentes
            if (!string.IsNullOrEmpty(usuarioUpdateDto.nombreUsuario) && usuario.UserName != usuarioUpdateDto.nombreUsuario)
            {
                usuario.UserName = usuarioUpdateDto.nombreUsuario;
                usuario.Email = usuarioUpdateDto.nombreUsuario;
                usuario.NormalizedEmail = usuarioUpdateDto.nombreUsuario.ToUpper();
                isUpdated = true;
            }

            if (!string.IsNullOrEmpty(usuarioUpdateDto.nombre) && usuario.nombre != usuarioUpdateDto.nombre)
            {
                usuario.nombre = usuarioUpdateDto.nombre;
                isUpdated = true;
            }

            // Si no hubo cambios, no es necesario llamar a UpdateAsync
            if (!isUpdated)
            {
                return true; // No se realizaron cambios, pero la operación es exitosa
            }

            var resultado = await _userManager.UpdateAsync(usuario);
            return resultado.Succeeded;
        }


        public async Task<bool> DeleteUsuario(string usuarioId)
        {
            var usuario = await _userManager.FindByIdAsync(usuarioId);
            if (usuario == null)
            {
                return false; // Usuario no encontrado
            }

            var resultado = await _userManager.DeleteAsync(usuario);
            return resultado.Succeeded;
        }

       
    }
}
