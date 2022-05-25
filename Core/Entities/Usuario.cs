using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class Usuario: IdentityUser
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public Direccion Direccion { get; set; }
        public string Imagen { get; set; }

    }
}
