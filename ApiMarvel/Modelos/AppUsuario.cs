using Microsoft.AspNetCore.Identity;

namespace ApiMarvel.Modelos
{
    public class AppUsuario : IdentityUser
    {

        // se pueden agregar campos personalizados 
        public string nombre { get; set; }

        public string identificacion { get; set; }

    }
}
