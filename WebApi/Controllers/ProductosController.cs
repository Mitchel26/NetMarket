using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.Errors;

namespace WebApi.Controllers
{
    public class ProductosController : BaseApiController
    {
        private readonly IGenericRepository<Producto> productoRepository;
        private readonly IMapper mapper;

        public ProductosController(IGenericRepository<Producto> productoRepository, IMapper mapper)
        {
            this.productoRepository = productoRepository;
            this.mapper = mapper;
        }

        [HttpGet()]
        public async Task<ActionResult<Pagination<ProductoDTO>>> GetProductos([FromQuery] ProductoSpecificationParams productoParams)
        {
            var spec = new ProductoWithCategoriaAndMarcaSpecification(productoParams);
            var productos = await productoRepository.GetAllWithspec(spec);

            var specCount = new ProductoForCountingSpecification(productoParams);
            var totalProductos = await productoRepository.CountAsync(specCount);

            var rounded = Math.Ceiling(Convert.ToDecimal(totalProductos / productoParams.PageSize));
            var totalPages = Convert.ToInt32(rounded);

            var data = mapper.Map<IReadOnlyList<ProductoDTO>>(productos);

            return Ok(new Pagination<ProductoDTO>
            {
                Count = totalProductos,
                Data = data,
                PageCount = totalPages,
                PageIndex = productoParams.PageIndex,
                PageSize = productoParams.PageSize
            });

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDTO>> GetProducto(int id)
        {
            var spec = new ProductoWithCategoriaAndMarcaSpecification(id);
            var producto = await productoRepository.GetByIdWithspec(spec);
            if (producto is null)
            {
                return NotFound(new CodeErrorResponse(404, "El producto no existe"));
            }
            return mapper.Map<ProductoDTO>(producto);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        public async Task<ActionResult<Producto>> Post([FromBody] Producto producto)
        {
            var resultado = await productoRepository.Add(producto);
            if (resultado == 0)
            {
                throw new Exception("No se inserto el producto");
            }
            return Ok(producto);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        public async Task<ActionResult<Producto>> Update(int id, [FromBody] Producto producto)
        {
            producto.Id = id;
            var resultado = await productoRepository.Update(producto);
            if (resultado == 0)
            {
                throw new Exception("No se puede actualizar el producto");

            }
            return Ok(producto);
        }
    }
}
