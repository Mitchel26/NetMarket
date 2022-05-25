using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class UsuarioDTO
    {
        public string    Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public bool Admin { get; set; }
        public string Imagen { get; set; }


    }
}
