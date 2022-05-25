using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.Errors;
using WebApi.Extensions;

namespace WebApi.Controllers
{
    public class UsuariosController : BaseApiController
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;
        private readonly IPasswordHasher<Usuario> passwordHasher;
        private readonly IGenericSeguridadRepository<Usuario> seguridadRepository;
        private readonly RoleManager<IdentityRole> roleManager;

        public UsuariosController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, ITokenService tokenService,
            IMapper mapper, IPasswordHasher<Usuario> passwordHasher, IGenericSeguridadRepository<Usuario> seguridadRepository,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
            this.passwordHasher = passwordHasher;
            this.seguridadRepository = seguridadRepository;
            this.roleManager = roleManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UsuarioDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            var usuario = await userManager.FindByEmailAsync(loginDTO.Email);

            if (usuario is null)
            {
                return Unauthorized(new CodeErrorResponse(401));
            }

            var resultado = await signInManager.CheckPasswordSignInAsync(usuario, loginDTO.Password, false);

            if (!resultado.Succeeded)
            {
                return Unauthorized(new CodeErrorResponse(401));
            }

            var roles = await userManager.GetRolesAsync(usuario);

            return new UsuarioDTO()
            {
                Email = usuario.Email,
                UserName = usuario.UserName,
                Token = tokenService.CreateToken(usuario, roles),
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Imagen = usuario.Imagen,
                Admin = roles.Contains("ADMIN") ? true : false

            };
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<UsuarioDTO>> Registrar(RegistrarDTO registrarDTO)
        {
            var usuario = new Usuario()
            {
                Email = registrarDTO.Email,
                UserName = registrarDTO.Username,
                Nombre = registrarDTO.Nombre,
                Apellido = registrarDTO.Apellido,
                PasswordHash = registrarDTO.Password
            };

            var resultado = await userManager.CreateAsync(usuario);

            if (!resultado.Succeeded)
            {
                return BadRequest(new CodeErrorResponse(400));
            }

            return new UsuarioDTO()
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                UserName = usuario.UserName,
                Email = usuario.Email,
                Token = tokenService.CreateToken(usuario, null),
                Admin = false

            };
        }

        [HttpPut("actualizar/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UsuarioDTO>> Actualizar(string id, RegistrarDTO registrarDTO)
        {
            var usuario = await userManager.FindByIdAsync(id);
            if (usuario is null)
            {
                return NotFound(new CodeErrorResponse(404, "El usuario no exíste"));

            }
            usuario.Nombre = registrarDTO.Nombre;
            usuario.Apellido = registrarDTO.Apellido;
            usuario.Imagen = registrarDTO.Imagen;

            if (!string.IsNullOrEmpty(registrarDTO.Password))
            {
                usuario.PasswordHash = passwordHasher.HashPassword(usuario, registrarDTO.Password);
            }


            var resultado = await userManager.UpdateAsync(usuario);
            if (!resultado.Succeeded)
            {
                return BadRequest(new CodeErrorResponse(400, "No se pudo actualizar el usuario"));
            }

            var roles = await userManager.GetRolesAsync(usuario);

            return new UsuarioDTO()
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                UserName = usuario.UserName,
                Token = tokenService.CreateToken(usuario, roles),
                Imagen = usuario.Imagen,
                Admin = roles.Contains("ADMIN") ? true : false
            };
        }

        [HttpGet("paginacion")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        public async Task<ActionResult<Pagination<UsuarioDTO>>> GetUsuarios([FromQuery] UsuarioSpecificationParams usuarioParams)
        {
            var spec = new UsuarioSpecification(usuarioParams);
            var usuario = await seguridadRepository.GetAllWithspec(spec);

            var specCount = new UsuarioForCountingSpecification(usuarioParams);
            var totalUsuarios = await seguridadRepository.CountAsync(specCount);

            var rounded = Math.Ceiling(Convert.ToDecimal(totalUsuarios) / Convert.ToDecimal(usuarioParams.PageSize));
            var totalPages = Convert.ToInt32(rounded);

            var data = mapper.Map<IReadOnlyList<UsuarioDTO>>(usuario);

            return Ok(new Pagination<UsuarioDTO>
            {
                Count = totalUsuarios,
                Data = data,
                PageCount = totalPages,
                PageIndex = usuarioParams.PageIndex,
                PageSize = usuarioParams.PageSize
            });
        }

        [HttpPut("role/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        public async Task<ActionResult<UsuarioDTO>> UpdateRole(string id, RoleDTO roleDTO)
        {
            var role = await roleManager.FindByNameAsync(roleDTO.Nombre);
            if (role is null)
            {
                return NotFound(new CodeErrorResponse(404, "El role no exíste"));
            }
            var usuario = await userManager.FindByIdAsync(id);
            if (usuario is null)
            {
                return NotFound(new CodeErrorResponse(404, "El usuario no exíste"));
            }

            var usuarioDTO = mapper.Map<UsuarioDTO>(usuario);
            if (roleDTO.Status)
            {
                var resultado = await userManager.AddToRoleAsync(usuario, roleDTO.Nombre);
                if (resultado.Succeeded)
                {
                    usuarioDTO.Admin = true;
                }

                if (resultado.Errors.Any())
                {
                    if (resultado.Errors.Where(x => x.Code == "UserAlreadyInRole").Any())
                    {
                        usuarioDTO.Admin = true;
                    }
                }
            }
            else
            {
                var resultado = await userManager.RemoveFromRoleAsync(usuario, roleDTO.Nombre);
                if (resultado.Succeeded)
                {
                    usuarioDTO.Admin = false;
                }
            }

            if (usuarioDTO.Admin)
            {
                var roles = new List<string>();
                roles.Add("ADMIN");
                usuarioDTO.Token = tokenService.CreateToken(usuario, roles);
            }
            else
            {
                usuarioDTO.Token = tokenService.CreateToken(usuario, null);

            }
            return usuarioDTO;
        }

        [HttpGet("account/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuarioById(string id)
        {
            var usuario = await userManager.FindByIdAsync(id);
            if (usuario is null)
            {
                return NotFound(new CodeErrorResponse(404, "El usuario no exíste"));
            }

            var roles = await userManager.GetRolesAsync(usuario);

            return new UsuarioDTO()
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                UserName = usuario.UserName,
                Token = tokenService.CreateToken(usuario, roles),
                Imagen = usuario.Imagen,
                Admin = roles.Contains("ADMIN") ? true : false
            };
        }

        [HttpGet()]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UsuarioDTO>> GetUsuario()
        {
            //var email = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            //var usuario = await userManager.FindByEmailAsync(email);

            var usuario = await userManager.BuscarUsuarioAsync(HttpContext.User);

            var roles = await userManager.GetRolesAsync(usuario);

            return new UsuarioDTO()
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                UserName = usuario.UserName,
                Imagen = usuario.Imagen,
                Token = tokenService.CreateToken(usuario, roles),
                Admin = roles.Contains("ADMIN") ? true : false
            };
        }

        [HttpGet("emailvalido")]
        public async Task<ActionResult<bool>> ValidarEmail([FromQuery] string email)
        {
            var usuario = await userManager.FindByEmailAsync(email);
            if (usuario is null)
            {
                return false;
            }
            return true;
        }

        [HttpGet("direccion")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<DireccionDTO>> GetDireccion()
        {
            var usuario = await userManager.BuscarUsuarioConDireccionAsync(HttpContext.User);
            var direccionDTO = mapper.Map<DireccionDTO>(usuario.Direccion);
            return direccionDTO;
        }

        [HttpPut("direccion")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<DireccionDTO>> UpdateDireccion(DireccionDTO direccionDTO)
        {
            var usuario = await userManager.BuscarUsuarioConDireccionAsync(HttpContext.User);
            usuario.Direccion = mapper.Map<Direccion>(direccionDTO);
            var resultado = await userManager.UpdateAsync(usuario);

            if (!resultado.Succeeded)
            {
                return BadRequest("No se pudo actualizar la dirección del usuario");
            }

            return Ok(mapper.Map<DireccionDTO>(usuario.Direccion));

        }

    }
}
