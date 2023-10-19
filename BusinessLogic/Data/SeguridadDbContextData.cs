using Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Data
{
    public class SeguridadDbContextData
    {
        public static async Task SeedUserAsync(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!userManager.Users.Any())
            {
                var usuario = new Usuario()
                {
                    Nombre = "Jhon",
                    Apellido = "Varas García",
                    UserName = "jvaras",
                    Email = "jhonmitchelupn@gmail.com",
                    Direccion = new Direccion()
                    {
                        Calle = "28 Julio",
                        Ciudad = "Laredo",
                        CodigoPostal = "123",
                        Departamento = "La Libertad",
                        Pais = "Perú"
                    }
                };

                await userManager.CreateAsync(usuario, "Codigo2022@");
            }

            if (!roleManager.Roles.Any())
            {
                var role = new IdentityRole()
                {
                    Name = "ADMIN"
                };

                await roleManager.CreateAsync(role);
            }
        }
    }
}
