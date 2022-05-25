using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class CategoriasController : BaseApiController
    {
        private readonly IGenericRepository<Categoria> genericRepository;

        public CategoriasController(IGenericRepository<Categoria> genericRepository)
        {
            this.genericRepository = genericRepository;
        }

        [HttpGet()]
        public async Task<ActionResult<IReadOnlyList<Categoria>>> GetCategorias()
        {
            var categorias = await genericRepository.GetAllAsync();
            return Ok(categorias);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            var categoria = await genericRepository.GetByIdAsync(id);
            return categoria;
        }
    }
}
