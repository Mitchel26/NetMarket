using AutoMapper;
using Core.Entities.OrdenCompra;
using Core.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.DTOs;
using WebApi.Errors;

namespace WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdenCompraController : BaseApiController
    {
        private readonly IOrdenCompraService ordenCompraService;
        private readonly IMapper mapper;

        public OrdenCompraController(IOrdenCompraService ordenCompraService, IMapper mapper)
        {
            this.ordenCompraService = ordenCompraService;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<OrdenCompraResponseDTO>> ActionOrdenCompra(OrdenCompraDTO ordenCompraDTO)
        {
            var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            var direccion = mapper.Map<Direccion>(ordenCompraDTO.DireccionEnvio);
            var ordenCompra = await ordenCompraService.AddOrdenCompraAsync(email, ordenCompraDTO.TipoEnvio, ordenCompraDTO.CarritoCompraId, direccion);

            if (ordenCompra is null)
            {
                return BadRequest(new CodeErrorResponse(400, "Errores creando la orden de compra"));
            }

            return mapper.Map<OrdenCompraResponseDTO>(ordenCompra);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrdenCompraResponseDTO>>> GetOrdenCompras()
        {
            var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            var ordenCompras = await ordenCompraService.GetOrdenComprasByUserEmailAsync(email);
            return Ok(mapper.Map<IReadOnlyList<OrdenCompraResponseDTO>>(ordenCompras));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenCompraResponseDTO>> GetOrdenComprasById(int id)
        {
            var email = HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            var ordenCompras = await ordenCompraService.GetOrdenComprasByIdAsync(id, email);
            if (ordenCompras is null)
            {
                return NotFound(new CodeErrorResponse(404, "No se encontro la orden de compra"));
            }
            return mapper.Map<OrdenCompraResponseDTO>(ordenCompras);
        }

        [HttpGet("tipoEnvio")]
        public async Task<ActionResult<IReadOnlyList<TipoEnvio>>> GetTipoEnvios()
        {
            return Ok(await ordenCompraService.GetTipoEnvios());
        }
    }
}
